using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using DuplicateCheckerLib;
using MySql.Data.MySqlClient;

namespace LogSystem
{
    public class LogEntryViewModel : INotifyPropertyChanged
    {
        private IDbConnection dbConnection;

        //private LogEntryViewModel _addLogViewModel;
        private LogEntryViewModel _logEntryViewModel;
        private ObservableCollection<LoggingEntry> _loggingEntries;
        private ObservableCollection<LoggingEntry> _duplicateLoggingEntries;
        private LoggingEntry _selectedEntries;
        private LoggingEntry _selectedDuplicate;
        private int _pod;
        private string _devicenameItem;
        private int _level;
        private string _message;
        private string _connectionString;

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand LoadCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand FindDuplicateCommand { get; }

        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (!(_connectionString != value))
                    return;
                _connectionString = value;
                NotifyPropertyChanged(nameof(ConnectionString));
            }
        }

        public int Pod
        {
            get { return _pod; }
            set
            {
                if (!(_pod != value))
                    return;
                _pod = value;
                NotifyPropertyChanged(nameof(Pod));
            }
        }

        public string DevicenameItem
        {
            get { return _devicenameItem; }
            set
            {
                if (!(_devicenameItem != value))
                    return;
                _devicenameItem = value;
                NotifyPropertyChanged(nameof(DevicenameItem));
            }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                if (!(_level != value))
                    return;
                _level = value;
                NotifyPropertyChanged(nameof(Level));
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (!(_message != value))
                    return;
                _message = value;
                NotifyPropertyChanged("Pod");
            }
        }
        public ObservableCollection<LoggingEntry> LoggingEntries
        {
            get { return _loggingEntries; }
            set
            {
                if (_loggingEntries == value)
                    return;
                _loggingEntries = value;
                NotifyPropertyChanged(nameof(LoggingEntries));
            }
        }

        public ObservableCollection<LoggingEntry> DuplicateLogginEntries
        {
            get { return _duplicateLoggingEntries; }
            set
            {
                if (_duplicateLoggingEntries == value)
                    return;
                _duplicateLoggingEntries = value;
                NotifyPropertyChanged(nameof(DuplicateLogginEntries));
            }
        }


        public LogEntryViewModel()
        {
            LoggingEntries = new ObservableCollection<LoggingEntry>();
            _connectionString = "server=localhost;database=musterag;uid=demo;password=JusKo1986;";
            //_logEntryViewModel = new LogEntryViewModel();
            LoadCommand = new RelayCommand(LoadEntries, CanLoadEntries);
            AddCommand = new RelayCommand(AddEntries, CanAddEntries);
            ConfirmCommand = new RelayCommand(ConfirmEntries, CanConfirmEntries);
            FindDuplicateCommand = new RelayCommand(FindDuplicates, CanFindDuplicates);

        }

        public void ReadEntries()
        {
            if (!ConnectionOpen())
                return;
            try
            {
                //ObservableCollection<LoggingEntry> observableCollection = new ObservableCollection<LoggingEntry>();
                IDbCommand command = dbConnection.CreateCommand();
                command.CommandText = "SELECT * FROM v_logentries;";
                IDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    object[] values = new object[dataReader.FieldCount];
                    dataReader.GetValues(values);
                    LoggingEntry loggingEntry = new LoggingEntry
                    {
                        Id = (int) dataReader["Id"],
                        Hostname = dataReader["hostname"].ToString(),
                        Location = dataReader["location"].ToString(),
                        Message = dataReader["message"].ToString(),
                        Pod = dataReader["pod"].ToString(),
                        Severity = (int) dataReader["severity"],
                        Timestamp = (DateTime) dataReader["timestamp"]
                    };
                    LoggingEntries.Add(loggingEntry);
                }

                dataReader.Close();
                //LoggingEntries = observableCollection;
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
            if (string.IsNullOrEmpty(ConnectionString))
            {
                Console.Error.WriteLine("ERROR, Wrong Connection");
                throw new InvalidExpressionException("ERROR, Wrong Connection");
            }

            try
            {
                dbConnection = new MySqlConnection(ConnectionString);
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
            return !string.IsNullOrEmpty(ConnectionString);
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
                    var pod = Pod;
                    var devicenameItem = DevicenameItem;
                    int num = Level;
                    string message = Message;
                    IDbCommand command = dbConnection.CreateCommand();
                    command.CommandText = "CALL `LogMessageAdd`(@pod, @hostname, @level, @message);";
                    command.Parameters.Add(new MySqlParameter("@pod", pod));
                    command.Parameters.Add(new MySqlParameter("@hostname", devicenameItem));
                    command.Parameters.Add(new MySqlParameter("@level", num));
                    command.Parameters.Add(new MySqlParameter("@message", message));
                    if (command.ExecuteNonQuery() <= 0)
                        return;
                    Pod = 0;
                    DevicenameItem = "";
                    Message = "";
                    Level = 0;
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
            var selectedLoggingEntries = SelectedEntries;
            return selectedLoggingEntries != null && (uint) selectedLoggingEntries.Id > 0U;
        }

        internal void ConfirmEntries()
        {
            if (!ConnectionOpen())
            {
                try
                {
                    int id = SelectedEntries.Id;
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
            return LoggingEntries.Count > 0;
        }

        internal void FindDuplicates()
        {
            try
            {
                IEnumerable<IEntity> moreTimes = new DuplicateChecker().FindDuplicates(LoggingEntries);
                ObservableCollection<LoggingEntry> observableCollection = new ObservableCollection<LoggingEntry>();
                using (IEnumerator<IEntity> enumerator = moreTimes.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IEntity now = enumerator.Current;
                        observableCollection.Add((LoggingEntry) now);
                    }
                }

                DuplicateLogginEntries = observableCollection;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR READING DUPLICATE ENTRY: {0}", ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

       
        public LoggingEntry SelectedEntries
        {
            get { return _selectedEntries; }
            set
            {
                if (_selectedEntries == value)
                    return;
                _selectedEntries = value;
                NotifyPropertyChanged(nameof(SelectedEntries));
            }
        }

        public LoggingEntry SelectedDuplicate
        {
            get { return _selectedDuplicate; }
            set
            {
                if (_selectedDuplicate == value)
                    return;
                _selectedDuplicate = value;
                NotifyPropertyChanged(nameof(SelectedDuplicate));
            }
        }

       

        public void NotifyPropertyChanged(string name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs(name));
        }

      

    }
}



