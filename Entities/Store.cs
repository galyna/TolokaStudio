using System.Collections.Generic;

namespace TolokaStudio.Entities
{
    public class Store
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }      
        public virtual string Description { get; set; }

        public virtual IList<Product> Products { get; set; }
        public virtual IList<Employee> Staff { get; set; }
        public virtual string HtmlBanner { get; set; }
        public virtual string ImagePath { get; set; }
       
       
        public Store()
        {
            Products = new List<Product>();
            Staff = new List<Employee>();
        }

        public virtual Product AddProduct(Product product)
        {
            product.StoresStockedIn.Add(this);
            Products.Add(product);
            return product;
        }

        public virtual IList<Product> GetProducts()
        {          
           return Products;
        }  

        public virtual void AddEmployee(Employee employee)
        {
            employee.Store = this;
            Staff.Add(employee);
        }
    }
}