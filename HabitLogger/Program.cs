using HabitLogger.Helper_Methods;
using System.Data.SQLite;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HabitLogger
{
    public class Program
    {
      private static string connectionString = @"Data Source=habit-Tracker.db";

        static void Main(string[] args)
        {

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var tableCmd= connection.CreateCommand();
                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
                );";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
            GetUserInput();
        }
        static void InsertData()
        {
            string date = InputHelpers.GetDateInput();
            int quantity = InputHelpers.GetNumberInput("Insert number of glasses or other measure of your choice (no decimals)\n\n");

            using(var connection=  new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd= connection.CreateCommand();
                tableCmd.CommandText = $"INSERT INTO drinking_water(Date,Quantity) VALUES ('{date}', {quantity})";
                tableCmd.ExecuteNonQuery();
                connection.Close();


            }

        }
        static void Delete()
        {
            Console.Clear();
            GetAllRecords();
            var recordId = InputHelpers.GetNumberInput("\n\n Select the ID of the record you wish to delete or 0 to return to menu");
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id={recordId}";
                int rowCount=tableCmd.ExecuteNonQuery();
                if (rowCount == 0)
                {
                    Console.WriteLine("The record doesn't exist");
                    Delete();
                }
                Console.WriteLine("Record deleted");
                connection.Close();


            }
        }
        static void GetAllRecords()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "SELECT * FROM drinking_water";
                List<DrinkingWater> tableData = new();
                SQLiteDataReader reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date=DateTime.ParseExact(reader.GetString(1),"dd-MM-yy",new CultureInfo ("en-US")),
                                Quantity = reader.GetInt32(2)
                                
                            }
                            );
                            
                            
                    }

                }
                else
                {
                    Console.WriteLine("No data!");
                }
                connection.Close();

                Console.WriteLine("--------------------------------\n");
                foreach(var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MMM-yyyy")} - Quantity: {dw.Quantity}");

                }
                Console.WriteLine("--------------------------------\n");

            }
        }
        static void Update()
        {
            Console.Clear();
            GetAllRecords();
            var recordId = InputHelpers.GetNumberInput("\n\nType the id of the record you want to edit");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id={recordId})";
                int checkValue = Convert.ToInt32(checkCmd.ExecuteScalar());
                if(checkValue == 0)
                {
                    Console.WriteLine("This records does not exist");
                    connection.Close();
                    Update();
                }

                string date = InputHelpers.GetDateInput();
                int quantity = InputHelpers.GetNumberInput("Insert number of glasses or other measure of your choice (no decimals)\n\n");
                var tableCmd= connection.CreateCommand();
                tableCmd.CommandText = @$"UPDATE drinking_water SET Date='{date}',
                Quantity={quantity} 
                WHERE Id= {recordId}";
                tableCmd.ExecuteNonQuery();
                connection.Close();


            }

        }
       public static void GetUserInput()
        {
            Console.Clear();

            bool closeApp=false;

            while (!closeApp)
            {
                Console.WriteLine("\n\nMenu:");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to close the app");
                Console.WriteLine("Type 1 to View all Records");
                Console.WriteLine("Type 2 to Inser Record");
                Console.WriteLine("Type 3 to Delete Record");
                Console.WriteLine("Type 4 to Update Record");
                Console.WriteLine("----------------------------\n"); 

                string commandInput= Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("Goodbye!");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        InsertData();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;

                }
            }
        }
    }
}
