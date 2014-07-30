using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Webcal.Shared;
using Webcal.DataModel;

namespace Webcal.DataModel.Repositories
{
    public class UserRepository : BaseRepository, IRepository<User>
    {
        #region Implementation of IRepository<User>

        public void AddOrUpdate(User entity)
        {
            Safely(() =>
                       {
                           var existing = Context.Users.Find(entity.Id);
                           if (existing != null)
                           {
                               Context.Entry(entity).State = EntityState.Modified;
                           }
                           else
                           {
                               Context.Set<User>().Add(entity);
                           }
                       });
        }

        public void Add(User entity)
        {
            Safely(() => Context.Users.Add(entity));
        }

        public void Remove(User entity)
        {
            Safely(() => Context.Users.Remove(entity));
        }

        public ICollection<User> GetAll()
        {
            return Safely(() => Context.Users.ToList());
        }

        public ICollection<User> Get(Expression<Func<User, bool>> predicate)
        {
            return Safely(() => Context.Users.Where(predicate.Compile()).ToList());
        }

        public User FirstOrDefault(Expression<Func<User, bool>> predicate)
        {
            return Safely(() => Context.Users.FirstOrDefault(predicate.Compile()));
        }

        public User First(Expression<Func<User, bool>> predicate)
        {
            return Safely(() => Context.Users.First(predicate.Compile()));
        }

        #endregion
    }
}
