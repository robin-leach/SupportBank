using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank
{
    class Program
    {

        /* Below is a function that carries out the List All process, if the user inputs that command */
        static void List_all(List<string> people, int entries, Debt[] debts)
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
                Console.WriteLine("{0} owes {1} and is owed {2}. In total, they owe {3}.", current_person,
                    Math.Round(owes,2), Math.Round(owed,2), Math.Round(total,2));   // List every person and their total owed/owes
            }

            /* At the end of this, the user will need to press another key to get back to the main menu of command input */
        }


        /* Here is the function for when listing the transactions of a specific person */
        static void List_person(string specific_person, List<string> people, int entries, Debt[] debts)
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
        }


        /* Here is the function for importing a .csv file */
        static Debt[] Import_csv(string file, int entries, Debt[] debts)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();


            /* Log things, in case they go wrong */
            logger.Info("Now we use the Import_csv function");


            /* Obtain the raw data */
            try
            {

                /* Try to read the data. If not, the function will cancel and return what you started with. */
                string raw_data_csv = System.IO.File.ReadAllText(file);
                raw_data_csv = raw_data_csv.Replace("Date,From,To,Narrative,Amount", "");
                raw_data_csv = raw_data_csv.Replace('\n', '\r');
                string[] csv_lines = raw_data_csv.Split(new char[] { '\r' },
         StringSplitOptions.RemoveEmptyEntries);

                logger.Info("We've read the file");


                /* Create a new array to store all the new data, alongside the old data */
                Debt[] newdebts = new Debt[entries + csv_lines.Length];

                for (int i = 0; i < entries; i++)
                    newdebts[i] = debts[i];


                /* Read through line by line, and add in all the new data */
                for (int i = 0; i < csv_lines.Length; i++)
                {
                    string[] transaction = csv_lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);       // Split the transactions into their different components
                    try
                    {
                        Debt temp = new Debt();      // Fill in all the details of the current transaction in a temporary instance
                        temp.Date = Convert.ToDateTime(transaction[0]);   // Make sure the format is correct
                        temp.From = transaction[1];
                        temp.To = transaction[2];
                        temp.Narrative = transaction[3];
                        temp.Amount = Convert.ToDouble(transaction[4]);
                        newdebts[i + entries] = temp;      // Transfer accross the current transaction
                    }
                    catch    // If something goes wrong, we'll know about it, and record the data as 0
                    {
                        logger.Info("There was an issue when reading the following transaction: " + "date: " + transaction[0] + ", from " + transaction[1]
                             + " to " + transaction[2] + " for " + transaction[3] + " of the amount " + transaction[4] + ". This set of data will now just become zero/empty.");
                        // If there's an error, display the details of the offending entry so as to work out what went wrong.
                        Debt temp = new Debt();      // Fill in all the details of the current transaction in a temporary instance
                        temp.Date = new DateTime();
                        temp.From = "";
                        temp.To = "";
                        temp.Narrative = "";
                        temp.Amount = 0;
                        newdebts[i + entries] = temp;
                    }
                }


                entries = entries + csv_lines.Length;
                logger.Info("I mean hopefully now it's done, right?");
                return newdebts;
            }
            catch
            {
                Console.WriteLine("Invalid file. Please try again.");
                return debts;
            }


        }


        /* Here is the function for importing a .json file */
        static Debt[] Import_json(string file, int entries, Debt[] debts)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();


            /* Log things, in case they go wrong */
            logger.Info("Now we use the Import_json function");


            /* Obtain the raw data */
            try
            {
                /* Try to read the data. If not, the function will cancel and return what you started with. */
                List<json_Debt> json_result;

                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    json_result = JsonConvert.DeserializeObject<List<json_Debt>>(json);
                }


                logger.Info("We've read the file");

                json_Debt[] json_debts = new json_Debt[json_result.Count];
                Debt[] converted_debts = new Debt[json_result.Count];
                for (int i = 0; i < json_result.Count; i++)
                {
                    json_debts[i] = json_result[i];
                    converted_debts[i] = new Debt(json_debts[i]);
                }


                /* Create a new array to store all the new data, alongside the old data */
                Debt[] newdebts = new Debt[entries+json_result.Count];

                for (int i = 0; i < entries; i++)
                    newdebts[i] = debts[i];


                /* Read through line by line, and add in all the new data */
                for (int i = 0; i < json_result.Count; i++)
                    newdebts[i + entries] = converted_debts[i];



                logger.Info("I mean hopefully now it's done, right?");
                return newdebts;
            }
            catch
            {
                Console.WriteLine("Invalid file. Please try again.");
                return debts;
            }


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

            int entries = 0;
            Debt[] total_debts = new Debt[0];
            total_debts = Import_csv(@"C:\Work\Training\SupportBank\Transactions2014.csv", entries, total_debts);
            entries = total_debts.Length;

            logger.Info("Finished reading 2014 entries.");

            total_debts = Import_csv(@"C:\Work\Training\SupportBank\DodgyTransactions2015.csv", entries, total_debts);
            entries = total_debts.Length;

            logger.Info("Finished reading dodgy 2015 entries.");
            logger.Info("We've read the CSV files");


            /* Now we read the Json file. We use the function above to turn it into a string, deserialise it, convert it to the debt format, and add to the list */

            total_debts = Import_json(@"C:\Work\Training\SupportBank\Transactions2013.json", entries, total_debts);
            entries = total_debts.Length;

            logger.Info("We've read the Json file");


            /* Now we read the xml file */
            string json_of_xml = import_xml_to_json(@"C:\Users\RTL\Documents\Transactions2012.xml");
            total_debts = Import_json_string(json_of_xml, entries, total_debts);
            entries = total_debts.Length;

            /* Now we get a list of all the different people involved */

            List<string> people = new List<string>();
            for (int i = 0; i < entries; i++)  // Look at each transaction in turn
            {
                Debt transaction = total_debts[i];
                try
                {
                    if (transaction.From != "")
                        people.Add(transaction.From);                       // Add whoever owes to the list
                    if (transaction.To != "")
                        people.Add(transaction.To);                       // Add whoever is owed to the list
                }
                catch
                {
                    //
                }
            }
            people = people.Distinct().ToList();        // Remove any duplicates in the list of people.





            /* That finishes out compilation and sorting of the data. Here we start the actual process of taking user commands, and
             * churning out whatever data they have requested */

            bool running = true;
            while (running == true)
            {
                Console.WriteLine("Enter a command (List All, List [Specific person], Import File or Exit): ");
                string input = Console.ReadLine(); // Get user input for the command"

                var case_string = "";

                if (input == "List All" || input == "Exit" || input == "Import File")
                    case_string = input;
                else if (input.StartsWith("List"))
                    case_string = "List Person";

                switch (case_string) {

                    case ("List All"):
                    {
                        logger.Info("User wants a list of all the data!");
                        List_all(people, entries, total_debts);        // If they want List All, run that function
                            break;
                    }

                    case ("Exit"):
                    {
                        logger.Info("User wants to leave me. What have I done??");
                        running = false;                               // If they input Exit, end the program
                            break;
                    }

                    case ("Import File"):              // Add functionality for the user to specify a file to import.
                    {
                            logger.Info("User wants to import another file. Eurgh!");
                            Console.WriteLine("Please enter the file that you would like to import");
                            string file_name = Console.ReadLine(); // Get user input for the file to import.


                            try
                            {
                                string file_type = file_name.Substring(file_name.Length - 4, 4);


                                /* Now we deal with the different cases of different filetypes */

                                switch (file_type)
                                {

                                    /* At this point it makes sense to create new functions to deal with the different cases */

                                    case ".csv":
                                        {
                                            logger.Info("User wants to import a CSV. We all love commas!");
                                            total_debts = Import_csv(file_name, entries, total_debts);
                                            entries = total_debts.Length;

                                            people = new List<string>();    // Update the list of people
                                            for (int i = 0; i < entries; i++)  // Look at each transaction in turn
                                            {
                                                Debt transaction = total_debts[i];
                                                try
                                                {
                                                    if (transaction.From != "")
                                                        people.Add(transaction.From);                       // Add whoever owes to the list
                                                    if (transaction.To != "")
                                                        people.Add(transaction.To);                       // Add whoever is owed to the list
                                                }
                                                catch
                                                {
                                                    //
                                                }
                                            }
                                            people = people.Distinct().ToList();    // Remove any duplicates in the list of people.
                                            break;
                                        }

                                    case "json":
                                        {
                                            logger.Info("User wants to import a json???? WHYYYYYY");
                                            total_debts = Import_json(file_name, entries, total_debts);
                                            entries = total_debts.Length;

                                            people = new List<string>();    // Update the list of people
                                            for (int i = 0; i < entries; i++)  // Look at each transaction in turn
                                            {
                                                Debt transaction = total_debts[i];
                                                try
                                                {
                                                    if (transaction.From != "")
                                                        people.Add(transaction.From);                       // Add whoever owes to the list
                                                    if (transaction.To != "")
                                                        people.Add(transaction.To);                       // Add whoever is owed to the list
                                                }
                                                catch
                                                {
                                                    //
                                                }
                                            }
                                            people = people.Distinct().ToList();    // Remove any duplicates in the list of people.
                                            break;
                                        }

                                    default:
                                        Console.WriteLine("Invalid file name! Please try again.");
                                        break;
                                }

                            }

                            /* If the filename they inputted doesn't end in a sensible way, or is not long enough, we reach this part */
                            catch
                            {
                                Console.WriteLine("Invalid file name! Please try again.");
                            }
                            break;
                            
                    }

                    case ("List Person"):
                    {
                            string specific_person = input.Substring(5);
                            if(people.Contains(specific_person))
                            {
                                logger.Info("User wants to know the specific transactions of " + specific_person + ". Curious...");
                                List_person(specific_person, people, entries, total_debts);
                            }
                            else
                            {
                                logger.Info("User wants to find the transactions of " + specific_person + " but they don't exist :(");
                                Console.WriteLine("Error - cannot find this person. Please try again.");
                            }
                            break;
                    }

                    default:
                        {
                            logger.Info("User can't even do a valid input command. They tried to hit me with {0}. User? More like loser!", input);
                            Console.WriteLine("Invalid input. Please enter a valid command.");
                            break;
                        }
                }
                
            }


        }

    }

}

