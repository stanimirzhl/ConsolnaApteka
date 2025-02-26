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
