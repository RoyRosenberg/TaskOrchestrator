using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capito.Orchestrator.Tasks
{
    class CopyTask : IOperator
    {
        public Request Request { get; set; }

        public Task<ProcessorOutput> Perform()
        {
            return null;
        }
    }
}
