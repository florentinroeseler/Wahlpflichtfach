using System;
using System.Collections.Generic;
using System.Text;

namespace BankUeberweisung
{
    public class TransferEventArgs : EventArgs
    {
        public string From { get; }
        public string To { get; }
        public int Amount { get; }

        public TransferEventArgs(string from, string to, int amount)
        {
            From = from;
            To = to;
            Amount = amount;
        }
    }

    // Delegate für Event
    public delegate void TransferEventHandler(object sender, TransferEventArgs e);

    public class Bank
    {
        private Dictionary<string, int> _konten = new Dictionary<string, int>();

        // Event wird bei Erfolg ausgelöst
        public event TransferEventHandler OnTransfer;

        public Bank(int startGuthaben)
        {
            _konten["Bank"] = startGuthaben;
        }

        public void Ueberweisen(string empfaenger, int betrag)
        {
            if (betrag <= 0)
            {
                throw new ArgumentException("Betrag muss größer 0 sein.");
            }

            // Überweisung prüfen
            if (!_konten.ContainsKey("Bank"))
            {
                throw new InvalidOperationException("Das Bankkonto existiert nicht.");
            }
            if (_konten["Bank"] - betrag < 0)
            {
                throw new InvalidOperationException("Das Guthaben der Bank ist zu gering, Überweisung nicht möglich!");
            }

            _konten["Bank"] -= betrag;

            // Empfänger-Konto anlegen, falls noch nicht vorhanden
            if (!_konten.ContainsKey(empfaenger))
            {
                _konten[empfaenger] = 0;
            }

            _konten[empfaenger] += betrag;

            // Event
            OnTransfer?.Invoke(this, new TransferEventArgs("Bank", empfaenger, betrag));
        }

        // Geld-Drucken a la Zentralbank
        public void Einzahlen(string name, int betrag)
        {
            if (!_konten.ContainsKey(name))
            {
                _konten[name] = 0;
            }
            _konten[name] += betrag;
        }

        public int GetGuthaben(string name)
        {
            if (_konten.ContainsKey(name))
            {
                return _konten[name];
            }
            return 0; // Hätte man auch mit Exception lösen können
        }
    }

    class Uebung5
    {
        static void Main(string[] args)
        {
            // Benötigt für korrekte Darstellung von €
            Console.OutputEncoding = Encoding.UTF8;

            // Bank mit Startguthaben 10.000 € anlegen
            Bank bank = new Bank(10000);

            // Event abonnieren
            bank.OnTransfer += Bank_OnTransfer;

            Console.WriteLine("Willkommen im Bank-System!");
            Console.WriteLine("Mögliche Eingaben:");
            Console.WriteLine("   <Name> <Betrag>   (z.B. Linus 1000) -> Überweisung von Bank an 'Name'");
            Console.WriteLine("   Bank <Betrag>     (z.B. Bank 5000)  -> Bank-Guthaben erhöhen ('Geld drucken')");
            Console.WriteLine("   Guthaben <Name>   (z.B. Guthaben Bank, Guthaben Linus) -> Aktuelles Guthaben abfragen");
            Console.WriteLine("   exit             -> Beenden");
            Console.WriteLine();

            while (true)
            {
                Console.Write("> ");
                string eingabe = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(eingabe))
                {
                    continue;
                }
                if (eingabe.ToLower() == "exit")
                {
                    break;
                }

                string[] teile = eingabe.Split(' ');
                if (teile.Length == 2)
                {
                    // Zwei Wörter -> könnte "Name Betrag", "Bank Betrag" oder "Guthaben Name" sein
                    string erstesWort = teile[0];
                    string zweitesWort = teile[1];

                    if (erstesWort.Equals("Guthaben", StringComparison.OrdinalIgnoreCase))
                    {
                        // Guthaben abfragen
                        string kontoName = zweitesWort;
                        int guthaben = bank.GetGuthaben(kontoName);
                        Console.WriteLine($"{kontoName} hat ein Guthaben von {guthaben} \u20AC.");
                    }
                    else
                    {
                        if (int.TryParse(zweitesWort, out int betrag))
                        {
                            if (erstesWort.Equals("Bank", StringComparison.OrdinalIgnoreCase))
                            {
                                bank.Einzahlen("Bank", betrag);
                                Console.WriteLine($"Die Bank hat jetzt {bank.GetGuthaben("Bank")} \u20AC Guthaben.");
                            }
                            else
                            {
                                try
                                {
                                    bank.Ueberweisen(erstesWort, betrag);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Fehler bei der Überweisung: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ungültige Eingabe. Betrag muss eine ganze Zahl sein.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unbekanntes Format. Bitte geben Sie z.B. 'Linus 1000' oder 'Bank 5000' ein.");
                }
            }
            bank.OnTransfer -= Bank_OnTransfer;
            Console.WriteLine("Programm beendet.");
        }

        // wird bei Event ausgeführt
        private static void Bank_OnTransfer(object sender, TransferEventArgs e)
        {
            Console.WriteLine($"Es werden von {e.From} {e.Amount} \u20AC an {e.To} überwiesen.");
        }
    }
}
