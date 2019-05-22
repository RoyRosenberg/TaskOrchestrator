using System.Threading;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    interface IOperator
    {
        string Name { get; }
        Request Request { get; set; }
        ProcessorOutput Perform();

    }
}