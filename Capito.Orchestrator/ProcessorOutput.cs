using System;

namespace Capito.Orchestrator
{
    public class ProcessorOutput
    {
        public ResultStatus Status { get; set; }
        public Exception LastError { get; set; }
    }

    public enum ResultStatus
    {
        Failed,
        Completed,
        InProgress
    }
}