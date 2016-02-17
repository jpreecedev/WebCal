namespace TachographReader.Controls.DayOfWeekPicker
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}