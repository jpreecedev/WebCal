namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class TachographMakesRepository : BaseRepository, IRepository<TachographMake>
    {
        public void AddOrUpdate(TachographMake entity)
        {
            Safely(() =>
            {
                TachographMake existing = Context.TachographMakes.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<TachographMake>().Add(entity);
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
                //Workaround to avoid the selected item being defaulted in
                List<TachographMake> items = Context.TachographMakes.Include(c => c.Models).OrderBy(c => c.Name).ToList();

                foreach (TachographMake item in items)
                {
                    item.Models = new ObservableCollection<TachographModel>(item.Models.OrderBy(c => c.Name));
                    item.Models.Insert(0, new TachographModel());
                }

                items.Insert(0, null);

                return items;
            });
        }

        public ICollection<TachographMake> Get(Expression<Func<TachographMake, bool>> predicate)
        {
            return Safely(() => Context.TachographMakes.Include(c => c.Models).Where(predicate.Compile()).ToList());
        }

        public TachographMake FirstOrDefault(Expression<Func<TachographMake, bool>> predicate)
        {
            return Safely(() => Context.TachographMakes.FirstOrDefault(predicate.Compile()));
        }

        public TachographMake First(Expression<Func<TachographMake, bool>> predicate)
        {
            return Safely(() => Context.TachographMakes.First(predicate.Compile()));
        }
    }
}