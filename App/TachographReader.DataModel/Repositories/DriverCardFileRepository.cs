using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Webcal.DataModel;
using Webcal.Shared;

namespace Webcal.DataModel.Repositories
{
    public class DriverCardFileRepository : BaseRepository, IRepository<DriverCardFile>
    {
        #region Implementation of IRepository<DriverCardFile>

        public void AddOrUpdate(DriverCardFile entity)
        {
            Safely(() =>
            {
                var existing = Context.DriverCardFiles.Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Context.Set<DriverCardFile>().Add(entity);
                }
            });
        }

        public void Add(DriverCardFile entity)
        {
            Safely(() => Context.DriverCardFiles.Add(entity));
        }

        public void Remove(DriverCardFile entity)
        {
            Safely(() => Context.DriverCardFiles.Remove(entity));
        }

        public ICollection<DriverCardFile> GetAll()
        {
            return Safely(() => Context.DriverCardFiles.Include("Customer").ToList());
        }

        public ICollection<DriverCardFile> Get(Expression<Func<DriverCardFile, bool>> predicate)
        {
            return Safely(() => Context.DriverCardFiles.Include("Customer").Where(predicate.Compile()).ToList());
        }

        public DriverCardFile FirstOrDefault(Expression<Func<DriverCardFile, bool>> predicate)
        {
            return Safely(() => Context.DriverCardFiles.FirstOrDefault(predicate.Compile()));
        }

        public DriverCardFile First(Expression<Func<DriverCardFile, bool>> predicate)
        {
            return Safely(() => Context.DriverCardFiles.First(predicate.Compile()));
        }

        #endregion
    }
}
