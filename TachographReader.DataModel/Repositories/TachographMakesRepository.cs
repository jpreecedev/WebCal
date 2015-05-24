namespace TachographReader.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using Library;
    using Microsoft.Practices.ObjectBuilder2;

    public class TachographMakesRepository : Repository<TachographMake>
    {
        public override void Remove(TachographMake entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var existingEntity = context.TachographMakes.Include(m => m.Models).FirstOrDefault(c => c.Id == entity.Id);
                    if (existingEntity != null)
                    {
                        existingEntity.Models.ForEach(m => m.Deleted = DateTime.Now);
                        existingEntity.Deleted = DateTime.Now;
                    }

                    context.SaveChanges();
                }
            });
        }

        public void Remove(TachographModel entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var tachographModels = context.TachographMakes.SelectMany(c => c.Models);
                    var existingEntity = tachographModels.FirstOrDefault(c => c.Id == entity.Id);

                    if (existingEntity != null)
                    {
                        existingEntity.Deleted = DateTime.Now;
                        context.Entry(existingEntity).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            });
        }

        public override void AddOrUpdate(TachographMake entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    TachographMake existingMake = context.Set<TachographMake>().Find(entity.Id);
                    if (existingMake != null)
                    {
                        context.Entry(existingMake).CurrentValues.SetValues(entity);
                        foreach (var tachographModel in entity.Models)
                        {
                            if (tachographModel.IsNewEntity)
                            {
                                if (!string.IsNullOrEmpty(tachographModel.Name))
                                {
                                    tachographModel.TachographMake = existingMake;
                                    context.Entry(tachographModel).State = EntityState.Added;
                                }
                            }
                        }
                    }
                    else
                    {
                        context.Set<TachographMake>().Add(entity);
                    }

                    context.SaveChanges();
                }
            });
        }

        public override ICollection<TachographMake> GetAll(bool includeDeleted, params string[] includes)
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var query = context.Set<TachographMake>().WithIncludes(context, includes).Where(c => c.Name != null);

                    if (!includeDeleted)
                    {
                        query = query.Where(c => c.Deleted == null);
                    }

                    query = query.OrderBy(c => c.Name);

                    List<TachographMake> items = query.ToList();

                    foreach (TachographMake item in items)
                    {
                        item.Models = new ObservableCollection<TachographModel>(item.Models.Where(m => m.Name != null).OrderBy(c => c.Name));

                        if (!includeDeleted)
                        {
                            item.Models = new ObservableCollection<TachographModel>(item.Models.Where(c => c.Deleted == null));
                        }

                        if (item.Models.All(m => m.Name != null))
                        {
                            item.Models.Insert(0, new TachographModel());
                        }
                    }

                    items.Insert(0, null);
                    return items;
                }
            });
        }
    }
}