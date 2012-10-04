using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Data.Entities;
using FluentNHibernate.Mapping;

namespace Core.Data.Mappings
{
   public class WebTemplateMap: ClassMap<WebTemplate>
    {
       public WebTemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Html).CustomSqlType("nvarchar(max)");
        }
    }
}
