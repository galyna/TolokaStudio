namespace TolokaStudio.Entities
{
    public class Location
    {
        public virtual int Id { get; protected set; }
        public virtual int Aisle { get; set; }
        public virtual int Shelf { get; set; }
    }
}