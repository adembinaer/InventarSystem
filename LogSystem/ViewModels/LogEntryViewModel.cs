using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using DuplicateCheckerLib;
using LogSystem.DataAccess.ADO.NET;
using MySql.Data.MySqlClient;

namespace LogSystem
{
    public class LogEntryViewModel : INotifyPropertyChanged
    {
        private IDbConnection dbConnection;
        private readonly LogEntryRepository _dataRepository;
        private readonly LogEntryRepository _clearRepository;

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
        public RelayCommand LocationCommand { get; }

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
            //_connectionString = "server=localhost;database=musterag;uid=demo;password=JusKo1986;";
            _connectionString = @"Data Source = BPMNFOREVER\ZBWBINAER; Integrated Security = SSPI; Initial Catalog=musterag;";
            //_connectionString = "data source = BPMNFOREVER\\ZBWBINAER; initial catalog = musterag; Integrated Security =SSPI;";
            _dataRepository = new LogEntryRepository();
            _clearRepository = new LogEntryRepository();
            LoadCommand = new RelayCommand(LoadEntries, CanLoadEntries);
            AddCommand = new RelayCommand(AddEntries, CanAddEntries);
            ConfirmCommand = new RelayCommand(ConfirmEntries, CanConfirmEntries);
            FindDuplicateCommand = new RelayCommand(FindDuplicates, CanFindDuplicates);
            LocationCommand = new RelayCommand(OpenLocationDialog);
        }
        public void ReadEntries()
        {
            if (!ConnectionOpen())
                return;
            try
            {
                LoggingEntries.Clear();
                var result = _dataRepository.GetAll();
                foreach (var i in result)
                {
                    LoggingEntries.Add(i);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0), ex.Message);
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
                dbConnection = new SqlConnection(ConnectionString);
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0), ex.Message);
            }
        }
        //private bool ConnectionOpen() => !string.IsNullOrWhiteSpace(ConnectionString);
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
        private void OpenLocationDialog()
        {
            new LogSystem.Views.Location().ShowDialog();
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
            try
            {
                _dataRepository.Add(new LoggingEntry
                {
                    Id = Pod,
                    Hostname = DevicenameItem,
                    Severity = Level,
                    Message = Message
                });
                ReadEntries();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0),
                    ex.Message);
            }
        }

        internal bool CanConfirmEntries()
        {
            var selectedLoggingEntries = SelectedEntries;
            return selectedLoggingEntries != null && (uint)selectedLoggingEntries.Id > 0U;
        }
        internal void ConfirmEntries()
        {
            try
            {
                _clearRepository.Delete(_selectedEntries);

                ReadEntries();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0),
                    ex.Message);
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
                        observableCollection.Add((LoggingEntry)now);
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



