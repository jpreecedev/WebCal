﻿namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class InspectionMethodsRepository : BaseRepository, IRepository<InspectionMethod>
    {
        public void AddOrUpdate(InspectionMethod entity)
        {
            Safely(() =>
            {
                InspectionMethod existing = Context.InspectionMethods.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<InspectionMethod>().Add(entity);
            });
        }

        public void Add(InspectionMethod entity)
        {
            Safely(() => Context.InspectionMethods.Add(entity));
        }

        public void Remove(InspectionMethod entity)
        {
            Safely(() => Context.InspectionMethods.Remove(entity));
        }

        public ICollection<InspectionMethod> GetAll()
        {
            return Safely(() => Context.InspectionMethods.OrderBy(c => c.Name).ToList());
        }

        public ICollection<InspectionMethod> Get(Expression<Func<InspectionMethod, bool>> predicate)
        {
            return Safely(() => Context.InspectionMethods.Where(predicate.Compile()).ToList());
        }

        public InspectionMethod FirstOrDefault(Expression<Func<InspectionMethod, bool>> predicate)
        {
            return Safely(() => Context.InspectionMethods.FirstOrDefault(predicate.Compile()));
        }

        public InspectionMethod First(Expression<Func<InspectionMethod, bool>> predicate)
        {
            return Safely(() => Context.InspectionMethods.First(predicate.Compile()));
        }
    }
}