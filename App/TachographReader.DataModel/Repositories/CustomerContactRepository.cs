using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Webcal.DataModel;
using Webcal.Shared;

namespace Webcal.DataModel.Repositories
{
    public class CustomerContactRepository : BaseRepository, IRepository<CustomerContact>
    {
        #region Implementation of IRepository<CustomerContact>

        public void AddOrUpdate(CustomerContact entity)
        {
            Safely(() =>
            {
                var existing = Context.CustomerContacts.Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Context.Set<CustomerContact>().Add(entity);
                }
            });
        }

        public void Add(CustomerContact entity)
        {
            Safely(() => Context.CustomerContacts.Add(entity));
        }

        public void Remove(CustomerContact entity)
        {
            Safely(() => Context.CustomerContacts.Remove(entity));
        }

        public ICollection<CustomerContact> GetAll()
        {
            return Safely(() => Context.CustomerContacts.ToList().OrderBy(c => c.Name)).ToList();
        }

        public ICollection<CustomerContact> Get(Expression<Func<CustomerContact, bool>> predicate)
        {
            return Safely(() => Context.CustomerContacts.Where(predicate.Compile()).ToList());
        }

        public CustomerContact FirstOrDefault(Expression<Func<CustomerContact, bool>> predicate)
        {
            return Safely(() => Context.CustomerContacts.FirstOrDefault(predicate.Compile()));
        }

        public CustomerContact First(Expression<Func<CustomerContact, bool>> predicate)
        {
            return Safely(() => Context.CustomerContacts.First(predicate.Compile()));
        }

        #endregion
    }
}
