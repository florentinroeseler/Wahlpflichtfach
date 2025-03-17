using System;

class Uebung2
{
    static void Main(string[] args)
    {
        for (int i = 0; i <= int.MaxValue; i++)
        {
            if (i % 2 == 0)
            {
                Console.WriteLine(i);
            }
        }
    }
}

// Bonusaufgabe: i wird zu -2147483648 wegen eines Overflows. Genauer liegt es
// daran, dass int intern das Zweierkomplement verwendet.
