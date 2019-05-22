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
        private Request _req;
        private List<IOperator> _tasks;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private Task _worker;
        public ResultStatus Status { get; set; }

        public RequestProcessor(Request req)
        {
            this._req = req;
            //load all tasks (consider using factory)
            _tasks = new List<IOperator>();
            _tasks.Add(new Tasks.CopyTask() { Request = req });
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
        }

        public void Process()
        {
            _worker = Task.Factory.StartNew(() =>
            {
                if (_cancellationToken.IsCancellationRequested)
                    return; //todo: handle cancelation 
                Status = ResultStatus.InProgress;
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
                        //Update DB with Status and error
                        return;
                    }
                    
                }
            }, _cancellationToken);
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
            _worker.Wait();
        }
    }
}
