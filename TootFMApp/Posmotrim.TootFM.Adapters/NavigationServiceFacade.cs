

namespace Posmotrim.TootFM.Adapters
{
    using System;
    using Microsoft.Phone.Controls;

    public class NavigationServiceFacade : INavigationServiceFacade
    {
        readonly PhoneApplicationFrame frame;

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back navigation history.
        /// </summary>
        public bool CanGoBack
        {
            get { return frame.CanGoBack; }
        }

        /// <summary>
        /// Gets the uniform resource identifier (URI) of the content that is currently displayed.
        /// </summary>
        public Uri CurrentSource
        {
            get { return frame.CurrentSource; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationServiceFacade"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">If frame is null.</exception>
        /// <param name="frame">The frame.</param>
        public NavigationServiceFacade(PhoneApplicationFrame frame)
        {
            if (frame == null) throw new ArgumentNullException("frame");
            this.frame = frame;
        }

        /// <summary>
        /// Navigates to the content specified by the uniform resource identifier (URI).
        /// </summary>
        /// <param name="source">The URI of the content to navigate to.</param>
        /// <returns>Returns bool. True if the navigation started successfully; otherwise, false.</returns>
        public bool Navigate(Uri source)
        {
            return frame.Navigate(source);
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or throws an exception if no entry exists in back navigation.
        /// </summary>
        public void GoBack()
        {
            frame.GoBack();
        }
    }
}