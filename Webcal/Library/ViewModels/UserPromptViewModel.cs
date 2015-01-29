namespace Webcal.Library.ViewModels
{
    using System.Windows.Input;
    using Shared;

    public class UserPromptViewModel : BaseNotification
    {
        public string FirstPrompt { get; set; }

        public string FirstInput { get; set; }

        public string SecondPrompt { get; set; }

        public string SecondInput { get; set; }

        public ICommand AddSignatureCommand { get; set; }

        public bool HasSecondPrompt
        {
            get { return !string.IsNullOrEmpty(SecondPrompt); }
        }
    }
}