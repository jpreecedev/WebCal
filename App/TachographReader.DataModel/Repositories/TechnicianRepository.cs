using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Webcal.Shared;
using Webcal.DataModel;

namespace Webcal.DataModel.Repositories
{
    public class TechnicianRepository : BaseRepository, IRepository<Technician>
    {
        #region Implementation of IRepository<Technician>

        public void AddOrUpdate(Technician entity)
        {
            Safely(() =>
            {
                var existing = Context.Technicians.Find(entity.Id);
                if (existing != null)
                {
                    Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Context.Set<Technician>().Add(entity);
                }
            });
        }

        public void Add(Technician entity)
        {
            Safely(() => Context.Technicians.Add(entity));
        }

        public void Remove(Technician entity)
        {
            Safely(() => Context.Technicians.Remove(entity));
        }

        public ICollection<Technician> GetAll()
        {
            return Safely(() =>
                              {
                                  List<Technician> technicians = Context.Technicians.ToList();
                                  technicians.Insert(0, null);
                                  return technicians;
                              });
        }

        public ICollection<Technician> Get(Expression<Func<Technician, bool>> predicate)
        {
            return Safely(() => Context.Technicians.Where(predicate.Compile()).ToList());
        }

        public Technician FirstOrDefault(Expression<Func<Technician, bool>> predicate)
        {
            return Safely(() => Context.Technicians.FirstOrDefault(predicate.Compile()));
        }

        public Technician First(Expression<Func<Technician, bool>> predicate)
        {
            return Safely(() => Context.Technicians.First(predicate.Compile()));
        }

        #endregion
    }
}
