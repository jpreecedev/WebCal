namespace TachographReader.DataModel.Library
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
                return (bool)attribute;
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
                var value = attribute.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
                return value;
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
                var value = element.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Trim();
                }
                return value;
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
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            var objectSet = objectContext.CreateObjectSet<T>();

            var query = (ObjectQuery<T>)objectSet;

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
                                       !string.IsNullOrEmpty(w.Address3) ||
                                       !string.IsNullOrEmpty(w.Office) ||
                                       !string.IsNullOrEmpty(w.PostCode) ||
                                       !string.IsNullOrEmpty(w.Town) ||
                                       !string.IsNullOrEmpty(w.WorkshopName), "CustomDayOfWeeks");
        }

        public static PrinterSettings GetPrinterSettings(this ISettingsRepository<PrinterSettings> repository)
        {
            return repository.Get(w => !string.IsNullOrEmpty(w.DefaultPrinterName));
        }

        public static ThemeSettings GetThemeSettings(this ISettingsRepository<ThemeSettings> repository)
        {
            var themeSettings = repository.Get(w => !string.IsNullOrEmpty(w.SelectedTheme));
            if (themeSettings == null)
            {
                using (var context = new TachographContext())
                {
                    themeSettings = context.ThemeSettings.Add(new ThemeSettings { SelectedTheme = "Silver" });
                    context.SaveChanges();
                }
            }
            return themeSettings;
        }

        public static MiscellaneousSettings GetMiscellaneousSettings(this ISettingsRepository<MiscellaneousSettings> repository)
        {
            return repository.Get(w => w.IsDeleted == false);
        }
    }
}