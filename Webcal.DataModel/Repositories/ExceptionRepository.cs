namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class ExceptionRepository : BaseRepository, IRepository<DetailedException>
    {
        public void AddOrUpdate(DetailedException entity)
        {
            Safely(() =>
            {
                DetailedException existing = Context.Exceptions.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<DetailedException>().Add(entity);
            });
        }

        public void Add(DetailedException entity)
        {
            Safely(() => Context.Exceptions.Add(entity));
        }

        public void Remove(DetailedException entity)
        {
            Safely(() => Context.Exceptions.Remove(entity));
        }

        public ICollection<DetailedException> GetAll()
        {
            return Safely(() => Context.Exceptions.ToList());
        }

        public ICollection<DetailedException> Get(Expression<Func<DetailedException, bool>> predicate)
        {
            return Safely(() => Context.Exceptions.Where(predicate.Compile()).ToList());
        }

        public DetailedException FirstOrDefault(Expression<Func<DetailedException, bool>> predicate)
        {
            return Safely(() => Context.Exceptions.FirstOrDefault(predicate.Compile()));
        }

        public DetailedException First(Expression<Func<DetailedException, bool>> predicate)
        {
            return Safely(() => Context.Exceptions.First(predicate.Compile()));
        }

        public IEnumerable<DetailedException> Where(Expression<Func<DetailedException, bool>> predicate)
        {
            return Safely(() => Context.Exceptions.Where(predicate.Compile()));            
        }
    }
}