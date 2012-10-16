using TolokaStudio.Entities;
using FluentNHibernate.Mapping;

namespace TolokaStudio.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Id(x => x.Id);
            Map(x => x.FirstName);
            Map(x => x.Name);
            Map(x => x.LastName);
            Map(x => x.Email);
            References(x => x.Store);
            Map(x => x.HtmlBanner).CustomSqlType("nvarchar(max)");
            Map(x => x.HtmlDetail).CustomSqlType("ntext");
            Map(x => x.ImagePath);
            HasMany(x => x.Orders)
                         .Cascade.All()
                         .Inverse();
        }
    }
}