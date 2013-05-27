namespace Posmotrim.TootFM.Adapters
{
    using System;

    public interface INavigationServiceFacade
    {

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back navigation history.
        /// </summary>
        /// <remarks>Returns bool. True if there is at least one entry in the back navigation history; otherwise, false.</remarks>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets the uniform resource identifier (URI) of the content that is currently displayed.
        /// </summary>
        /// <remarks>Returns System.Uri. A value that represents the URI of content that is currently displayed.</remarks>
        Uri CurrentSource { get; }

        /// <summary>
        /// Navigates to the content specified by the uniform resource identifier (URI).
        /// </summary>
        /// <param name="source">The URI of the content to navigate to.</param>
        /// <returns>Returns bool. True if the navigation started successfully; otherwise, false.</returns>
        bool Navigate(Uri source);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or throws an exception if no entry exists in back navigation.
        /// </summary>
        void GoBack();
    }
}