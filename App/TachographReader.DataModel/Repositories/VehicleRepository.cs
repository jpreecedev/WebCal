using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Webcal.DataModel;
using Webcal.Shared;

namespace Webcal.DataModel.Repositories
{
    public class VehicleRepository : BaseRepository, IRepository<VehicleMake>
    {
        #region Implementation of IRepository<VehicleMake>

        public void AddOrUpdate(VehicleMake entity)
        {
            Safely(() =>
            {
                var existing = Context.VehicleMakes.Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Context.Set<VehicleMake>().Add(entity);
                }
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
                                  List<VehicleMake> items = Context.VehicleMakes.Include("Models").ToList();
                                  

                                  foreach (var item in items)
                                  {
                                      item.Models.Insert(0, new VehicleModel());
                                  }

                                  items.Insert(0, null);

                                  return items;
                              });
        }

        public ICollection<VehicleMake> Get(Expression<Func<VehicleMake, bool>> predicate)
        {
            return Safely(() => Context.VehicleMakes.Include("Models").Where(predicate.Compile()).ToList());
        }

        public VehicleMake FirstOrDefault(Expression<Func<VehicleMake, bool>> predicate)
        {
            return Safely(() => Context.VehicleMakes.FirstOrDefault(predicate.Compile()));
        }

        public VehicleMake First(Expression<Func<VehicleMake, bool>> predicate)
        {
            return Safely(() => Context.VehicleMakes.First(predicate.Compile()));
        }

        #endregion
    }
}
