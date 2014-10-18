namespace Webcal.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    public class TechnicianRepository : Repository<Technician>
    {
        public ICollection<Technician> GetAll()
        {
            return Safely(() =>
            {
                List<Technician> technicians = Context.Technicians.OrderBy(c => c.Name).ToList();
                technicians.Insert(0, null);
                return technicians;
            });
        }
    }
}