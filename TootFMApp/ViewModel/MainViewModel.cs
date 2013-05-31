using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Reactive;
using Posmotrim.TootFM.Adapters;
using Posmotrim.TootFM.App.Views;
using Posmotrim.TootFM.PhoneServices.Models;
using Posmotrim.TootFM.PhoneServices.Services;
using Posmotrim.TootFM.PhoneServices.Services.Clients;
using Posmotrim.TootFM.PhoneServices.Services.Stores;
using Posmotrim.TootFM.PhoneServices.Services.TootFMService;
using TootFM.Agent;

namespace Posmotrim.TootFM.App.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        #region Fields



        private ISettingsStore _settingsStore;
        private readonly ILocationService _locationService;
        private bool isSyncing = false;
        private Func<ITootFMServiceClient> _serviceClient;
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ISettingsStore settingsStore, ILocationService locationService, ITootFMServiceClient serviceClient)
        {
            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;
            StartSyncCommand = new RelayCommand(StartSync, () => !this.IsSynchronizing && !this.SettingAreNotConfigured);
          
            StopCommand = new RelayCommand(Stop);
            PlayCommand = new RelayCommand<ObsTrack>(Play);
            MapCommand = new RelayCommand(Map);
            Messenger.Default.Register<int>(this, "Play", SyncPlay);
            Messenger.Default.Register<Venue>(this, "ChangeLocation", BindingCheckin);
            _serviceClient = () => serviceClient;
            _settingsStore = settingsStore;
            ExitCommand = new RelayCommand(() => _settingsStore.CurrentSessionToken = null );
            this._settingsStore.UserChanged += SettingsStoreUserChanged;

            if (SettingAreConfigured)
            {
                Refresh();
            }
            else
            {
                IsLoginPopupOpen = true;
            }

        }




        private void SettingsStoreUserChanged(object sender, EventArgs e)
        {
            IsLoginPopupOpen = !SettingAreConfigured;
            if (SettingAreConfigured)
                Refresh();
        }

        #region Properties

        public bool IsSynchronizing
        {
            get { return this.isSyncing; }
            set
            {
                this.isSyncing = value;
                this.RaisePropertyChanged(() => this.IsSynchronizing);
                this.StartSyncCommand.RaiseCanExecuteChanged();

            }
        }

        private bool _noCurrentTracksVisibility = false;
        public bool NoCurrentTracksVisibility
        {
            get
            {
                return _noCurrentTracksVisibility;
            }
            set
            {
                if (value != _noCurrentTracksVisibility)
                {
                    _noCurrentTracksVisibility = value;
                    RaisePropertyChanged("NoCurrentTracksVisibility");
                }
            }
        }

        private bool _noGeneralTracksVisibility = false;
        public bool NoGeneralTracksVisibility
        {
            get
            {
                return _noGeneralTracksVisibility;
            }
            set
            {
                if (value != _noGeneralTracksVisibility)
                {
                    _noGeneralTracksVisibility = value;
                    RaisePropertyChanged("NoGeneralTracksVisibility");
                }
            }
        }

        public bool SettingAreConfigured
        {
            get { return !SettingAreNotConfigured; }
        }

        public bool SettingAreNotConfigured
        {
            get { return string.IsNullOrEmpty(_settingsStore.CurrentSessionToken); }
        }

      
        public bool IsBarVisible
        {
            get { return !_isLoginPopupOpen; }

            set
            {
                if (_isLoginPopupOpen != value)
                {
                    _isLoginPopupOpen = !value;

                    RaisePropertyChanged("IsBarVisible");
                    RaisePropertyChanged("IsLoginPopupOpen");
                }
            }
        }
        private bool _isLoginPopupOpen = false;
        public bool IsLoginPopupOpen
        {
            get { return _isLoginPopupOpen; }

            set
            {
                if (_isLoginPopupOpen != value)
                {
                    _isLoginPopupOpen = value;
                    RaisePropertyChanged("IsBarVisible");
                    RaisePropertyChanged("IsLoginPopupOpen");
                }
            }
        }
        private string _currentLocationName;
        public string CurrentLocationName
        {
            get { return _currentLocationName; }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _currentLocationName = value;

                    RaisePropertyChanged("CurrentLocationName");
                }
            }
        }

        private IEnumerable<ObsTrack> _generalTracks;
        public IEnumerable<ObsTrack> GeneralTracks
        {
            get { return _generalTracks; }
            set
            {
                _generalTracks = value;
                RaisePropertyChanged("GeneralTracks");
            }
        }

        private IEnumerable<ObsTrack> _currentTracks;
        public IEnumerable<ObsTrack> CurrentTracks
        {
            get { return _currentTracks; }
            set
            {
                _currentTracks = value;
                RaisePropertyChanged("CurrentTracks");
            }
        }

        private string _currentLocationDescription;
        public string CurrentLocationDescription
        {
            get { return _currentLocationDescription; }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _currentLocationDescription = value;

                    RaisePropertyChanged("CurrentLocationDescription");
                }
            }
        }

        #endregion

        #region Messenger Events


        #endregion

        #region Navigation Events

        public void NavigatedFrom(IDictionary<string, object> pageState)
        {

        }

        public void NavigatedTo(bool isBeingActived, IDictionary<string, object> state)
        {

        }


        #endregion

        #region BackgroundWorkers



        #endregion

        #region Method

        public void Refresh()
        {
            this.StartSyncCommand.RaiseCanExecuteChanged();
            this.StartSyncCommand.Execute(null);
        }

        public void StartSync()
        {
            if (this.IsSynchronizing)
            {
                return;
            }
            var checkin = _serviceClient().GetLastCheckin().Catch(
                (Exception exception) =>
                {
                    if (exception is WebException)
                    {
                        var webException = exception as WebException;
                        //var summary = ExceptionHandling.GetSummaryFromWebException(GetSurveysTask, webException);
                        return Observable.Return<Venue>(null);
                    }

                    if (exception is UnauthorizedAccessException)
                    {
                        return Observable.Return<Venue>(null);
                    }

                    throw exception;
                }).ObserveOnDispatcher().Subscribe(BindingCheckin);



        }



        private void BindingCheckin(Venue checkin)
        {
            if (checkin != null)
            {
                CurrentLocationName = checkin.Name;
                if (checkin.Location != null)
                    CurrentLocationDescription = checkin.Location.Address;

                Messenger.Default.Send(checkin.Id, "Play");
            }
        }

        private void SyncPlay(int obj)
        {
            _serviceClient().GetCurrentPlaylist(obj).Catch(
              (Exception exception) =>
              {
                  if (exception is WebException)
                  {
                      var webException = exception as WebException;
                      //var summary = ExceptionHandling.GetSummaryFromWebException(GetSurveysTask, webException);
                      return Observable.Return(new List<Track>());
                  }

                  if (exception is UnauthorizedAccessException)
                  {
                      return Observable.Return(new List<Track>());
                  }

                  throw exception;
              }).ObserveOnDispatcher().Subscribe(BindingCurrentTrack);

            _serviceClient().GetGeneralPlaylist(obj).Catch(
              (Exception exception) =>
              {
                  if (exception is WebException)
                  {
                      var webException = exception as WebException;
                      //var summary = ExceptionHandling.GetSummaryFromWebException(GetSurveysTask, webException);
                      return Observable.Return(new List<Track>());
                  }

                  if (exception is UnauthorizedAccessException)
                  {
                      return Observable.Return(new List<Track>());
                  }

                  throw exception;
              }).ObserveOnDispatcher().Subscribe(BindingGeneralTrack);

        }

        private void BindingCurrentTrack(IEnumerable<Track> checkin)
        {

            CurrentTracks = checkin.Select(s=>new ObsTrack { IsPlay = false,Track = s}).ToList();
            NoCurrentTracksVisibility = !CurrentTracks.Any();
        }

        private void BindingGeneralTrack(IEnumerable<Track> checkin)
        {
            GeneralTracks = checkin.Select(s => new ObsTrack { IsPlay = false, Track = s }).ToList();
            NoGeneralTracksVisibility = !GeneralTracks.Any();
        }

        private void Play(ObsTrack obj)
        {
            //List<AudioTrack> list = new List<AudioTrack>();
            //if (PivotIndex == 0)
            //{
            //    list =
            //        CurrentTracks.Select(
            //            t => new AudioTrack(new Uri(obj.Track.Audio.Preview), obj.Track.Name, obj.Track.Artist.Name, obj.Track.Album, null)).ToList();
            //   AudioPlayer.currentTrackNumber= CurrentTracks.IndexOf(obj);
            //}
            //else
            //{
            //    list =
            //        GeneralTracks.Select(
            //            t => new AudioTrack(new Uri(obj.Track.Audio.Preview), obj.Track.Name, obj.Track.Artist.Name, obj.Track.Album, null)).ToList();
            //    AudioPlayer.currentTrackNumber = GeneralTracks.IndexOf(obj);
            //}
            var i = 0;
            var list = new List<Track>();
            if (PivotIndex == 0)
            {
                list =
                    CurrentTracks.Select(
                        t => t.Track).ToList();
                i = CurrentTracks.IndexOf(obj);
            }
            else
            {
                list =
                    GeneralTracks.Select(
                        t => t.Track).ToList();
                i = GeneralTracks.IndexOf(obj);
            }
            using (var ifs = new IsolatedStorageFileStream("playlist.xml", FileMode.Create,IsolatedStorageFile.GetUserStoreForApplication()))
            {
                var ser = new DataContractJsonSerializer(list.GetType());
               
                ser.WriteObject(ifs, list);
            }
            using (var ifs = new IsolatedStorageFileStream("track.xml", FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication()))
            {
                var ser = new DataContractJsonSerializer(i.GetType());

                ser.WriteObject(ifs, i);
            }


            BackgroundAudioPlayer.Instance.Play();
            //BackgroundAudioPlayer.Instance.Track = new AudioTrack(new Uri(obj.Track.Audio.Preview), obj.Track.Name, obj.Track.Artist.Name, obj.Track.Album, null);
            Messenger.Default.Send(new Uri("/Views/PlayerView.xaml", UriKind.Relative), "NavigationRequest");
            
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                var track = BackgroundAudioPlayer.Instance.Track;
                if (PivotIndex == 0)
                {
                    var _a = CurrentTracks.FirstOrDefault(a => a.Track.Audio.Preview.Contains(track.Source.ToString()));
                    foreach (var obsTrack in CurrentTracks)
                    {
                        obsTrack.IsPlay = false;
                        if (_a != null && obsTrack.Track.Id == _a.Track.Id && BackgroundAudioPlayer.Instance.PlayerState != PlayState.Paused)
                            obsTrack.IsPlay = true;
                    }
                }
                else
                {
                    var _a = GeneralTracks.FirstOrDefault(a => a.Track.Audio.Preview.Contains(track.Source.ToString()));
                    foreach (var obsTrack in GeneralTracks)
                    {
                        obsTrack.IsPlay = false;
                        if (_a != null && obsTrack.Track.Id == _a.Track.Id && BackgroundAudioPlayer.Instance.PlayerState != PlayState.Paused)
                            obsTrack.IsPlay = true;
                    }
                }
                
            }
        }


        private void Stop()
        {
            BackgroundAudioPlayer.Instance.Pause();
            foreach (var obsTrack in CurrentTracks)
            {
                obsTrack.IsPlay = false;
                
            }
            foreach (var obsTrack in GeneralTracks)
            {
                obsTrack.IsPlay = false;
                
            }
        }

        private void Map()
        {
            Messenger.Default.Send(new Uri("/Views/MapView.xaml", UriKind.Relative), "NavigationRequest");
        }

        #endregion

        public RelayCommand StartSyncCommand { get; set; }

        public RelayCommand<ObsTrack> PlayCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }

        public RelayCommand MapCommand { get; set; }

        private int _pivotIndex = 0;
        public int PivotIndex
        {
            get { return _pivotIndex; }
            set
            {
                _pivotIndex = value;
                RaisePropertyChanged("PivotIndex");
            }
        }
    }

    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> obj, T value)
        {
            return obj
                .Select((a, i) => (a.Equals(value)) ? i : -1)
                .Max();
        }

        public static int IndexOf<T>(this IEnumerable<T> obj, T value
               , IEqualityComparer<T> comparer)
        {
            return obj
                .Select((a, i) => (comparer.Equals(a, value)) ? i : -1)
                .Max();
        }
    }
}