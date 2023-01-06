using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
// La clase abstracta ArithmeticExpressions hereda de Expressions y sobreescribe el ExpressionType con un valor numerico
public abstract class ArithmeticExpressions : Expressions
{
    public override ExpressionType Type
    {
        get { return ExpressionType.number; }
        set { }
    }
}
// La clase abstracta BinaryExpression hereda de ArithmeticExpressions y posee dos ArithmeticExpressions, Left y Right
public abstract class BinaryExpressions : ArithmeticExpressions
{
    public ArithmeticExpressions Left;
    public ArithmeticExpressions Right;
}
// La clase abstracta UnaryExpressions hereda de ArithmeticExpressions y posee una ArithmeticExpressions Expression
public abstract class UnaryExpressions : ArithmeticExpressions
{
    public ArithmeticExpressions Expression;
}
// La clase Sum hereda de BinaryExpressions y recibe en su constructor dos ArithmeticExpressions, asignandolas a los campos Left y Right
// de BinaryExpressions. La clase sobreescribe el metodo evaluate
public class Sum : BinaryExpressions
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public Sum(ArithmeticExpressions left, ArithmeticExpressions right)
    {
        this.Left = left;
        this.Right = right;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = (double)this.Left.Value + (double)this.Right.Value;
    }
}
public class Sub : BinaryExpressions
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public Sub(ArithmeticExpressions left, ArithmeticExpressions right)
    {
        this.Left = left;
        this.Right = right;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = (double)this.Left.Value - (double)this.Right.Value;
    }
}
public class Mult : BinaryExpressions
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public Mult(ArithmeticExpressions left, ArithmeticExpressions right)
    {
        this.Left = left;
        this.Right = right;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = (double)this.Left.Value * (double)this.Right.Value;
    }
}
public class Div : BinaryExpressions
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public Div(ArithmeticExpressions left, ArithmeticExpressions right)
    {
        this.Left = left;
        this.Right = right;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = (double)this.Left.Value / (double)this.Right.Value;
    }
}
public class Number : UnaryExpressions
{
    public override object Value { get; set; }
    private MethodInfo Method;
    private string CardName;
    private string IDValue;
    private string Zone;
    public override ExpressionType Type
    {
        get { return ExpressionType.number; }
        set { }
    }
    public Number(int value)
    {
        this.Value = value;
    }
    public Number(MethodInfo method, string IdValue, string zone)
    {
        this.Method = method;
        this.IDValue = IdValue;
        this.Zone = zone;
    }
    public Number(string CardName)
    {
        this.CardName = CardName;
    }
    public override void Evaluate()
    {
        if (Method != null)
        {
            switch (IDValue)
            {
                case TokenValues.numberofcardsin:
                    List<Cards> cards = (List<Cards>)Method.Invoke(null, new object[] { this.Zone });
                    this.Value = cards.Count();
                    break;
                case TokenValues.damagein:
                    List<Cards> cards2 = (List<Cards>)Method.Invoke(null, new object[] { this.Zone });
                    int sum = 0;
                    foreach (var item in cards2)
                    {
                        if (item is UnitCard)
                        {
                            sum += ((UnitCard)item).damage;
                        }
                    }
                    this.Value = sum;
                    break;
                case TokenValues.highestattackin:
                    List<Cards> cards3 = (List<Cards>)Method.Invoke(null, new object[] { this.Zone });
                    int highest = 0;
                    foreach (var item in cards3)
                    {
                        if (item is UnitCard && ((UnitCard)item).damage > highest)
                        {
                            highest = ((UnitCard)item).damage;
                        }
                    }
                    this.Value = highest;
                    break;
                case TokenValues.lowestattackin:
                    List<Cards> cards4 = (List<Cards>)Method.Invoke(null, new object[] { this.Zone });
                    int lowest = 0;
                    foreach (var item in cards4)
                    {
                        if (item is UnitCard && ((UnitCard)item).damage < lowest)
                        {
                            lowest = ((UnitCard)item).damage;
                        }
                    }
                    this.Value = lowest;
                    break;
                default:
                    break;
            }

        }
        else if (!(CardName is null))
        {
            int attack = 0;
            List<Cards> cards = FieldZones.Llama(TokenValues.allexistingcards);
            foreach (var item in cards)
            {
                if (item is UnitCard && item.name == CardName)
                {
                    attack = ((UnitCard)item).damage;
                }
            }
            this.Value = attack;
        }
    }
}