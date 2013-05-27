using System;
using System.Collections.Generic;
using Microsoft.Phone.Reactive;
using Posmotrim.Phone.Adapters;

namespace Posmotrim.TootFM.PhoneServices.Services.Clients
{
    public interface IHttpClient
    {
        IHttpWebRequest GetRequest(IHttpWebRequest httpWebRequest, string authToken = null);
        IObservable<T> GetJson<T>(IHttpWebRequest httpWebRequest, string authToken = null);
        IObservable<Unit> PostJson<T>(IHttpWebRequest httpWebRequest, T obj, string authToken = null);

        IObservable<T> Post<T>(IHttpWebRequest httpWebRequest, IDictionary<string, string> param,
                               string authToken = null);
    }
}