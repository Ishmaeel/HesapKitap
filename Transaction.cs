using System;

namespace HesapKitap
{
    public class Transaction
    {
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Detail { get; set; }

        public string Category { get; set; }

        public double Bonus { get; set; }

        public double Amount { get; set; }

        public TransactionGroup Group { get; set; }

        public string Marker { get; set; }

        public string LogOutput
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd}|{1}|{2}|{3}|{4}|{5}", Date, Description, Detail, Category, Bonus, Amount);
            }
        }

        public string ExcelOutput
        {
            get
            {
                return string.Format("{0}\t{1}", Amount, Description);
            }
        }

        public int SortOrder { get; private set; }

        public bool Pending { get; private set; }

        internal static Transaction Parse(string oneLine)
        {
            string[] parts = oneLine.Split(new char[] { '\t' });

            if (parts.Length != 6)
                return null;

            DateTime date;
            string description;
            string category;
            double bonus;
            double amount;
            string legend;

            if (!DateTime.TryParse(parts[0], out date))
                return null;

            if (!StringHelper.TryParse(parts[1], out description))
                return null;

            StringHelper.TryParse(parts[2], out category);

            double.TryParse(parts[3], out bonus);

            if (!double.TryParse(parts[4], out amount))
                return null;

            StringHelper.TryParse(parts[5], out legend);

            return new Transaction
            {
                Date = date,
                Amount = amount,
                Bonus = bonus,
                Category = category,
                Description = description,
                Group = TransactionGroup.Diğer,
                SortOrder = GetSortOrder(legend),
                Pending = legend == "orange"
            };
        }

        private static int GetSortOrder(string legend)
        {
            switch (legend)
            {
                case "orange":
                    return 100;

                default:
                    return 50;
            }
        }
    }
}