using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Phone.Reactive;
using Posmotrim.Phone.Adapters;
using Posmotrim.TootFM.PhoneServices.Models;
using Posmotrim.TootFM.PhoneServices.Services.Clients;
using Posmotrim.TootFM.PhoneServices.Services.Stores;

namespace Posmotrim.TootFM.PhoneServices.Services.TootFMService
{
    public class TootFMServiceClient : ITootFMServiceClient
    {
        private  Uri deezerBaseUrl = new Uri("http://api.deezer.com/2.0/");
        private readonly Uri serviceUri;
        private readonly ISettingsStore settingsStore;
        private readonly IHttpClient httpClient;

        public TootFMServiceClient(Uri serviceUri, ISettingsStore settingsStore, IHttpClient httpClient)
        {
            this.serviceUri = serviceUri;
            this.settingsStore = settingsStore;
            this.httpClient = httpClient;
        }
        public IObservable<User> GetFoursqareAuth(string foursqareToken)
        {
            var surveysPath = string.Format(CultureInfo.InvariantCulture, "users/foursquare_auth");
            var uri = new Uri(serviceUri, surveysPath);
            var param = new Dictionary<string, string>();
            param.Add("token",foursqareToken);
            return
                httpClient
                    .Post<Account>(new HttpWebRequestAdapter(uri),param)
                    .Select(User);
        }

        private User User(Account arg)
        {
            if (arg == null && arg.User == null)
                return null;
            return arg.User;
        }

        public IObservable<Venue> GetLastCheckin()
        {
            var surveysPath = string.Format(CultureInfo.InvariantCulture, "users/last_checkin.json");
            var uri = new Uri(serviceUri, surveysPath);

            return
                httpClient
                    .GetJson<CheckInW>(new HttpWebRequestAdapter(uri), settingsStore.CurrentSessionToken)
                    .Select(Check);
        }

        private Venue Check(CheckInW arg)
        {
            if (arg == null || arg.CheckIn == null)
                return null;
            return arg.CheckIn.Venue;
        }

        public IObservable<IEnumerable<Track>> GetCurrentPlaylist(int venueId)
        {
            var surveysPath = string.Format(CultureInfo.InvariantCulture, "venues/{0}/current_playlist.json", venueId);
            var uri = new Uri(serviceUri, surveysPath);

            return
                httpClient
                    .GetJson<Playlist>(new HttpWebRequestAdapter(uri))
                    .Select(TemplateTracks);
        }

        public IObservable<DeezerTrack> GetTrackDeezer(int id)
        {
            var uri = new Uri(deezerBaseUrl, string.Format("track/{0}", id));
            return
                httpClient
                    .GetJson<DeezerTrack>(new HttpWebRequestAdapter(uri));

        }



        public IObservable<IEnumerable<Track>> GetGeneralPlaylist(int venueId)
        {
            var surveysPath = string.Format(CultureInfo.InvariantCulture, "venues/{0}/general_playlist.json", venueId);
            var uri = new Uri(serviceUri, surveysPath);

            return
                httpClient
                    .GetJson<Playlist>(new HttpWebRequestAdapter(uri))
                    .Select(TemplateTracks);
        }

        private IEnumerable<Track> TemplateTracks(Playlist arg)
        {
            if (arg != null)
            {
                if (arg.Current != null && arg.Current.Any())
                {
                    foreach (var track in arg.Current)
                    {
                        track.Audio = this.GetTrackDeezer(track.DeezerId).ObserveOnDispatcher().FirstOrDefault();
                        
                    }
                    return arg.Current.Where(t => !string.IsNullOrEmpty(t.Audio.Preview)).ToList();
                }

                if (arg.General != null && arg.General.Any())
                {
                    foreach (var track in arg.General)
                    {
                        track.Audio = this.GetTrackDeezer(track.DeezerId).ObserveOnDispatcher().FirstOrDefault();
                    }
                   
                    return arg.General.Where(t=>!string.IsNullOrEmpty(t.Audio.Preview)).ToList();
                }

            }
            return new List<Track>();
        }

        public IObservable<IEnumerable<Venue>> ListVenues(string search = null, GeoCoordinate geo = null)
        {
            Uri uri = new Uri(serviceUri, "venues.json");
            if (!string.IsNullOrEmpty(search))
            {
                var surveysPath = string.Format(CultureInfo.InvariantCulture, "venues.json?q={0}", search);
                uri = new Uri(serviceUri, surveysPath);

                return
                httpClient
                    .GetJson<Venues>(new HttpWebRequestAdapter(uri)).Select(s => s == null ? null : s.Items).Select(MergeVenue);
            }
            if (geo != null)
            {
                var surveysPath = string.Format(CultureInfo.InvariantCulture, "venues/nearby.json?lat={0}&lng={1}", geo.Latitude, geo.Longitude);
                uri = new Uri(serviceUri, surveysPath);
                
                return
                httpClient
                    .GetJson<IEnumerable<Venue>>(new HttpWebRequestAdapter(uri)).Select(MergeVenue);
            }


            return Observable.Return(new List<Venue>());
        }

        private IEnumerable<Venue> MergeVenue(IEnumerable<Venue> arg)
        {
            var userCheckin = ListUserVenues().ObserveOnDispatcher().FirstOrDefault();
            foreach (var venue in arg)
            {
                venue.IsUser = userCheckin.Any(u => u.Id == venue.Id);
            }
            return arg;
        }

        public IObservable<IEnumerable<Venue>> ListUserVenues()
        {
            var surveysPath = string.Format(CultureInfo.InvariantCulture, "users/venues.json");
            var uri = new Uri(serviceUri, surveysPath);

            return
                httpClient
                    .GetJson<Venues>(new HttpWebRequestAdapter(uri), settingsStore.CurrentSessionToken)
                    .Select(s => s == null ? null : s.Items);
        }
    }
}