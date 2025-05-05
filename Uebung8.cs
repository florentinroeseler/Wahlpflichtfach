using System;
using System.Collections.Generic;
using System.Threading;

// Interfaces
public interface IAttack
{
    string Type { get; set; }
    int Damage { get; set; }
    bool DoubleAttackChance { get; set; }
}

public interface IItem
{
    string Name { get; }
    void ApplyEffect(ICharacter character);
}

public interface ICharacter
{
    IAttack Attack { get; set; }
    string Name { get; }
    int Health { get; set; }
    int HealthPotionsCount { get; }
    void Defend(int damage);
    void TryUseItem(IItem item);
}

// Implementierung von Items
public class HealthPotion : IItem
{
    public string Name { get; private set; }
    private int HealAmount { get; set; }

    public HealthPotion(int healAmount = 20)
    {
        Name = "Heiltrank";
        HealAmount = healAmount;
    }

    public void ApplyEffect(ICharacter character)
    {
        Console.WriteLine($"{character.Name} verwendet einen {Name} und heilt {HealAmount} Lebenspunkte.");
        character.Health += HealAmount;
    }
}

// Implementierung von Angriffstypen
public class WarriorAttack : IAttack
{
    public string Type { get; set; }
    public int Damage { get; set; }
    public bool DoubleAttackChance { get; set; }

    public WarriorAttack()
    {
        Type = "Schwertschlag";
        Damage = 10;
        DoubleAttackChance = true;
    }
}

public class WizardAttack : IAttack
{
    public string Type { get; set; }
    public int Damage { get; set; }
    public bool DoubleAttackChance { get; set; }

    public WizardAttack()
    {
        Type = "Feuerzauber";
        Damage = 15;
        DoubleAttackChance = false;
    }
}

// Abstrakte Basisklasse für Charaktere
public abstract class Character : ICharacter
{
    public IAttack Attack { get; set; }
    public string Name { get; protected set; }
    public int Health { get; set; }
    private int _healthPotionsCount;
    public int HealthPotionsCount => _healthPotionsCount;

    protected Character(string name, int health, IAttack attack)
    {
        Name = name;
        Health = health;
        Attack = attack;
        _healthPotionsCount = 5; // Maximal 5 Heiltränke
    }

    public virtual void Defend(int damage)
    {
        Health -= damage;
    }

    public void TryUseItem(IItem item)
    {
        if (item is HealthPotion && _healthPotionsCount > 0)
        {
            _healthPotionsCount--;
            item.ApplyEffect(this);
        }
        else if (_healthPotionsCount <= 0)
        {
            Console.WriteLine($"{Name} hat keine Heiltränke mehr übrig!");
        }
    }
}

// Charakterklassen
public class Warrior : Character
{
    public Warrior(string name, int health, IAttack attack) : base(name, health, attack)
    {
    }

    public override void Defend(int damage)
    {
        Random rnd = new Random();
        if (rnd.Next(1, 7) == 4) // Bei einer 4 wird der Schaden halbiert
        {
            Console.WriteLine($"{Name} pariert geschickt und halbiert den Schaden!");
            base.Defend(damage / 2);
        }
        else
        {
            base.Defend(damage);
        }
    }
}

public class Wizard : Character
{
    public Wizard(string name, int health, IAttack attack) : base(name, health, attack)
    {
    }

    public override void Defend(int damage)
    {
        Random rnd = new Random();
        if (rnd.Next(1, 7) == 4) // Bei einer 4 wird der Schaden halbiert
        {
            Console.WriteLine($"{Name} weicht aus und halbiert den Schaden!");
            base.Defend(damage / 2);
        }
        else
        {
            base.Defend(damage);
        }
    }
}

// Game-Klasse für die Spiellogik
public class Game
{
    private Random rnd = new Random();

    public void Battle(ICharacter character1, ICharacter character2)
    {
        Console.WriteLine($"Kampf zwischen {character1.Name} und {character2.Name} beginnt!");

        int round = 1;
        while (character1.Health > 0 && character2.Health > 0)
        {
            Console.WriteLine($"\nRunde {round}:");
            ProcessRound(character1, character2);

            if (character1.Health <= 0 || character2.Health <= 0)
                break;

            Thread.Sleep(1000); // Kleine Pause für bessere Lesbarkeit
            round++;
        }

        // Ermittlung des Gewinners
        if (character1.Health <= 0 && character2.Health <= 0)
        {
            Console.WriteLine("Unentschieden! Beide Charaktere wurden besiegt.");
        }
        else if (character1.Health <= 0)
        {
            Console.WriteLine($"{character2.Name} gewinnt!");
        }
        else
        {
            Console.WriteLine($"{character1.Name} gewinnt!");
        }
    }

    private void ProcessRound(ICharacter attacker, ICharacter defender)
    {
        // Versuch, einen Heiltrank zu benutzen
        TryUseHealthPotion(attacker);
        TryUseHealthPotion(defender);

        // Angriff durchführen
        PerformAttack(attacker, defender);

        // Wenn der Verteidiger noch lebt, führt er einen Gegenangriff durch
        if (defender.Health > 0)
        {
            PerformAttack(defender, attacker);
        }
    }

    private void TryUseHealthPotion(ICharacter character)
    {
        int diceRoll = rnd.Next(1, 7);
        if (diceRoll == 6)
        {
            character.TryUseItem(new HealthPotion());
        }
    }

    private void PerformAttack(ICharacter attacker, ICharacter defender)
    {
        // Berechne Schaden
        int damage = attacker.Attack.Damage;

        // Prüfe auf doppelten Angriff
        bool doubleAttack = false;
        if (attacker.Attack.DoubleAttackChance)
        {
            int diceRoll = rnd.Next(1, 7);
            doubleAttack = (diceRoll == 2);

            if (doubleAttack)
            {
                Console.WriteLine($"{attacker.Name} ist sehr schnell und greift zweimal an!");
                damage *= 2;
            }
        }

        Console.WriteLine($"{attacker.Name} greift mit {attacker.Attack.Type} an!");

        // Verteidigung
        int originalHealth = defender.Health;
        defender.Defend(damage);
        int actualDamage = originalHealth - defender.Health;

        Console.WriteLine($"{defender.Name} erhielt {actualDamage} Schaden. \tVerbleibende Gesundheit: {defender.Health}");
    }
}

// Hauptprogramm
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Willkommen zum Kampfspiel!");

        // Charaktere erstellen
        var warrior = new Warrior("Connor MacLeod", 100, new WarriorAttack());
        var wizard = new Wizard("Gandalf", 75, new WizardAttack());

        // Spiel starten
        var game = new Game();
        game.Battle(warrior, wizard);

        Console.WriteLine("\nDrücken Sie eine Taste, um das Spiel zu beenden...");
        Console.ReadKey();
    }
}