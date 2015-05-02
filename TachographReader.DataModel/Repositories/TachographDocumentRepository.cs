namespace TachographReader.DataModel.Repositories
{
    using System.Linq;
    using Connect.Shared.Models;

    public class TachographDocumentRepository : Repository<TachographDocument>
    {
        public override void AddOrUpdate(TachographDocument entity)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    TachographDocument existing = context.TachographDocuments.Find(entity.Id);
                    if (existing != null)
                    {
                        context.Entry(existing).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        context.Set<TachographDocument>().Add(entity);
                    }

                    CheckVehicleExists(context, entity);
                    context.SaveChanges();
                }
            });
        }

        private void CheckVehicleExists(TachographContext context, TachographDocument entity)
        {
            if (entity == null)
            {
                return;
            }

            var vehicleMakes = context.VehicleMakes.ToList();
            VehicleMake vehicleMake = vehicleMakes.FirstOrDefault(v => string.Equals(v.Name, entity.VehicleMake));

            if (vehicleMake != null)
            {
                bool vehicleModelExists = vehicleMakes.First(m => string.Equals(m.Name, entity.VehicleMake)).Models.Any(c => string.Equals(c.Name, entity.VehicleModel));
                if (!vehicleModelExists && !string.IsNullOrEmpty(entity.VehicleModel))
                {
                    VehicleModel vehicleModel = new VehicleModel { Name = entity.VehicleModel };
                    vehicleMake.Models.Add(vehicleModel);
                }
            }
            else
            {
                vehicleMake = context.VehicleMakes.Add(new VehicleMake { Name = entity.VehicleMake });

                if (!string.IsNullOrEmpty(entity.VehicleModel))
                {
                    vehicleMake.Models.Add(new VehicleModel { Name = entity.VehicleModel });
                }
            }
        }
    }
}