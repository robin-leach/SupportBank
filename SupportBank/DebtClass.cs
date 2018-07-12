using System;

namespace SupportBank
{
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
}
