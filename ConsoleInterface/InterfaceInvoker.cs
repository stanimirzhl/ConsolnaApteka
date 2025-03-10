using CommandsInvoker;
using Database;
using Microsoft.SqlServer.Server;
using System.Globalization;
namespace ConsoleInterface
{
    public class InterfaceInvoker
    {
        private static Queries queries;
        public static void MainInterface()
        {
            Console.WriteLine("Welcome to the Mareshki's pharmacy, do you wish to create new database: Y/N");
            string answer = Console.ReadLine().ToLower();
            while (answer != "y" && answer != "n")
            {
                Console.WriteLine("Invalid format try again with: Y/N");
                answer = Console.ReadLine().ToLower();
            }
            bool IsAnswered = answer == "y" ? true : false;
            if (IsAnswered)
            {
                Console.WriteLine("Choose the name of the database below:");
                string name = Console.ReadLine();
                DatabaseInvoker.CreateDb(name, true);
                queries = new Queries(name);
            }
            else
            {
                Console.WriteLine("Write the name of the existent database below:");
                string name = Console.ReadLine();
                DatabaseInvoker.CreateDb(name, false);
                queries = new Queries(name);
                Console.WriteLine();
            }
            Console.WriteLine("Below you will be given list of commands you can perform on the database. You can perform them until you write 0." + "\n");

            Console.WriteLine("0. Exiting the Program");
            Console.WriteLine("1. View All Medicines by Category");
            Console.WriteLine("2. View All Medicines by Manufacturer");
            Console.WriteLine("3. View Prescriptions for a Specific Patient");
            Console.WriteLine("4. View Sales for a Specific Date");
            Console.WriteLine("5. View Total Spendings by a Specific Patient");
            Console.WriteLine("6. View Total Sales Within a Date Range");
            Console.WriteLine("7. View Medicines with Low Stock");
            Console.WriteLine("8. View Total Sold Medicines");
            Console.WriteLine("9. View Medicines Prescribed by a Specific Doctor");
            Console.WriteLine("10. View the Most Prescribed Medicine");
            Console.WriteLine("11. View Patients Who Bought a Specific Medicine");
            Console.WriteLine("12. Add New Data to a Table" + "\n");

            int number;
            while (true)
            {
                string stringNumber = Console.ReadLine();
                while (!int.TryParse(stringNumber, out number))
                {
                    Console.WriteLine("Invalid type. Try with a number");
                    stringNumber = Console.ReadLine();
                }
                switch (number)
                {
                    case 1:
                        var categories = queries.GetAllCategories();
                        Console.WriteLine($"Available categories you can choose from: {string.Join(", ", categories)}");
                        string category = Console.ReadLine();
                        while (!categories.Contains(category))
                        {
                            Console.WriteLine($"Invalid category, please choose from the available ones: {string.Join(", ", categories)}");
                            category = Console.ReadLine();
                        }
                        queries.SelectAllMedicineByGivenCategory(category);
                        break;
                    case 2:
                        var manufacturers = queries.GetAllManufacturers();
                        Console.WriteLine($"Available manufacturers you can choose from: {string.Join(", ", manufacturers)}");
                        string manufacturer = Console.ReadLine();
                        while (!manufacturers.Contains(manufacturer))
                        {
                            Console.WriteLine($"Invalid manufacturer, please choose from the available ones: {string.Join(", ", manufacturers)}");
                            manufacturer = Console.ReadLine();
                        }
                        queries.SelectAllMedicineByGivenManufacturerName(manufacturer);
                        break;
                    case 3:
                        var patients = queries.GetAllPatients();
                        Console.WriteLine($"All patients in the register: {string.Join(", ", patients)}, choose one whose prescription you would like to see");
                        string patient = Console.ReadLine();
                        while (!patients.Contains(patient))
                        {
                            Console.WriteLine($"Inexistent patient name, try again with the given ones: {string.Join(", ", patients)}");
                            patient = Console.ReadLine();
                        }
                        queries.SelectAllPrescriptionsAndDoctorsThatWroteThePrescriptionForTheSpecificPatient(patient);
                        break;
                    case 4:
                        Console.WriteLine($"All the dates that a sale had taken place: {string.Join(", ", queries.GetAllSaleDates())}, choose one you want to see more details of(in format yyyy-MM-dd)");
                        string date = Console.ReadLine();
                        DateTime parsedDate;
                        while (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                        {
                            Console.WriteLine("Invalid format, please keep this one yyyy-MM-dd.");
                            date = Console.ReadLine();
                        }
                        queries.SelectAllSalesPrescriptionsAndMedicineInformationForTheSpecificDate(parsedDate);
                        break;
                    case 5:
                        var patientsToDisplay = queries.GetAllPatients();
                        Console.WriteLine($"All patients in the register: {string.Join(", ", patientsToDisplay)}, choose one whose total spendings on medicine you would like to see");
                        string patientToSearch = Console.ReadLine();
                        while (!patientsToDisplay.Contains(patientToSearch))
                        {
                            Console.WriteLine($"Inexistent patient name, try again with the given ones: {string.Join(", ", patientsToDisplay)}");
                            patientToSearch = Console.ReadLine();
                        }
                        queries.SelectTheWholeAmountOfSalesAGivenPatientHasMade(patientToSearch);
                        break;
                    case 6:
                        Console.WriteLine($"All the dates that a sale had taken place: {string.Join(", ", queries.GetAllSaleDates())}, choose the range you want to see the total sales of(in format yyyy-MM-dd, yyyy-MM-dd)");
                        string[] dateRange = Console.ReadLine().Split(", ", StringSplitOptions.RemoveEmptyEntries);
                        DateTime startDate;
                        DateTime endDate;
                        while (dateRange.Length != 2 || !DateTime.TryParseExact(dateRange[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) || !DateTime.TryParseExact(dateRange[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate) || startDate > endDate)
                        {
                            Console.WriteLine("Invalid format. Please use the correct format: yyyy-MM-dd, yyyy-MM-dd, or first type the earlier date");
                            dateRange = Console.ReadLine().Split(", ", StringSplitOptions.RemoveEmptyEntries);
                        }
                        queries.SelectTheTotalSoldInARangeBetweenTwoDates(startDate, endDate);
                        break;
                    case 7:
                        Console.WriteLine("Write the quantity to see which medicine/s has/ve less than the given one:");
                        string quantity = Console.ReadLine();
                        int parsedQuantity;
                        while (!int.TryParse(quantity, out parsedQuantity))
                        {
                            Console.WriteLine("Invalid type. Try with a number");
                            quantity = Console.ReadLine();
                        }
                        queries.SelectTheMedicinesThatAreLowOnStock(parsedQuantity);
                        break;
                    case 8:
                        Console.WriteLine("You will be given a list of the sold medicine/s if there were any sold:");
                        queries.SelectTotalSoldMedicineForAllTimes();
                        break;
                    case 9:
                        var doctors = queries.GetAllDoctors();
                        Console.WriteLine($"All doctors in the register: {string.Join(", ", doctors)}, choose one whose prescribed medicine/s you would like to see");
                        string doctor = Console.ReadLine();
                        queries.SelectAllTheMedicinesPrescribedByGivenDoctor(doctor);
                        break;
                    case 10:
                        queries.SelectTheMostPrescribedMedicine();
                        break;
                    case 11:
                        var medicines = queries.GetAllMedicines();
                        Console.WriteLine($"All available medicines in the register: {string.Join(", ", medicines)}, choose one to see who has bought it:" + "\n");
                        string medicine = Console.ReadLine();
                        queries.SelectAllThePatientsWhoGotTheSpecificMedicine(medicine);
                        break;
                    case 12:
                        Console.WriteLine($"Choose the table you want to add new data to:");
                        Console.WriteLine(string.Join("\n", queries.GetAllTables())); 
                        string table = Console.ReadLine();
                        switch (table)
                        {
                            case "categories":
                                Console.WriteLine("Write category name:");
                                string categoryName = Console.ReadLine();
                                Console.WriteLine("Description of the new category:");
                                string description = Console.ReadLine();
                                queries.AddCategory(categoryName, description);
                                break;
                            case "manufacturers":
                                Console.WriteLine("Write manufacturer name:");
                                string manufacturerName = Console.ReadLine();
                                Console.WriteLine("Website of the new manufacturer:");
                                string website = Console.ReadLine();
                                Console.WriteLine("Email of the new manufacturer:");
                                string email = Console.ReadLine();
                                Console.WriteLine("Phone number of the new manufacturer:");
                                string phoneNumber = Console.ReadLine();
                                queries.AddManufacturer(manufacturerName, website, email, phoneNumber);
                                break;
                            case "doctors":
                                Console.WriteLine("Write doctor name:");
                                string doctorName = Console.ReadLine();
                                Console.WriteLine("Specialty of the new doctor:");
                                string specialty = Console.ReadLine();
                                Console.WriteLine("Email of the new doctor:");
                                string emailOfTheNewDoctor = Console.ReadLine();
                                Console.WriteLine("Phone number of the new doctor:");
                                string doctorNumber = Console.ReadLine();
                                queries.AddDoctor(doctorName, specialty, emailOfTheNewDoctor, doctorNumber);
                                break;
                            case "patients":
                                Console.WriteLine("Write patient name:");
                                string patientName = Console.ReadLine();
                                Console.WriteLine("Date of birth of the new patient:");
                                string dateOfBirth = Console.ReadLine();
                                DateTime parsedBirthDate;
                                while (!DateTime.TryParseExact(dateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedBirthDate))
                                {
                                    Console.WriteLine("Invalid format, please keep this one yyyy-MM-dd.");
                                    dateOfBirth = Console.ReadLine();
                                }
                                Console.WriteLine("Email of the new patient:");
                                string emailOfThePatient = Console.ReadLine();
                                Console.WriteLine("Phone number of the new patient:");
                                string patientNumber = Console.ReadLine();
                                queries.AddPatient(patientName, parsedBirthDate, emailOfThePatient, patientNumber);
                                break;
                            case "prescriptions":
                                Console.WriteLine($"All doctors in the register: {string.Join(", ", queries.GetAllDoctors())}, choose one you would like to add into the prescription");
                                string doctorToFind = Console.ReadLine();
                                doctorToFind = $"Dr. {doctorToFind}";
                                while (!queries.GetAllDoctors().Contains(doctorToFind))
                                {
                                    Console.WriteLine($"Inexistent doctor name, try again with the given ones: {string.Join(", ", queries.GetAllDoctors())}");
                                    doctorToFind = Console.ReadLine();
                                    doctorToFind = $"Dr. {doctorToFind}";
                                }
                                Console.WriteLine();
                                Console.WriteLine($"All patients in the register: {string.Join(", ", queries.GetAllPatients())}, choose one you would like to add into the prescription");
                                string patientToFind = Console.ReadLine();
                                while (!queries.GetAllPatients().Contains(patientToFind))
                                {
                                    Console.WriteLine($"Inexistent patient name, try again with the given ones: {string.Join(", ", queries.GetAllPatients())}");
                                    patientToFind = Console.ReadLine();
                                }
                                Console.WriteLine();
                                Console.WriteLine("Date of the prescription format (yyyy-MM-dd):");
                                string dateOfPrescription = Console.ReadLine();
                                DateTime parsedPrescription;
                                while (!DateTime.TryParseExact(dateOfPrescription, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedPrescription))
                                {
                                    Console.WriteLine("Invalid format, please keep this one yyyy-MM-dd.");
                                    dateOfPrescription = Console.ReadLine();
                                }
                                Console.WriteLine();
                                int prescriptionId = queries.AddPrescription(queries.GetDoctorId(doctorToFind), queries.GetPatientId(patientToFind), parsedPrescription);
                                
                                int medicineQuantity = 0;
                                List<(int, string, int)> prescriptionDescription = new List<(int, string, int)>(); 

                                Console.WriteLine("Now, choose from the existing medicines in the pharmacy the ones that need to be included in the prescription until the command 'Enough':");

                                Console.WriteLine($"All medicines in the register: {string.Join(", ", queries.GetAllMedicines())}, choose one you would like to add into the prescription, write in the format (Medicine_Name-Dosage-Quantity), the dosage should be written as (integer)mg:");

                                while (true)
                                {
                                    string[] values = Console.ReadLine().Split("-", StringSplitOptions.RemoveEmptyEntries);
                                    if (values[0] == "Enough")
                                    {
                                        break;
                                    }
                                    if (values.Length == 3)
                                    {
                                        if (!queries.GetAllMedicines().Contains(values[0]))
                                        {
                                            Console.WriteLine($"Medicine {values[0]} does not exist, try again with these: {string.Join(", ", queries.GetAllMedicines())}");
                                        }
                                        else
                                        {
                                            medicineQuantity = queries.GetMedicineQuantity(values[0]);
                                            if (medicineQuantity < int.Parse(values[2]))
                                            {
                                                Console.WriteLine("There isn't enough quantity for the desired medicine, try again later or with less quantity!");
                                            }
                                            else
                                            {
                                                prescriptionDescription.Add((queries.GetMedicineId(values[0]), values[1], int.Parse(values[2])));
                                                Console.WriteLine($"Choose again: {string.Join(", ", queries.GetAllMedicines())} or exit with the command 'Enough':");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid format, please keep the (Medicine_Name-Dosage-Quantity)");
                                    }
                                    
                                }

                                foreach (var med in prescriptionDescription)
                                {
                                    queries.AddPrescriptionMedicine(prescriptionId, med.Item1, med.Item2, med.Item3);
                                }
                                break;
                            case "medicines":
                                Console.WriteLine("Write medicine name:");
                                string medicineName = Console.ReadLine();
                                Console.WriteLine("Description of the new medicine:");
                                string medicineDescription = Console.ReadLine();
                                Console.WriteLine("Price of the new medicine:");
                                double price = double.Parse(Console.ReadLine());
                                Console.WriteLine("Quantity of the new medicine:");
                                int quantityMedicine = int.Parse(Console.ReadLine());
                                Console.WriteLine($"All categories in the register: {string.Join(", ", queries.GetAllCategories())}, choose one you would like to add into the medicine");
                                string categoryToFind = Console.ReadLine();
                                while (!queries.GetAllCategories().Contains(categoryToFind))
                                {
                                    Console.WriteLine($"Inexistent category name, try again with the given ones: {string.Join(", ", queries.GetAllCategories())}");
                                    categoryToFind = Console.ReadLine();
                                }
                                Console.WriteLine();
                                Console.WriteLine($"All manufacturers in the register: {string.Join(", ", queries.GetAllManufacturers())}, choose one you would like to add into the medicine");
                                string manufacturerToFind = Console.ReadLine();
                                while (!queries.GetAllManufacturers().Contains(manufacturerToFind))
                                {
                                    Console.WriteLine($"Inexistent manufacturer name, try again with the given ones: {string.Join(", ", queries.GetAllManufacturers())}");
                                    manufacturerName = Console.ReadLine();
                                }
                                Console.WriteLine();
                                queries.AddMedicine(medicineName, medicineDescription,price, quantityMedicine, queries.GetCategoryId(categoryToFind), queries.GetManufacturerId(manufacturerToFind));
                                break;
                            case "sales":
                                List<int> ids = queries.ShowUnBoughtPrescriptionsAndTheirMedicines();
                                Console.WriteLine($"Choose an Id from the given prescriptions to buy:");
                                int id = int.Parse(Console.ReadLine());
                                while (!ids.Contains(id))
                                {
                                    Console.WriteLine($"Try again with an Id that exists:");
                                    id = int.Parse(Console.ReadLine());
                                }
                                queries.AddSale(id);
                                break;
                            default:
                                Console.WriteLine("No such table, try again with a valid table name!" + "\n");
                                break;
                        }
                        break;
                    case 0:
                        Console.WriteLine("Exiting the program....");
                        return;
                    default:
                        Console.WriteLine("Invalid operation, please choose one from the list above!");
                        break;
                }
            }
        }
    }
}
