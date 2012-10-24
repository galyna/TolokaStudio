using System.Collections.Generic;

namespace Core.Data.Entities
{
    public class Product
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual Employee OwnerEmployee { get; set; }
        public virtual Store Store { get; set; }
        public virtual string HtmlBannerOrdered { get; set; }
        public virtual string HtmlBannerOrderedNot { get; set; }
        public virtual string HtmlBanner { get; set; }
        public virtual string HtmlDetail { get; set; }
        public virtual string ImagePath { get; set; }
    }
}