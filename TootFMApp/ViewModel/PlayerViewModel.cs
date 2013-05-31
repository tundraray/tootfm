using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.BackgroundAudio;
using Posmotrim.TootFM.PhoneServices.Models;
using Posmotrim.TootFM.PhoneServices.Services;
using Posmotrim.TootFM.PhoneServices.Services.Stores;
using Posmotrim.TootFM.PhoneServices.Services.TootFMService;

namespace Posmotrim.TootFM.App.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private ISettingsStore _settingsStore;
        private Func<ITootFMServiceClient> _serviceClient;
        private Timer _timer;
        public PlayerViewModel(ISettingsStore settingsStore,
                             ITootFMServiceClient serviceClient)
        {
            StopCommand = new RelayCommand( () =>BackgroundAudioPlayer.Instance.Pause());
            PlayCommand = new RelayCommand(() => BackgroundAudioPlayer.Instance.Play());
            PrevCommand = new RelayCommand(() => BackgroundAudioPlayer.Instance.SkipPrevious());
            ForwardCommand = new RelayCommand(() => BackgroundAudioPlayer.Instance.SkipNext());
            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;
            _serviceClient = () => serviceClient;
            _settingsStore = settingsStore;
        }

        public RelayCommand PrevCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand ForwardCommand { get; set; }


        private string _trackName;
        public string TrackName
        {
            get { return _trackName; }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _trackName = value;

                    RaisePropertyChanged("TrackName");
                }
            }
        }

        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _artistName = value;

                    RaisePropertyChanged("ArtistName");
                }
            }
        }

        private string _time;
        public string Time
        {
            get { return _time; }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _time = value;

                    RaisePropertyChanged("Time");
                }
            }
        }

        private bool isPlay = false;
        public bool IsPlay
        {
            get { return isPlay; }
            set
            {
                isPlay = value;
                RaisePropertyChanged("IsPlay");
            }
        }

        private double _timeSlider;
        public double TimeSlider
        {
            get { return _timeSlider; }

            set
            {

                _timeSlider = value;
                RaisePropertyChanged("TimeSlider");

            }
        }

        private double _currentPositionSlider;
        public double CurrentPositionSlider
        {
            get { return _currentPositionSlider; }

            set
            {

                _currentPositionSlider = value;
                RaisePropertyChanged("CurrentPositionSlider");

            }
        }

        private string _currentPosition;
        public string CurrentPosition
        {
            get { return _currentPosition; }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _currentPosition = value;

                    RaisePropertyChanged("");
                }
            }
        }

        private Uri _picture;
        public Uri Picture
        {
            get { return _picture; }

            set
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    _picture = value;

                    RaisePropertyChanged("Picture");
                }
            }
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            IsPlay = false;
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
            {
                IsPlay = true;
                if (_timer != null)
                    _timer.Dispose();
                _timer = null;
                _timer = new Timer(state => Deployment.Current.Dispatcher.BeginInvoke(delegate
                                                                                          {
                                                                                              this.CurrentPositionSlider = (int)BackgroundAudioPlayer.Instance.Position.TotalSeconds;
                                                                                              CurrentPosition = (BackgroundAudioPlayer.Instance.Position).ToString("mm\\:ss");
                                                                                          }), null, new TimeSpan(0, 0, 0, 0), new TimeSpan(0, 0, 0, 1));
            }
            else if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Paused)
            {
               
                if (_timer != null)
                    _timer.Dispose();
                _timer = null;
            }
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                ArtistName = BackgroundAudioPlayer.Instance.Track.Artist;
                TrackName = BackgroundAudioPlayer.Instance.Track.Title;
                Picture = BackgroundAudioPlayer.Instance.Track.AlbumArt;
                this.TimeSlider = BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
                Time = (BackgroundAudioPlayer.Instance.Track.Duration).ToString("mm\\:ss");

            }

        }
    }
}