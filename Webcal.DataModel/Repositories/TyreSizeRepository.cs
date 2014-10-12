namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class TyreSizeRepository : BaseRepository, IRepository<TyreSize>
    {
        public void AddOrUpdate(TyreSize entity)
        {
            Safely(() =>
            {
                TyreSize existing = Context.TyreSizes.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<TyreSize>().Add(entity);
            });
        }

        public void Add(TyreSize entity)
        {
            Safely(() => Context.TyreSizes.Add(entity));
        }

        public void Remove(TyreSize entity)
        {
            Safely(() => Context.TyreSizes.Remove(entity));
        }

        public ICollection<TyreSize> GetAll()
        {
            return Safely(() => Context.TyreSizes.OrderBy(c => c.Size).ToList());
        }

        public ICollection<TyreSize> Get(Expression<Func<TyreSize, bool>> predicate)
        {
            return Safely(() => Context.TyreSizes.Where(predicate.Compile()).ToList());
        }

        public TyreSize FirstOrDefault(Expression<Func<TyreSize, bool>> predicate)
        {
            return Safely(() => Context.TyreSizes.FirstOrDefault(predicate.Compile()));
        }

        public TyreSize First(Expression<Func<TyreSize, bool>> predicate)
        {
            return Safely(() => Context.TyreSizes.First(predicate.Compile()));
        }

        public IEnumerable<TyreSize> Where(Expression<Func<TyreSize, bool>> predicate)
        {
            return Safely(() => Context.TyreSizes.Where(predicate.Compile()));
        }
    }
}