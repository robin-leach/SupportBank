using System;
using System.Collections.Generic;
using System.Linq;



namespace SupportBank
{
    class Program
    {
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

            List<string> people = new List<string>();
            for (int i = 1; i <= entries; i++)  // Look at each transaction in turn
            {
                string[] transaction = lines[i].Split(',');
                people.Add(transaction[1]);
                people.Add(transaction[2]);
            }
            people = people.Distinct().ToList();
            foreach(string x in people)
            {
                Console.WriteLine("{0}", x);
            }
            Console.ReadLine();

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
