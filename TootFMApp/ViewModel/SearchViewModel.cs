using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Reactive;
using Posmotrim.Phone.Adapters;
using Posmotrim.TootFM.PhoneServices.Models;
using Posmotrim.TootFM.PhoneServices.Services;
using Posmotrim.TootFM.PhoneServices.Services.Stores;
using Posmotrim.TootFM.PhoneServices.Services.TootFMService;

namespace Posmotrim.TootFM.App.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        #region Fields

        private ISettingsStore _settingsStore;
        private ILocationService _locationService;
        private Func<ITootFMServiceClient> _serviceClient;
        private IGeoCoordinateWatcher _geoCoordinateWatcher;
        #endregion
        public SearchViewModel(ISettingsStore settingsStore, ILocationService locationService,
                             ITootFMServiceClient serviceClient, IGeoCoordinateWatcher geoCoordinateWatcher)
        {
            this._locationService = locationService;
            _geoCoordinateWatcher = geoCoordinateWatcher;
            _geoCoordinateWatcher.PositionChanged += GeoCoordinateWatcherOnPositionChanged;

            _serviceClient = () => serviceClient;
            _settingsStore = settingsStore;
            //MapCommand = new RelayCommand(() => { MapCenter = this._locationService.TryToGetCurrentLocation(); });
            SelectedCommand = new RelayCommand<Venue>(v =>
                                                   {
                                                       Messenger.Default.Send(v, "ChangeLocation");
                                                       Messenger.Default.Send(
                                                           new Uri("/GoBack.xaml", UriKind.Relative),
                                                           "NavigationRequest");
                                                   });
            LoadVenues();

        }

        private string _searchVenue;
        public string SearchVenue
        {
            get { return _searchVenue; }
            set
            {
                if (_searchVenue != value)
                {
                    _searchVenue = value;
                    LoadVenues(value);
                    RaisePropertyChanged("SearchVenue");
                    
                }


                
            }
        }

        private IEnumerable<Venue> _listVenue = new List<Venue>();
        public IEnumerable<Venue> Venues
        {
            get
            {
                if (!string.IsNullOrEmpty(SearchVenue))
                    return _listVenue.Where(v => v.Name.ToLower().Contains(SearchVenue)).ToList();
                return _listVenue;
            }
            set
            {
                _listVenue = value;
                RaisePropertyChanged("Venues");
            }
        }

        private void LoadVenues()
        {
            _serviceClient()
                .ListVenues(null, _locationService.TryToGetCurrentLocation())
                .ObserveOnDispatcher()
                .Subscribe(ListVenue);
        }

        private void LoadVenues(string search)
        {
            _serviceClient()
                .ListVenues(search, null)
                .ObserveOnDispatcher()
                .Subscribe(ListVenue);
        }

        public RelayCommand<Venue> SelectedCommand { get; set; }

        private void GeoCoordinateWatcherOnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> geoPositionChangedEventArgs)
        {
            LoadVenues();
        }

        private void ListVenue(IEnumerable<Venue> venues)
        {
            Venues = venues.Take(25).ToList();
        }
    }
}