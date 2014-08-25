namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class InspectionEquipmentRepository : BaseRepository, IRepository<InspectionEquipment>
    {
        public void AddOrUpdate(InspectionEquipment entity)
        {
            Safely(() =>
            {
                InspectionEquipment existing = Context.InspectionEquipments.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<InspectionEquipment>().Add(entity);
            });
        }

        public void Add(InspectionEquipment entity)
        {
            Safely(() => Context.InspectionEquipments.Add(entity));
        }

        public void Remove(InspectionEquipment entity)
        {
            Safely(() => Context.InspectionEquipments.Add(entity));
        }

        public ICollection<InspectionEquipment> GetAll()
        {
            return Safely(() => Context.InspectionEquipments.ToList());
        }

        public ICollection<InspectionEquipment> Get(Expression<Func<InspectionEquipment, bool>> predicate)
        {
            return Safely(() => Context.InspectionEquipments.Where(predicate.Compile()).ToList());
        }

        public InspectionEquipment FirstOrDefault(Expression<Func<InspectionEquipment, bool>> predicate)
        {
            return Safely(() => Context.InspectionEquipments.FirstOrDefault(predicate.Compile()));
        }

        public InspectionEquipment First(Expression<Func<InspectionEquipment, bool>> predicate)
        {
            return Safely(() => Context.InspectionEquipments.First(predicate.Compile()));
        }
    }
}