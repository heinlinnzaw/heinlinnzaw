using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Services;
using Webapi.Helpers;
namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentTypeController : ControllerBase
    {
        private readonly IPaymentMethodRepository _paymentMethod;        
        public PaymentTypeController(IPaymentMethodRepository paymentMethod)
        {
            _paymentMethod = paymentMethod;
        }

        [HttpGet("GetAll_PaymentMethods")]
        public async Task<IActionResult> GetAll_PaymentMethods()
        {
            try
            {   
                return Ok(await Task.Run(()=> _paymentMethod.GetPayment_Types().ToList()));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Code = "1020", Message = ex.Message }) { StatusCode = 500 };
            }            
        }
    }
}
