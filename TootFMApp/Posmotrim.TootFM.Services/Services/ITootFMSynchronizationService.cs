using System;
using System.Device.Location;

namespace Posmotrim.TootFM.PhoneServices.Services
{
    public interface ITootFMSynchronizationService
    {
        IObservable<TaskCompletedSummary> GetFoursqareAuth();
        IObservable<TaskCompletedSummary> GetLastCheckin();
        IObservable<TaskCompletedSummary> GetCurrentPlaylist(int venueId);
        IObservable<TaskCompletedSummary> GetGeneralPlaylist(int venueId);
        IObservable<TaskCompletedSummary> ListVenues(string search = null,GeoCoordinate geo = null);
        IObservable<TaskCompletedSummary> ListUserVenues();
    }
}