using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capito.Orchestrator.Tasks
{
    public class CopyTask : IOperator
    {
        public string Name { get; set; }
        public Request Request { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private Task _worker;
        private bool isWorking;

        public CopyTask()
        {
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;
        }

        public ProcessorOutput Perform()
        {
            if (string.IsNullOrEmpty(Source))
                throw new ArgumentNullException("Source");
            if (string.IsNullOrEmpty(Destination))
                throw new ArgumentNullException("Destination");
            if (!File.Exists(Source))
                throw new FileNotFoundException($"Cannot find file");
            try
            {
                _worker = CopyFileAsync();
                _worker.Wait();
                return new ProcessorOutput()
                {
                    Status = ResultStatus.Completed
                };
            }
            catch (Exception ex)
            {
                return new ProcessorOutput()
                {
                    Status = ResultStatus.Failed,
                    LastError = ex
                };
            }
        }

        public void Cancel()
        {
            if (isWorking)
            {
                _tokenSource.Cancel();
                _worker.Wait();
            }
        }

        private async Task CopyFileAsync()
        {
            isWorking = true;
            using (Stream source = File.Open(Source, FileMode.Open))
            {
                using (Stream destination = File.Create(Destination))
                {
                    await source.CopyToAsync(destination, 1000, _cancellationToken);
                    if (_cancellationToken.IsCancellationRequested)
                        throw new Exception("Operation Cancelled");
                    isWorking = false;
                }
            }
        }

    }
}
