using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.Models.Contexts;

namespace InitialRound.Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter administrator's username: ");
            var adminUsername = Console.ReadLine();
            Console.Write("Enter administrator's password: ");
            var adminPassword = Console.ReadLine();
            Console.Write("Enter administrator's email address: ");
            var adminEmail = Console.ReadLine();
            Console.Write("Enter administrator's first name: ");
            var adminFirstName = Console.ReadLine();
            Console.Write("Enter administrator's last name: ");
            var adminLastName = Console.ReadLine();

            Console.Write("Enter database server name: ");
            var dbServer = Console.ReadLine();
            Console.Write("Enter database name: ");
            var dbName = Console.ReadLine();
            Console.Write("Enter database user name: ");
            var dbUsername = Console.ReadLine();
            Console.Write("Enter database password: ");
            var dbPassword = Console.ReadLine();

            try
            {
                string connectionString = string.Format(
                    "Server=tcp:{0}.database.windows.net,1433;Database={1};User ID={2}@{0};Password={3};Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",
                    dbServer,
                    dbName,
                    dbUsername,
                    dbPassword);

                var context = DataController.CreateDbContext(connectionString);

                UserController.CreateUser(context, adminUsername, adminPassword, adminEmail, adminFirstName, adminLastName, true);

                context.SaveChanges();

                Console.WriteLine("Done");
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
