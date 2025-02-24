using Database;
namespace ConsoleInterface
{
    public class InterfaceInvoker
    {
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
                DatabaseInvoker.CreateDb(name);
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine("Below you will be given list of commands you can perform on the database. You can perform them until you write 0.");

            while (true)
            {
                int number = int.Parse(Console.ReadLine());
                switch (number) 
                {
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid operation, please choose one from the list above!");
                        break;
                }
            }
        }
    }
}
