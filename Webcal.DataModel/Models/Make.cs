namespace Webcal.DataModel
{
    using Shared;
    using Shared.Core;

    public abstract class Make : BaseModel
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}