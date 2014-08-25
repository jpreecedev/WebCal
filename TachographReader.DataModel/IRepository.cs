namespace TachographReader.DataModel
{
    public interface IRepository
    {
        void Add<T>(T entity);

        void SaveChanges();
    }
}
