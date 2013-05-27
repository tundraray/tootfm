using System;

namespace Posmotrim.TootFM.PhoneServices.Services.Stores
{
    public interface ISettingsStore
    {
        string DeezerToken { get; set; }
        string FoursquareToken { get; set; }
        string CurrentSessionToken { get; set; }
        bool SubscribeToPushNotifications { get; set; }
        bool LocationServiceAllowed { get; set; }
        bool BackgroundTasksAllowed { get; set; }
        event EventHandler UserChanged;
    }
}