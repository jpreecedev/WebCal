namespace Webcal.Library.ViewModels
{
    using Shared;

    public class UserPromptViewModel : BaseNotification
    {
        public string FirstPrompt { get; set; }

        public string FirstInput { get; set; }

        public string SecondPrompt { get; set; }

        public string SecondInput { get; set; }

        public bool HasSecondPrompt
        {
            get { return !string.IsNullOrEmpty(SecondPrompt); }
        }
    }
}