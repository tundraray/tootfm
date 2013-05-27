using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Phone.Reactive;
using Posmotrim.Phone.Adapters;

namespace Posmotrim.TootFM.PhoneServices.Services.Clients
{
    public class HttpClient : IHttpClient
    {
        public IHttpWebRequest GetRequest(IHttpWebRequest httpWebRequest, string authToken = null)
        {
            if(!string.IsNullOrEmpty(authToken))
                httpWebRequest.Headers["X-Tootfm-token"] = authToken;
            
            return httpWebRequest;
        }

      

        public IObservable<T> GetJson<T>(IHttpWebRequest httpWebRequest, string authToken = null)
        {
            var request = GetRequest(httpWebRequest, authToken);
            request.Method = "GET";
            request.Accept = "application/json";

            return
                Observable
                    .FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                    .Select(
                        response =>
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                var serializer = new DataContractJsonSerializer(typeof(T));
                                return (T)serializer.ReadObject(responseStream);
                            }
                        });
        }

        public IObservable<Unit> PostJson<T>(IHttpWebRequest httpWebRequest, T obj, string authToken = null)
        {
            var request = GetRequest(httpWebRequest, authToken);
            request.Method = "POST";
            request.ContentType = "application/json";

            return
                from requestStream in
                    Observable
                    .FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream)()
                from response in WriteContentToStream(requestStream, request, obj)
                select new Unit();
        }

        public IObservable<T> Post<T>(IHttpWebRequest httpWebRequest, IDictionary<string, string> param, string authToken = null)
        {
            var request = GetRequest(httpWebRequest, authToken);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            return
                (from requestStream in
                    Observable
                    .FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream)()
                from response in WritePostToStream(requestStream, request, param)
                     select response).Select(response =>
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                var serializer = new DataContractJsonSerializer(typeof(T));
                                return (T)serializer.ReadObject(responseStream);
                            }
                        });
               
        }

        private IObservable<WebResponse> WriteContentToStream<T>(Stream requestStream, IHttpWebRequest request, T obj)
        {
            using (requestStream)
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(requestStream, obj);
            }

            return
                Observable.FromAsyncPattern<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse)();
        }

        private IObservable<WebResponse> WritePostToStream(Stream requestStream, IHttpWebRequest request, IDictionary<string,string> param)
        {
            using (requestStream)
            {
                var postData = new StringBuilder();

                foreach (var item in param)
                {
                    postData.Append(string.Format("&{0}={1}", item.Key, HttpUtility.UrlEncode(item.Value)));
                }
                byte[] jsonAsBytes = Encoding.UTF8.GetBytes(postData.ToString());
                requestStream.Write(jsonAsBytes, 0, jsonAsBytes.Length);

            }

            return
                Observable.FromAsyncPattern<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse)();
        }
    }
}