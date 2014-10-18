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

        public ICollection<User> Get(Expression<Func<User, bool>> predicate, params string[] includes)
        {
            throw new NotImplementedException();
        }

        public User FirstOrDefault(Expression<Func<User, bool>> predicate)
        {
            return _users.FirstOrDefault(predicate.Compile());
        }

        public User First(Expression<Func<User, bool>> predicate)
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

        public ICollection<User> Get(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}