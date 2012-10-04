using Core.Data.Entities;
using FluentNHibernate.Mapping;

namespace Core.Data.Mappings
{
    public class LocationMap : ComponentMap<Location>
    {
        public LocationMap()
        {
           // Id(x => x.Id);
            Map(x => x.Aisle);
            Map(x => x.Shelf);
        }
    }
}