using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Core.Data.Entities;

namespace Core.Data.Mappings
{
    public class UserMap : ClassMap<User>
    {
      public UserMap()
        {
            Id(x => x.Id);
            Map(x => x.UserName);
            Map(x => x.Email);
         
          }           
    }
}
