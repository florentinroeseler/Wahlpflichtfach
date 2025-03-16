// Repository: https://github.com/florentinroeseler/Wahlpflichtfach
using System;
using System.Reflection;

public class Uebung1
{
    private static int a = 1, b = 2, c = 3; // Durch static muss ich kein Objekt erstellen, um auf die Variablen zuzugreifen

    static void Main(string[] args)
    {
        Console.WriteLine($"a = {a}, b = {b}, c = {c} and a + b + c = {a + b + c}");
        Console.WriteLine("Bitte geben Sie den Namen der zu ändernden Variable ein:");
        string changeParam = Console.ReadLine();

        // Reflections-Objekt für das Feld ermitteln
        FieldInfo fieldInfo = typeof(Uebung1).GetField(
            changeParam,
            BindingFlags.Static | BindingFlags.NonPublic 
        );

        // Wenn das Feld nicht gefunden wurde, gebe eine Fehlermeldung aus
        if (fieldInfo == null)
        {
            Console.WriteLine($"\nFeld '{changeParam}' wurde nicht gefunden.");
        }
        else
        {
            // Den aktuellen Wert des Feldes ermitteln
            int currentValue = (int)fieldInfo.GetValue(null);
            Console.WriteLine($"\nDer aktuelle Wert von \"{changeParam}\" ist {currentValue}. Geben Sie einen neuen Wert ein:");
            string changeValue = Console.ReadLine();

            // Wenn der eingegebene Wert eine Zahl ist, setze den neuen Wert
            if (int.TryParse(changeValue, out int newValue))
            {
                fieldInfo.SetValue(null, newValue);
                Console.WriteLine($"Der neue Wert von {changeParam} ist {newValue}.");
                Console.WriteLine($"a = {a}, b = {b}, c = {c} and a + b + c = {a + b + c}\n");
            }
            else
            {
                Console.WriteLine("\nDas war keine gültige Zahl.");
            }
        }
    }
}
