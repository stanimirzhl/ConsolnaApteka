using Microsoft.Data.SqlClient;

namespace CommandsInvoker
{
    public class Queries
    {
        private string connection;

        public Queries(string database)
        {
            connection = $@"Server=(localdb)\ProjectModels;Database={database};Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public void SelectAllMedicineByGivenCategory(string categoryName)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select m.medicine_name as med_name, m.[description] as med_desc from medicines as m join categories as c on m.category_id = c.id where c.category_name = @categoryName";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@categoryName", categoryName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medicine/s found by the given category, try again or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("Medicine/s by the given category:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Medicine: {reader["med_name"]}, Description: {reader["med_desc"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }

        public void SelectAllMedicineByGivenManufacturerName(string manName)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select m.medicine_name as med_name, m.[description] as med_desc from medicines as m join manufacturers as mf on m.manufacturer_id = mf.id where mf.manufacturer_name = @manName";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@manName", manName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medicine/s found by the given manufacturer's name, try again or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("Medicine/s by the given manufacturer name:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Medicine: {reader["med_name"]}, Description: {reader["med_desc"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectAllPrescriptionsAndDoctorsThatWroteThePrescriptionForTheSpecificPatient(string patientName)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select p.prescription_date as p_date, d.doctor_name as d_name from prescriptions as p join doctors as d on p.doctor_id = d.id join patients as pt on p.patient_id = pt.id where pt.patient_name = @patientName";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@patientName", patientName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No prescription/s found by the given patient's name, try again or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("Prescription/s by the given patient name:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Prescription date: {reader.GetDateTime(0).ToString("yyyy/MM/dd")}, Doctor's name: {reader["d_name"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectAllSalesPrescriptionsAndMedicineInformationForTheSpecificDate(DateTime date)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select s.sale_date, m.medicine_name, pm.dosage, pm.quantity, m.price ,(pm.quantity * m.price) as total_sold_amount from sales as s join prescriptions as p on s.prescription_id = p.id join prescription_medicines as pm on p.id = pm.prescription_id join medicines as m on pm.medicine_id = m.id where s.sale_date = @date";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@date", date);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No sale/s found by the given date, try again or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("Sale/s information by the given date:");
                        double totalForTheDay = 0;
                        while (reader.Read())
                        {
                            totalForTheDay += (double)reader.GetDecimal(4) * reader.GetInt32(3);
                            Console.WriteLine($"Sale date: {reader.GetDateTime(0).ToString("yyyy-MM-dd")}, medicine's name: {reader[1]}, dosage: {reader[2]}, quantity: {reader[3]}, price: {reader[4]}, total sold amount: {reader["total_sold_amount"]}");
                        }
                        Console.WriteLine($"Total for the day: {totalForTheDay}");
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectTheWholeAmountOfSalesAGivenPatientHasMade(string patient)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select sum(m.price * pm.quantity) as total from sales as s join prescriptions as p on s.prescription_id = p.patient_id join patients as pt on p.patient_id = pt.id join prescription_medicines AS pm ON p.id = pm.prescription_id join medicines AS m ON pm.medicine_id = m.id where pt.patient_name = @patient";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@patient", patient);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No sale/s found by the patient name, try again or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("The total amount spent by the given patient:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Amount: {reader["total"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectTheTotalSoldInARangeBetweenTwoDates(DateTime startDate, DateTime endDate)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string queryTotal = "SELECT c.category_name, SUM(pm.quantity * m.price) AS total_category_sales FROM sales AS s JOIN prescriptions AS p ON s.prescription_id = p.id JOIN prescription_medicines AS pm ON p.id = pm.prescription_id JOIN medicines AS m ON pm.medicine_id = m.id JOIN categories AS c ON m.category_id = c.id WHERE s.sale_date BETWEEN @startDate AND @endDate GROUP BY c.category_name ORDER BY total_category_sales DESC;";
                string queryForMedicine = "SELECT c.category_name, m.medicine_name, pm.dosage,  pm.quantity,  m.price, pm.quantity * m.price) AS total_medicine_sales FROM sales AS s JOIN prescriptions AS p ON s.prescription_id = p.id JOIN prescription_medicines AS pm ON p.id = pm.prescription_id JOIN medicines AS m ON pm.medicine_id = m.id JOIN categories AS c ON m.category_id = c.id WHERE s.sale_date BETWEEN @startDate AND @endDate ORDER BY total_medicine_sales DESC;";
                using (SqlCommand commandTotalForPeriod = new SqlCommand(queryTotal, sqlConnection))
                {
                    commandTotalForPeriod.Parameters.AddWithValue("@startDate", startDate);
                    commandTotalForPeriod.Parameters.AddWithValue("@endDate", endDate);
                    using (SqlCommand commandEachMedicinesCost = new SqlCommand(queryForMedicine, sqlConnection))
                    {
                        commandEachMedicinesCost.Parameters.AddWithValue("@startDate", startDate);
                        commandEachMedicinesCost.Parameters.AddWithValue("@endDate", endDate);
                        using (SqlDataReader readerTotalSales = commandTotalForPeriod.ExecuteReader())
                        {
                            Console.WriteLine($"Sales Report from {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}:");

                            while (readerTotalSales.Read())
                            {
                                string categoryName = readerTotalSales["category_name"].ToString();
                                double totalCategorySales = (double)readerTotalSales.GetDecimal(1);

                                Console.WriteLine($"Category: {categoryName}");
                                Console.WriteLine($"Total Sales for {categoryName}: {totalCategorySales:C}");

                                using (SqlDataReader readerMedicines = commandEachMedicinesCost.ExecuteReader())
                                {
                                    while (readerMedicines.Read())
                                    {
                                        string medicineCategoryName = readerMedicines["category_name"].ToString();
                                        if (medicineCategoryName == categoryName)
                                        {
                                            string medicineName = readerMedicines["medicine_name"].ToString();
                                            string dosage = readerMedicines["dosage"].ToString();
                                            int quantity = readerMedicines.GetInt32(readerMedicines.GetOrdinal("quantity"));
                                            decimal price = readerMedicines.GetDecimal(readerMedicines.GetOrdinal("price"));
                                            decimal totalSales = readerMedicines.GetDecimal(readerMedicines.GetOrdinal("total_medicine_sales"));

                                            Console.WriteLine($"{medicineName} ({dosage}) - Quantity: {quantity} - Price: {price:C} - Total Sales: {totalSales:C}");
                                        }
                                    }
                                }
                            }
                        }
                    }


                }
            }
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select category_name from categories";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return categories;
        }

        public List<string> GetAllManufacturers()
        {
            var manufacturers = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select manufacturer_name from manufacturers";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            manufacturers.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return manufacturers;
        }
        public List<string> GetAllPatients()
        {
            var patients = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select patient_name from patients";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patients.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return patients;
        }
        public List<string> GetAllSaleDates()
        {
            var saleDates = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select sale_date from sales";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            saleDates.Add(reader.GetDateTime(0).ToString("yyyy-MM-dd"));
                        }
                    }
                }
            }
            return saleDates;
        }
    }
}
