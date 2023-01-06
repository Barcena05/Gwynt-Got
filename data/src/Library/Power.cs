using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using Godot;

public class Power : AST_Root
{

    public string Name { get; private set; }
    List<Condition> Conditions = new List<Condition>();
    List<Instruction> Instructions = new List<Instruction>();
    public Power(string name, List<Condition> conditions, List<Instruction> instructions)
    {
        this.Name = name;
        this.Conditions = conditions;
        this.Instructions = instructions;
    }
    public bool Check()
    {
        foreach (var condition in Conditions)
        {
            if (condition.EvaluateCondition() == false) return false;
        }
        return true;
    }
    public void Execute()
    {
        GameHUD.powerData = new List<PowerData>();

        if (Check())
        {
            foreach (var instruction in Instructions)
            {
                Process.Execute(instruction.Name, instruction.Commands);
            }

            GameHUD.startProcessing = true;
            if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) GameHUD.phase = (int)GameHUD.Phase.PlayerWaiting;
            else if (GameHUD.phase == (int)GameHUD.Phase.EnemyTurn) GameHUD.phase = (int)GameHUD.Phase.EnemyWaiting;

        }else{
            if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) GameHUD.phase = (int)GameHUD.Phase.PlayerWaiting;
            else if (GameHUD.phase == (int)GameHUD.Phase.EnemyTurn) GameHUD.phase = (int)GameHUD.Phase.EnemyWaiting;
        }

        
    }
}

