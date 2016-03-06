namespace TachographReader.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using Size = System.Drawing.Size;

    public static class Extensions
    {
        public static string SplitByCapitals(this string source)
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z])"));
        }

        public static bool IsBetween(this DateTime dateTime, DateTime start, DateTime end)
        {
            return dateTime >= start && dateTime <= end;
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null || items == null)
            {
                return;
            }

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static ICollection<T> Remove<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            if (collection == null || condition == null)
            {
                return collection;
            }

            List<T> itemsToRemove = collection.Where(condition).ToList();

            foreach (T itemToRemove in itemsToRemove)
            {
                collection.Remove(itemToRemove);
            }

            return collection;
        }

        public static ICollection<T> RemoveAt<T>(this ICollection<T> collection, int index)
        {
            if (index < 0 || index >= collection.Count)
            {
                return collection;
            }

            collection.Remove(collection.ElementAt(index));
            return collection;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count < 1;
        }

        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child == null)
            {
                return null;
            }

            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }

        public static ICollection<IValidate> FindValidatableChildren(this DependencyObject root)
        {
            if (root == null)
            {
                return null;
            }

            Type[] controls = Assembly.GetCallingAssembly().GetTypes().Where(type => Attribute.IsDefined(type, typeof(BaseControlAttribute))).ToArray();

            return FindVisualChildren(root, controls).Cast<IValidate>().ToList();
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                yield break;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        private static IEnumerable<object> FindVisualChildren(DependencyObject depObj, Type[] types)
        {
            if (depObj == null)
            {
                yield break;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && types.Any(t => t.IsInstanceOfType(child)))
                {
                    yield return child;
                }

                foreach (object childOfChild in FindVisualChildren(child, types))
                {
                    yield return childOfChild;
                }
            }
        }

        public static Bitmap ToBitmap(this BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public static BitmapSource ToBitmapSource(this Image image)
        {
            if (image == null)
            {
                return null;
            }

            var bitmap = new Bitmap(image);

            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public static Image Resize(this Image image, Size size)
        {
            return new Bitmap(image, size);
        }

        public static int ToInt(this string str)
        {
            int parsed;
            if (int.TryParse(str, out parsed))
            {
                return parsed;
            }

            return -1;
        }

        public static void EmptyFolder(this DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
        }

        public static T Downcast<T>(this object obj) where T : new()
        {
            var properties = typeof(T).GetProperties();
            var b = new T();

            foreach (var pi in properties)
            {
                if (pi.GetSetMethod() != null)
                {
                    pi.SetValue(b, pi.GetValue(obj, null), null);
                }
            }

            return b;
        }
        
        public static IEnumerable<Document> GetAllDocuments(this TachographContext context)
        {
            IEnumerable<Document> result = new List<Document>();

            result = result.Concat(GetDocuments<TachographDocument>(context));
            result = result.Concat(GetDocuments<UndownloadabilityDocument>(context));
            result = result.Concat(GetDocuments<LetterForDecommissioningDocument>(context));

            return result.OrderByDescending(c => c.InspectionDate.GetValueOrDefault());
        }

        public static IEnumerable<BaseReport> GetQCReports(this TachographContext context)
        {
            IEnumerable<BaseReport> result = new List<BaseReport>();

            result = result.Concat(GetDocuments<QCReport>(context));

            return result.OrderByDescending(c => c.Created.Date);
        }

        private static IEnumerable<T> GetDocuments<T>(DbContext context) where T : BaseModel
        {
            var documentCount = context.Set<T>().Count();
            if (documentCount > 0)
            {
                return from document in context.Set<T>()
                    where document.Deleted == null
                    select document;
            }

            return new List<T>();
        }

        public static ReportItemStatus QuarterlyStatus(this Technician technician)
        {
            if (technician == null || technician.DateOfLastCheck == null)
            {
                return ReportItemStatus.Unknown;
            }

            var lastCheck = technician.DateOfLastCheck.GetValueOrDefault();
            var nextCheckDue = lastCheck.AddMonths(3).AddDays(-7).Date;
            var expiration = lastCheck.AddMonths(3).Date;
            var now = DateTime.Now.Date;
            
            if (now >= nextCheckDue && now <= expiration)
            {
                return ReportItemStatus.CheckDue;
            }
            if (now >= lastCheck && now < nextCheckDue)
            {
                return ReportItemStatus.Ok;
            }
            return ReportItemStatus.Expired;
        }

        public static ReportItemStatus ThreeYearStatus(this Technician technician)
        {
            if (technician == null)
            {
                return ReportItemStatus.Unknown;
            }

            var lastCheck = technician.DateOfLastCheck.GetValueOrDefault().Date;
            var now = DateTime.Now.Date;
            if (lastCheck > now)
            {
                return ReportItemStatus.Unknown;
            }

            var expiration = lastCheck.Date.AddYears(3).Date;
            var checkDue = expiration.AddMonths(-1).Date;

            if (now >= checkDue && now <= expiration)
            {
                return ReportItemStatus.CheckDue;
            }
            if (now < checkDue)
            {
                return ReportItemStatus.Ok;
            }
            return ReportItemStatus.Expired;
        }
    }
}