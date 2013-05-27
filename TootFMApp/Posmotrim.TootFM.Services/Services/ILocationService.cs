using System.Device.Location;

namespace Posmotrim.TootFM.PhoneServices.Services
{
    public interface ILocationService
    {
        GeoCoordinate TryToGetCurrentLocation();
        void StartWatcher();
        void StopWatcher();
    }
}