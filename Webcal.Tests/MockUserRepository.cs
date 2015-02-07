namespace Webcal.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using DataModel;
    using Shared;

    public class MockUserRepository : IRepository<User>
    {
        private readonly IList<User> _users;

        public MockUserRepository()
        {
            _users = new List<User>();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdate(User entity)
        {
            throw new NotImplementedException();
        }

        public void Add(User entity)
        {
            _users.Add(entity);
        }

        public void Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> GetAll(params string[] includes)
        {
            return _users;
        }

        public ICollection<User> GetAll(bool includeDeleted, params string[] includes)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> Get(Expression<Func<User, bool>> predicate, params string[] includes)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> Get(Expression<Func<User, bool>> predicate, bool includeDeleted, params string[] includes)
        {
            throw new NotImplementedException();
        }

        public User FirstOrDefault(Expression<Func<User, bool>> predicate)
        {
            return _users.FirstOrDefault(predicate.Compile());
        }

        public User First()
        {
            throw new NotImplementedException();
        }

        public User First(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        ICollection<User> IRepository<User>.Where(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> Where(Expression<Func<User, bool>> predicate, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Where(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(User entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> Get(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}