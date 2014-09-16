namespace Webcal.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using Repositories;
    using Shared;

    public class UndownloadabilityDocumentRepository : BaseRepository, IRepository<UndownloadabilityDocument>
    {
        public void AddOrUpdate(UndownloadabilityDocument entity)
        {
            Safely(() =>
            {
                UndownloadabilityDocument existing = Context.UndownloadabilityDocuments.Find(entity.Id);
                if (existing != null)
                    Context.Entry(entity).State = EntityState.Modified;
                else
                    Context.Set<UndownloadabilityDocument>().Add(entity);
            });
        }

        public void Add(UndownloadabilityDocument entity)
        {
            Safely(() => Context.UndownloadabilityDocuments.Add(entity));
        }

        public void Remove(UndownloadabilityDocument entity)
        {
            Safely(() => Context.UndownloadabilityDocuments.Remove(entity));
        }

        public ICollection<UndownloadabilityDocument> GetAll()
        {
            return Safely(() => Context.UndownloadabilityDocuments.OrderByDescending(d => d.Created).ToList());
        }

        public ICollection<UndownloadabilityDocument> Get(Expression<Func<UndownloadabilityDocument, bool>> predicate)
        {
            return Safely(() => Context.UndownloadabilityDocuments.Where(predicate.Compile()).ToList());
        }

        public UndownloadabilityDocument FirstOrDefault(Expression<Func<UndownloadabilityDocument, bool>> predicate)
        {
            return Safely(() => Context.UndownloadabilityDocuments.FirstOrDefault(predicate.Compile()));
        }

        public UndownloadabilityDocument First(Expression<Func<UndownloadabilityDocument, bool>> predicate)
        {
            return Safely(() => Context.UndownloadabilityDocuments.First(predicate.Compile()));
        }
    }
}