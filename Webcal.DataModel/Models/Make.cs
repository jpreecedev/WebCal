namespace Webcal.DataModel
{
    public abstract class Make : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}