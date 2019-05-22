using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capito.Test
{
    [TestClass]
    public class OrchestratorTest
    {
        public TestContext TestContext { get; set; }
        [TestMethod]
        public void Orchestrator_StartListeningForNewRequests()
        {
            Orchestrator.Orchestrator target = new Orchestrator.Orchestrator();
            target.Logger = new TestLogger(TestContext);
            target.StartListeningForNewRequests();
            System.Threading.Thread.Sleep(500);
            target.Cancel();
        }
    }
}
