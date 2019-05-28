using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public class RequestProcessor
    {
        public Request Request { get; private set; }
        private IList<IOperator> _tasks;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private Task _worker;
        public ResultStatus Status { get; set; }
        public IRequestRepository RequestRepository { get; set; }

        public RequestProcessor(Request req)
        {
            this.Request = req;            
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
        }

        public void Process()
        {
            _worker = Task.Factory.StartNew(() =>
            {
                if (_cancellationToken.IsCancellationRequested)
                    return; //todo: handle cancelation 
                LoadRequestOperation();
                Status = ResultStatus.InProgress;
                Request.Status = RequestStatus.InProgress;
                foreach (var item in _tasks)
                {
                    //todo: handle retry in case of failure
                    if (Status != ResultStatus.InProgress)
                        break;
                    if (_cancellationToken.IsCancellationRequested)
                        return;                    
                    var res = item.Perform();
                    if (res.Status == ResultStatus.Failed)
                    {
                        this.Status = res.Status;
                        Request.Status = RequestStatus.Completed;//todo: change?
                        //Update DB with Status and error
                        return;
                    }
                }
                this.Status = ResultStatus.Completed;
                Request.Status = RequestStatus.Completed;
            }, _cancellationToken);
        }

        private void LoadRequestOperation()
        {
            //load all tasks (consider using factory)
            _tasks = RequestRepository.GetRequestOperations(Request);            
        }

        public void Cancel()
        {
            //todo: cancel to IOperator
            _tokenSource.Cancel();
            _worker.Wait();
        }
    }
}
