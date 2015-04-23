namespace TachographReader.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Library;
    using Shared;
    using Shared.Core;

    public class SettingsRepository<T> : BaseRepository, ISettingsRepository<T> where T : BaseSettings
    {
        public T Get(Func<T, bool> filter, params string[] includes)
        {
            List<T> settings = Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    return context.Set<T>().WithIncludes(context, includes).ToList();
                }
            });

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
                using (var context = new TachographContext())
                {
                    context.Set<T>().Attach(settings);
                    context.Entry(settings).State = EntityState.Modified;
                    context.SaveChanges();
                }
            });
        }
    }
}