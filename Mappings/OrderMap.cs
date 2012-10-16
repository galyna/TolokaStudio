using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TolokaStudio.Entities;
using FluentNHibernate.Mapping;

namespace TolokaStudio.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id);
            References(x => x.Employee);
            References(x => x.Product);
            Map(x => x.ContactEmail);
        }
    }
}
