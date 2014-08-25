namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Shared;

    public class TachographDocumentRepository : BaseRepository, IRepository<TachographDocument>
    {
        public void AddOrUpdate(TachographDocument entity)
        {
            Safely(() =>
            {
                TachographDocument existing = Context.TachographDocuments.Find(entity.Id);
                if (existing != null)
                    Context.Entry(existing).CurrentValues.SetValues(entity);
                else
                    Context.Set<TachographDocument>().Add(entity);
            });
        }

        public void Add(TachographDocument entity)
        {
            Safely(() => Context.TachographDocuments.Add(entity));
        }

        public void Remove(TachographDocument entity)
        {
            Safely(() => Context.TachographDocuments.Remove(entity));
        }

        public ICollection<TachographDocument> GetAll()
        {
            return Safely(() => Context.TachographDocuments.ToList());
        }

        public ICollection<TachographDocument> Get(Expression<Func<TachographDocument, bool>> predicate)
        {
            return Safely(() => Context.TachographDocuments.Where(predicate.Compile()).ToList());
        }

        public TachographDocument FirstOrDefault(Expression<Func<TachographDocument, bool>> predicate)
        {
            return Safely(() => Context.TachographDocuments.FirstOrDefault(predicate.Compile()));
        }

        public TachographDocument First(Expression<Func<TachographDocument, bool>> predicate)
        {
            return Safely(() => Context.TachographDocuments.First(predicate.Compile()));
        }
    }
}