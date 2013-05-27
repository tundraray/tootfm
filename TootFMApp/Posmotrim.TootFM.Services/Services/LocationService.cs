using System;
using System.Device.Location;
using Posmotrim.Phone.Adapters;
using Posmotrim.TootFM.PhoneServices.Services.Stores;

namespace Posmotrim.TootFM.PhoneServices.Services
{
    public class LocationService : ILocationService
    {
        private readonly TimeSpan _maximumAge = TimeSpan.FromMinutes(15);
        private readonly ISettingsStore _settingsStore;
        private GeoCoordinate _lastCoordinate = GeoCoordinate.Unknown;
        private DateTime _lastCoordinateTime;
        private readonly IGeoCoordinateWatcher _geoCoordinateWatcher;

        public LocationService(ISettingsStore settingsStore, IGeoCoordinateWatcher geoCoordinateWatcher)
        {
            this._settingsStore = settingsStore;
            this._geoCoordinateWatcher = geoCoordinateWatcher;
        }

        public GeoCoordinate TryToGetCurrentLocation()
        {
            if (!_settingsStore.LocationServiceAllowed)
            {
                return GeoCoordinate.Unknown;
            }

            if (_geoCoordinateWatcher.Status == GeoPositionStatus.Ready)
            {
                _lastCoordinate = _geoCoordinateWatcher.Position.Location;
                _lastCoordinateTime = _geoCoordinateWatcher.Position.Timestamp.DateTime;
                return _lastCoordinate;
            }

            if (_maximumAge < (DateTime.Now - _lastCoordinateTime))
            {
                return GeoCoordinate.Unknown;
            }
            else
            {
                return _lastCoordinate;
            }
        }


        public void StartWatcher()
        {
            this._geoCoordinateWatcher.Start();
        }

        public void StopWatcher()
        {
            this._geoCoordinateWatcher.Stop();
        }
    }
}