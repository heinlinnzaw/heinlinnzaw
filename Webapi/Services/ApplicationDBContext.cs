using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;

namespace Webapi.Services
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<TokenEntity> Token { get; set; }
        public virtual DbSet<Payment_Type> Payment_Type { get; set; }
        public virtual DbSet<eVoucher> EVouchers { get; set; }
        public virtual DbSet<Buy_Type> BT { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
    }
}