using System;
using System.Collections.Generic;
using System.Text;

namespace BankUeberweisung
{
    // Event Argumente
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

    // Verwaltet Konten und wirft Event bei Überweisung
    public class Bank
    {
        private Dictionary<string, int> _konten = new Dictionary<string, int>();

        // Das Event, das im Erfolgsfall ausgelöst werden soll
        public event TransferEventHandler OnTransfer;

        public Bank(int startGuthaben)
        {
            // Bank-Konto mit Startguthaben anlegen
            _konten["Bank"] = startGuthaben;
        }

        // Methode zum Überweisen an einen bestimmten Empfänger
        public void Ueberweisen(string empfaenger, int betrag)
        {
            if (betrag <= 0)
            {
                throw new ArgumentException("Betrag muss größer 0 sein.");
            }

            // Bank-Guthaben prüfen
            if (!_konten.ContainsKey("Bank"))
            {
                throw new InvalidOperationException("Das Bankkonto existiert nicht.");
            }
            if (_konten["Bank"] - betrag < 0)
            {
                // Wenn das Guthaben nicht reicht, Exception werfen
                throw new InvalidOperationException("Das Guthaben der Bank ist zu gering, Überweisung nicht möglich!");
            }

            // Bankguthaben abbuchen
            _konten["Bank"] -= betrag;

            // Empfänger-Konto anlegen, falls noch nicht vorhanden
            if (!_konten.ContainsKey(empfaenger))
            {
                _konten[empfaenger] = 0;
            }

            // Empfänger-Guthaben erhöhen
            _konten[empfaenger] += betrag;

            // Event auslösen
            OnTransfer?.Invoke(this, new TransferEventArgs("Bank", empfaenger, betrag));
        }

        // Methode, um das Guthaben eines Kontos zu erhöhen (z.B. wenn die Bank Geld "druckt")
        public void Einzahlen(string name, int betrag)
        {
            if (!_konten.ContainsKey(name))
            {
                _konten[name] = 0;
            }
            _konten[name] += betrag;
        }

        // Gibt das aktuelle Guthaben eines Kontos zurück
        public int GetGuthaben(string name)
        {
            if (_konten.ContainsKey(name))
            {
                return _konten[name];
            }
            return 0; // Keine Exception, sondern einfach 0 für nicht existentes Konto
        }
    }

    // 4) Die Konsolenanwendung
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

                    // Prüfe, ob das erste Wort "Guthaben" ist
                    if (erstesWort.Equals("Guthaben", StringComparison.OrdinalIgnoreCase))
                    {
                        // Guthaben abfragen
                        string kontoName = zweitesWort;
                        int guthaben = bank.GetGuthaben(kontoName);
                        Console.WriteLine($"{kontoName} hat ein Guthaben von {guthaben} \u20AC.");
                    }
                    else
                    {
                        // Versuche, das zweite Wort als Betrag zu interpretieren
                        if (int.TryParse(zweitesWort, out int betrag))
                        {
                            if (erstesWort.Equals("Bank", StringComparison.OrdinalIgnoreCase))
                            {
                                // Bank-Guthaben erhöhen
                                bank.Einzahlen("Bank", betrag);
                                Console.WriteLine($"Die Bank hat jetzt {bank.GetGuthaben("Bank")} \u20AC Guthaben.");
                            }
                            else
                            {
                                // Überweisung von Bank an erstesWort
                                try
                                {
                                    bank.Ueberweisen(erstesWort, betrag);
                                }
                                catch (Exception ex)
                                {
                                    // Fehler abfangen und ausgeben
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

        // Diese Methode wird aufgerufen, sobald das Event OnTransfer ausgelöst wird
        private static void Bank_OnTransfer(object sender, TransferEventArgs e)
        {
            // Beispiel-Ausgabe:
            Console.WriteLine($"Es werden von {e.From} {e.Amount} \u20AC an {e.To} überwiesen.");
        }
    }
}
