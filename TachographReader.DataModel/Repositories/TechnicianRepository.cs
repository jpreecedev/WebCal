namespace TachographReader.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Connect.Shared.Models;

    public class TechnicianRepository : Repository<Technician>
    {
        public ICollection<Technician> GetAll()
        {
            return Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    List<Technician> technicians = context.Technicians.OrderBy(c => c.Name).ToList();
                    technicians.Insert(0, null);
                    return technicians;
                }
            });
        }
    }
}