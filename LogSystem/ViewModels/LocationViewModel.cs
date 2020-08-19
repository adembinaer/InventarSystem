using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using LogSystem.DataAccess.ADO.NET;
using LogSystem.Models;
using MySql.Data.MySqlClient;

namespace LogSystem
{
    public class LocationViewModel : INotifyPropertyChanged
    {
        private IDbConnection dbConnection;

        private readonly LocationRepository _clearRepository;
        private readonly LocationRepository _dataRepository;

        private ObservableCollection<Location> _locationEntries;
        private Location _selectedEntries;
        private string _connectionString;
        private int _id;
        private int _podId;
        private int _adressId;

        public RelayCommand LocationDeleteCommand { get; }
        public RelayCommand LocationAddCommand { get; }
        public RelayCommand LocationLoadCommand { get; }

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

        public int Id
        {
            get { return _id; }
            set
            {
                if (!(_id != value))
                    return;
                _id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }
        public int FK_PointOfDelivery
        {
            get { return _podId; }
            set
            {
                if (!(_podId != value))
                    return;
                _podId = value;
                NotifyPropertyChanged(nameof(FK_PointOfDelivery));
            }
        }

        public int FK_Address
        {
            get { return _adressId; }
            set
            {
                if (!(_adressId != value))
                    return;
                _adressId = value;
                NotifyPropertyChanged(nameof(FK_Address));
            }
        }

        public ObservableCollection<Location> LocationEntries
        {
            get { return _locationEntries; }
            set
            {
                if (_locationEntries == value)
                    return;
                _locationEntries = value;
                NotifyPropertyChanged(nameof(LocationEntries));
            }
        }

        public LocationViewModel()
        {
            LocationEntries = new ObservableCollection<Location>();
            _connectionString = @"Data Source = BPMNFOREVER\ZBWBINAER; Integrated Security = SSPI; Initial Catalog=musterag;";
            _clearRepository = new LocationRepository();
            _dataRepository = new LocationRepository();
            LocationAddCommand = new RelayCommand(AddEntries, CanAddEntries);
            LocationLoadCommand = new RelayCommand(LoadEntries, CanLoadEntries);
            LocationDeleteCommand = new RelayCommand(DeleteEntries, CanDeleteEntries);
        }
        public void ReadEntries()
        {
            if (!ConnectionOpen())
                return;
            try
            {
                LocationEntries.Clear();
                var result = _dataRepository.GetAll();
                foreach (var i in result)
                {
                    LocationEntries.Add(i);
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

        internal bool CanDeleteEntries()
        {
            var selectedLocationEntries = SelectedEntries;
            return selectedLocationEntries != null && (uint)selectedLocationEntries.Id > 0U;
        }
        internal void DeleteEntries()
        {

            try
            {
                _clearRepository.Delete(_selectedEntries);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0),
                    ex.Message);
            }
        }
        internal bool CanLoadEntries()
        {
            return !string.IsNullOrEmpty(ConnectionString);
        }

        internal void LoadEntries()
        {
            ReadEntries();
        }
        public Location SelectedEntries
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
        internal bool CanAddEntries()
        {
            return dbConnection != null;
        }
        internal void AddEntries()
        {
            try
            {
                _dataRepository.Add(new Location
                {
                    Id = Id,
                    FK_PointOfDelivery = FK_PointOfDelivery,
                    FK_Address = FK_Address
                });
                ReadEntries();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("ERROR, Wrong Connection: {0}", 0),
                    ex.Message);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
