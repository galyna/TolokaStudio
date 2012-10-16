using TolokaStudio.Entities;
using FluentNHibernate.Mapping;

namespace TolokaStudio.Mappings
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Price);
            HasManyToMany(x => x.StoresStockedIn)
                .Cascade.SaveUpdate()
                .Table("StoreProduct")
                .Inverse();

            Component(x => x.Location);
            References(x => x.OwnerEmployee);
            Map(x => x.HtmlBanner).CustomSqlType("nvarchar(max)");
            Map(x => x.HtmlDetail).CustomSqlType("ntext");
            Map(x => x.ImagePath);
            Map(x => x.ImagePathDetail);
        }
    }
}