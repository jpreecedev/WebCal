namespace TachographReader.Views
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Core;
    using Library;
    using Shared;
    using Resources = Shared.Properties.Resources;

    public class M1N1ViewModel : BaseNavigationViewModel
    {
        public DelegateCommand<Grid> GenerateM1N1Command { get; set; }
        public ICollection<string> DocumentTypes { get; set; }
        public M1N1Document M1N1Document { get; set; }

        protected override void InitialiseCommands()
        {
            GenerateM1N1Command = new DelegateCommand<Grid>(OnGenerateM1N1);
        }

        protected override void Load()
        {
            DocumentTypes = new List<string>
            {
                Resources.TXT_LABEL_INITIAL_CALIBRATION,
                Resources.TXT_LABEL_MINOR_WORK,
                Resources.TXT_LABEL_RECALIBRATION,
                Resources.TXT_LABEL_TWO_YEAR_INSPECTION,
                Resources.TXT_LABEL_SIX_YEAR_CALIBRATION,
                Resources.TXT_LABEL_DIGITAL_INITIAL_CALIBRATION,
                Resources.TXT_LABEL_DIGITAL_TWO_YEAR
            };

            M1N1Document = new M1N1Document();
        }

        private void OnGenerateM1N1(Grid root)
        {
            if (!IsValid(root))
            {
                ShowError(Properties.Resources.EXC_MISSING_FIELDS);
                return;
            }

            LabelHelper.Print(M1N1Document);
            ShowMessage(Properties.Resources.TXT_LABEL_ADDED_TO_QUEUE, Properties.Resources.TXT_PRINT);
        }
    }
}