using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Webapi.Entities;
using Webapi.Models;
namespace Webapi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<eVoucher, EVoucherModel>();
            CreateMap<EVoucherModel, eVoucher>();
        }
    }
}
