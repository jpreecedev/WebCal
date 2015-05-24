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
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    return context.Set<T>().Any();
                }
            });
        }

        public virtual void AddOrUpdate(T entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    T existing = context.Set<T>().Find(entity.Id);
                    if (existing != null)
                    {
                        context.Entry(existing).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        context.Set<T>().Add(entity);
                    }

                    context.SaveChanges();
                }
            });
        }

        public virtual void Add(T entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    context.Set<T>().Add(entity);
                    context.SaveChanges();
                }
            });
        }

        public virtual void Remove(T entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    entity.Deleted = DateTime.Now;
                    context.Entry(entity).State = EntityState.Modified;
                    context.SaveChanges();
                }
            });
        }

        public virtual ICollection<T> GetAll(params string[] includes)
        {
            return GetAll(false, includes);
        }

        public virtual ICollection<T> GetAll(bool includeDeleted, params string[] includes)
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var query = context.Set<T>().WithIncludes(context, includes);

                    if (!includeDeleted)
                    {
                        query = query.Where(c => c.Deleted == null);
                    }

                    return query.ToList();
                }
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
                using (var context = new TachographContext())
                {
                    var query = context.Set<T>().WithIncludes(context, includes).Where(predicate.Compile());

                    if (!includeDeleted)
                    {
                        query = query.Where(c => c.Deleted == null);
                    }

                    return query.ToList();
                }
            });
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    return context.Set<T>().FirstOrDefault(predicate.Compile());
                }
            });
        }

        public T First()
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    return context.Set<T>().First();
                }
            });
        }

        public virtual T First(Expression<Func<T, bool>> predicate)
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    return context.Set<T>().First(predicate.Compile());
                }
            });
        }

        public virtual ICollection<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate, false);
        }

        public ICollection<T> Where(Expression<Func<T, bool>> predicate, bool includeDeleted)
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var query = context.Set<T>().Where(predicate.Compile());

                    if (!includeDeleted)
                    {
                        query = query.Where(c => c.Deleted == null);
                    }

                    return query.ToList();
                }
            });
        }

        public void Dispose()
        {
            
        }
    }
}