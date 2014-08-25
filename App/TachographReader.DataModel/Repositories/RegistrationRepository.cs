namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class RegistrationRepository : BaseRepository, IRepository<RegistrationData>
    {
        public void AddOrUpdate(RegistrationData entity)
        {
            Safely(() =>
            {
                RegistrationData existing = Context.RegistrationData.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<RegistrationData>().Add(entity);
            });
        }

        public void Add(RegistrationData entity)
        {
            Safely(() => Context.RegistrationData.Add(entity));
        }

        public void Remove(RegistrationData entity)
        {
            Safely(() => Context.RegistrationData.Add(entity));
        }

        public ICollection<RegistrationData> GetAll()
        {
            return Safely(() => Context.RegistrationData.ToList());
        }

        public ICollection<RegistrationData> Get(Expression<Func<RegistrationData, bool>> predicate)
        {
            return Safely(() => Context.RegistrationData.Where(predicate.Compile()).ToList());
        }

        public RegistrationData FirstOrDefault(Expression<Func<RegistrationData, bool>> predicate)
        {
            return Safely(() => Context.RegistrationData.FirstOrDefault(predicate.Compile()));
        }

        public RegistrationData First(Expression<Func<RegistrationData, bool>> predicate)
        {
            return Safely(() => Context.RegistrationData.First(predicate.Compile()));
        }
    }
}