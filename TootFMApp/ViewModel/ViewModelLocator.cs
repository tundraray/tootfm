/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Posmotrim.TootFM.App"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.ComponentModel;
using System.Device.Location;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Posmotrim.Phone.Adapters;
using Posmotrim.TootFM.Adapters;
using Posmotrim.TootFM.PhoneServices.Services;
using Posmotrim.TootFM.PhoneServices.Services.Clients;
using Posmotrim.TootFM.PhoneServices.Services.Stores;
using Posmotrim.TootFM.PhoneServices.Services.TootFMService;

namespace Posmotrim.TootFM.App.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //


            SimpleIoc.Default.Register(() => new Uri("https://tootfm.com/api/v1/"));
            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            //SimpleIoc.Default.Register(() => new NavigationServiceFacade((App.RootFrame)));

            if (DesignerProperties.IsInDesignTool)
            {
                // this.Container.Register<ISettingsStore>(c => new DesignerSettingsStore());
            }
            else
            {

            }
            SimpleIoc.Default.Register<IProtectData,ProtectDataAdapter>();
            SimpleIoc.Default.Register<IHttpClient, HttpClient>();
            SimpleIoc.Default.Register<IGeoCoordinateWatcher>(() => new GeoCoordinateWatcherAdapter(GeoPositionAccuracy.Default));

            
            SimpleIoc.Default.Register<ISettingsStore>(() => new SettingsStore(SimpleIoc.Default.GetInstance<IProtectData>()));
            SimpleIoc.Default.Register<ILocationService>(() => new LocationService(SimpleIoc.Default.GetInstance<ISettingsStore>(), SimpleIoc.Default.GetInstance<IGeoCoordinateWatcher>()));
            SimpleIoc.Default.Register<ITootFMServiceClient,TootFMServiceClient>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MapViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                var main = ServiceLocator.Current.GetInstance<MainViewModel>();
                return main;
            }
        }

        public LoginViewModel Login
        {
            get
            {
                var main = ServiceLocator.Current.GetInstance<LoginViewModel>();
                return main;
            }
        }

        public MapViewModel Map
        {
            get
            {
                var main = ServiceLocator.Current.GetInstance<MapViewModel>();
                return main;
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}