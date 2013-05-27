using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
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
        #endregion
        public MapViewModel(ISettingsStore settingsStore, ILocationService locationService,
                             ITootFMServiceClient serviceClient)
        {
           
            _serviceClient = () => serviceClient;
            _settingsStore = settingsStore;
            this._locationService = locationService;
          
            MapCenter = this._locationService.TryToGetCurrentLocation();
           
        }

        private List<MapLayer> _mapItemsControl = new List<MapLayer>();
        public List<MapLayer> MapItemsControl
        {
            get { return _mapItemsControl; }
            set
            {
                _mapItemsControl = value;
               
            }
        }

        private GeoCoordinate _mapCenter;
        public GeoCoordinate MapCenter
        {
            get { return _mapCenter; }
            set
            {
                _mapCenter = value;
                UpdateMap();
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

        private void BindingMaps(IEnumerable<Venue> venues)
        {

            MapItemsControl.Clear();
            foreach (var venue in venues)
            {
                var layer = new MapLayer();
                var canvas = new Button()
                                 {
                                     BorderThickness = new Thickness(0,0,0,0),
                                     Width = 65,
                                     Height = 97,
                                     Background =
                                         new ImageBrush()
                                             {
                                                 ImageSource =
                                                     new BitmapImage(new Uri(
                                                                         "/Assets/Icons/map.layer.png",
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
                                                Content = canvas,
                                                GeoCoordinate = new GeoCoordinate(venue.Lat, venue.Lng)
                                            });

                MapItemsControl.Add(layer);
            }
            Messenger.Default.Send<bool>(true, "MapUpdate");
            
            IsSync = false;

        }

        
    }
}