using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;




namespace SupportBank
{
    class Program
    {


        /* First we set up appropriate classes. For now this will involve a separate class for the Json version, and for the CSV version */


        /* This is the Json version */
        public class json_Debt
        {
            public DateTime Date;
            public string FromAccount;
            public string ToAccount;
            public string Narrative;
            public double Amount;

        }
    


        /* ...and this is the standard version, used first for CSV */
        public class Debt 
        {
            public DateTime Date;
            public string From;
            public string To;
            public string Narrative;
            public double Amount;


            /* We want to be able to make a new Debt() without any defaults */
            public Debt()
            {
                //
            }


            /* We also here have a construct that will take the Json form of a debt, and convert it to the standard form */
            public Debt(json_Debt raw)
            {
                Date = raw.Date;
                From = raw.FromAccount;
                To = raw.ToAccount;
                Narrative = raw.Narrative;
                Amount = raw.Amount;
            }

        }


        /* Below is a function that carries out the List All process, if the user inputs that command */

        static void List_all(List<string> people, int entries, string[] lines, Debt[] debts)
        {
            /* We want to be able to log, in case something goes wrong */
            ILogger logger = LogManager.GetCurrentClassLogger();

            for (int n=0; n<people.Count; n++)                       // Now we go through each person and find out how much they owe/are owed
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

            /* At the end of this, the user will need to press another key to get back to the main menu of command input */

            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }



        /* Here is the function for when listing the transactions of a specific person */
        static void List_person(string specific_person, List<string> people, int entries, string[] lines, Debt[] debts)
        {
            for (int i = 0; i < entries; i++)  // Go through each transaction
            {
                if (debts[i].From == specific_person || debts[i].To == specific_person)     // If the person is the ower or the owee
                {
                    DateTime temp_date = debts[i].Date;
                    Console.WriteLine("{0}: {1} paid {3} to {2} for {4}", temp_date.ToString("dd/MM/yy"), debts[i].From, debts[i].To, debts[i].Amount, debts[i].Narrative);
                    // write the details out
                }
                    
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }



        /* Now we write our main program, making reference to the functions above where necessary. */
        static void Main(string[] args)
        {
            /* Set up log functionality */
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, target));
            LogManager.Configuration = config;

            ILogger logger = LogManager.GetCurrentClassLogger();
            logger.Info("And so we begin!");



            /* This section reads all the data we'll be using */


            /* Start with the two CSV files. Read them into strings, then remove the initial line with headings. Concatenate them into
             * one long string.*/
            string file_1 = System.IO.File.ReadAllText(@"C:\Work\Training\SupportBank\Transactions2014.csv");
            string file_2 = System.IO.File.ReadAllText(@"C:\Work\Training\SupportBank\DodgyTransactions2015.csv");
            file_2 = file_2.Replace("Date,From,To,Narrative,Amount", "");
            string whole_file = file_1 + file_2;

            /* Replaces '\n' in the new file by '\r' */
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                 StringSplitOptions.RemoveEmptyEntries);
            // Splits the whole file into lines, wherever there is an instance of '\r', and removes any empty entries
            int entries = lines.Length - 1;  // create a variable to know how many transactions we're dealing with

            logger.Info("We've read the CSV files");




            /* Now we will convert our array of strings into the standard Debt class that we will be using, so as to combine with the Json later */

            Debt[] debts = new Debt[entries];        // Record in an array, debts, each transaction as an instance of a class

            List<string> people = new List<string>();    // Here we aim to just list everybody involved
            for (int i = 1; i <= entries; i++)  // Look at each transaction in turn
            {
                string[] transaction = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);       // Split the transactions into their different components
                try
                {
                    Debt temp = new Debt();      // Fill in all the details of the current transaction in a temporary instance
                    temp.Date = Convert.ToDateTime(transaction[0]);   // Make sure the format is correct
                    temp.From = transaction[1];
                    temp.To = transaction[2];
                    temp.Narrative = transaction[3];
                    temp.Amount = Convert.ToDouble(transaction[4]);
                    debts[i - 1] = temp;      // Transfer accross the current transaction

                    people.Add(transaction[1]);                       // Add whoever owes to the list
                    people.Add(transaction[2]);                       // Add whoever is owed to the list
                }
                catch    // If something goes wrong, we'll know about it, and record the data as 0
                {
                    logger.Info("There was an issue when reading the following transaction: " + "date: " + transaction[0] + ", from " + transaction[1]
                         + " to " + transaction[2] + " for " + transaction[3] + " of the amount " + transaction [4] + ". This set of data will now just become zero/empty.");
                    // If there's an error, display the details of the offending entry so as to work out what went wrong.
                    Debt temp = new Debt();      // Fill in all the details of the current transaction in a temporary instance
                    temp.Date = new DateTime();
                    temp.From = "";
                    temp.To = "";
                    temp.Narrative = "";
                    temp.Amount = 0;
                    debts[i - 1] = temp;
                }
            }
            people = people.Distinct().ToList();                  // Remove any duplicates in the list of people.





            /* Now we read the Json file. We read it into a string, deserialise it and put it into the Json debt class format */
            List<json_Debt> result;

            using (StreamReader r = new StreamReader(@"C:\Work\Training\SupportBank\Transactions2013.json"))
            {
                string json = r.ReadToEnd();
                result = JsonConvert.DeserializeObject<List<json_Debt>>(json);
            }



            /* With that done, we convert it into our standard debt format, which we will have for the CSV files */
            json_Debt[] json_debts = new json_Debt[result.Count];
            Debt[] converted_debts = new Debt[result.Count];
            for (int i = 1; i <= result.Count; i++)
            {
                json_debts[i - 1] = result[i - 1];
                converted_debts[i - 1] = new Debt(json_debts[i - 1]);
            }

            logger.Info("We've read the Json file");


            /* Finally, we merge the data from the Json with that from the CSVs */

            Debt[] total_debts = new Debt[entries + result.Count];    // Merge all the data into one big array.
            for (int i = 0; i < result.Count; i++)
                total_debts[i] = converted_debts[i];
            for (int i = 0; i < entries; i++)
                total_debts[i + result.Count] = debts[i];

            entries = entries + result.Count;

            logger.Info("We've amalgamated into one large array");




            /* That finishes out compilation and sorting of the data. Here we start the actual process of taking user commands, and
             * churning out whatever data they have requested */

            bool running = true;
            while (running == true)
            {
                Console.WriteLine("Enter a command (List All, List [Specific person] or Exit): ");
                string input = Console.ReadLine(); // Get user input for the command"
                bool valid_input = false;
                if (input == "List All")
                {
                    logger.Info("User wants a list of all the data!");
                    List_all(people, entries, lines, total_debts);        // If they want List All, run that function
                    valid_input = true;
                }
                else if (input == "Exit")
                {
                    logger.Info("User wants to leave me. What have I done??");
                    running = false;                               // If they input Exit, end the program
                    valid_input = true;
                }
                else
                {
                    for (int m = 0; m < people.Count; m++)
                    {
                        if (input == "List " + people[m])           // If they input a specific person's account, run that function
                        {
                            logger.Info("User wants to know the specific transactions of " + people[m] + ". Curious...");
                            List_person(people[m], people, entries, lines, total_debts);
                            valid_input = true;
                        }
                    }
                }
                if (valid_input == false)                  // If none of the above happened, they haven't given a valid command.
                {
                    logger.Info("User can't even do a valid input command. They tried to hit me with {0}. User? More like loser!", input);
                    Console.WriteLine("Invalid input. Please enter a valid command.");
                }
                
            }


        }

    }

}

