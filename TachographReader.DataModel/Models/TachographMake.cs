namespace TachographReader.DataModel
{
    using System.Collections.ObjectModel;

    public class TachographMake : Make
    {
        private ObservableCollection<TachographModel> _models;

        public ObservableCollection<TachographModel> Models
        {
            get { return _models ?? (_models = new ObservableCollection<TachographModel>()); }
            set { _models = value; }
        }
    }
}