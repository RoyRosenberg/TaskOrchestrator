using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capito.Orchestrator;

namespace Capito.UITester
{
    public partial class Form1 : Form, IRequestRepository, ILogger
    {
        public Form1()
        {
            InitializeComponent();
            _orc = new Orchestrator.Orchestrator();
            _orc.RequestRepository = this;
            _orc.Logger = this;
        }

        Orchestrator.Orchestrator _orc;
        List<Request> _list = new List<Request>();
        Random _rnd = new Random();
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                Request r = new Request() { Id = i };
                r.Status = RequestStatus.New;
                _list.Add(r);
            }
            //_orc.StartListeningForNewRequests();
            //_orc.StartProcessingRequests();
            _orc.Start();
        }
        
        public IList<Request> GetOpenRequests()
        {
            return _list
                .Where(t => t.Status == RequestStatus.Ready)
                .ToList();
        }

        int num_op = 0;
        public IList<IOperator> GetRequestOperations(Request request)
        {
            int waitTime = _rnd.Next(10, 20);
            List<IOperator> demoList = new List<IOperator>();
            demoList.Add(new MockTask() { Request = request, WaitTime = waitTime, Name = $"Wait Task No.{++num_op}" });
            return demoList;
        }

        public IList<Request> GetNewRequests()
        {
            return _list
                .Where(t => t.Status == RequestStatus.New)
                .ToList();
        }

        public void WriteInfo(string message)
        {
            Action act = () => { listBox1.Items.Add($"{DateTime.Now.ToShortTimeString()} {message}"); };
            listBox1.Invoke((Delegate)act);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = _list.Max(x => x.Id) + 1;
            Request r = new Request() { Id = id };
            r.Status = RequestStatus.New;
            _list.Add(r);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _orc.Cancel();
        }
    }

    public class MockTask : Orchestrator.IOperator
    {
        public string Name { get; set; }

        public Request Request { get; set; }
        public int WaitTime { get; set; }
        public ProcessorOutput Perform()
        {
            System.Threading.Thread.Sleep(WaitTime * 1000);
            return new ProcessorOutput() { Status = ResultStatus.Completed };
        }
    }
}
