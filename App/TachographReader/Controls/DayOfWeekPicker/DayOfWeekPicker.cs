using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Webcal.DataModel;

namespace Webcal.Controls.DayOfWeekPicker
{
    public class DayOfWeekPicker : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        public ICollection<CustomDayOfWeek> DaysOfWeek
        {
            get { return (ICollection<CustomDayOfWeek>)GetValue(DaysOfWeekProperty); }
            set { SetValue(DaysOfWeekProperty, value); }
        }

        public static readonly DependencyProperty DaysOfWeekProperty =
            DependencyProperty.Register("DaysOfWeek", typeof(ICollection<CustomDayOfWeek>), typeof(DayOfWeekPicker), new PropertyMetadata(null));

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
