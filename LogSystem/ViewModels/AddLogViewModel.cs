using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace LogSystem
{
    public class AddLogViewModel : INotifyPropertyChanged
    {
        private LogEntryViewModel _logEntryViewModel;
        private ObservableCollection<LoggingEntry> _loggingEntries;
        private ObservableCollection<LoggingEntry> _duplicateLoggingEntries;
        private LoggingEntry _selectedEntries;
        private LoggingEntry _selectedDuplicate;
        private ICommand confirmCommand;
        private ICommand loadCommand;
        private ICommand addCommand;
        private ICommand findDuplicateCommand;
        private int _pod;
        private string _devicenameItem;
        private int _level;
        private string _message;
        private string _connectionString;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<LoggingEntry> LoggingEntries
        {
            get
            {
                return _loggingEntries;
            }
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
            get
            {
                return _duplicateLoggingEntries;
            }
            set
            {
                if (_duplicateLoggingEntries == value)
                    return;
                _duplicateLoggingEntries = value;
                NotifyPropertyChanged(nameof(DuplicateLogginEntries));
            }
        }
        public LoggingEntry SelectedEntries
        {
            get
            {
                return _selectedEntries;
            }
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
            get
            {
                return _selectedDuplicate;
            }
            set
            {
                if (_selectedDuplicate == value)
                    return;
                _selectedDuplicate = value;
                NotifyPropertyChanged(nameof(SelectedDuplicate));
            }
        }
        public AddLogViewModel()
        {
            _connectionString = "server=localhost;database=musterag;uid=demo;password=JusKo1986;";
            _logEntryViewModel = new LogEntryViewModel(this);
        }
        public void NotifyPropertyChanged(string name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public ICommand LoadCommand
        {
            get
            {
                if (loadCommand == null)
                    loadCommand = new RelayCommand(param => _logEntryViewModel.LoadEntries(), param => _logEntryViewModel.CanLoadEntries());
                return loadCommand;
            }
        }
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(param => _logEntryViewModel.AddEntries(), param => _logEntryViewModel.CanAddEntries());
                }

                return addCommand;
            }
        }
        public ICommand ConfirmCommand
        {
            get
            {
                if (confirmCommand == null)
                    confirmCommand = new RelayCommand(param => _logEntryViewModel.ConfirmEntries(), param => _logEntryViewModel.CanConfirmEntries());
                return confirmCommand;
            }
        }
        public ICommand FindDuplicateCommand
        {
            get
            {
                if (findDuplicateCommand == null)
                    findDuplicateCommand = new RelayCommand(param => _logEntryViewModel.FindDuplicates(),param =>_logEntryViewModel.CanFindDuplicates());
                return findDuplicateCommand;
            }
        }
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
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
            get
            {
                return _pod;
            }
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
            get
            {
                return _devicenameItem;
            }
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
            get
            {
                return _level;
            }
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
            get
            {
                return _message;
            }
            set
            {
                if (!(_message != value))
                    return;
                _message = value;
                NotifyPropertyChanged("Pod");
            }
        }
    }
}
