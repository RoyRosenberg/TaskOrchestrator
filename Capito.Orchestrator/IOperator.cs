using System.Threading;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public interface IOperator
    {
        string Name { get; set; }
        Request Request { get; set; }
        ProcessorOutput Perform();

    }
}