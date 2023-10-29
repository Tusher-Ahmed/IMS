using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IOrderHeaderService
    {

    }
    public class OrderHeaderService:IOrderHeaderService
    {
        private readonly BaseDAO<OrderHeader> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
    }
}
