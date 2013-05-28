using System;
using System.Device.Location;

namespace Posmotrim.Phone.Adapters
{
    public class GeoCoordinateWatcherAdapter : IGeoCoordinateWatcher
    {
        public GeoCoordinateWatcherAdapter()
        {
            WrappedSubject = new GeoCoordinateWatcher();
            AttachToEvents();
        }

        public GeoCoordinateWatcherAdapter(GeoPositionAccuracy desiredAccuracy)
        {
            WrappedSubject = new GeoCoordinateWatcher(desiredAccuracy);
            AttachToEvents();
        }

        private GeoCoordinateWatcher WrappedSubject { get; set; }

        public void Start()
        {
            WrappedSubject.Start();
        }

        public void Start(bool suppressPermissionPrompt)
        {
            WrappedSubject.Start(suppressPermissionPrompt);
        }

        public bool TryStart(bool suppressPermissionPrompt, TimeSpan timeout)
        {
            return WrappedSubject.TryStart(suppressPermissionPrompt, timeout);
        }

        public void Stop()
        {
            WrappedSubject.Stop();
        }

        public void Dispose()
        {
            WrappedSubject.Dispose();
        }

        public GeoPositionAccuracy DesiredAccuracy
        {
            get { return WrappedSubject.DesiredAccuracy; }
        }

        public double MovementThreshold
        {
            get { return WrappedSubject.MovementThreshold; }
            set { WrappedSubject.MovementThreshold = value; }
        }

        public GeoPosition<GeoCoordinate> Position
        {
            get { return WrappedSubject.Position; }
        }

        public GeoPositionStatus Status
        {
            get { return WrappedSubject.Status; }
        }

        public GeoPositionPermission Permission
        {
            get { return WrappedSubject.Permission; }
        }

        public event EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> PositionChanged;
        public event EventHandler<GeoPositionStatusChangedEventArgs> StatusChanged;

        private void AttachToEvents()
        {
            
            WrappedSubject.PositionChanged += WrappedSubjectPositionChanged;
            WrappedSubject.StatusChanged += WrappedSubjectStatusChanged;
        }

        void WrappedSubjectPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            PositionChangedRelay(sender, e);
        }

        private void PositionChangedRelay(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var handler = PositionChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        void WrappedSubjectStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            StatusChangedRelay(sender, e);
        }

        private void StatusChangedRelay(object sender, GeoPositionStatusChangedEventArgs e)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}