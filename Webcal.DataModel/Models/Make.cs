namespace Webcal.DataModel
{
    using Shared;

    public abstract class Make : BaseModel
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}