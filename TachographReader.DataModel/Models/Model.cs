namespace TachographReader.DataModel
{
    using Connect.Shared.Models;

    public abstract class Model : BaseModel
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}