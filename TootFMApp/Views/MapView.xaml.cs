using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Posmotrim.TootFM.App.ViewModel;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Posmotrim.TootFM.App.Views
{
    public partial class MapView : PhoneApplicationPage
    {
        public MapView()
        {
            InitializeComponent();
           
        }

        private void MapUpdate(List<MapLayer> obj)
        {

            
            foreach (var l in obj)
            {
                try
                {
                    this.Map.Layers.Add(l);
                }
                catch 
                {
                  
                }
                
            }
            
            
        }

        #region Fields

        private MapViewModel _mainViewModel;
       
        #endregion

        public void NavigateTo(Uri uri)
        {
            if (uri.ToString() == "/GoBack.xaml")
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
                NavigationService.Navigate(uri);
        
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Map.Layers.Clear();
            MainViewModel.Clear();
            Messenger.Default.Unregister<List<MapLayer>>(this);
            Messenger.Default.Unregister<Uri>(this);
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
           // Map.Layers.Clear();
            MainViewModel.Load();
           
            //MainViewModel.MapItemsControlAdd = new List<MapLayer>();
            Messenger.Default.Register<List<MapLayer>>(this, "MapUpdate", MapUpdate);
            Messenger.Default.Register<Uri>(this, "NavigationRequest", NavigateTo);
            base.OnNavigatedTo(e);

           
        }

       

        #region Properties

        public MapViewModel MainViewModel
        {
            get
            {
                return _mainViewModel ?? (_mainViewModel = this.DataContext as MapViewModel);
            }
        }

        #endregion

        private void OnMapHold(object sender, GestureEventArgs e)
        {
            
        }

        private void Map_OnLoaded(object sender, RoutedEventArgs e)
        {
            var map = sender as Map;
            if (map.Layers.Count == 0 && MainViewModel.MapItemsControl.Count > 0)
            {
                MapUpdate(MainViewModel.MapItemsControl);
            }
        }
    }
}