namespace Webcal.DataModel
{
    using Shared;

    public abstract class Model : BaseModel
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}