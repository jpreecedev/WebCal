namespace Webcal.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IRepository<T> : IDisposable
    {
        void AddOrUpdate(T entity);

        void Add(T entity);

        void Remove(T entity);

        ICollection<T> GetAll();

        ICollection<T> Get(Expression<Func<T, bool>> predicate);

        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        T First(Expression<Func<T, bool>> predicate);

        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);

        void Save();
    }
}