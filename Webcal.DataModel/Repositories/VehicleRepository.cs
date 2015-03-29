namespace TachographReader.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using Library;

    public class VehicleRepository : Repository<VehicleMake>
    {
        public override ICollection<VehicleMake> GetAll(params string[] includes)
        {
            return Safely(() =>
            {
                //Workaround to avoid the selected item being defaulted in
                List<VehicleMake> items = Context.VehicleMakes.Include(c => c.Models).WithIncludes(Context, includes).Where(c => c.Name != null).OrderBy(c => c.Name).ToList();

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
    }
}