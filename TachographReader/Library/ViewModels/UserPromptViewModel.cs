namespace TachographReader.Library.ViewModels
{
    using System;
    using System.Windows.Input;
    using Connect.Shared.Models;

    public class UserPromptViewModel : BaseNotification
    {
        public string FirstPrompt { get; set; }

        public string FirstInput { get; set; }

        public string SecondPrompt { get; set; }

        public string SecondInput { get; set; }

        public string DatePrompt { get; set; }

        public DateTime? DateInput { get; set; }

        public string SecondDatePrompt { get; set; }

        public DateTime? SecondDateInput { get; set; }

        public ICommand AddSignatureCommand { get; set; }

        public bool HasSecondPrompt
        {
            get { return !string.IsNullOrEmpty(SecondPrompt); }
        }

        public bool HasDatePrompt
        {
            get { return !string.IsNullOrEmpty(DatePrompt); }
        }

        public bool HasSecondDatePrompt
        {
            get { return !string.IsNullOrEmpty(SecondDatePrompt); }
        }
    }
}