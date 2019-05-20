using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    interface IOperator
    {
        Request Request { get; set; }
        Task<ProcessorOutput> Perform();
    }
}