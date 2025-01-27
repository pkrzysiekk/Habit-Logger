﻿using System.Data.SQLite;
using System.Globalization;


namespace HabitLogger
{
    public static class DBController
    {
        private static readonly string connectionString = @"Data Source=habit-Tracker.db";

        static DBController()
        {
            InitializeDB();
        }

        private static void InitializeDB()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
                );";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static void InsertData()
        {
            string date = InputHelpers.GetDateInput();
            int quantity = InputHelpers.GetNumberInput("Insert number of glasses or other measure of your choice (no decimals)\n\n");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"INSERT INTO drinking_water(Date,Quantity) VALUES ('{date}', {quantity})";
                tableCmd.ExecuteNonQuery();
                connection.Close();


            }

        }
        public static void Delete()
        {
            Console.Clear();
            GetAllRecords();
            var recordId = InputHelpers.GetNumberInput("\n\n Select the ID of the record you wish to delete or 0 to return to menu");
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id={recordId}";
                int rowCount = tableCmd.ExecuteNonQuery();
                if (rowCount == 0)
                {
                    Console.WriteLine("The record doesn't exist");
                    Delete();
                }
                Console.WriteLine("Record deleted");
                connection.Close();


            }
        }
        public static void GetAllRecords()
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
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
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
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MMM-yyyy")} - Quantity: {dw.Quantity}");

                }
                Console.WriteLine("--------------------------------\n");

            }
        }
        public static void Update()
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
                if (checkValue == 0)
                {
                    Console.WriteLine("This records does not exist");
                    connection.Close();
                    Update();
                }

                string date = InputHelpers.GetDateInput();
                int quantity = InputHelpers.GetNumberInput("Insert number of glasses or other measure of your choice (no decimals)\n\n");
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"UPDATE drinking_water SET Date='{date}',
                Quantity={quantity} 
                WHERE Id= {recordId}";
                tableCmd.ExecuteNonQuery();
                connection.Close();


            }

        }

    }
}
