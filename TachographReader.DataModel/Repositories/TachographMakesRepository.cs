namespace TachographReader.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using Library;

    public class TachographMakesRepository : Repository<TachographMake>
    {
        public override ICollection<TachographMake> GetAll(params string[] includes)
        {
            return Safely(() =>
            {
                //Workaround to avoid the selected item being defaulted in
                using (var context = new TachographContext())
                {
                    List<TachographMake> items = context.TachographMakes.Include(c => c.Models).WithIncludes(context, includes).OrderBy(c => c.Name).ToList();

                    foreach (TachographMake item in items)
                    {
                        item.Models = new ObservableCollection<TachographModel>(item.Models.OrderBy(c => c.Name));
                        item.Models.Insert(0, new TachographModel());
                    }

                    items.Insert(0, null);

                    return items;
                }
            });
        }
    }
}