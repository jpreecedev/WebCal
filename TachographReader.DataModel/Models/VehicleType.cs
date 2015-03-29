namespace TachographReader.DataModel
{
    using System.Collections.ObjectModel;
    using Properties;

    public static class VehicleType
    {
        public static ObservableCollection<string> GetVehicleTypes()
        {
            return new ObservableCollection<string>
            {
                Resources.TXT_GOODS_VEHICLE,
                Resources.TXT_PASSENGER_VEHICLE
            };
        }
    }
}