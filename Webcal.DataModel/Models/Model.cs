namespace Webcal.DataModel
{
    using Connect.Shared.Models;
    using Shared;
    using Shared.Core;

    public abstract class Model : BaseModel
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}