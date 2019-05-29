using System;
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
        private List<RequestProcessor> _list;
        public IRequestRepository RequestRepository { get; set; }
        public ILogger Logger { get; set; }
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        const int PERIOD = 10;        
        private Task _runner;
        private object _locker = new object();
        public Orchestrator()
        {
            _list = new List<RequestProcessor>();
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
        }

        public void Start()
        {            
            _runner = Task.Factory.StartNew(() => {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (!_cancellationToken.IsCancellationRequested)
                        //check for new request and make them Ready
                        HandleNewRequests();

                    if (!_cancellationToken.IsCancellationRequested)
                        //check if orchestrator can handle new request
                        HandleIncomingReadyRequests();

                    Wait();
                }
                Logger.WriteInfo($"Orchestrator Cancelled");
            }, _cancellationToken);
            TrackRequestStatus();
        }

        private void HandleIncomingReadyRequests()
        {
            int currentCount = _list.Count;
            if(currentCount < 10)
            {
                int amountToAdd = 10 - currentCount;
                var readyRequests = RequestRepository.GetOpenRequests().Take(amountToAdd);
                readyRequests.ToList().ForEach(r => {
                    RequestProcessor proc = new RequestProcessor(r);
                    proc.RequestRepository = RequestRepository;
                    AddNewRequestProcessor(proc);
                    proc.Process();
                    Logger.WriteInfo($"Request {r.Id} started");
                });
            }
        }

        private void HandleNewRequests()
        {
            var requests = RequestRepository.GetNewRequests();
            if (requests.Count > 0)
            {
                foreach (var r in requests)
                {
                    //Todo: make sure the request is ready to start and use cancel token if needed
                    r.Status = RequestStatus.Ready;
                }
                Logger.WriteInfo($"Made {requests.Count} requests in ready state");
            }
        }        

        /// <summary>
        /// Create new task that checks when a request processor finishes and start the next one 
        /// </summary>
        private void TrackRequestStatus()
        {
            Task.Factory.StartNew(() => {
                while(!_cancellationToken.IsCancellationRequested)
                {
                    var doneProcesses = _list.Where(t => t.Status == ResultStatus.Completed).ToList(); ;
                    foreach (var process in doneProcesses)
                    {
                        Logger.WriteInfo($"Request {process.Request.Id} completed");                        
                        RemoveRequestProcessor(process);                        
                        //update DB
                    }                    
                    Wait();
                }
                //Logger.WriteInfo($"Stopped tracking ");
            }, _cancellationToken);
        }

        public void Cancel()
        {
            Logger.WriteInfo("Cancelling...");
            _tokenSource.Cancel();
            _list.ToList().ForEach(t => t.Cancel());
            Logger.WriteInfo("Waiting for all to response...");
            //Task.WaitAll(new Task[] { _newRequestHanlder });
            //_newRequestHanlder.Wait();
            _runner.Wait();
            Logger.WriteInfo("Finished");
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
            try
            {
                Task.Delay(PERIOD, _cancellationToken).Wait();
            }
            catch
            {
                //do nothing

            }
            
        }

        private void AddNewRequestProcessor(RequestProcessor proc)
        {
            if (proc == null)
                throw new ArgumentNullException("proc");
            lock (_locker)
            {
                _list.Add(proc);
            }
        }

        private void RemoveRequestProcessor(RequestProcessor proc)
        {
            if (proc == null)
                throw new ArgumentNullException("proc");
            lock (_locker)
            {
                _list.Remove(proc);
            }
        }
    }
}
