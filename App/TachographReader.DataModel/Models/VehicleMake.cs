using System.Collections.ObjectModel;

namespace Webcal.DataModel
{
    public class VehicleMake : Make
    {
        private ObservableCollection<VehicleModel> _models;

        public ObservableCollection<VehicleModel> Models
        {
            get { return _models ?? (_models = new ObservableCollection<VehicleModel>()); }
            set { _models = value; }
        }
    }
}