﻿namespace TachographReader.DataModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Connect.Shared;
    using Library;
    using Shared;

    public class SettingsRepository<T> : BaseRepository, ISettingsRepository<T> where T : BaseSettings, new()
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

        public virtual void Save(T settings)
        {
            Safely(() =>
            {
                using (var context = new TachographContext())
                {
                    if (settings == null)
                    {
                        context.Set<T>().Add(new T());
                    }
                    else
                    {
                        context.Set<T>().Attach(settings);
                        context.Entry(settings).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            });
        }
    }
}