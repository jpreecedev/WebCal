using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Webcal.Shared;
using Webcal.DataModel;

namespace Webcal.DataModel.Repositories
{
    public class TachographMakesRepository : BaseRepository, IRepository<TachographMake>
    {
        #region Implementation of IRepository<TachographMake>

        public void AddOrUpdate(TachographMake entity)
        {
            Safely(() =>
            {
                var existing = Context.TachographMakes.Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Context.Set<TachographMake>().Add(entity);
                }
            });
        }

        public void Add(TachographMake entity)
        {
            Safely(() => Context.TachographMakes.Add(entity));
        }

        public void Remove(TachographMake entity)
        {
            Safely(() => Context.TachographMakes.Remove(entity));
        }

        public ICollection<TachographMake> GetAll()
        {
            return Safely(() =>
                              {
                                  List<TachographMake> items = Context.TachographMakes.Include("Models").ToList();
                                  items.Insert(0, null);
                                  return items;
                              });
        }

        public ICollection<TachographMake> Get(Expression<Func<TachographMake, bool>> predicate)
        {
            return Safely(() => Context.TachographMakes.Include("Models").Where(predicate.Compile()).ToList());
        }

        public TachographMake FirstOrDefault(Expression<Func<TachographMake, bool>> predicate)
        {
            return Safely(() => Context.TachographMakes.FirstOrDefault(predicate.Compile()));
        }

        public TachographMake First(Expression<Func<TachographMake, bool>> predicate)
        {
            return Safely(() => Context.TachographMakes.First(predicate.Compile()));
        }

        #endregion
    }
}
