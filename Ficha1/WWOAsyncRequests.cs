using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Ficha1
{
    class WWOAsyncRequests
    {
        private int size;
        private Dictionary<RestRequestAsyncHandle, RestResponse<Data>> _requestDict = new Dictionary<RestRequestAsyncHandle, RestResponse<Data>>();
        public Dictionary<RestRequestAsyncHandle, RestResponse<Data>> RequestDict
        {
            get{return _requestDict;}
        }

        private static object _sync = new object();

        public WWOAsyncRequests(int size)
        {
            this.size = size;
            //_requestDict = new Dictionary<RestRequestAsyncHandle, RestResponse<Data>>();
        }

        public void AddRequest(RestRequestAsyncHandle asyncHandle, IRestResponse<Data> response)
        {
            //For multithreading protection propose
            lock (_sync)
            {
                _requestDict.Add(asyncHandle, (RestResponse<Data>)response);
            }

            //TODO delete this
            Console.WriteLine(response.ResponseStatus + ": Status Code " + response.StatusCode);
        }


        public void CancelRequests()
        {
            foreach (RestRequestAsyncHandle r in RequestDict.Keys)
                r.Abort();
        }

        public Boolean AllRequestsFinished()
        {
            return RequestDict.Keys.Count == size;
        }

        public bool CheckStatusCodes()
        {
            foreach (RestResponse<Data> r in RequestDict.Values)
                if (r.StatusCode != HttpStatusCode.OK) return false;

            return true;
        }
    

    }
}
