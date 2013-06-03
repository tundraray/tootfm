using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Posmotrim.TootFM.Adapters;
using Posmotrim.TootFM.App.ViewModel;

namespace Posmotrim.TootFM.App.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Fields

        private MainViewModel _mainViewModel;
        Popup _popup;
        #endregion

        #region Properties

        public MainViewModel MainViewModel
        {
            get
            {
                return _mainViewModel ?? (_mainViewModel = this.DataContext as MainViewModel);
            }
        }

        #endregion
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
           

        }


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
            Messenger.Default.Unregister<Uri>(this);
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Messenger.Default.Register<Uri>(this, "NavigationRequest", NavigateTo);
            base.OnNavigatedTo(e);

            if (this.MainViewModel != null)
            {
                bool isBeingActivated = PhoneApplicationService.Current.StartupMode == StartupMode.Activate;
                this.MainViewModel.NavigatedTo(isBeingActivated, this.State);
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}