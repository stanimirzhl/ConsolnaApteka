using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Medicines : BaseModel
    {
        static string tableQuery = @"CREATE TABLE medicines (
        id INT PRIMARY KEY identity,
        medicine_name VARCHAR(100) NOT NULL,
        [description] varchar(255) not null,
        price DECIMAL(8, 2) not null,
        stock_quantity INT not null,
        category_id INT not null,
        manufacturer_id INT not null,
        FOREIGN KEY (category_id) REFERENCES categories(id),
        FOREIGN KEY (manufacturer_id) REFERENCES manufacturers(id));";

        static string tableValuesQuery = @"INSERT INTO medicines (medicine_name, description, price, stock_quantity, category_id, manufacturer_id) 
        VALUES 
        ('Ibuprofen', 'Pain reliever and anti-inflammatory', 10.99, 100, 1, 1), 
        ('Amoxicillin', 'Antibiotic for bacterial infections', 15.49, 200, 2, 2), 
        ('Vitamin C', 'Vitamin supplement for immunity', 7.99, 150, 3, 3), 
        ('Paracetamol', 'Pain reliever for mild to moderate pain', 8.99, 120, 1, 1), 
        ('Cough Syrup', 'Cough suppressant for cold and flu', 12.49, 80, 4, 2);";
        public Medicines(SqlConnection connection) : base("medicines", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
