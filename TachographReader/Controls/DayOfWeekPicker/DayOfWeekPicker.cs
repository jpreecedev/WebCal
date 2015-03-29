namespace TachographReader.Controls.DayOfWeekPicker
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using DataModel;

    public class DayOfWeekPicker : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DaysOfWeekProperty =
            DependencyProperty.Register("DaysOfWeek", typeof (ICollection<CustomDayOfWeek>), typeof (DayOfWeekPicker), new PropertyMetadata(null));

        public ICollection<CustomDayOfWeek> DaysOfWeek
        {
            get { return (ICollection<CustomDayOfWeek>) GetValue(DaysOfWeekProperty); }
            set { SetValue(DaysOfWeekProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}