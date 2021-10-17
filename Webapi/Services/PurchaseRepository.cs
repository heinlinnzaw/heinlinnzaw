using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;
namespace Webapi.Services
{
    public interface IPurchaseRepository
    {
        void checkout(Purchase entity);
        Task<Purchase> GetPromocode(string phone = null, string id = null, string promocode = null);
        Task<int> PurchaseCountById(string custId, string bt_id);
        Task<bool> ExistPromoCode(string promocode);
        Task UpdateQrCode(Purchase entity);
        Task<byte[]> GetQR(string Id);
        Task<List<Purchase>> GetPurchaseByStatus(bool is_used);
    }
    public class PurchaseRepository : IPurchaseRepository
    {
        UnitOfWork uow;
        public PurchaseRepository()
        {
            uow = new UnitOfWork();
        }

        public void checkout(Purchase entity)
        {
            uow.PurchaseRepo.Insert(entity);
            try
            {
                uow.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<int> PurchaseCountById(string custId, string bt_id)
        {
            return await Task.Run(() => uow.PurchaseRepo.GetAll().Where(o => o.customer_id == custId && o.buy_type_id == bt_id).Count());
        }
        
        public async Task<Purchase> GetPromocode(string phone = null, string id = null, string promocode = null)
        {
            return await Task.Run(() => uow.PurchaseRepo.GetAll().Where(o=> o.phoneno.Equals(phone) || o.ev_id.Equals(id) || o.promocode.Equals(promocode)).FirstOrDefault());
        }

        public async Task<bool> ExistPromoCode(string promocode)
        {
            if (await Task.Run(()=> uow.PurchaseRepo.GetAll().Where(o => o.promocode == promocode).Count()) > 0)
                return true;
            else
                return false;
        }

        public async Task UpdateQrCode(Purchase entity)
        {
            Purchase oldentity = uow.PurchaseRepo.Query(o => o.Id.Equals(entity.Id)).FirstOrDefault();            
            try
            {
                if(entity.verify_date == null)
                await Task.FromResult(uow.PurchaseRepo.UpdatePartial(oldentity, entity, o => o.qrcode));
                else
                    await Task.FromResult(uow.PurchaseRepo.UpdatePartial(oldentity, entity, o => o.is_verify, o=> o.verify_date));
                uow.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<byte[]> GetQR(string Id)
        {
             return await Task.Run(()=> uow.PurchaseRepo.GetAll().Where(o => o.Id == Id).Select(o => o.qrcode).FirstOrDefault());
        }

        public async Task<List<Purchase>> GetPurchaseByStatus(bool is_used)
        {
            return await Task.Run(() => uow.PurchaseRepo.GetAll().Where(o => o.is_verify == is_used).Select(o => new Purchase
            {
                promocode = o.promocode,
                qrcode = o.qrcode,
                is_verify = o.is_verify
            }).ToList());
        }
    }
}
