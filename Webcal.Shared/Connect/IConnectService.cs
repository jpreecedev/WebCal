namespace Webcal.Shared.Connect
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IConnectService
    {
        [OperationContract]
        void Connect();

        [OperationContract]
        void Close();
    }
}