namespace TachographReader.Windows.ProgressWindow
{
    using Core;
    using Properties;

    public class ProgressWindowViewModel : BaseModalWindowViewModel
    {
        public ProgressWindowViewModel()
        {
            ProgressText = Resources.TXT_PLEASE_WAIT;
        }

        public string ProgressText { get; set; }
    }
}