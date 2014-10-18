namespace Webcal.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Library;
    using Shared;

    public class SettingsRepository<T> : BaseRepository, ISettingsRepository<T> where T : BaseSettings
    {
        public T Get(Func<T, bool> filter, params string[] includes)
        {
            List<T> settings = Safely(() => Context.Set<T>().WithIncludes(Context, includes).ToList());

            if (settings.Count == 0)
            {
                return null;
            }
            if (settings.Count == 1)
            {
                return settings.First();
            }

            if (filter != null)
            {
                return settings.FirstOrDefault(filter);
            }

            return settings.FirstOrDefault();
        }

        public void Save(T settings)
        {
            Safely(() =>
            {
                Context.Set<T>().Attach(settings);
                Context.Entry(settings).State = EntityState.Modified;
                Context.SaveChanges();
            });
        }
    }
}