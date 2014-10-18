namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Library;
    using Shared;

    public class Repository<T> : BaseRepository, IRepository<T> where T : BaseModel
    {
        public virtual void AddOrUpdate(T entity)
        {
            Safely(() =>
            {
                T existing = Context.Set<T>().Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Context.Set<T>().Add(entity);
                }
            });
        }

        public virtual void Add(T entity)
        {
            Safely(() => Context.Set<T>().Add(entity));
        }

        public virtual void Remove(T entity)
        {
            Safely(() => Context.Set<T>().Remove(entity));
        }

        public virtual ICollection<T> GetAll(params string[] includes)
        {
            return Safely(() => Context.Set<T>().WithIncludes(Context, includes).ToList());
        }

        public virtual ICollection<T> Get(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return Safely(() => Context.Set<T>().WithIncludes(Context, includes).Where(predicate.Compile()).ToList());
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Safely(() => Context.Set<T>().FirstOrDefault(predicate.Compile()));
        }

        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return Safely(() => Context.Set<T>().First(predicate.Compile()));
        }

        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Safely(() => Context.Set<T>().Where(predicate.Compile()));
        }
    }
}