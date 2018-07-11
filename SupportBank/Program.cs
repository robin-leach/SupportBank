﻿using System;
using System.Collections.Generic;
using System.Linq;



namespace SupportBank
{
    class Program
    {
        public class Debt
        {
            public string Date;
            public string From;
            public string To;
            public string Narrative;
            public double Amount;
        }

        static void List_all(List<string> people, int entries, string[] lines, Debt[] debts)
        {
            for(int n=0; n<people.Count; n++)                       // Now we go through each person and find out how much they owe/are owed
            {
                string current_person = people[n];
                double owed = 0;    // Start owed at 0
                double owes = 0;    // Start owes at 0
                for (int i = 0; i < entries; i++)  // Go through each transaction
                {
                    if (debts[i].From == current_person)     // If the person is the ower...
                        owes += debts[i].Amount;    //... add it to owes
                    if (debts[i].To == current_person)   // If the person is the owee (is that a word?)...
                        owed += debts[i].Amount;    //... add it to owed
                }
                double total = owes - owed;
                Console.WriteLine("{0} owes {1} and is owed {2}. In total, they owe {3}.", current_person, owes, owed, total);   // List every person and their total owed/owes
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }


        static void List_person(string specific_person, List<string> people, int entries, string[] lines, Debt[] debts)
        {
            for (int i = 0; i < entries; i++)  // Go through each transaction
            {
                if (debts[i].From == specific_person || debts[i].To == specific_person)     // If the person is the ower or the owee
                    Console.WriteLine("{0}: {1} paid {2} to {3} for {4}", debts[i].Date, debts[i].From, debts[i].To, debts[i].Amount, debts[i].Narrative);
                              // write the details out
                    
            }
            Console.WriteLine("Press enter to continue.");
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

            Debt[] debts = new Debt[entries];        // Record in an array, debts, each transaction as an instance of a class

            List<string> people = new List<string>();    // Here we aim to just list everybody involved
            for (int i = 1; i <= entries; i++)  // Look at each transaction in turn
            {
                string[] transaction = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);       // Split the transactions into their different components

                Debt temp = new Debt();      // Fill in all the details of the current transaction in a temporary instance
                temp.Date = transaction[0];  
                temp.From = transaction[1];
                temp.To = transaction[2];
                temp.Narrative = transaction[3];
                temp.Amount = Convert.ToDouble(transaction[4]);
                debts[i - 1] = temp;      // Transfer accross the current transaction

                people.Add(transaction[1]);                       // Add whoever owes to the list
                people.Add(transaction[2]);                       // Add whoever is owed to the list
            }
            people = people.Distinct().ToList();                  // Remove any duplicates



            bool running = true;
            while (running == true)
            {
                Console.WriteLine("Enter a command (List All, List [Specific person] or Exit): ");
                string input = Console.ReadLine(); // Get user input for the command"
                bool valid_input = false;
                if (input == "List All")
                {
                    List_all(people, entries, lines, debts);        // If they want List All, run that function
                    valid_input = true;
                }
                else if (input == "Exit")
                {
                    running = false;                               // If they input Exit, end the program
                }
                else
                {
                    for (int m = 0; m < people.Count; m++)
                    {
                        if (input == "List " + people[m])           // If they input a specific person's account, run that function
                        {
                            List_person(people[m], people, entries, lines, debts);
                            valid_input = true;
                        }
                    }
                }
                if (valid_input == false)                  // If none of the above happened, they haven't given a valid command.
                    Console.WriteLine("Invalid input. Please enter a valid command.");
                
            }


        }
    }
}
