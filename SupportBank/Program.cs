using System;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            string whole_file = System.IO.File.ReadAllText(@"C:\Work\Training\SupportBank\Transactions2014.csv");
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);
            int entries = lines.Length - 1;
            Console.WriteLine(entries);
            Console.ReadLine();
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
