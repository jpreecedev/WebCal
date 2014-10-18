namespace Webcal.DataModel
{
    using Shared;

    public class Technician : BaseModel
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public override string ToString()
        {
            if (IsDefault)
            {
                return string.Format("{0} (Default)", Name);
            }

            return Name;
        }
    }
}