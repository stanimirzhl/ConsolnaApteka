using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Patients : BaseModel
    {

        static string tableQuery = @"CREATE TABLE patients (
        id INT PRIMARY KEY identity,
        patient_name VARCHAR(100) NOT NULL,
        date_of_birth DATE default getdate(),
        email VARCHAR(255) not null,
        phone VARCHAR(14) not null);";

        static string tableValuesQuery = @"INSERT INTO patients (patient_name, date_of_birth, email, phone) 
        VALUES 
        ('Alice Cooper', '1985-03-15', 'alice@domain.com', '08555111223'),
        ('Bob Marley', '1992-07-25', 'bob.marley@domain.com', '+359555555123'),
        ('Charlie Brown', '2000-01-05', 'charlie@domain.com', '08555222334');";
        public Patients(SqlConnection connection) : base("patients", tableQuery, tableValuesQuery, connection)
        {
        }
    }
}
