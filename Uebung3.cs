using System;
using System.Text.RegularExpressions;

class Uebung3
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Geben Sie die Berechnung ein (Beispiel: 2 + 3 oder 2+3): ");
            string input = Console.ReadLine();
            if (input.ToLower() == "exit")
            {
                Console.WriteLine("Programm beendet.");
                break;
            }
            try
            {
                double result = ParseAndCalculate(input);
                Console.WriteLine($"Ergebnis: {result}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ungültiges Format! Bitte verwende das Format 'Zahl Operator Zahl' (z. B. '5 + 10' oder '5+10).");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Fehler: Division durch 0 ist nicht erlaubt.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }    
    }
    static double ParseAndCalculate(string input)
    {
        // Wegen der regionalen Einstellungen wir der Punkt standardmäßig nicht als Dezimaltrennzeichen erkannt
        // -> Ersetze Punkt durch Komma
        input = input.Replace('.', ',');
        input = Regex.Replace(input, @"(\d)([+\-*/])(\d)", "$1 $2 $3");

        string[] parts = input.Split(' ');

        if (parts.Length != 3)
            throw new FormatException();

        if (!double.TryParse(parts[0], out double num1) || !double.TryParse(parts[2], out double num2))
            throw new FormatException();

        char op = parts[1][0];

        return Calculate(num1, op, num2);
    }

    static double Calculate(double num1, char op, double num2)
    {
        double result;

        switch (op)
        {
            case '+':
                result = num1 + num2;
                break;
            case '-':
                result = num1 - num2;
                break;
            case '*':
                result = num1 * num2;
                break;
            case '/':
                if (num2 == 0)
                    throw new DivideByZeroException("Fehler: Division durch 0 ist nicht erlaubt.");
                result = num1 / num2;
                break;
            default:
                throw new FormatException("Ungültiger Operator.");
        }

        return result;
    }
}
