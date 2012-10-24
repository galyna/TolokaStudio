using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Data.Entities;

namespace TolokaStudio.Models
{
    public class BascetModel
    {
        public IList<Order> Orders { get; set; }
        public string Comments { get; set; }
        public string Message { get; set; }
        public int Order { get; set; }
        public User User { get; set; }
    }
    public class BascetOrder
    {
        public IList<Order> Orders { get; set; }
        public string Comments { get; set; }
        public string Message { get; set; }
        public Order Order { get; set; }
        public User User { get; set; }

    }
}