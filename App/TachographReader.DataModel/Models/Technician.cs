namespace Webcal.DataModel
{
    public class Technician : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public override string ToString()
        {
            if (IsDefault)
                return string.Format("{0} (Default)", Name);

            return Name;
        }
    }
}