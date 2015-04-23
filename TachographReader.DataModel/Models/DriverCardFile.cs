namespace TachographReader.DataModel
{
    using Connect.Shared.Models;

    public class DriverCardFile : BaseFile
    {
        public CustomerContact Customer { get; set; }
        public string Driver { get; set; }
    }
}