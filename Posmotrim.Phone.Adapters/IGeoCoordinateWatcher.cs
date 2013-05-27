using System;
using System.Device.Location;

namespace Posmotrim.Phone.Adapters
{
    public interface IGeoCoordinateWatcher
    {
        void Start();
        void Start(bool suppressPermissionPrompt);
        bool TryStart(bool suppressPermissionPrompt, TimeSpan timeout);
        void Stop();
        GeoPositionAccuracy DesiredAccuracy { get; }
        double MovementThreshold { get; set; }
        GeoPosition<GeoCoordinate> Position { get; }
        GeoPositionStatus Status { get; }
        GeoPositionPermission Permission { get; }
        event EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> PositionChanged;
        event EventHandler<GeoPositionStatusChangedEventArgs> StatusChanged;
    }
}