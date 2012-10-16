using System.Collections.Generic;

namespace TolokaStudio.Entities
{
    public class Product
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual Employee OwnerEmployee { get; set; }
        public virtual Location Location { get; set; }
        public virtual IList<Store> StoresStockedIn { get; set; }
        public virtual string HtmlBanner { get; set; }
        public virtual string HtmlDetail { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual string ImagePathDetail { get; set; }
        public Product()
        {
            StoresStockedIn = new List<Store>();
        }
    }
}