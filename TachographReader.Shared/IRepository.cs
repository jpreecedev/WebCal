namespace TachographReader.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using global::Connect.Shared.Models;

    public interface IRepository<T> : IDisposable where T : BaseModel
    {
        bool Any();
        bool Any(Expression<Func<T, bool>> predicate);
        void AddOrUpdate(T entity);
        void Add(T entity);
        void Remove(T entity);
        ICollection<T> GetAll(params string[] includes);
        ICollection<T> GetAll(bool includeDeleted, params string[] includes);
        ICollection<T> Get(Expression<Func<T, bool>> predicate, params string[] includes);
        ICollection<T> Get(Expression<Func<T, bool>> predicate, bool includeDeleted, params string[] includes);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        T First();
        T First(Expression<Func<T, bool>> predicate);
        ICollection<T> Where(Expression<Func<T, bool>> predicate);
        ICollection<T> Where(Expression<Func<T, bool>> predicate, bool includeDeleted);
    }
}