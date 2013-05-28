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

namespace Posmotrim.TootFM.App.Views
{
    public partial class PlayerView : PhoneApplicationPage
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        public void NavigateTo(Uri uri)
        {
            if (uri.ToString() == "/GoBack.xaml")
                NavigationService.GoBack();
            else
                NavigationService.Navigate(uri);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {

            Messenger.Default.Unregister<Uri>(this);
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            Messenger.Default.Register<Uri>(this, "NavigationRequest", NavigateTo);
            base.OnNavigatedTo(e);


        }
    }
}