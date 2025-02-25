using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Prescriptions : BaseModel
    {
        static string tableQuery = @"CREATE TABLE prescriptions (
        id INT PRIMARY KEY identity,
        patient_id INT not null,
        doctor_id INT not null,
        prescription_date DATE default getdate(),
        FOREIGN KEY (patient_id) REFERENCES patients(id),
        FOREIGN KEY (doctor_id) REFERENCES doctors(id));";

        static string tableValuesQuery = @"INSERT INTO prescriptions (patient_id, doctor_id, prescription_date) 
        VALUES 
        (1, 1, '2025-02-20'),
        (2, 2, '2025-02-21'),
        (3, 3, '2025-02-22');";
        public Prescriptions(SqlConnection connection) : base("prescriptions", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
