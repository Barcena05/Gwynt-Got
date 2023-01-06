using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

//La clase cards, es una clase abstracta, donde los subtipos UnitCard, EffectCard y LeaderCard heredaran de ella
//propiedades que son generales para todas las cartas del juego. Independientemente a eso, los subtipos tendran
//sus propiedades especificas que lo diferenciaran de los otros subtipos.
public abstract class Cards : AST_Root
{
    public abstract string name { get; protected set; }
    public abstract string type { get; protected set; }
    public abstract string imagePath { get; protected set; }
    public abstract Power power { get; protected set; }
    public abstract string band {get; set;}

}
public class UnitCard : Cards
{
    public override string name { get; protected set; }
    public override string type { get; protected set; }
    public override string imagePath { get; protected set; }
    public override string band { get; set; }
    public override Power power { get; protected set; }
    public string phrase { get; protected set; }
    public string position { get; protected set; }
    public int damage { get; protected set; }
    public UnitCard(string Name, string Type, string ImagePath, Power Power, string Phrase, string Position, int Damage)
    {
        this.name = Name;
        this.type = Type;
        this.imagePath = ImagePath;
        this.power = Power;
        this.phrase = Phrase;
        this.position = Position;
        this.damage = Damage;
    }
}
public class LeaderCard : Cards
{
    public override string name { get; protected set; }
    public override string type { get; protected set; }
    public override string imagePath { get; protected set; }
    public override string band { get; set; }
    public override Power power { get; protected set; }
    public string phrase { get; protected set; }
    public LeaderCard(string Name, string Type, string ImagePath, Power Power, string Phrase)
    {
        this.name = Name;
        this.type = Type;
        this.imagePath = ImagePath;
        this.power = Power;
        this.phrase = Phrase;
    }
}
public class EffectCard : Cards
{
    public override string name { get; protected set; }
    public override string type { get; protected set; }
    public override string imagePath { get; protected set; }
    public override string band { get; set; }
    public override Power power { get; protected set; }
    public string position { get; protected set; }
    public EffectCard(string Name, string Type, string ImagePath, Power Power, string Position)
    {
        this.name = Name;
        this.type = Type;
        this.imagePath = ImagePath;
        this.power = Power;
        this.position = Position;
    }
}