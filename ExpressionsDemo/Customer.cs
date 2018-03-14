using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace ExpressionsDemo
{
    [Table(Name="Customers")]
    public class Customer
    {
        [Column]
        public string CustomerID { get; set; }
        [Column]
        public string CompanyName { get; set; }
        [Column]
        public string ContactName { get; set; }
        [Column]
        public string Address { get; set; }
        [Column]
        public string City { get; set; }
        [Column]
        public string PostalCode { get; set; }
        [Column]
        public string Country { get; set; }
        [Column]
        public string Phone { get; set; }
        [Column]
        public string Fax { get; set; }
    }

    public class CustomerCollection:List<Customer>
    {
        public static CustomerCollection GetCustomers(object criteria)
        {
            CustomerCollection result = new CustomerCollection();
            DataContext db = new DataContext(@"Data Source=localhost\sql2008_r2; Initial Catalog=Northwind;Integrated Security=True;MultipleActiveResultSets=True");
            Table<Customer> table = db.GetTable<Customer>();
            System.Linq.Expressions.Expression<Func<Customer, bool>> exp = ExprGen.WhereExpression<Customer>(criteria);
            result.AddRange(table.Where(exp).AsEnumerable());
            return result;
        }
    }
}
