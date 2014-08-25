namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class WorkshopCardFileRepository : BaseRepository, IRepository<WorkshopCardFile>
    {
        public void AddOrUpdate(WorkshopCardFile entity)
        {
            Safely(() =>
            {
                WorkshopCardFile existing = Context.WorkshopCardFiles.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<WorkshopCardFile>().Add(entity);
            });
        }

        public void Add(WorkshopCardFile entity)
        {
            Safely(() => Context.WorkshopCardFiles.Add(entity));
        }

        public void Remove(WorkshopCardFile entity)
        {
            Safely(() => Context.WorkshopCardFiles.Remove(entity));
        }

        public ICollection<WorkshopCardFile> GetAll()
        {
            return Safely(() => Context.WorkshopCardFiles.ToList());
        }

        public ICollection<WorkshopCardFile> Get(Expression<Func<WorkshopCardFile, bool>> predicate)
        {
            return Safely(() => Context.WorkshopCardFiles.Where(predicate.Compile()).ToList());
        }

        public WorkshopCardFile FirstOrDefault(Expression<Func<WorkshopCardFile, bool>> predicate)
        {
            return Safely(() => Context.WorkshopCardFiles.FirstOrDefault(predicate.Compile()));
        }

        public WorkshopCardFile First(Expression<Func<WorkshopCardFile, bool>> predicate)
        {
            return Safely(() => Context.WorkshopCardFiles.First(predicate.Compile()));
        }
    }
}