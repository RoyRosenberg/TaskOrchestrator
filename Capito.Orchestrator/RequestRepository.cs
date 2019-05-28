using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capito.Orchestrator
{
    public class RequestRepository : IRequestRepository
    {
        public IList<Request> GetOpenRequests()
        {
            throw new NotImplementedException();
        }

        public IList<Request> GetNewRequests()
        {
            throw new NotImplementedException();
        }

        public IList<IOperator> GetRequestOperations(Request request)
        {
            throw new NotImplementedException();
        }
    }

    public interface IRequestRepository
    {
        IList<Request> GetOpenRequests();
        IList<IOperator> GetRequestOperations(Request request);
        IList<Request> GetNewRequests();
    }
}
