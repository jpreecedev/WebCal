namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.EntityClient;
    using System.Linq;
    using System.Linq.Expressions;
    using Library;
    using Shared;

    public class TachographDocumentRepository : BaseRepository, IRepository<TachographDocument>
    {
        public void AddOrUpdate(TachographDocument entity)
        {
            Safely(() =>
            {
                TachographDocument existing = Context.TachographDocuments.Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(existing).CurrentValues.SetValues(entity);
                }
                else
                {
                    Context.Set<TachographDocument>().Add(entity);
                }

                CheckVehicleExists(entity);
            });
        }

        public void Add(TachographDocument entity)
        {
            Safely(() => Context.TachographDocuments.Add(entity));
        }

        public void Remove(TachographDocument entity)
        {
            Safely(() => Context.TachographDocuments.Remove(entity));
        }

        public ICollection<TachographDocument> GetAll()
        {
            return Safely(() => Context.TachographDocuments.OrderByDescending(d => d.Created).ToList());
        }

        public ICollection<TachographDocument> Get(Expression<Func<TachographDocument, bool>> predicate)
        {
            return Safely(() => Context.TachographDocuments.Where(predicate.Compile()).ToList());
        }

        public TachographDocument FirstOrDefault(Expression<Func<TachographDocument, bool>> predicate)
        {
            return Safely(() => Context.TachographDocuments.FirstOrDefault(predicate.Compile()));
        }

        public TachographDocument First(Expression<Func<TachographDocument, bool>> predicate)
        {
            return Safely(() => Context.TachographDocuments.First(predicate.Compile()));
        }

        private void CheckVehicleExists(TachographDocument entity)
        {
            if (entity == null)
            {
                return;
            }

            var vehicleMakes = Context.VehicleMakes.ToList();
            VehicleMake vehicleMake = vehicleMakes.FirstOrDefault(v => string.Equals(v.Name, entity.VehicleMake));

            if (vehicleMake != null)
            {
                bool vehicleModelExists = vehicleMakes.First(m => string.Equals(m.Name, entity.VehicleMake)).Models.Any(c => string.Equals(c.Name, entity.VehicleModel));
                if (!vehicleModelExists && !string.IsNullOrEmpty(entity.VehicleModel))
                {
                    VehicleModel vehicleModel = new VehicleModel {Name = entity.VehicleModel};
                    vehicleMake.Models.Add(vehicleModel);
                }
            }
            else
            {
                vehicleMake = Context.VehicleMakes.Add(new VehicleMake {Name = entity.VehicleMake});

                if (!string.IsNullOrEmpty(entity.VehicleModel))
                {
                    vehicleMake.Models.Add(new VehicleModel {Name = entity.VehicleModel});
                }
            }
        }
    }
}