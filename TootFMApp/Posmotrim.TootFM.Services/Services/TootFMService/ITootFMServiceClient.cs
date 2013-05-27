using System;
using System.Collections.Generic;
using System.Device.Location;
using Posmotrim.TootFM.PhoneServices.Models;

namespace Posmotrim.TootFM.PhoneServices.Services.TootFMService
{
    public interface ITootFMServiceClient
    {
        IObservable<User> GetFoursqareAuth(string foursqareToken);
        IObservable<Venue> GetLastCheckin();
        IObservable<IEnumerable<Track>> GetCurrentPlaylist(int venueId);
        IObservable<IEnumerable<Track>> GetGeneralPlaylist(int venueId);
        IObservable<IEnumerable<Venue>> ListVenues(string search = null, GeoCoordinate geo = null);
        IObservable<IEnumerable<Venue>> ListUserVenues(); 
    }
}