using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Manufacturers : BaseModel
    {
        static string tableQuery = @"CREATE TABLE manufacturers (
        id INT PRIMARY KEY identity,
        manufacturer_name VARCHAR(100) NOT NULL,
        website VARCHAR(50) not null,
        email VARCHAR(255) not null,
        phone VARCHAR(14) not null);";

        static string tableValuesQuery = @"INSERT INTO manufacturers (manufacturer_name, website, email, phone) 
        VALUES 
        ('PharmaCorp', 'http://pharmacorp.com', 'contact@pharmacorp.com', '+359123456789'),
        ('MediTech', 'http://mediatech.com', 'info@mediatech.com', '08987654321'),
        ('HealthPlus', 'http://healthplus.com', 'support@healthplus.com', '08555123456');";
        public Manufacturers(SqlConnection connection) : base("manufacturers", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
