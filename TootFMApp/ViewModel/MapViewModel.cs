using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Reactive;
using Posmotrim.Phone.Adapters;
using Posmotrim.TootFM.PhoneServices.Models;
using Posmotrim.TootFM.PhoneServices.Services;
using Posmotrim.TootFM.PhoneServices.Services.Stores;
using Posmotrim.TootFM.PhoneServices.Services.TootFMService;

namespace Posmotrim.TootFM.App.ViewModel
{
    public class MapViewModel : ViewModelBase
    {
        #region Fields

        private ISettingsStore _settingsStore;
        private readonly ILocationService _locationService;
        private Func<ITootFMServiceClient> _serviceClient;
        private IGeoCoordinateWatcher _geoCoordinateWatcher;
        #endregion
        public MapViewModel(ISettingsStore settingsStore, ILocationService locationService,
                             ITootFMServiceClient serviceClient, IGeoCoordinateWatcher geoCoordinateWatcher)
        {
            this._locationService = locationService;
            _geoCoordinateWatcher = geoCoordinateWatcher;

            //_locationService.StartWatcher();
            _serviceClient = () => serviceClient;
            _settingsStore = settingsStore;
            MapCommand = new RelayCommand(() => { MapCenter = this._locationService.TryToGetCurrentLocation(); });


        }


        
        private List<MapLayer> _mapItemsControl = new List<MapLayer>();
        public List<MapLayer> MapItemsControl
        {
            get
            {
                return _mapItemsControl;
            }
            set
            {
                _mapItemsControl = value;
                RaisePropertyChanged("MapItemsControl");
            }
        }
      

        private int zoomLevel = 16;
        public int ZoomLevel
        {
            get { return zoomLevel; }
            set
            {
                zoomLevel = value;
                MapItemsControl.Clear();
                Messenger.Default.Send<bool>(true, "MapUpdate");
                RaisePropertyChanged("ZoomLevel");
            }
        }

        public RelayCommand MapCommand { get; set; }
        private GeoCoordinate _lastMapCenter;
        private GeoCoordinate _mapCenter = null;
        public GeoCoordinate MapCenter
        {
            get
            {
                if (_mapCenter == null)
                {
                    _serviceClient().GetLastCheckin().ObserveOnDispatcher().Catch((Exception exception) => ErrorLastCheckin(exception)).Subscribe(SetPosition);
                    return _locationService.TryToGetCurrentLocation();
                }
                return _mapCenter;
            }
            set
            {
                _mapCenter = value;
                try
                {
                    if (_lastMapCenter == null || (_mapCenter.GetDistanceTo(_lastMapCenter)) > (156543.04 * Math.Cos(MapCenter.Latitude) / (2 ^ ZoomLevel)) * 100)
                    {
                        UpdateMap();
                        _lastMapCenter = value;
                    }
                }
                catch
                {
                    UpdateMap();
                    _lastMapCenter = value;
                }


                RaisePropertyChanged("MapCenter");
            }
        }

        public bool IsSync { get; set; }

        private void UpdateMap()
        {
            if (!IsSync)
            {
                IsSync = true;
                _serviceClient().ListVenues(null, MapCenter).ObserveOnDispatcher().Subscribe(BindingMaps);
            }

        }



        private int maxPoint = 20;

        private void BindingMaps(IEnumerable<Venue> venues)
        {

            var items =  new List<MapLayer>();
            //MapItemsControl.Clear();
            if (ZoomLevel < 11)
                return;

            var userLocation = venues.Where(v => v.IsUser).ToList();
            var location = (from _venue in venues.Where(v => !v.IsUser)
                            let temp = MapCenter.GetDistanceTo(new GeoCoordinate(_venue.Lat, _venue.Lng)) % (156543.04 * Math.Cos(MapCenter.Latitude) / (2 ^ ZoomLevel))


                            orderby temp descending
                            select _venue).Take(maxPoint - userLocation.Count() <= 0 ? 1 : maxPoint - userLocation.Count());
            userLocation.AddRange(location);

            foreach (var venue in userLocation.Where(venue => !MapItemsControl.Any(m => m.Any(o => o.GeoCoordinate.Latitude == venue.Lat && o.GeoCoordinate.Longitude == venue.Lng))))
            {
                var layer = new MapLayer();
                var canvas = new Button()
                                 {
                                     BorderThickness = new Thickness(0, 0, 0, 0),
                                     Width = 65,
                                     Height = 97,
                                     
                                     Background =
                                         new ImageBrush()
                                             {
                                                 ImageSource =
                                                     new BitmapImage(new Uri(string.Format("/Assets/Icons/map{0}.layer.png", venue.IsUser ? "" : ".othere"),
                                                                         UriKind.Relative))
                                             },
                                     Command = new RelayCommand(() =>
                                                                    {
                                                                        Messenger.Default.Send(venue, "ChangeLocation");
                                                                        Messenger.Default.Send(new Uri("/GoBack.xaml", UriKind.Relative), "NavigationRequest");
                                                                    })
                                 };

                layer.Add(new MapOverlay()
                                             {
                                                 PositionOrigin = new Point(0.5,0.5),
                                                 Content = canvas,
                                                 GeoCoordinate = new GeoCoordinate(venue.Lat, venue.Lng)
                                             });

                items.Add(layer);
            }
           
            MapItemsControl.AddRange(items);

            Messenger.Default.Send(items, "MapUpdate");

            IsSync = false;

        }



        public IObservable<Venue> ErrorLastCheckin(Exception second)
        {
            return Observable.Return<Venue>(null);
        }

        private void SetPosition(Venue venue)
        {
            if (venue != null)
            {
                _lastMapCenter = GeoCoordinate.Unknown;
                MapCenter = new GeoCoordinate(venue.Lat, venue.Lng);

            }
        }

        public void Clear()
        {
           _mapItemsControl  = new List<MapLayer>();
            _mapCenter = null;
        }

        public void Load()
        {
            _mapItemsControl  = new List<MapLayer>();
            _mapCenter = null;
        }
    }
}