public class Condition
{
    public BooleanExpresion Exp { get; }
    public Condition(BooleanExpresion exp)
    {
        this.Exp = exp;
    }
    public bool EvaluateCondition()
    {
        this.Exp.Evaluate();
        return (bool)this.Exp.Value;
    }
}
public class Instruction
{
    public string Name { get; }
    public IEnumerable<object> Commands { get; }
    public Instruction(string name, IEnumerable<object> commands)
    {
        this.Name = name;
        this.Commands = commands;
    }
}
public static class Process
{
    private static void Reborn(IEnumerable<object> indications)
    {
        int place = 0;
        List<string> names = new List<string>();
        bool reborn;
        bool select;
        int cardsCounter = 0;
        List<object> Indications = indications.ToList();
        if (Indications[0] is MethodInfo)
        {
            MethodInfo method = (MethodInfo)Indications[0];
            switch (method.Name)
            {
                case "OwnGraveryard":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) place = 10;
                    else place = 11;
                    break;
                case "EnemyGraveryard":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) place = 11;
                    else place = 10;
                    break;
            }


        }

        if (Indications[1] is IEnumerable<string>)
        {
            names = (List<string>)Indications[1];
            reborn = true;
            select = false;
        }
        else
        {
            select = true;
            reborn = true;
            cardsCounter = (int)Indications[1];
        }

        GameHUD.powerData.Add(new RebornPower(place, select, names, cardsCounter));
    }
    private static void Summon(IEnumerable<object> indications)
    {
        List<string> names = new List<string>();
        bool summon;
        bool select;
        int cardsCounter = 0;
        List<object> Indications = indications.ToList();

        if (Indications[0] is IEnumerable<string>)
        {
            GD.Print("Ejuna lista");
            names = (List<string>)Indications[0];
            summon = true;
            select = false;
            foreach(var item in names){
                GD.Print(item);
            }
        }
        else
        {
            GD.Print("Ejun numero");
            select = true;
            summon = true;
            cardsCounter = (int)Indications[0];
        }
        GameHUD.powerData.Add(new SummonPower(names, summon, select, cardsCounter));
    }
    private static void SwitchBand(IEnumerable<object> indications)
    {

        string cardName = "";
        List<object> Indications = indications.ToList();

        if (Indications[0] is string) cardName = (string)Indications[0];

        GameHUD.powerData.Add(new SwitchBandPower(cardName));

    }
    private static void Destroy(IEnumerable<object> indications)
    {
        List<string> names = new List<string>();
        int identifier = 0;
        int from = 0;
        int cardsCounter = 0;
        List<object> Indications = indications.ToList();
        if (Indications[0] is MethodInfo)
        {
            MethodInfo method = (MethodInfo)Indications[0];
            switch (method.Name)
            {
                case "OwnMelee":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) from = 4;
                    else from = 5;
                    break;
                case "OwnMiddle":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) from = 6;
                    else from = 7;
                    break;
                case "OwnSiege":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) from = 8;
                    else from = 9;
                    break;
                case "EnemyMelee":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) from = 5;
                    else from = 4;
                    break;
                case "EnemyMiddle":
                    if(GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) from = 7;
                    else from = 6;
                    break;
                case "EnemySiege":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) from = 9;
                    else from = 8;
                    break;
            }
            identifier = 0;
        }
        else if (Indications[0] is IEnumerable<string>)
        {
            names = (List<string>)Indications[0];
            identifier = 1;
        }
        else if (Indications[0] is int)
        {
            cardsCounter = (int)Indications[0];
            identifier = 2;
        }

        GameHUD.powerData.Add(new DestroyPower(identifier, from, names, cardsCounter));
    }
    private static void ModifyAttack(IEnumerable<object> indications)
    {
        List<object> Indications = (List<object>)indications;
        int where = 0;
        int ammount = 0;
        int identifier = 0;
        ammount = (int)Indications[0];
        int cardsCounter = 0;
        List<string> names = new List<string>();

        if (Indications[1] is MethodInfo)
        {
            identifier = 0;
            MethodInfo method = (MethodInfo)Indications[1];
            switch (method.Name)
            {
                case "OwnMelee":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) where = 4;
                    else where = 5;
                    break;
                case "OwnMiddle":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) where = 6;
                    else where = 7;
                    break;
                case "OwnSiege":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) where = 8;
                    else where = 9;
                    break;
                case "EnemyMelee":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) where = 5;
                    else where = 4;
                    break;
                case "EnemyMiddle":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) where = 7;
                    else where = 6;
                    break;
                case "EnemySiege":
                    if (GameHUD.phase == (int)GameHUD.Phase.PlayerTurn) where = 9;
                    else  where = 8;
                    break;
            }

        }
        else if (Indications[1] is IEnumerable<string>)
        {
            identifier = 1;
            names = (List<string>)Indications[1];
        }
        else if (Indications[1] is int)
        {
            identifier = 2;
            cardsCounter = (int)Indications[1];
        }
        else if (Indications[1] == TokenValues.freeelection)
        {
            GD.Print("Esto es un cuerno");
            identifier = 3;
        }

        GD.Print("este es el were" + where);
        GameHUD.powerData.Add(new ModifyAttackPower(where, ammount, identifier, cardsCounter, names));
    }
    private static void Draw(IEnumerable<object> indications)
    {
        int cardsCounter = 0;
        List<object> Indications = (List<object>)indications;
        if (Indications[0] is int) cardsCounter = (int)Indications[0];

        GameHUD.powerData.Add(new DrawPower(cardsCounter));
    }
    public static void Execute(string id, IEnumerable<object> commands)
    {
        switch (id)
        {
            case "Reborn":
                Process.Reborn(commands);
                break;
            case "Summon":
                Process.Summon(commands);
                break;
            case "Destroy":
                Process.Destroy(commands);
                break;
            case "ModifyAttack":
                Process.ModifyAttack(commands);
                break;
            case "Draw":
                Process.Draw(commands);
                break;
            case "SwitchBand":
                Process.SwitchBand(commands);
                break;
            default:
                break;
        }

    }
}
public static class FieldZones
{
    public static List<Cards> Llama(string name)
    {
        string classname = "FieldZones";
        Type called = Type.GetType(classname);
        MethodInfo Method = called.GetMethod(name);
        if (Method is null) throw new Exception("No existe el metodo");
        List<Cards> result = ((IEnumerable<Cards>)Method.Invoke(null, null)).ToList();
        if (result is null) throw new Exception("Parametros incorrectos");
        else return result;
    }
    public static IEnumerable<Cards> OwnHand()
    {
        return GameHUD.Positions.Places[2].Values.ToList();
    }
    public static IEnumerable<Cards> OwnMelee()
    {
        return GameHUD.Positions.Places[4].Values.ToList();
    }
    public static IEnumerable<Cards> OwnMiddle()
    {
        return GameHUD.Positions.Places[6].Values.ToList();
    }
    public static IEnumerable<Cards> OwnSiege()
    {
        return GameHUD.Positions.Places[8].Values.ToList();
    }
    public static IEnumerable<Cards> OwnGraveryard()
    {
        return GameHUD.Positions.Places[10].Values.ToList();
    }
    public static IEnumerable<Cards> OwnDeck()
    {
        return GameHUD.Positions.Places[0].Values.ToList();
    }
    public static IEnumerable<Cards> EnemyHand()
    {
        return GameHUD.Positions.Places[3].Values.ToList();
    }
    public static IEnumerable<Cards> EnemyMelee()
    {
        return GameHUD.Positions.Places[5].Values.ToList();
    }
    public static IEnumerable<Cards> EnemyMiddle()
    {
        return GameHUD.Positions.Places[7].Values.ToList();
    }
    public static IEnumerable<Cards> EnemySiege()
    {
        return GameHUD.Positions.Places[9].Values.ToList();
    }
    public static IEnumerable<Cards> EnemyGraveryard()
    {
        return GameHUD.Positions.Places[11].Values.ToList();
    }
    public static IEnumerable<Cards> EnemyDeck()
    {
        return GameHUD.Positions.Places[1].Values.ToList();
    }
    public static IEnumerable<Cards> AllOwnCards()
    {
        return (GameHUD.Positions.Places[4].Values.ToList().Concat(GameHUD.Positions.Places[6].Values.ToList())).Concat(GameHUD.Positions.Places[8].Values.ToList());
    }
    public static IEnumerable<Cards> AllEnemyCards()
    {
        return (GameHUD.Positions.Places[5].Values.ToList().Concat(GameHUD.Positions.Places[7].Values.ToList())).Concat(GameHUD.Positions.Places[9].Values.ToList());
    }
    public static IEnumerable<Cards> AllExistingCards()
    {
        List<Cards> List0 = AllOwnCards().ToList();
        List<Cards> List1 = AllEnemyCards().ToList();        
        return (List0.Concat(List1));
    }
}
// public static class FieldZonesConsults
// {
//     public static object Llama(string name, object[] parameters)
//     {
//         string classname = "Proyecto_2do_semestre.FieldZonesConsults";
//         Type called = Type.GetType(classname);
//         MethodInfo Method = called.GetMethod(name);
//         if(Method is null) throw new Exception("No existe el metodo");
//         object result = Method.Invoke(null, parameters);
//         if(result is null) throw new Exception("Parametros incorrectos");
//         else return result;
//     }
//     private static Card HighestAttackIn(IEnumerable<object> indications)
//     {
//         return null;
//     }
//     private static Card LowestAttackIn(IEnumerable<object> indications)
//     {
//         return null;
//     }
//     private static int NumberOfCardsIn(IEnumerable<object> indications)
//     {
//         return 0;
//     }
//     private static int Damage(IEnumerable<object> indications)
//     {
//         return 0;
//     }
//     private static int DamageIn(IEnumerable<object> indications)
//     {
//         return 0;
//     }
// }

