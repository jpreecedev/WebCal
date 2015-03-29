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
        public DelegateCommand<object> ExportCommand { get; set; }
        public DelegateCommand<object> RemoveCommand { get; set; }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            EmptyFieldsCommand = new DelegateCommand<Grid>(OnEmptyFields);
            AddStoredFileCommand = new DelegateCommand<Grid>(OnAddStoredFile);
            ExportCommand = new DelegateCommand<object>(OnExport);
            RemoveCommand = new DelegateCommand<object>(OnRemove);
        }

        protected virtual void OnAddStoredFile()
        {
        }

        protected virtual void OnStoredFileRemoved()
        {
        }

        protected virtual void OnEmptyFields()
        {
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

            OnEmptyFields();
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