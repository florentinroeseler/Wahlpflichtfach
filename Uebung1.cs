using System;
using System.Reflection;

class ReflectionExample
{
    // Private statische Felder
    private static int a = 5, b = 10, c = 20;

    static void Main(string[] args)
    {
        Console.WriteLine("a + b + c =" + (a + b + c) + "\nBitte geben Sie den Namen der zu ändernden Variable ein: ");
        var varName = Console.ReadLine();

        // 1. Typinformation besorgen
        Type t = typeof(ReflectionExample);

        // 2. Über Reflection das Feld mit dem gegebenen Namen holen
        // Da die Felder private und statisch sind, brauchen wir entsprechende BindingFlags
        FieldInfo fieldInfo = t.GetField(varName, BindingFlags.Static | BindingFlags.NonPublic);

        // Sicherheitscheck: existiert das Feld mit diesem Namen wirklich?
        if (fieldInfo == null)
        {
            Console.WriteLine($"Ein Feld namens '{varName}' konnte nicht gefunden werden.");
            return;
        }

        // 3. Den aktuellen Wert auslesen und dem Nutzer anzeigen
        object currentValue = fieldInfo.GetValue(null); // null, weil es ein statisches Feld ist
        Console.WriteLine($"Der aktuelle Wert von '{varName}' ist {currentValue}. Geben Sie einen neuen Wert ein:");

        // 4. Wert einlesen und in int konvertieren
        string newValueString = Console.ReadLine();
        if (int.TryParse(newValueString, out int parsedValue))
        {
            // 5. Neuen Wert via Reflection in das Feld schreiben
            fieldInfo.SetValue(null, parsedValue);

            // 6. Ausgabe des neuen Wertes und der neuen Summe
            Console.WriteLine($"Der neue Wert von '{varName}' ist {parsedValue}.\n" +
                              $"a = {a}, b = {b}, c = {c} und a + b + c = {a + b + c}");
        }
        else
        {
            Console.WriteLine("Eingabe war keine gültige Ganzzahl!");
        }
    }
}
