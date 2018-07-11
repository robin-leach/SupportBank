using System;

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
            Console.WriteLine(entries);
            Console.ReadLine();

            for(int i = 1; i<= entries; i++)
            {
                string[] transaction = lines[i].Split(',');
                Console.WriteLine(transaction[0]);
                Console.ReadLine();
            }

        }
    }

    public class transaction
    {
        public string Date;
        public string From;
        public string To;
        public float Amount;
    }
}
