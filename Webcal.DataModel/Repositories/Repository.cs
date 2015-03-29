namespace TachographReader.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Connect.Shared.Models;
    using Library;
    using Shared;

    public class Repository<T> : BaseRepository, IRepository<T> where T : BaseModel
    {
        public bool Any()
        {
            return Safely(() => Context.Set<T>().Any());
        }

        public virtual void AddOrUpdate(T entity)
        {
            Safely(() =>
            {
                T existing = Context.Set<T>().Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(existing).CurrentValues.SetValues(entity);
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
            Safely(() =>
            {
                entity.Deleted = DateTime.Now;
                Context.Entry(entity).State = EntityState.Modified;
            });
        }

        public virtual ICollection<T> GetAll(params string[] includes)
        {
            return GetAll(false, includes);
        }

        public ICollection<T> GetAll(bool includeDeleted, params string[] includes)
        {
            return Safely(() =>
            {
                var query = Context.Set<T>().WithIncludes(Context, includes);

                if (!includeDeleted)
                {
                    query = query.Where(c => c.Deleted == null);
                }

                return query.ToList();
            });
        }

        public virtual ICollection<T> Get(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return Get(predicate, false, includes);
        }

        public ICollection<T> Get(Expression<Func<T, bool>> predicate, bool includeDeleted, params string[] includes)
        {
            return Safely(() =>
            {
                var query = Context.Set<T>().WithIncludes(Context, includes).Where(predicate.Compile());

                if (!includeDeleted)
                {
                    query = query.Where(c => c.Deleted == null);
                }

                return query.ToList();
            });
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Safely(() => Context.Set<T>().FirstOrDefault(predicate.Compile()));
        }

        public T First()
        {
            return Safely(() => Context.Set<T>().First());
        }

        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return Safely(() => Context.Set<T>().First(predicate.Compile()));
        }

        public virtual ICollection<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate, false);
        }

        public ICollection<T> Where(Expression<Func<T, bool>> predicate, bool includeDeleted)
        {
            return Safely(() =>
            {
                var query = Context.Set<T>().Where(predicate.Compile());

                if (!includeDeleted)
                {
                    query = query.Where(c => c.Deleted == null);
                }

                return query.ToList();
            });
        }
    }
}