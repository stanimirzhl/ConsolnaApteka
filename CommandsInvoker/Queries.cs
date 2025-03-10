using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;

namespace CommandsInvoker
{
    public class Queries
    {
        private string connection;

        public Queries(string database)
        {
            connection = $@"Server=.\SQLEXPRESS;Database={database};Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
            //connection = $@"Server=(localdb)\ProjectModels;Database={database};Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
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
                string queryForMedicine = "SELECT c.category_name, m.medicine_name, pm.dosage,  pm.quantity,  m.price, (pm.quantity * m.price) AS total_medicine_sales FROM sales AS s JOIN prescriptions AS p ON s.prescription_id = p.id JOIN prescription_medicines AS pm ON p.id = pm.prescription_id JOIN medicines AS m ON pm.medicine_id = m.id JOIN categories AS c ON m.category_id = c.id WHERE s.sale_date BETWEEN @startDate AND @endDate ORDER BY total_medicine_sales DESC;";
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

                            double maxAmount = 0;
                            if (!readerTotalSales.HasRows)
                            {
                                Console.WriteLine("No found report on sales for the given period, try again or try new command" + "\n");
                                return;
                            }
                            while (readerTotalSales.Read())
                            {
                                string categoryName = readerTotalSales["category_name"].ToString();
                                double totalCategorySales = (double)readerTotalSales.GetDecimal(1);
                                maxAmount += totalCategorySales;

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
                                    Console.WriteLine();
                                }
                            }
                            Console.WriteLine($"The whole sold amount: {maxAmount}" + "\n");
                        }
                    }


                }
            }
        }
        public void SelectTheMedicinesThatAreLowOnStock(int quantity)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select medicine_name,[description] ,stock_quantity from medicines where stock_quantity < @quantity order by stock_quantity asc;";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@quantity", quantity);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medicine/s found with quantity under the given one, try again with higher quantity or new command." + "\n");
                            return;
                        }
                        Console.WriteLine("Medicines with less quantity than the given one:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Medicine's name: {reader["medicine_name"]}, Description: {reader["description"]}, Quantity left: {reader["stock_quantity"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectTotalSoldMedicineForAllTimes()
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select m.medicine_name,m.[description],pm.dosage ,sum(pm.quantity) as sold_quantity,sum(pm.quantity * m.price) as total_sales from prescription_medicines as pm join medicines as m on pm.medicine_id = m.id join prescriptions as p on pm.prescription_id = p.id join sales as s on p.id = s.prescription_id group by m.medicine_name, m.[description], pm.dosage order by total_sales asc;";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medicine/s have been sold yet, try again later or try new command." + "\n");
                            return;
                        }
                        double wholePrice = 0;
                        Console.WriteLine("Sold medicine/s:");
                        while (reader.Read())
                        {
                            string message = $"Medicine's name: {reader["medicine_name"]}, Description: {reader["description"]}, Dosage: {reader["dosage"]}, Sold quantity per medicine: {reader["sold_quantity"]}, Total sales per medicine: {reader["total_sales"]}";
                            string newMsg = message.Replace(", ", "\n");
                            Console.WriteLine(newMsg + "\n");
                            wholePrice += (double)reader.GetDecimal(4);
                        }
                        Console.WriteLine($"Total sales for all medicines: {wholePrice}" + "\n");
                    }
                }
            }
        }
        public void SelectAllTheMedicinesPrescribedByGivenDoctor(string doctor)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select m.medicine_name, m.[description], pm.dosage from doctors as d join prescriptions as p on p.doctor_id = d.id join prescription_medicines as pm on p.id = pm.prescription_id join medicines as m on pm.medicine_id = m.id group by m.medicine_name, m.[description], pm.dosage, d.doctor_name having d.doctor_name = @doctor";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@doctor", $"Dr. {doctor}");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("The desired doctor does not exist or has not yet prescribed medicine to a patient, try again later or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("The prescribed medicine/s by the given doctor:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Medicine's name: {reader["medicine_name"]}, Description: {reader["description"]}, Prescribed dosage: {reader["dosage"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectTheMostPrescribedMedicine()
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select top(1) m.medicine_name, count(pm.medicine_id) as prescription_count from prescription_medicines as pm join medicines as m on pm.medicine_id = m.id group by m.medicine_name order by prescription_count desc;";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No medicine/s has/ve been yet prescribed, try again later or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("The most prescribed medicine:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Medicine's name: {reader["medicine_name"]}, Count of prescriptions: {reader["prescription_count"]}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        public void SelectAllThePatientsWhoGotTheSpecificMedicine(string medicine)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select pt.patient_name,pt.date_of_birth ,pt.email, pt.phone, p.prescription_date, count(*) as prescription_count from patients as pt join prescriptions as p on pt.id = p.patient_id join prescription_medicines as pm on p.id = pm.prescription_id join medicines as m on pm.medicine_id = m.id group by pt.patient_name,pt.date_of_birth ,pt.email, pt.phone, m.medicine_name, p.prescription_date having m.medicine_name = @medicine;";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@medicine", medicine);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("The desired medicine does not exist or no one has bought it yet, try again later or try new command." + "\n");
                            return;
                        }
                        Console.WriteLine("Patient/s who bought the specific medicine:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Patient's name: {reader["patient_name"]}, Date of birth: {reader.GetDateTime(1).ToString("yyyy-MM-dd")}, Patient's email: {reader["email"]}, Patient's phone number: {reader["phone"]}, Prescription date: {reader.GetDateTime(4).ToString("yyyy-MM-dd")}, Prescription count: {reader["prescription_count"]}");
                        }
                    }
                    Console.WriteLine();
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
        public List<string> GetAllDoctors()
        {
            var doctors = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select doctor_name from doctors";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctors.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return doctors;
        }
        public List<string> GetAllMedicines()
        {
            var medicines = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select medicine_name from medicines";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicines.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return medicines;
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
        public List<string> GetAllTables()
        {
            var tableNames = new List<string>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                string query = "select table_name from information_schema.tables where table_name <> 'prescription_medicines'";
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return tableNames;
        }

        public int GetDoctorId(string name)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand command = new SqlCommand("select id from doctors where doctor_name = @name", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public int GetPatientId(string name)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand command = new SqlCommand("select id from patients where patient_name = @name", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public int GetCategoryId(string name)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand command = new SqlCommand("select id from categories where category_name = @name", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public int GetManufacturerId(string name)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand command = new SqlCommand("select id from manufacturers where manufacturer_name = @name", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public int GetMedicineId(string name)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand command = new SqlCommand("select id from medicines where medicine_name = @name", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public int GetMedicineQuantity(string name)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand("select stock_quantity from medicines where medicine_name = @name", sqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    return (int)command.ExecuteScalar();
                }
            }
        }
        public List<int> ShowUnBoughtPrescriptionsAndTheirMedicines()
        {
            List<int> ids = new List<int>();
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand command = new SqlCommand("select p.id, p.prescription_date, m.medicine_name, pm.dosage, pm.quantity\r\nfrom prescriptions as p\r\nleft join sales as s on p.id = s.prescription_id\r\nleft join prescription_medicines as pm on p.id = pm.prescription_id\r\nleft join medicines as m on pm.medicine_id = m.id\r\nwhere s.prescription_id is null and m.medicine_name is not null;", sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No prescriptions found.");
                            return new List<int>() { 0 };
                        }
                        Dictionary<int, (string date, List<string> medicine)> prescriptions = new Dictionary<int, (string, List<string>)>();
                        while (reader.Read())
                        {
                            if (!prescriptions.ContainsKey((int)reader[0]))
                            {
                                prescriptions[(int)reader[0]] = (reader.GetDateTime(1).ToString("yyyy-MM-dd"), new List<string>());
                            }
                            prescriptions[(int)reader[0]].medicine.Add($"Medicine name: {reader["medicine_name"].ToString()},Dosage: {reader["dosage"].ToString()},Quantity: {reader["quantity"].ToString()}");
                        }
                        foreach (var p in prescriptions)
                        {
                            ids.Add(p.Key);
                            Console.WriteLine($"Prescription id: {p.Key}, Prescription date: {p.Value.date}");
                            foreach (var m in p.Value.medicine)
                            {
                                Console.WriteLine(m);
                            }
                            Console.WriteLine(new string('-', 25));
                        }
                        Console.WriteLine();
                    }
                }
            }
            return ids;
        }

        public void AddCategory(string name, string description)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand check = new SqlCommand("select count(*) from categories where category_name = @name", sqlConnection))
                {
                    check.Parameters.AddWithValue("@name", name);
                    int count = (int)check.ExecuteScalar();
                    if (count > 0)
                    {
                        Console.WriteLine("Category already exists, try with new name or new command." + "\n");
                        return;
                    }
                }
                using (SqlCommand insert = new SqlCommand("insert into categories (category_name, category_description) values (@name, @description)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@name", name);
                    insert.Parameters.AddWithValue("@description", description);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Category added successfully!" + "\n");
            }
        }
        public void AddManufacturer(string name, string website, string email, string phone)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand check = new SqlCommand("select count(*) from manufacturers where manufacturer_name = @name or email = @email or phone = @phone or @website", sqlConnection))
                {
                    check.Parameters.AddWithValue("@name", name);
                    check.Parameters.AddWithValue("@email", email);
                    check.Parameters.AddWithValue("@phone", phone);
                    int count = (int)check.ExecuteScalar();
                    if (count > 0)
                    {
                        Console.WriteLine("Manufacturer already exists, try with different name, email, phone or website or new command." + "\n");
                        return;
                    }
                }
                using (SqlCommand insert = new SqlCommand("insert into manufacturers (manufacturer_name, website, email, phone) VALUES (@name, @website, @email, @phone)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@name", name);
                    insert.Parameters.AddWithValue("@website", website);
                    insert.Parameters.AddWithValue("@email", email);
                    insert.Parameters.AddWithValue("@phone", phone);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Manufacturer added successfully!" + "\n");
            }
        }
        public void AddDoctor(string name, string specialty, string email, string phone)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand check = new SqlCommand("select count(*) from doctors where doctor_name = @name or email = @email or phone = @phone", sqlConnection))
                {
                    check.Parameters.AddWithValue("@name", name);
                    check.Parameters.AddWithValue("@email", email);
                    check.Parameters.AddWithValue("@phone", phone);
                    int count = (int)check.ExecuteScalar();
                    if (count > 0)
                    {
                        Console.WriteLine("Doctor already exists, try with different name, email, phone or new command." + "\n");
                        return;
                    }
                }
                using (SqlCommand insert = new SqlCommand("insert into doctors (doctor_name, specialty, email, phone) VALUES (@name, @specialty, @email, @phone)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@name", name);
                    insert.Parameters.AddWithValue("@specialty", specialty);
                    insert.Parameters.AddWithValue("@email", email);
                    insert.Parameters.AddWithValue("@phone", phone);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Doctor added successfully!" + "\n");
            }
        }
        public void AddPatient(string name, DateTime date_of_birth, string email, string phone)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand check = new SqlCommand("select count(*) from patients where patient_name = @name or email = @email or phone = @phone", sqlConnection))
                {
                    check.Parameters.AddWithValue("@name", name);
                    check.Parameters.AddWithValue("@email", email);
                    check.Parameters.AddWithValue("@phone", phone);
                    int count = (int)check.ExecuteScalar();
                    if (count > 0)
                    {
                        Console.WriteLine("Patient already exists, try with different name, email, phone or new command." + "\n");
                        return;
                    }
                }
                using (SqlCommand insert = new SqlCommand("insert into patients (patient_name, date_of_birth, email, phone) VALUES (@name, @date, @email, @phone)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@name", name);
                    insert.Parameters.AddWithValue("@date", date_of_birth);
                    insert.Parameters.AddWithValue("@email", email);
                    insert.Parameters.AddWithValue("@phone", phone);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Patient added successfully!" + "\n");
            }
        }
        public int AddPrescription(int doctor_id, int patient_id, DateTime date_of_prescription)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand insert = new SqlCommand("insert into prescriptions (doctor_id, prescription_date,patient_id) VALUES (@doctor_id, @date, @patient_id); select scope_identity();", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@doctor_id", doctor_id);
                    insert.Parameters.AddWithValue("@date", date_of_prescription);
                    insert.Parameters.AddWithValue("@patient_id", patient_id);
                    int result = Convert.ToInt32(insert.ExecuteScalar());

                    Console.WriteLine("Prescription added successfully!" + "\n");

                    return result;
                }
            }
        }
        public void AddPrescriptionMedicine(int prescription_id, int medicine_id, string dosage, int quantity)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand insert = new SqlCommand("insert into prescription_medicines (prescription_id, medicine_id, dosage, quantity) VALUES (@prescription_id, @medicine_id, @dosage, @quantity)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@prescription_id", prescription_id);
                    insert.Parameters.AddWithValue("@medicine_id", medicine_id);
                    insert.Parameters.AddWithValue("@dosage", dosage);
                    insert.Parameters.AddWithValue("@quantity", quantity);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Medicine to prescription added successfully!" + "\n");
            }
        }
        public void AddMedicine(string medicine_name, string description, double price, int quantity, int category_id, int manufacturer_id)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand check = new SqlCommand("select count(*) from medicines where medicine_name = @name or description = @description", sqlConnection))
                {
                    check.Parameters.AddWithValue("@name", medicine_name);
                    check.Parameters.AddWithValue("@description", description);
                    int count = (int)check.ExecuteScalar();
                    if (count > 0)
                    {
                        Console.WriteLine("Medicine already exists, try with new name or description or new command." + "\n");
                        return;
                    }
                }
                using (SqlCommand insert = new SqlCommand("insert into medicines (medicine_name, description, price, stock_quantity,category_id,manufacturer_id) values (@name, @description, @price, @quantity,@category_id,@manufacturer_id)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@name", medicine_name);
                    insert.Parameters.AddWithValue("@description", description);
                    insert.Parameters.AddWithValue("@price", price);
                    insert.Parameters.AddWithValue("@quantity", quantity);
                    insert.Parameters.AddWithValue("@category_id", category_id);
                    insert.Parameters.AddWithValue("@manufacturer_id", manufacturer_id);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Medicine added successfully!" + "\n");
            }
        }
        public void AddSale(int id)
        {
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                using (SqlCommand insert = new SqlCommand("insert into sales (prescription_id) values (@id)", sqlConnection))
                {
                    insert.Parameters.AddWithValue("@id", id);
                    insert.ExecuteNonQuery();
                }

                Console.WriteLine("Sale added successfully!" + "\n");
            }
        }

    }
}
