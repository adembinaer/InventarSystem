﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace LogSystem
{
    public class LoggViewModel : INotifyPropertyChanged
    {
        private LoggEntryReppo _loggEntryReppo;
        private ObservableCollection<LoggingEntry> _loggingEntries;
        private LoggingEntry _selectedEntries;
        private ICommand confirmCommand;
        private ICommand loadCommand;
        private ICommand addCommand;
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
        public LoggViewModel()
        {
            _connectionString = "server=localhost;database=musterag;uid=demo;password=JusKo1986;";
            _loggEntryReppo = new LoggEntryReppo(this);
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
                    loadCommand = new RelayCommand(param => _loggEntryReppo.LoadEntries(), param => _loggEntryReppo.CanLoadEntries());
                return loadCommand;
            }
        }
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(param => _loggEntryReppo.AddEntries(), param => _loggEntryReppo.CanAddEntries());
                }

                return addCommand;
            }
        }
        public ICommand ConfirmCommand
        {
            get
            {
                if (confirmCommand == null)
                    confirmCommand = new RelayCommand(param => _loggEntryReppo.ConfirmEntries(), param => _loggEntryReppo.CanConfirmEntries());
                return confirmCommand;
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