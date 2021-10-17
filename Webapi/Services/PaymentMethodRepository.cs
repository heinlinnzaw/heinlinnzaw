using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;
namespace Webapi.Services
{
    public interface IPaymentMethodRepository
    {
        List<Payment_Type> GetPayment_Types();
    }

    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        UnitOfWork uow;
        public PaymentMethodRepository()
        {
            uow = new UnitOfWork();
        }

        public List<Payment_Type> GetPayment_Types()
        {
            return uow.PaymentRepo.GetAll().ToList();
        }
    }
}
