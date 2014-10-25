namespace Webcal.Windows.ProgressWindow
{
    using Properties;
    using Shared;

    public class ProgressWindowViewModel : BaseNotification
    {
        public ProgressWindowViewModel()
        {
            ProgressText = Resources.TXT_PLEASE_WAIT;
        }

        public string ProgressText { get; set; }
    }
}