namespace Capito.Orchestrator
{
    public class ProcessorOutput
    {
        public ResultStatus Status { get; set; }
    }

    public enum ResultStatus
    {
        Failed,
        Completed,
        InProgress
    }
}