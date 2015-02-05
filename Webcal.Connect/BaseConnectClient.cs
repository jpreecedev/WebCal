namespace Webcal.Connect
{
    using System;
    using System.ServiceModel;
    using Webcal.Shared.Connect;

    public class BaseConnectClient
    {
        protected IConnectOperationResult Try(Action action)
        {
            try
            {
                action();
            }
            catch (CommunicationException e)
            {
                ForceClose();

                FaultException fe = null;
                Exception tmp = e;

                while (tmp != null)
                {
                    fe = tmp as FaultException;
                    if (fe != null)
                    {
                        break;
                    }
                    tmp = tmp.InnerException;
                }

                if (fe != null)
                {
                    return new FailureOperationResult(string.Format("The server sent back a fault: {0}", fe.CreateMessageFault().Reason.GetMatchingTranslation().Text));
                }

                return new FailureOperationResult(string.Format("The request failed with exception: {0}", e));
            }
            catch (TimeoutException)
            {
                ForceClose();
                return new FailureOperationResult("The request timed out");
            }
            catch (Exception e)
            {
                ForceClose();
                return new FailureOperationResult(string.Format("The request failed with unexpected exception: {0}", e));
            }

            return new SuccessOperationResult();
        }

        public virtual void ForceClose()
        {
        }
    }
}