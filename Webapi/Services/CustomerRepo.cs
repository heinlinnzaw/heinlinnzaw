using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;

namespace Webapi.Services
{
    public interface ICustomerRepository
    {
        void UpdateLastLogin(Customer entity);
        Customer GetLogin(string username, string pwd);
        
    }
    public class CustomerRepository : ICustomerRepository
    {
        UnitOfWork uow;
        public CustomerRepository()
        {
            uow = new UnitOfWork();
        }

        public void UpdateLastLogin(Customer entity)
        {
            Customer Oldentity = uow.CustomerRepo.Query(o => o.Id == entity.Id).FirstOrDefault();
            uow.CustomerRepo.UpdatePartial(Oldentity,entity, o => o.last_login_datetime);
            try
            {
                uow.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Customer GetLogin (string username, string pwd)
        {            
            try
            {
                Customer entity = uow.CustomerRepo.Query(o => o.name == username && o.pwd== pwd).ToList().FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
