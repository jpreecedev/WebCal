namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class VehicleRepository : BaseRepository, IRepository<VehicleMake>
    {
        public void AddOrUpdate(VehicleMake entity)
        {
            Safely(() =>
            {
                VehicleMake existing = Context.VehicleMakes.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<VehicleMake>().Add(entity);
            });
        }

        public void Add(VehicleMake entity)
        {
            Safely(() => Context.VehicleMakes.Add(entity));
        }

        public void Remove(VehicleMake entity)
        {
            Safely(() => Context.VehicleMakes.Remove(entity));
        }

        public ICollection<VehicleMake> GetAll()
        {
            return Safely(() =>
            {
                //Workaround to avoid the selected item being defaulted in
                List<VehicleMake> items = Context.VehicleMakes.Include(c => c.Models).Where(c => c.Name != null).OrderBy(c => c.Name).ToList();
                
                foreach (VehicleMake item in items)
                {
                    item.Models = new ObservableCollection<VehicleModel>(item.Models.Where(m => m.Name != null).OrderBy(c => c.Name));

                    if (item.Models.All(m => m.Name != null))
                    {
                        item.Models.Insert(0, new VehicleModel());                        
                    }
                }

                items.Insert(0, null);
                return items;
            });
        }

        public ICollection<VehicleMake> Get(Expression<Func<VehicleMake, bool>> predicate)
        {
            return Safely(() => Context.VehicleMakes.Include(c => c.Models).Where(predicate.Compile()).ToList());
        }

        public VehicleMake FirstOrDefault(Expression<Func<VehicleMake, bool>> predicate)
        {
            return Safely(() => Context.VehicleMakes.FirstOrDefault(predicate.Compile()));
        }

        public VehicleMake First(Expression<Func<VehicleMake, bool>> predicate)
        {
            return Safely(() => Context.VehicleMakes.First(predicate.Compile()));
        }
    }
}