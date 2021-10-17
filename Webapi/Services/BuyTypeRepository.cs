using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;
namespace Webapi.Services
{
    public interface IBuyTypeRepository
    {
        List<Buy_Type> GetAll();
    }
    public class BuyTypeRepository : IBuyTypeRepository
    {
        UnitOfWork uow;
        public BuyTypeRepository()
        {
            uow = new UnitOfWork();
        }

        public List<Buy_Type> GetAll()
        {
            return uow.BTRepo.GetAll().ToList();
        }
    }
}
