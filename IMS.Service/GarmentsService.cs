using IMS.DAO;
using IMS.Models;
using IMS.Models.ViewModel;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IGarmentsService
    {
        IEnumerable<GarmentsProduct> GetAllP();
        GarmentsProductViewModel GetAllProduct(int pageNumber);
        void CreateGarmentsProduct(GarmentsProduct garmentsProduct);
    }
    public class GarmentsService:IGarmentsService
    {
        private readonly BaseDAO<GarmentsProduct> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }

        public GarmentsService()
        {
            _repository = new BaseDAO<GarmentsProduct>();
        }

        public IEnumerable<GarmentsProduct> GetAllP()
        {
            return _repository.GetAll().ToList();
        }
        public GarmentsProductViewModel GetAllProduct(int pageNumber)
        {
            var query= _repository.GetAll().ToList();

            int pageSize = 10;
            int totalCount = query.Count();

            IEnumerable<GarmentsProduct> products = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var resultModel = new GarmentsProductViewModel
            {
                GarmentsProducts = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return resultModel;
        }

        public void CreateGarmentsProduct(GarmentsProduct model)
        {
            int prod = this.GetAllP().Count();
            int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            GarmentsProduct garmentsProduct = new GarmentsProduct
            {
                Name = model.Name,
                Image = model.Image,
                SKU = model.SKU,
                Price = model.Price,
                GarmentsId = 1,
                Description = model.Description,
                ProductType = model.ProductType,
                Department = model.Department,
                ProductCode = prod + 1,
                CreatedBy=1,
                CreationDate = DateTime.Now,
                Status=1,
                Rank=highRank+1,
                VersionNumber=1,
                BusinessId = Guid.NewGuid().ToString()
            };

            _repository.Add(garmentsProduct);
        }
    }
}
