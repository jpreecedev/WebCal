using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Webcal.Controls;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Properties;

namespace Webcal.Core
{
    public class BaseFilesViewModel : BaseMainViewModel
    {
        #region Constructor

        public BaseFilesViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            StoredFiles = new ObservableCollection<BaseFile>();
            SelectedDate = DateTime.Now;
        }

        #endregion

        #region Public Properties

        public string FilePath { get; set; }

        public DateTime SelectedDate { get; set; }

        public ObservableCollection<BaseFile> StoredFiles { get; set; }

        public BaseFile SelectedStoredFile { get; set; }

        #endregion

        #region Overrides

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

        #endregion

        #region Commands

        #region Command : Empty Fields

        public DelegateCommand<Grid> EmptyFieldsCommand { get; set; }

        private void OnEmptyFields(Grid grid)
        {
            IList<BaseInputField> inputFields = grid.FindVisualChildren<BaseInputField>().ToList();
            IList<BaseInputTextField> inputTextFields = grid.FindVisualChildren<BaseInputTextField>().ToList();
            IList<BaseInputDatePickerField> inputDatePickers = grid.FindVisualChildren<BaseInputDatePickerField>().ToList();

            foreach (BaseInputField inputField in inputFields)
                inputField.Clear();

            foreach (BaseInputTextField inputTextField in inputTextFields)
                inputTextField.OnClear();

            foreach (BaseInputDatePickerField inputDatePicker in inputDatePickers)
                inputDatePicker.Clear();

            OnEmptyFields();
        }

        #endregion

        #region Command : Add Stored File

        public DelegateCommand<Grid> AddStoredFileCommand { get; set; }

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

        #endregion

        #region Command : Export Command

        public DelegateCommand<object> ExportCommand { get; set; }

        private void OnExport(object obj)
        {
            if (SelectedStoredFile == null) return;

            DialogHelperResult dialogResult = DialogHelper.SaveFile(DialogFilter.All, SelectedStoredFile.FileName);
            if (dialogResult.Result == true)
            {
                File.WriteAllBytes(dialogResult.FileName, SelectedStoredFile.SerializedFile);
            }
        }

        #endregion

        #region Command : Remove

        public DelegateCommand<object> RemoveCommand { get; set; }

        private void OnRemove(object arg)
        {
            if (SelectedStoredFile == null)
                return;

            OnStoredFileRemoved();
            StoredFiles.Remove(SelectedStoredFile);
        }

        #endregion

        #endregion
    }
}
