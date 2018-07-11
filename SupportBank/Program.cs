using System;
using System.Collections.Generic;
using System.Linq;



namespace SupportBank
{
    class Program
    {
        static void List_all(List<string> people, int entries, string[] lines)
        {
            foreach (string human in people)                       // Now we go through each person and find out how much they owe/are owed
            {
                double owed = 0;    // Start owed at 0
                double owes = 0;    // Start owes at 0
                for (int i = 1; i <= entries; i++)  // Go through each transaction
                {
                    string[] transaction = lines[i].Split(',');  // Again, split each transaction into pieces
                    if (transaction[1] == human)    // If the person is the ower...
                        owes += Convert.ToDouble(transaction[4]);    //... add it to owes
                    if (transaction[2] == human)   // If the person is the owee (is that a word?)...
                        owed += Convert.ToDouble(transaction[4]);    //... add it to owed
                }
                double total = owes - owed;
                Console.WriteLine("{0} owes {1} and is owed {2}. In total, they owe {3}", human, owes, owed, total);   // List every person and their total owed/owes
            }
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            string whole_file = System.IO.File.ReadAllText(@"C:\Work\Training\SupportBank\Transactions2014.csv");
            // Reads CSV file, stores it as one long string called whole_file
            whole_file = whole_file.Replace('\n', '\r');
            // Replaces '\n' in the new file by '\r'
            string[] lines = whole_file.Split(new char[] { '\r' },
                 StringSplitOptions.RemoveEmptyEntries);
            // Splits the whole file into lines, wherever there is an instance of '\r', and removes any empty entries
            int entries = lines.Length - 1;  // create a variable to know how many transactions we're dealing with

            List<string> people = new List<string>();    // Here we aim to just list everybody involved
            for (int i = 1; i <= entries; i++)  // Look at each transaction in turn
            {
                string[] transaction = lines[i].Split(',');       // Split the transactions into their different components
                people.Add(transaction[1]);                       // Add whoever owes to the list
                people.Add(transaction[2]);                       // Add whoever is owed to the list
            }
            people = people.Distinct().ToList();                  // Remove any duplicates

            bool running = true;
            while (running == true)
            {
                Console.WriteLine("Enter a command (List All or Exit): ");
                string input = Console.ReadLine(); // Get user input for the command"
                if (input == "List All")
                    List_all(people, entries, lines);
                else if (input == "Exit")
                    running = false;
                else
                    Console.WriteLine("Invalid command, try again");
            }


        }
    }


    
    public class Debt
    {
        public string Date;
        public string From;
        public string To;
        public float Amount;
    }

    public class Person
    {
        public float Debts_to_others;
        public float Debts_from_others;
    }
}
