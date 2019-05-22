using Capito.Orchestrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capito.Test
{
    class TestLogger : ILogger
    {
        private TestContext _tc;
        public TestLogger(TestContext tc)
        {
            _tc = tc;
        }

        public void WriteInfo(string message)
        {
            _tc.WriteLine(message);
        }
    }
}
