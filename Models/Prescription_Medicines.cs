using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Prescription_Medicines : BaseModel
    {
        static string tableQuery = @"CREATE TABLE prescription_medicines (
        id INT PRIMARY KEY identity,
        prescription_id INT not null, 
        medicine_id INT not null,     
        dosage VARCHAR(255) not null,
        quantity INT not null,
        FOREIGN KEY (prescription_id) REFERENCES prescriptions(id),
        FOREIGN KEY (medicine_id) REFERENCES medicines(id),
        UNIQUE (prescription_id, medicine_id, dosage, quantity));";

        static string tableValuesQuery = @"INSERT INTO prescription_medicines (prescription_id, medicine_id, dosage, quantity)
        VALUES 
        (1, 1, '200mg', 2),
        (1, 2, '500mg', 1), 
        (2, 3, '250mg', 3),
        (3, 4, '500mg', 2);";
        public Prescription_Medicines(SqlConnection connection) : base("prescription_medicines", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
