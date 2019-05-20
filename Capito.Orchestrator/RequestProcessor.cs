using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public class RequestProcessor
    {
        private Request _req;
        List<IOperator> _tasks;
        public ResultStatus Status { get; set; }

        public RequestProcessor(Request req)
        {
            this._req = req;
            //load all tasks (consider using factory for creating all tasks)
            _tasks = new List<IOperator>();
            _tasks.Add(new Tasks.CopyTask() { Request = req });
        }

        public Task Process()
        {
            return Task.Factory.StartNew(() =>
            {
                this.Status = ResultStatus.InProgress;
                foreach (var item in _tasks)
                {
                    if (this.Status != ResultStatus.InProgress)
                        break;
                    item.Perform()
                        .ContinueWith((task) =>
                        {
                            if (task.Result.Status == ResultStatus.Failed)
                                this.Status = task.Result.Status;
                        });
                }
            });
        }
    }
}
