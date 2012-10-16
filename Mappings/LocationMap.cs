using TolokaStudio.Entities;
using FluentNHibernate.Mapping;

namespace TolokaStudio.Mappings
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