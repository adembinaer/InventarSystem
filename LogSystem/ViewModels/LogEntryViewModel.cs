using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using DuplicateCheckerLib;
using MySql.Data.MySqlClient;

namespace LogSystem
{
    public class LogEntryViewModel 
    {
        private IDbConnection dbConnection;
        private AddLogViewModel _addLogViewModel;
        public LogEntryViewModel(AddLogViewModel addLogViewModel)
        {
            _addLogViewModel = addLogViewModel;
            _addLogViewModel.LoggingEntries = new ObservableCollection<LoggingEntry>();
        }
        public void ReadEntries()
        {
            if (!ConnectionOpen())
                return;
            try
            {
                ObservableCollection<LoggingEntry> observableCollection = new ObservableCollection<LoggingEntry>();
                IDbCommand command = dbConnection.CreateCommand();
                command.CommandText = "SELECT * FROM v_logentries;";
                IDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    object[] values = new object[dataReader.FieldCount];
                    dataReader.GetValues(values);
                    LoggingEntry loggingEntry = new LoggingEntry
                    {
                        Id = (int)dataReader["Id"],
                        Hostname = dataReader["hostname"].ToString(),
                        Location = dataReader["location"].ToString(),
                        Message = dataReader["message"].ToString(),
                        Pod = dataReader["pod"].ToString(),
                        Severity = (int)dataReader["severity"],
                        Timestamp = (DateTime)dataReader["timestamp"]
                    };
                    observableCollection.Add(loggingEntry);
                }
                dataReader.Close();
                _addLogViewModel.LoggingEntries = observableCollection;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0), ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }
        private void ConnectingToDatabase()
        {
            if (string.IsNullOrEmpty(_addLogViewModel.ConnectionString))
            {
                Console.Error.WriteLine("ERROR, Wrong Connection");
                throw new InvalidExpressionException("ERROR, Wrong Connection");
            }
            try
            {
                dbConnection = new MySqlConnection(_addLogViewModel.ConnectionString);
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0), ex.Message);
            }
        }
        private bool ConnectionOpen()
        {
            if (dbConnection == null)
                ConnectingToDatabase();
            if (dbConnection == null)
                return false;
            if (dbConnection.State == ConnectionState.Open)
                return true;
            if (dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken)
                ConnectingToDatabase();
            return false;
        }
        private void CloseConnection()
        {
            if (dbConnection == null)
                return;
            dbConnection.Close();
        }
        internal bool CanLoadEntries()
        {
            return !string.IsNullOrEmpty(_addLogViewModel.ConnectionString);
        }
        internal void LoadEntries()
        {
            ReadEntries();
        }
        internal bool CanAddEntries()
        {
            return dbConnection != null;
        }

        internal void AddEntries()
        {
            if (!ConnectionOpen())
            {
                try
                {
                    var pod = _addLogViewModel.Pod;
                    var devicenameItem = _addLogViewModel.DevicenameItem;
                    int num = _addLogViewModel.Level;
                    string message = _addLogViewModel.Message;
                    IDbCommand command = dbConnection.CreateCommand();
                    command.CommandText = "CALL `LogMessageAdd`(@pod, @hostname, @level, @message);";
                    command.Parameters.Add(new MySqlParameter("@pod", pod));
                    command.Parameters.Add(new MySqlParameter("@hostname", devicenameItem));
                    command.Parameters.Add(new MySqlParameter("@level", num));
                    command.Parameters.Add(new MySqlParameter("@message", message));
                    if (command.ExecuteNonQuery() <= 0)
                        return;
                    _addLogViewModel.Pod = 0;
                    _addLogViewModel.DevicenameItem = "";
                    _addLogViewModel.Message = "";
                    _addLogViewModel.Level = 0;
                    ReadEntries();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0),
                        ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        internal bool CanConfirmEntries()
        {
            var selectedLoggingEntries = _addLogViewModel.SelectedEntries;
            return selectedLoggingEntries != null && (uint)selectedLoggingEntries.Id > 0U;
        }

        internal void ConfirmEntries()
        {
            if (!ConnectionOpen())
            {
                try
                {
                    int id = _addLogViewModel.SelectedEntries.Id;
                    IDbCommand command = dbConnection.CreateCommand();
                    command.CommandText = "CALL `LogClear`(@id);";
                    command.Parameters.Add(new MySqlParameter("@id", id));
                    if (command.ExecuteNonQuery() <= 0)
                        return;
                    ReadEntries();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0),
                        ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        internal bool CanFindDuplicates()
        {
            return _addLogViewModel.LoggingEntries.Count > 0;
        }
        internal void FindDuplicates()
        {
            try
            {
                IEnumerable<IEntity> moreTimes = new DuplicateChecker().FindDuplicates(_addLogViewModel.LoggingEntries);
                ObservableCollection<LoggingEntry> observableCollection = new ObservableCollection<LoggingEntry>();
                using (IEnumerator<IEntity> enumerator = moreTimes.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IEntity now = enumerator.Current;
                        observableCollection.Add((LoggingEntry)now);
                    }
                }
                _addLogViewModel.DuplicateLogginEntries = observableCollection;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR READING DUPLICATE ENTRY: {0}",ex.Message);
            }
        }
    }
}


