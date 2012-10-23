﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.Entities
{
   public class Order
    {
        public virtual int Id { get; protected set; }
        public virtual Product Product { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual User User { get; set; }
        public virtual string Comments { get; set; }
        public virtual bool Ordered { get; set; }
        public virtual string OrderDateTime { get; set; } 
       
    }
}
