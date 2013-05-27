using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
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

        private void MapUpdate(bool obj)
        {
            
           
            this.Map.Layers.Clear();
            foreach (var l in MainViewModel.MapItemsControl)
            {
                this.Map.Layers.Add(l);
            }
            
        }

        #region Fields

        private MapViewModel _mainViewModel;
       
        #endregion

        public void NavigateTo(Uri uri)
        {
            if (uri.ToString() == "/GoBack.xaml")
                NavigationService.GoBack();
            else
                NavigationService.Navigate(uri);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Messenger.Default.Unregister<bool>(this);
            Messenger.Default.Unregister<Uri>(this);
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Messenger.Default.Register<bool>(this, "MapUpdate", MapUpdate);
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
    }
}