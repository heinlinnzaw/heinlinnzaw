using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;

namespace Webapi.Services
{
    public class UnitOfWork
    {
        private ApplicationDBContext _ctx;
        public UnitOfWork(ApplicationDBContext ctx)
        {
            _ctx = ctx;
        }

        public UnitOfWork()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json")
              .Build();

            string connStr = configuration.GetConnectionString("DefaultConnection");
            var contextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
              .UseMySql(connStr, ServerVersion.AutoDetect(connStr))
              .Options;
            _ctx = new ApplicationDBContext(contextOptions);

        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _ctx.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Commit()
        {
            return _ctx.SaveChanges();
        }

        public List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
            var entities = new List<T>();
            using (var command = _ctx.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                _ctx.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }

                }
                _ctx.Database.CloseConnection();
            }
            return entities;
        }

        private IRepository<Customer> _customerrepo;
        public IRepository<Customer> CustomerRepo
        {
            get
            {
                if (_customerrepo == null)
                {
                    _customerrepo = new Repository<Customer>(_ctx);
                }
                return _customerrepo;
            }
        }

        private IRepository<TokenEntity> _tokenrepo;
        public IRepository<TokenEntity> TokenRepo
        {
            get
            {
                if (_tokenrepo == null)
                {
                    _tokenrepo = new Repository<TokenEntity>(_ctx);
                }
                return _tokenrepo;
            }
        }

        private IRepository<Payment_Type> _paymentrepo;
        public IRepository<Payment_Type> PaymentRepo
        {
            get
            {
                if (_paymentrepo == null)
                {
                    _paymentrepo = new Repository<Payment_Type>(_ctx);
                }
                return _paymentrepo;
            }
        }

        private IRepository<eVoucher> _evRepo;
        public IRepository<eVoucher> eVRepo
        {
            get
            {
                if (_evRepo == null)
                {
                    _evRepo = new Repository<eVoucher>(_ctx);
                }
                return _evRepo;
            }
        }

        private IRepository<Buy_Type> _bt;
        public IRepository<Buy_Type> BTRepo
        {
            get
            {
                if (_bt == null)
                {
                    _bt = new Repository<Buy_Type>(_ctx);
                }
                return _bt;
            }
        }

        private IRepository<Purchase> _purchase;
        public IRepository<Purchase> PurchaseRepo
        {
            get
            {
                if (_purchase == null)
                {
                    _purchase = new Repository<Purchase>(_ctx);
                }
                return _purchase;
            }
        }
    }
}
