using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public interface ILogger
    {
        void WriteInfo(string message);
    }

    public class Logger : ILogger
    {
        public void WriteInfo(string message)
        {
            throw new NotImplementedException();
        }
    }
}
