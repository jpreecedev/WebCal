namespace TachographReader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Controls;
    using DataModel;
    using Library;
    using Properties;
    using TachographReader.EventArguments;

    public class BaseFilesViewModel : BaseMainViewModel
    {
        public BaseFilesViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            StoredFiles = new ObservableCollection<BaseFile>();
            SelectedDate = DateTime.Now;
        }

        public string FilePath { get; set; }
        public DateTime SelectedDate { get; set; }
        public ObservableCollection<BaseFile> StoredFiles { get; set; }
        public BaseFile SelectedStoredFile { get; set; }
        public DelegateCommand<Grid> EmptyFieldsCommand { get; set; }
        public DelegateCommand<Grid> AddStoredFileCommand { get; set; }
        public DelegateCommand<object> ShowDetailsCommand { get; set; }
        public DelegateCommand<object> ExportCommand { get; set; }
        public DelegateCommand<object> RemoveCommand { get; set; }

        public bool IsReadFromCardEnabled { get; set; }
        public DelegateCommand<object> ReadFromCardCommand { get; set; }
        public string ReadFromCardContent { get; set; }
        public IDriverCardReader DriverCardReader { get; set; }

        public bool IsFormEnabled { get; set; }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            EmptyFieldsCommand = new DelegateCommand<Grid>(OnEmptyFields);
            AddStoredFileCommand = new DelegateCommand<Grid>(OnAddStoredFile);
            ShowDetailsCommand = new DelegateCommand<object>(OnShowDetails);
            ExportCommand = new DelegateCommand<object>(OnExport);
            RemoveCommand = new DelegateCommand<object>(OnRemove);
            ReadFromCardCommand = new DelegateCommand<object>(OnReadFromCard);
        }

        protected virtual void OnShowFileDetails()
        {            
        }

        protected virtual void OnAddStoredFile()
        {
        }

        protected virtual void OnStoredFileRemoved()
        {
        }

        protected virtual void OnReadComplete(string dumpFilePath)
        {
        }

        protected override void Load()
        {
            base.Load();

            IsFormEnabled = true;
            ReadFromCardContent = Resources.TXT_WORKSHOP_CARD_FILES_READ_FROM_CARD;

            DriverCardReader = new DriverCardReader();
            DriverCardReader.Completed += Completed;
            IsReadFromCardEnabled = true;
        }

        private void OnReadFromCard(object obj)
        {
            ReadFromCardContent = Resources.TXT_READING;
            IsReadFromCardEnabled = false;
            DriverCardReader.GenerateDump();
        }

        private void Completed(object sender, DriverCardCompletedEventArgs e)
        {
            ReadFromCardContent = Resources.TXT_WORKSHOP_CARD_FILES_READ_FROM_CARD;
            IsReadFromCardEnabled = true;

            if (!e.IsSuccess)
            {
                ShowError(Resources.TXT_UNABLE_READ_SMART_CARD);
                return;
            }

            OnReadComplete(e.DumpFilePath);
        }

        private void OnEmptyFields(Grid grid)
        {
            IList<BaseInputField> inputFields = grid.FindVisualChildren<BaseInputField>().ToList();
            IList<BaseInputTextField> inputTextFields = grid.FindVisualChildren<BaseInputTextField>().ToList();
            IList<BaseInputDatePickerField> inputDatePickers = grid.FindVisualChildren<BaseInputDatePickerField>().ToList();

            foreach (BaseInputField inputField in inputFields)
            {
                inputField.Clear();
            }

            foreach (BaseInputTextField inputTextField in inputTextFields)
            {
                inputTextField.OnClear();
            }

            foreach (BaseInputDatePickerField inputDatePicker in inputDatePickers)
            {
                inputDatePicker.Clear();
            }

            IsFormEnabled = true;
            IsReadFromCardEnabled = true;
        }

        private void OnAddStoredFile(Grid grid)
        {
            if (!IsValid(grid))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            OnAddStoredFile();
            SelectedStoredFile = null;
            IsFormEnabled = true;
        }

        private void OnShowDetails(object obj)
        {
            if (SelectedStoredFile == null)
            {
                return;
            }

            OnShowFileDetails();
        }

        private void OnExport(object obj)
        {
            if (SelectedStoredFile == null)
            {
                return;
            }

            DialogHelperResult dialogResult = DialogHelper.SaveFile(DialogFilter.All, SelectedStoredFile.FileName);
            if (dialogResult.Result == true)
            {
                File.WriteAllBytes(dialogResult.FileName, SelectedStoredFile.SerializedFile);
            }
        }

        private void OnRemove(object arg)
        {
            if (SelectedStoredFile == null)
            {
                return;
            }

            OnStoredFileRemoved();
            StoredFiles.Remove(SelectedStoredFile);
        }
    }
}