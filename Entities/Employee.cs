using System.Collections.Generic;
namespace TolokaStudio.Entities
{
    public class Employee
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual Store Store { get; set; }
        public virtual string HtmlBanner { get; set; }
        public virtual string HtmlDetail { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual string Email { get; set; }
        public virtual IList<Order> Orders { get; set; } 

    }
}