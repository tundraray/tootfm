using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Posmotrim.TootFM.App.Views
{
    public partial class PlayerView : PhoneApplicationPage
    {
        private Timer _timer;
        public PlayerView()
        {
            InitializeComponent();
            _timer = new Timer(state => Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
                {
                    slider.Value = (int)BackgroundAudioPlayer.Instance.Position.TotalSeconds;}
                else
                {
                    slider.Value = 0;
                }

            }), null, new TimeSpan(0, 0, 0, 0), new TimeSpan(0, 0, 0, 1));
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