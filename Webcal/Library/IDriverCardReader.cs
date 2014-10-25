namespace Webcal.Library
{
    using System;
    using EventArguments;

    public interface IDriverCardReader : IDisposable
    {
        EventHandler<DriverCardCompletedEventArgs> Completed { get; set; }
        EventHandler<DriverCardProgressEventArgs> Progress { get; set; }

        void FastRead(bool autoRead);
        void GetFullHistory();
        void GenerateDump();
    }
}