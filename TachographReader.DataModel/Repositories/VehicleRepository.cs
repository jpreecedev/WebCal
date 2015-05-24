namespace TachographReader.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using Library;

    public class VehicleRepository : Repository<VehicleMake>
    {
        public void Remove(VehicleModel entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var vehicleModels = context.VehicleMakes.SelectMany(c => c.Models);
                    var existingEntity = vehicleModels.FirstOrDefault(c => c.Id == entity.Id);

                    if (existingEntity != null)
                    {
                        existingEntity.Deleted = DateTime.Now;
                        context.Entry(existingEntity).State = EntityState.Modified;
                        context.SaveChanges();   
                    }
                }
            });
        }

        public override void AddOrUpdate(VehicleMake entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    VehicleMake existingMake = context.Set<VehicleMake>().Find(entity.Id);
                    if (existingMake != null)
                    {
                        context.Entry(existingMake).CurrentValues.SetValues(entity);
                        foreach (var vehicleModel in entity.Models)
                        {
                            if (vehicleModel.IsNewEntity)
                            {
                                if (!string.IsNullOrEmpty(vehicleModel.Name))
                                {
                                    vehicleModel.VehicleMake = existingMake;
                                    context.Entry(vehicleModel).State = EntityState.Added;
                                }
                            }
                        }
                    }
                    else
                    {
                        context.Set<VehicleMake>().Add(entity);
                    }

                    context.SaveChanges();
                }
            });
        }

        public override ICollection<VehicleMake> GetAll(bool includeDeleted, params string[] includes)
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    var query = context.Set<VehicleMake>().WithIncludes(context, includes).Where(c => c.Name != null);

                    if (!includeDeleted)
                    {
                        query = query.Where(c => c.Deleted == null);
                    }

                    query = query.OrderBy(c => c.Name);

                    List<VehicleMake> items = query.ToList();

                    foreach (VehicleMake item in items)
                    {
                        item.Models = new ObservableCollection<VehicleModel>(item.Models.Where(m => m.Name != null ).OrderBy(c => c.Name));

                        if (!includeDeleted)
                        {
                            item.Models = new ObservableCollection<VehicleModel>(item.Models.Where(c => c.Deleted == null));
                        }

                        if (item.Models.All(m => m.Name != null))
                        {
                            item.Models.Insert(0, new VehicleModel());
                        }
                    }

                    items.Insert(0, null);
                    return items;
                }
            });
        }
    }
}