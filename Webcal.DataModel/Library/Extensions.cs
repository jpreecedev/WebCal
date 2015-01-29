namespace Webcal.DataModel.Library
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Xml.Linq;
    using Shared;

    public static class Extensions
    {
        public static bool SafelyGetValueAsBool(this XAttribute attribute)
        {
            try
            {
                return (bool) attribute;
            }
            catch
            {
                return false;
            }
        }

        public static string SafelyGetValue(this XAttribute attribute)
        {
            try
            {
                return attribute.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SafelyGetValue(this XElement element)
        {
            try
            {
                return element.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DateTime SafelyGetValueAsDateTime(this XElement element)
        {
            try
            {
                DateTime parsed;
                if (DateTime.TryParse(element.Value, out parsed))
                {
                    return parsed;
                }

                return default(DateTime);
            }
            catch
            {
                return default(DateTime);
            }
        }

        public static double SafelyGetValueAsDouble(this XElement element)
        {
            try
            {
                double parsed;
                if (double.TryParse(element.Value, out parsed))
                {
                    return parsed;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static IQueryable<T> WithIncludes<T>(this IQueryable<T> source, DbContext context, params string[] associations) where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter) context).ObjectContext;
            ObjectSet<T> objectSet = objectContext.CreateObjectSet<T>();

            var query = (ObjectQuery<T>) objectSet;

            foreach (var assoc in associations)
            {
                query = query.Include(assoc);
            }

            return query;
        }

        public static WorkshopSettings GetWorkshopSettings(this ISettingsRepository<WorkshopSettings> repository)
        {
            return repository.Get(w => !string.IsNullOrEmpty(w.Address1) ||
                                       !string.IsNullOrEmpty(w.Address2) ||
                                       !string.IsNullOrEmpty(w.Office) ||
                                       !string.IsNullOrEmpty(w.PostCode) ||
                                       !string.IsNullOrEmpty(w.Town) ||
                                       !string.IsNullOrEmpty(w.WorkshopName));
        }

        public static PrinterSettings GetPrinterSettings(this ISettingsRepository<PrinterSettings> repository)
        {
            return repository.Get(w => !string.IsNullOrEmpty(w.DefaultPrinterName));
        }

        public static ThemeSettings GetThemeSettings(this ISettingsRepository<ThemeSettings> repository)
        {
            return repository.Get(w => !string.IsNullOrEmpty(w.SelectedTheme));            
        }

        public static MiscellaneousSettings GetMiscellaneousSettings(this ISettingsRepository<MiscellaneousSettings> repository)
        {
            return repository.Get(w => w.IsDeleted == false);
        }
    }
}