using System;
using System.Threading;
using System.Threading.Tasks;
using Capito.Orchestrator;
using Capito.Orchestrator.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capito.Test
{
    [TestClass]
    public class CopyTaskTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CopyTask_Perform()
        {
            CopyTask target = new CopyTask();
            target.Source = @"D:\Videos\DARDASIM_2.iso";
            target.Destination = @"c:\temp\DARDASIM_2.iso";          
            var res = target.Perform();
            Assert.AreEqual(ResultStatus.Completed, res.Status);
        }

        [TestMethod]
        public void CopyTask_CheckCancel()
        {
            CopyTask target = new CopyTask();
            target.Source = @"D:\Videos\DARDASIM_2.iso";
            target.Destination = @"c:\temp\DARDASIM_2.iso";
            Task.Factory.StartNew(() => {
                Thread.Sleep(2000);
                target.Cancel();
            });
            var res = target.Perform();
            Assert.AreEqual(ResultStatus.Failed, res.Status);
        }
    }
}
