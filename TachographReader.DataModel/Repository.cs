using System.Data.Entity;

namespace TachographReader.DataModel
{
    public class Repository : IRepository
    {
        #region Private Members

        private TachographContext _context;

        #endregion

        #region Constructor

        public Repository()
        {
            Database.SetInitializer(new TachographInitialiser());
            Context.Database.Initialize(true);
        }

        #endregion

        #region Public Properties

        public TachographContext Context
        {
            get { return _context ?? (_context = new TachographContext()); }
        }

        #endregion

        #region Public Methods

        public void Add<T>(T entity)
        {

        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        #endregion
    }
}
