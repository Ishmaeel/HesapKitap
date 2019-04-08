using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HesapKitap
{
    public class StatementParser
    {
        private StringBuilder _Output = new StringBuilder();
        private List<Transaction> _Transactions = new List<Transaction>();
        private const string NewTransactionMarker = "YENİ!";

        internal void Process(string text)
        {
            string fileName = GetTransactionLogFile();
            var knownTransactions = LoadKnownTransactions(fileName);

            string[] lines = text.Split(new char[] { '\n', '\r' });

            foreach (var oneLine in lines)
            {
                Transaction oneTransaction = Transaction.Parse(oneLine);

                if (oneTransaction != null)
                    _Transactions.Add(oneTransaction);
            }

            _Transactions.Where(t => t.Description.Contains("Son ekstre borcu")).ToList().ForEach(t => t.Group = TransactionGroup.Ödeme);
            _Transactions.Where(t => t.Amount > 0).ToList().ForEach(t => t.Group = TransactionGroup.Ödeme);
            _Transactions.Where(t => Regex.IsMatch(t.Description, @"\(\d+\/\d+\)")).ToList().ForEach(t => t.Group = TransactionGroup.Taksit);
            _Transactions.Where(t => !knownTransactions.Contains(t.LogOutput)).ToList().ForEach(t => t.Marker = NewTransactionMarker);

            var newTransactions = _Transactions.Where(t => t.Marker == NewTransactionMarker).OrderBy(t => t.Group).ThenBy(t => t.Date);
            var oldTransactions = _Transactions.Where(t => !newTransactions.Contains(t));

            var pendingTransactions = newTransactions.Where(t => t.Pending).OrderBy(t => t.SortOrder);
            var finalizedTransactions = newTransactions.Where(t => !t.Pending).OrderBy(t => t.SortOrder);

            if (finalizedTransactions.Any())
            {
                foreach (var oneTransaction in finalizedTransactions)
                    _Output.AppendLine(oneTransaction.ExcelOutput);

                _Output.AppendLine();
            }

            if (pendingTransactions.Any())
            {
                foreach (var oneTransaction in pendingTransactions)
                    _Output.AppendLine(oneTransaction.ExcelOutput);

                _Output.AppendLine();
            }

            foreach (var oneTransaction in oldTransactions)
                _Output.AppendLine(oneTransaction.ExcelOutput);
        }

        public string Output
        {
            get
            {
                return _Output.ToString();
            }
        }

        internal void Save()
        {
            string fileName = GetTransactionLogFile();
            var knownTransactions = LoadKnownTransactions(fileName);

            foreach (var item in _Transactions)
            {
                string logOutput = item.LogOutput;

                if (!knownTransactions.Contains(logOutput))
                    knownTransactions.Add(logOutput);
            }

            File.WriteAllLines(fileName, knownTransactions.ToArray(), Encoding.UTF8);
        }

        private static List<string> LoadKnownTransactions(string fileName)
        {
            List<string> knownTransactions = new List<string>();

            if (File.Exists(fileName))
                knownTransactions.AddRange(File.ReadAllLines(fileName, Encoding.UTF8));

            return knownTransactions;
        }

        private static string GetTransactionLogFile()
        {
            string fileName = ConfigurationManager.AppSettings["logFile"];

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ConfigurationErrorsException("Logfile location is not configured.");

            return fileName;
        }
    }
}