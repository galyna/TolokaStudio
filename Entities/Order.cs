using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TolokaStudio.Entities
{
   public class Order
    {
        public virtual int Id { get; protected set; }
        public virtual Product Product { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual string ContactEmail { get; set; }
 
    }
}
