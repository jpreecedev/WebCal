using System.Collections.ObjectModel;
using Webcal.DataModel.Properties;

namespace Webcal.DataModel
{
    public static class VehicleType
    {
        #region Public Methods

        public static ObservableCollection<string> GetVehicleTypes()
        {
            return new ObservableCollection<string>
                       {
                          Resources.TXT_GOODS_VEHICLE,
                          Resources.TXT_PASSENGER_VEHICLE 
                       };
        }

        #endregion
    }
}
