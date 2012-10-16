using TolokaStudio.Entities;
using FluentNHibernate.Mapping;

namespace TolokaStudio.Mappings
{
    public class StoreMap : ClassMap<Store>
    {
        public StoreMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.HtmlBanner).CustomSqlType("nvarchar(max)");
            Map(x => x.ImagePath);
            HasManyToMany(x => x.Products)
                .Cascade.All()
                .Table("StoreProduct");
                
             
            HasMany(x => x.Staff)
                .Cascade.All()
                .Inverse();
        }
    }
}