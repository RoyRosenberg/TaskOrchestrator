﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public class Orchestrator
    {
        private ICollection<RequestProcessor> _list;
        public ILogger Logger { get; set; }
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        const int PERIOD = 10;
        private Task _newRequestHanlder;
        public Orchestrator()
        {
            _list = new List<RequestProcessor>();
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
        }

        public void StartListeningForNewRequests()
        {
            _newRequestHanlder = Task.Factory.StartNew(() => {
                //create task that every 10 seconds checks for new requests in DB
                while (!_cancellationToken.IsCancellationRequested)
                {
                    Logger.WriteInfo($"Waiting for {PERIOD} seconds");
                    Wait();

                    if (!_cancellationToken.IsCancellationRequested)
                    {
                        //get all incoming requests
                        Logger.WriteInfo($"get all incoming requests");
                        Logger.WriteInfo($"for each request, create its operations to perform");
                        Logger.WriteInfo($"set request state to ready");
                        Task.Delay(PERIOD * 100, _cancellationToken);
                        //for each request, create its operations to perform
                        //set request state to ready 
                    }
                }
            }, _cancellationToken);
        }

        public void StartProcessingRequests()
        {
            // load all (10) not-done requests from DB
            BlockingCollection<Request> requestsToProcess = LoadOpenRequests();
                        
            // process each request
            foreach (Request req in requestsToProcess)
            {
                RequestProcessor proc = new RequestProcessor(req);
                _list.Add(proc);
                proc.Process();
            }
            TrackRequestStatus();
        }

        /// <summary>
        /// Create new task that checks when a request processor finishes and start the next one 
        /// </summary>
        private void TrackRequestStatus()
        {
            Task.Factory.StartNew(() => {
                while(!_cancellationToken.IsCancellationRequested)
                {
                    var proc = _list.FirstOrDefault(t => t.Status == ResultStatus.Completed);
                    if(proc != null)
                    {
                        //remove the finied processor
                        _list.Remove(proc);                      
                        if (_cancellationToken.IsCancellationRequested)
                            break;
                        //get the next request from DB
                        var nextReq = LoadOpenRequests().FirstOrDefault();
                        if (nextReq != null)
                        {
                            proc = new RequestProcessor(nextReq);
                            _list.Add(proc);
                            if (_cancellationToken.IsCancellationRequested)
                                break;
                            proc.Process();
                        }
                    }
                    Wait();
                }
            }, _cancellationToken);
        }

        public void Cancel()
        {
            Logger.WriteInfo("Cancelling...");
            _tokenSource.Cancel();
            _list.ToList().ForEach(t => t.Cancel());
            Logger.WriteInfo("Waiting for all to response...");
            Task.WaitAll(new Task[] { _newRequestHanlder });
            Logger.WriteInfo("Finished");
        }

        private BlockingCollection<Request> LoadOpenRequests()
        {
            //Get all ready, in progress requests
            throw new NotImplementedException();
        }  
        
        private void Wait()
        {
            //int waitTime = PERIOD;
            //while(waitTime > 0)
            //{
            //    Thread.Sleep(1000);
            //    if (_cancellationToken.IsCancellationRequested)
            //        return;
            //    waitTime--;
            //}
            Task.Delay(PERIOD, _cancellationToken).Wait();
        }
    }
}
