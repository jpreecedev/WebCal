namespace Webcal.DataModel
{
    using Connect.Shared.Models;

    public abstract class Make : BaseModel
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}