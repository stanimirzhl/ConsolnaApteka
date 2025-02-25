using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Doctors : BaseModel
    {
        static string tableQuery = @"CREATE TABLE doctors (
        id INT PRIMARY KEY identity,
        doctor_name VARCHAR(100) NOT NULL,
        specialty VARCHAR(100) not null, 
        email VARCHAR(255) not null,
        phone VARCHAR(14) not null);";

        static string tableValuesQuery = @"INSERT INTO doctors (doctor_name, specialty, email, phone) 
        VALUES 
        ('Dr. John Doe', 'Cardiologist', 'johndoe@hospital.com', '+359123987654'),
        ('Dr. Jane Smith', 'General Practitioner', 'janesmith@clinic.com', '08555333123'),
        ('Dr. Emily Johnson', 'Neurologist', 'emily.johnson@neuroclinic.com', '08987654321');";
        public Doctors(SqlConnection connection) : base("doctors", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
