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
            _timer = new Timer(state => Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
                {
                    IsPlay = true;
                    CurrentPosition = (BackgroundAudioPlayer.Instance.Position).ToString("mm\\:ss");
                    TimeSlider = BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
                }
                else
                {
                    isPlay = false;
                    CurrentPositionSlider = 0;
                }
               
            }), null, new TimeSpan(0, 0, 0, 0), new TimeSpan(0, 0, 0, 1));
        }

        public RelayCommand PrevCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand ForwardCommand { get; set; }


        private string _trackName;
        public string TrackName
        {
            get
            {
                if (string.IsNullOrEmpty(_trackName) && BackgroundAudioPlayer.Instance.Track != null)
                {
                    _trackName = BackgroundAudioPlayer.Instance.Track.Title;
                }
                return _trackName;
            }

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
            get
            {
                if (string.IsNullOrEmpty(_artistName) && BackgroundAudioPlayer.Instance.Track != null)
                {
                    _artistName = BackgroundAudioPlayer.Instance.Track.Artist;
                }
                return _artistName;
            }

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

                    RaisePropertyChanged("CurrentPosition");
                }
            }
        }

        private Uri _picture;
        public Uri Picture
        {
            get
            {
                if (_picture == null && BackgroundAudioPlayer.Instance.Track != null)
                {
                    Picture = BackgroundAudioPlayer.Instance.Track.AlbumArt;
                }
                return _picture;
            }

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
           
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                ArtistName = BackgroundAudioPlayer.Instance.Track.Artist;
                TrackName = BackgroundAudioPlayer.Instance.Track.Title;
                Picture = BackgroundAudioPlayer.Instance.Track.AlbumArt;
                TimeSlider = BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
                Time = (BackgroundAudioPlayer.Instance.Track.Duration).ToString("mm\\:ss");

            }

        }
    }
}