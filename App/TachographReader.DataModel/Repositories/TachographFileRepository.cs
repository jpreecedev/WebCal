namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class TachographFileRepository : BaseRepository, IRepository<TachographFile>
    {
        public void AddOrUpdate(TachographFile entity)
        {
            Safely(() =>
            {
                TachographFile existing = Context.TachographFiles.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<TachographFile>().Add(entity);
            });
        }

        public void Add(TachographFile entity)
        {
            Safely(() => Context.TachographFiles.Add(entity));
        }

        public void Remove(TachographFile entity)
        {
            Safely(() => Context.TachographFiles.Remove(entity));
        }

        public ICollection<TachographFile> GetAll()
        {
            return Safely(() => Context.TachographFiles.ToList());
        }

        public ICollection<TachographFile> Get(Expression<Func<TachographFile, bool>> predicate)
        {
            return Safely(() => Context.TachographFiles.Where(predicate.Compile()).ToList());
        }

        public TachographFile FirstOrDefault(Expression<Func<TachographFile, bool>> predicate)
        {
            return Safely(() => Context.TachographFiles.FirstOrDefault(predicate.Compile()));
        }

        public TachographFile First(Expression<Func<TachographFile, bool>> predicate)
        {
            return Safely(() => Context.TachographFiles.First(predicate.Compile()));
        }
    }
}