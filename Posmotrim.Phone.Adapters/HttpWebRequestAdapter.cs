using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Posmotrim.Phone.Adapters
{
    public class HttpWebRequestAdapter : IHttpWebRequest
    {
        public HttpWebRequestAdapter(Uri uri)
        {
            WrappedSubject = WebRequest.CreateHttp(uri);
        }

        public HttpWebRequestAdapter(string uriString)
        {
            WrappedSubject = WebRequest.CreateHttp(uriString);
        }

        private HttpWebRequest WrappedSubject { get; set; }

        public bool AllowReadStreamBuffering
        {
            get
            {
                return WrappedSubject.AllowReadStreamBuffering;
            }
            set
            {
                WrappedSubject.AllowReadStreamBuffering = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get
            {
                return WrappedSubject.CookieContainer;
            }
            set
            {
                WrappedSubject.CookieContainer = value;
            }
        }

        public Uri RequestUri
        {
            get
            {
                return WrappedSubject.RequestUri;
            }
        }

        public bool AllowAutoRedirect
        {
            get
            {
                return WrappedSubject.AllowAutoRedirect;
            }
            set
            {
                WrappedSubject.AllowAutoRedirect = value;
            }
        }

        public bool HaveResponse
        {
            get
            {
                return WrappedSubject.HaveResponse;
            }
        }

        public string Method
        {
            get
            {
                return WrappedSubject.Method;
            }
            set
            {
                WrappedSubject.Method = value;
            }
        }

        public ICredentials Credentials
        {
            get
            {
                return WrappedSubject.Credentials;
            }
            set
            {
                WrappedSubject.Credentials = value;
            }
        }

        public WebHeaderCollection Headers
        {
            get
            {
                return WrappedSubject.Headers;
            }
            set
            {
                WrappedSubject.Headers = value;
            }
        }

        public string ContentType
        {
            get
            {
                return WrappedSubject.ContentType;
            }
            set
            {
                WrappedSubject.ContentType = value;
            }
        }

        public string Accept
        {
            get
            {
                return WrappedSubject.Accept;
            }
            set
            {
                WrappedSubject.Accept = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return WrappedSubject.UserAgent;
            }
            set
            {
                WrappedSubject.UserAgent = value;
            }
        }

        public bool SupportsCookieContainer
        {
            get
            {
                return WrappedSubject.SupportsCookieContainer;
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                return WrappedSubject.UseDefaultCredentials;
            }
            set
            {
                WrappedSubject.UseDefaultCredentials = value;
            }
        }

        public IWebRequestCreate CreatorInstance
        {
            get
            {
                return WrappedSubject.CreatorInstance;
            }
        }

        public void Abort()
        {
            WrappedSubject.Abort();
        }

        public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return WrappedSubject.BeginGetRequestStream(callback, state);
        }

        public virtual Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return WrappedSubject.EndGetRequestStream(asyncResult);
        }

        public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return WrappedSubject.BeginGetResponse(callback, state);
        }

        public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return WrappedSubject.EndGetResponse(asyncResult);
        }

        public virtual bool RegisterPrefix(string prefix, IWebRequestCreate creator)
        {
            return WebRequest.RegisterPrefix(prefix, creator);
        }

        public virtual HttpWebRequest CreateHttp(Uri requestUri)
        {
            return WebRequest.CreateHttp(requestUri);
        }

        public virtual HttpWebRequest CreateHttp(string requestUriString)
        {
            return WebRequest.CreateHttp(requestUriString);
        }

        public virtual WebRequest Create(string requestUriString)
        {
            return WebRequest.Create(requestUriString);
        }

        public virtual WebRequest Create(Uri requestUri)
        {
            return WebRequest.Create(requestUri);
        }
    }
}
