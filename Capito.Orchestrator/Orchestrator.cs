using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public class Orchestrator
    {
        private List<RequestProcessor> _list;

        public Orchestrator()
        {
            _list = new List<RequestProcessor>();
        }

        public void StartListeningForNewRequests()
        {
            //create task that every 10 seconds checks for new requests in DB
        }

        public void StartProcessingRequests()
        {
            // load all (10) not-done requests from DB
            List<Request> requests = LoadRequests();

            // process each request
            foreach (Request req in requests)
            {
                RequestProcessor proc = new RequestProcessor(req);
                _list.Add(proc);
                proc.Process();
            }
        }

        private List<Request> LoadRequests()
        {
            throw new NotImplementedException();
        }
    }
}
