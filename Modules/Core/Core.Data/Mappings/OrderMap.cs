using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Data.Entities;
using FluentNHibernate.Mapping;

namespace Core.Data.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id);
            References(x => x.Employee);
            References(x => x.Product);
            References(x => x.User);
            Map(x => x.Comments);
        }
    }
}
