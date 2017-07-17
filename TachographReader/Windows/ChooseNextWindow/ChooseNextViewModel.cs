namespace TachographReader.Windows.ChooseNextWindow
{
    using System.Windows;
    using Core;
    using Properties;

    public class ChooseNextViewModel : BaseModalWindowViewModel
    {
        public ChooseNextViewModel()
        {
            NewTachographCommand = new DelegateCommand<Window>(OnNewTachograph);
            UndownloadabilityCommand = new DelegateCommand<Window>(OnUndownloadability);
            LetterForDecommissioningCommand = new DelegateCommand<Window>(OnLetterOnDecommisioning);
        }
        
        public DelegateCommand<Window> NewTachographCommand { get; set; }
        public DelegateCommand<Window> UndownloadabilityCommand { get; set; }
        public DelegateCommand<Window> LetterForDecommissioningCommand { get; set; }

        public string SelectedDocumentType { get; set; }
        
        private void OnNewTachograph(Window window)
        {
            SelectedDocumentType = Resources.TXT_DIGITAL_TACHOGRAPH_DOCUMENT;
            window.Close();
        }

        private void OnUndownloadability(Window window)
        {
            SelectedDocumentType = Resources.TXT_NEW_UNDOWNLOADABILITY_DOCUMENT;
            window.Close();
        }

        private void OnLetterOnDecommisioning(Window window)
        {
            SelectedDocumentType = Resources.TXT_LETTER_FOR_DECOMMISSIONING;
            window.Close();
        }
    }
}