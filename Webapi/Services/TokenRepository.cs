using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Entities;
namespace Webapi.Services
{
    public interface ITokenRepository
    {
        void CreateToken(TokenEntity entity);
        TokenEntity QueryByrefreshId(string Id);
        void RevokeToken(TokenEntity entity);
        void UpdateToken(TokenEntity entity);
        bool CheckValidToken(string Id);
    }
    public class TokenRepository : ITokenRepository
    {
        UnitOfWork uow;
        public TokenRepository()
        {
            uow = new UnitOfWork();
        }

        public void CreateToken(TokenEntity entity)
        {
            uow.TokenRepo.Insert(entity);
            try
            {
                uow.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckValidToken(string Id)
        {
            var query = (from user in uow.CustomerRepo.GetAll()
                         join t in uow.TokenRepo.GetAll() on user.Id equals t.Customer_Id 
                         where t.Is_Active == true && t.Token_Id.Equals(Id)
                         select t.Token_Id).ToList();
            if (query.Count > 0)
                return true;
            else
                return false;
        }
        public TokenEntity QueryByrefreshId(string Id)
        {
            try
            {
                return uow.TokenRepo.Query(o => o.Refresh_Token.Equals(Id) && o.Is_Active == true).ToList().FirstOrDefault();                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void RevokeToken(TokenEntity entity)
        {
            List<TokenEntity> deletelist = uow.TokenRepo.GetAll().Where(o => o.Token_Id != entity.Token_Id && o.Customer_Id == entity.Customer_Id).ToList();
            
            foreach (var item in deletelist)
            {
                uow.TokenRepo.Delete(item);
            }
            try
            {                
                uow.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateToken(TokenEntity entity)
        {
            TokenEntity oldEntity = uow.TokenRepo.Query(o => o.Token_Id == entity.Token_Id).FirstOrDefault();
            uow.TokenRepo.UpdatePartial(oldEntity, entity, o => o.Token, o => o.Refresh_Token);
            try
            {
                uow.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
