using System.Windows;
using System.Windows.Controls;

namespace TachographReader.Controls
{
    public class LineControl : ContentControl
    {
        static LineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineControl), new FrameworkPropertyMetadata(typeof(LineControl)));
        }
    }
}