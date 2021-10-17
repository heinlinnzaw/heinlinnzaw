
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;
using Webapi.Models;

namespace Webapi.Services
{
    public interface IeVoucherRepository
    {
        void CreateVoucher(eVoucher entity);
        void UpdateVoucher(eVoucher entity);
        Task<List<EVoucherModel>> GetAll(string id = null, string status = null);
    }
    public class eVoucherRepository : IeVoucherRepository
    {
        UnitOfWork uow;
        public eVoucherRepository()
        {
            uow = new UnitOfWork();
        }

        public void CreateVoucher(eVoucher entity)
        {
            uow.eVRepo.Insert(entity);
            try
            {
                uow.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateVoucher(eVoucher entity)
        {            
            var oldentity = uow.eVRepo.GetAll().Where(o => o.ev_Id == entity.ev_Id).FirstOrDefault();
            if (entity.is_active == false)
                uow.eVRepo.UpdatePartial(oldentity, entity, o => o.is_active);
            else if(entity.image.Length <= 0)
                uow.eVRepo.UpdatePartial(oldentity, entity, o => o.title, o => o.description, o => o.expiry_date, o => o.amount, o => o.qty, o => o.updated_on);
            else
                uow.eVRepo.Update(oldentity, entity);
            try
            {
                uow.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<EVoucherModel>> GetAll(string id= null,string status = null)
        {
            string query = "SELECT * FROM VW_EVOUCHER";
            if (!string.IsNullOrEmpty(status))
                query = String.Format("SELECT * FROM VW_EVOUCHER WHERE STATUS = '{0}'",status);
            if(!(string.IsNullOrEmpty(id)))
                query = String.Format("SELECT * FROM VW_EVOUCHER WHERE ev_Id = '{0}'", id);

            var list = await Task.Run(() => uow.RawSqlQuery(query, x => new EVoucherModel
            {
                ev_Id = (string)x["ev_Id"],
                title = (string)x["title"],
                description = Convert.ToString(x["description"]),
                expiry_date = (DateTime)x["expiry_date"],
                image = (byte[])x["image"],
                filename = (string)x["filename"],
                fileextension = (string)x["fileextension"],
                amount = (decimal)x["amount"],
                qty = Convert.ToInt32(x["qty"]),
                status = (string)x["status"]
            }).ToList());

            return list;
        }
    }
}
