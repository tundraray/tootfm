using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Reactive;
using Posmotrim.TootFM.Adapters;
using Posmotrim.TootFM.PhoneServices.Models;
using Posmotrim.TootFM.PhoneServices.Services.Stores;
using Posmotrim.TootFM.PhoneServices.Services.TootFMService;
using Microsoft.Phone.Controls;

namespace Posmotrim.TootFM.App.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private ISettingsStore _settingsStore;
        private Func<ITootFMServiceClient> _serviceClient;
        public LoginViewModel(ISettingsStore settingsStore, ITootFMServiceClient serviceClient)
        {
            _serviceClient = () => serviceClient;
            _settingsStore = settingsStore;
            ClickCommand = new RelayCommand(() =>
                                                {
                                                    BrowserVisibility = true;

                                                    UrlPage =
                                                        string.Format(
                                                            "https://foursquare.com/oauth2/authenticate?client_id={0}&response_type=token&redirect_uri={1}",
                                                            AppSettings.FoursquareClientId,
                                                            HttpUtility.UrlEncode(AppSettings.FoursquareRedirectUrl));
                                                });
        }

        private string _urlPage;
        public string UrlPage
        {
            get { return _urlPage; }

            set
            {
                if (_urlPage != value)
                {
                    _urlPage = value;
                    GetTokenUrl(value);
                    RaisePropertyChanged("UrlPage");
                }
            }
        }

        private void GetTokenUrl(string value)
        {
            if (value.StartsWith(string.Format("{0}#", AppSettings.FoursquareRedirectUrl)))
            {
                var _fragment = HttpUtility.HtmlDecode(value).Replace(AppSettings.FoursquareRedirectUrl, "");
                var _queryString = _fragment.TrimStart('#').Split('&')
                    .ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);

                if (_queryString.Where(x => x.Key == "error").Any())
                {
                    var error = _queryString.Where(x => x.Key == "error").First().Value;
                    MessageBox.Show(error, "Error", MessageBoxButton.OK);
                    return;
                }
                
                var token = _queryString.Where(x => x.Key == "access_token").First().Value;
                _serviceClient().GetFoursqareAuth(HttpUtility.UrlDecode(token)).Catch(
                    (Exception exception) =>
                        {
                            if (exception is WebException)
                            {
                                var webException = exception as WebException;
                                //var summary = ExceptionHandling.GetSummaryFromWebException(GetSurveysTask, webException);
                                return Observable.Return(new User());
                            }

                            if (exception is UnauthorizedAccessException)
                            {
                                return Observable.Return(new User());
                            }

                            throw exception;
                        }).ObserveOnDispatcher().Subscribe(user =>
                                                               {
                                                                   _settingsStore.CurrentSessionToken = user.Token;
                                                                   _settingsStore.FoursquareToken = user.FoursquareToken;
                                                               });
                

                
               
                BrowserVisibility = false;
            }
        }

        private bool _browserVisibility = false;
        public bool BrowserVisibility
        {
            get { return _browserVisibility; }

            set
            {
                if (_browserVisibility != value)
                {
                    _browserVisibility = value;
                    RaisePropertyChanged("BrowserVisibility");
                    RaisePropertyChanged("ButtonVisibility");
                }
            }
        }

        public bool ButtonVisibility { get { return !_browserVisibility; } }



        public RelayCommand ClickCommand
        {
            get;
            private set;
        }
    }
}