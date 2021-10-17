using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Helpers;
using Webapi.Services;
using Webapi.Entities;
using Webapi.Models;

namespace Webapi
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseController : ControllerBase
    {
        private readonly IBuyTypeRepository _btRepo;
        private readonly IPaymentMethodRepository _pmRepo;
        private readonly IPurchaseRepository _purRepo;
        private readonly IeVoucherRepository _evRepo;
        private static readonly Object _obj = new Object();
        public PurchaseController(IBuyTypeRepository btRepo, IPaymentMethodRepository pmRepo, IPurchaseRepository purRepo, IeVoucherRepository evRepo)
        {
            _btRepo = btRepo;
            _pmRepo = pmRepo;
            _purRepo = purRepo;
            _evRepo = evRepo;
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(Purchase entity)
        {
            try
            {
                var buy_Type = _btRepo.GetAll().Where(o => o.type_id == entity.buy_type_id).FirstOrDefault();
                if (buy_Type != null)
                {
                    if (buy_Type.gift_per_user_limit > 0)
                    {
                        if (_purRepo.PurchaseCountById(entity.customer_id, entity.buy_type_id).Result > buy_Type.gift_per_user_limit)
                        {
                            return BadRequest(new { code = "1004", message = "purchase limitation is exceeded." });
                        }
                    }
                }

                var ev = _evRepo.GetAll().Result.Where(o => o.ev_Id == entity.ev_id).FirstOrDefault();
                if (ev != null)
                {
                    if (ev.qty > entity.qty && ev.expiry_date.Date >= DateTime.Now.Date)
                    {
                        var pm = _pmRepo.GetPayment_Types().Where(o => o.payment_id == entity.payment_type_id).FirstOrDefault();
                        entity.discount = pm.discount;
                        if (pm.discount > 0)
                            entity.balance = ev.amount - ((ev.amount * pm.discount) / 100);
                        else
                            entity.balance = ev.amount;
                        entity.Id = Guid.NewGuid().ToString();
                        entity.promocode = JWTGenerator.RandomPromoCode();
                        while (await Task.Run(()=> _purRepo.ExistPromoCode(entity.promocode)))
                        {
                            entity.promocode = JWTGenerator.RandomPromoCode();
                        }                        
                        entity.created_on = DateTime.Now;
                        entity.expiry_date = ev.expiry_date;
                        await Task.Run(() => _purRepo.checkout(entity));
                        await Task.Run(()=> generateQR(entity.Id, entity.promocode));
                        return Ok(new { Id = entity.Id, PromoCode = entity.promocode });
                    }
                    else
                    {
                        if (ev.qty == 0)
                            return BadRequest(new { code = "1006", message = "out of stock." });
                        else
                            return BadRequest(new { code = "1006", message = String.Format("evoucher is only {0} left.", ev.qty) });
                    }
                }
                else
                {
                    return BadRequest(new { code = "1005", message = "invalid evoucher" });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Code = "1020", Message = ex.Message }) { StatusCode = 500 };
            }            
        }

        [HttpGet("GetQR")]
        public async Task<IActionResult> GetQR(string Id)
        {
            RespQRCode resp = new RespQRCode();
            resp.qr = await Task.Run(()=> _purRepo.GetQR(Id));
            return Ok(resp);
        }
        
        private async Task generateQR(string Id, string promocode)
        {
            Purchase entity = null;
            lock (_obj)
            {
               entity  = new Purchase { qrcode = QRCodeGenerate.GenerateQR(promocode), Id = Id };
            }
            await Task.Run(() => _purRepo.UpdateQrCode(entity));
        }

        [HttpPut("VerifyPromoCode")]
        public async Task<IActionResult> VerifyPromoCode(VerifyPromoCodeModel model)
        {
            try
            {
                Purchase entity = await Task.Run(() => _purRepo.GetPromocode(model.phone, null, model.promocode));
                if (entity == null)
                    return BadRequest(new { code = "1010", message = "Invalid phone no or promocode" });

                if (entity.expiry_date < DateTime.Now)
                    return BadRequest(new { code = "1011", message = "eVoucher is expired." });

                entity.is_verify = true;
                entity.verify_date = DateTime.Now;
                await Task.Run(() => _purRepo.UpdateQrCode(entity));
                return Ok(new { status = "valid" });
            }
            catch (Exception e)
            {
                return new ObjectResult(new { Code = "1020", Message = "server error" }) { StatusCode = 500 };
            }            
        }

        [HttpGet("GetPurchaseByStatus")]
        public async Task<IActionResult> GetPurchaseByStatus(bool is_verify)
        {
            return Ok(await Task.Run(() => _purRepo.GetPurchaseByStatus(is_verify)));
        }

    }
}
