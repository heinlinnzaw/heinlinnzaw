using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;
using Webapi.Models;
using Webapi.Services;
using Webapi.Helpers;
using AutoMapper;
using System.IO;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvoucherController : ControllerBase
    {
        private readonly IeVoucherRepository _eRepo;
        private readonly IMapper _mapper;
        
        public EvoucherController(IeVoucherRepository eRepo, IMapper mapper)
        {
            _eRepo = eRepo;
            _mapper = mapper;            
        }

        [HttpPost("CreateVoucher")]
        public async Task<IActionResult> CreateVoucher([FromForm]EVoucherModel model)
        {            
            eVoucher ev = _mapper.Map<eVoucher>(model);
            ev.ev_Id = Guid.NewGuid().ToString();
            ev.created_on = DateTime.Now;
            try
            {
                using (var target = new MemoryStream())
                {
                    model.uploadimage.CopyTo(target);
                    ev.image = target.ToArray();
                    ev.filename = model.uploadimage.FileName;
                    ev.fileextension = model.uploadimage.ContentType;
                }                
                await Task.Run(() => _eRepo.CreateVoucher(ev));
                return Ok(new { Code = "1010", Message = "Success"});
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Code = "1020", Message = ex.Message }) { StatusCode = 500 };
            }
            
        }

        [HttpPut("UpdateVoucher")]
        public async Task<IActionResult> UpdateVoucher([FromForm] EVoucherModel model)
        {
            eVoucher ev = _mapper.Map<eVoucher>(model);
            ev.updated_on = DateTime.Now;
            try
            {
                if (model.uploadimage != null)
                {
                    using (var target = new MemoryStream())
                    {
                        model.uploadimage.CopyTo(target);
                        ev.image = target.ToArray();
                        ev.filename = model.uploadimage.FileName;
                        ev.fileextension = model.uploadimage.ContentType;
                    }
                }
                await Task.Run(() => _eRepo.UpdateVoucher(ev));
                return Ok(new { Code = "1010", Message = "Success" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Code = "1020", Message = ex.Message }) { StatusCode = 500 };
            }
        }

        [HttpGet("GetAllVoucher")]
        public async Task<IActionResult> GetAllVoucher(string id = null, string status =null)
        {
            try
            {
                var list = _eRepo.GetAll(id, status).Result.Where(o => o.expiry_date.Date > DateTime.Now.Date);
                return Ok(await Task.Run(()=> (list)));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Code = "1020", Message = ex.Message }) { StatusCode = 500 };
            }             
        }
    }
}
