using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Categories : BaseModel
    {
        static string tableQuery = @"CREATE TABLE categories (
        id INT PRIMARY KEY Identity,
        category_name VARCHAR(100) NOT NULL,
	    category_description varchar(255) not null);";

        static string tableValuesQuery = @"INSERT INTO categories (category_name, category_description) 
        VALUES 
        ('Painkillers', 'Medications used to relieve pain'),
        ('Antibiotics', 'Medications used to treat bacterial infections'),
        ('Vitamins', 'Supplemental nutrients for maintaining health'),
        ('Cold and Flu', 'Medications for treating cold and flu symptoms'),
        ('Topical', 'Medications applied to the skin');";

        public Categories(SqlConnection connection) : base("categories",tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
