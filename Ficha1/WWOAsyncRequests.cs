using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Ficha1
{
    class WWOAsyncRequests
    {
        private const int MAX_CHECK_FINISH = 500;
        private Dictionary<RestRequestAsyncHandle, RestResponse<Data>> _requestDict;
        public Dictionary<RestRequestAsyncHandle, RestResponse<Data>> RequestDict
        {
            get{return _requestDict;}
        }

        private static object _sync = new object();

        public WWOAsyncRequests()
        {
            _requestDict = new Dictionary<RestRequestAsyncHandle, RestResponse<Data>>();
        }

        public void AddRequest(RestRequestAsyncHandle asyncHandle, IRestResponse<Data> response)
        {
            //For multithreading protection propose
            lock (_sync)
            {

                //This is the very exceptional case when the response gest first before
                //the item creation
                if (response == null && _requestDict.ContainsKey(asyncHandle))
                    return;//Do nothing

                _requestDict[asyncHandle] = (RestResponse<Data>)response;

                //TODO delete this
                if(response!=null)
                    Console.WriteLine(response.ResponseStatus + ": Status Code " + response.StatusCode);
            }

        }


        public void CancelRequests()
        {
            foreach (RestRequestAsyncHandle r in RequestDict.Keys)
                r.Abort();
        }

        private Boolean AllRequestsFinished()
        {
            foreach (RestResponse<Data> r in RequestDict.Values)
                if (r == null) return false;

            return true;
        }

        public bool CheckStatusCodes()
        {
            foreach (RestResponse<Data> r in RequestDict.Values)
                if (r.StatusCode != HttpStatusCode.OK) return false;

            return true;
        }

        public bool WaitForFinish(int timeoutInMilis)
        {
            while (!AllRequestsFinished())
            {
                if (timeoutInMilis <= 0)
                    return false;
                    
                Thread.Sleep(MAX_CHECK_FINISH);
                timeoutInMilis -= MAX_CHECK_FINISH;
            };

            return true;
        } 
    

    }
}
