using System;
using System.IO;
using System.Net;

namespace Posmotrim.Phone.Adapters
{
    public interface IHttpWebRequest
    {
        bool AllowReadStreamBuffering
        {
            get;
            set;
        }

        CookieContainer CookieContainer
        {
            get;
            set;
        }

        Uri RequestUri
        {
            get;
        }

        bool AllowAutoRedirect
        {
            get;
            set;
        }

        bool HaveResponse
        {
            get;
        }

        string Method
        {
            get;
            set;
        }

        ICredentials Credentials
        {
            get;
            set;
        }

        WebHeaderCollection Headers
        {
            get;
            set;
        }

        string ContentType
        {
            get;
            set;
        }

        string Accept
        {
            get;
            set;
        }

        string UserAgent
        {
            get;
            set;
        }

        bool SupportsCookieContainer
        {
            get;
        }

        bool UseDefaultCredentials
        {
            get;
            set;
        }

        IWebRequestCreate CreatorInstance
        {
            get;
        }

        void Abort();

        IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);

        Stream EndGetRequestStream(IAsyncResult asyncResult);

        IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

        WebResponse EndGetResponse(IAsyncResult asyncResult);

        bool RegisterPrefix(string prefix, IWebRequestCreate creator);

        HttpWebRequest CreateHttp(Uri requestUri);

        HttpWebRequest CreateHttp(string requestUriString);

        WebRequest Create(string requestUriString);

        WebRequest Create(Uri requestUri);
    }
}