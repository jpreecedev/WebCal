using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Webcal.Core;
using Image = System.Drawing.Image;
using Size = System.Windows.Size;

namespace Webcal.Library
{
    public static class Extensions
    {
        public static bool IsBetween(this DateTime dateTime, DateTime start, DateTime end)
        {
            return dateTime >= start && dateTime <= end;
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null || items == null) return;

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static ICollection<T> Remove<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            if (collection == null || condition == null) return collection;

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
                return collection;

            collection.Remove(collection.ElementAt(index));
            return collection;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count < 1;
        }

        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child == null) return null;

            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }

        public static ICollection<IValidate> FindValidatableChildren(this DependencyObject root)
        {
            if (root == null)
                return null;

            Type[] controls = Assembly.GetCallingAssembly().GetTypes().Where(type => Attribute.IsDefined(type, typeof(BaseControlAttribute))).ToArray();

            return FindVisualChildren(root, controls).Cast<IValidate>().ToList();
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

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
                yield break;

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

        public static BitmapSource ToBitmapSource(this Image image)
        {
            if (image == null)
                return null;

            Bitmap bitmap = new Bitmap(image);

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public static void Print(this FrameworkElement fe)
        {
            if (fe == null)
                return;

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() != true)
                return;

            fe.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    fe.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                                                    fe.Arrange(new Rect(fe.DesiredSize));
                                                    fe.UpdateLayout();
                                                }),
                                 DispatcherPriority.Render);

            int height = (int)pd.PrintableAreaHeight;
            int width = (int)pd.PrintableAreaWidth;
            int pages = (int)Math.Ceiling((fe.ActualHeight / height));

            FixedDocument document = new FixedDocument();

            for (int i = 0; i < pages; i++)
            {
                FixedPage page = new FixedPage
                                     {
                                         Height = height,
                                         Width = width
                                     };

                PageContent content = new PageContent { Child = page };

                document.Pages.Add(content);

                VisualBrush vb = new VisualBrush(fe)
                                     {
                                         AlignmentX = AlignmentX.Left,
                                         AlignmentY = AlignmentY.Top,
                                         Stretch = Stretch.None,
                                         TileMode = TileMode.None,
                                         Viewbox = new Rect(0, i * height, width, (i + 1) * height),
                                         ViewboxUnits = BrushMappingMode.Absolute
                                     };

                RenderOptions.SetBitmapScalingMode(vb, BitmapScalingMode.HighQuality);

                Canvas canvas = new Canvas
                                    {
                                        Background = vb,
                                        Height = height,
                                        Width = width
                                    };

                FixedPage.SetLeft(canvas, 0);
                FixedPage.SetTop(canvas, 0);

                page.Children.Add(canvas);
            }

            pd.PrintDocument(document.DocumentPaginator,
                             ((String.IsNullOrWhiteSpace(fe.Name) ? "Temp" : fe.Name) + " PRINT"));
        }

        public static Image Resize(this Image image, System.Drawing.Size size)
        {
            return new Bitmap(image, size);
        }

        public static int ToInt(this string str)
        {
            int parsed;
            if (int.TryParse(str, out parsed))
                return parsed;

            return -1;
        }
    }
}