using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using Godot;

public abstract class BooleanExpresion : Expressions
{
    public override ExpressionType Type { get => ExpressionType.boolean; set { Type = ExpressionType.boolean; } }

}
public abstract class BinaryBoolean : BooleanExpresion
{
    public Expressions Left;
    public Expressions Right;
}
public abstract class UnaryBoolean : BooleanExpresion
{
    public BooleanExpresion Expression;
}
public class Not : UnaryBoolean
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public Not(BooleanExpresion expression)
    {
        this.Expression = expression;
    }
    public override void Evaluate()
    {
        this.Expression.Evaluate();
        this.Value = !(bool)this.Expression.Value;
    }
}
public class And : BinaryBoolean
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public And(BooleanExpresion left, BooleanExpresion right)
    {
        this.Left = left;
        this.Right = right;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = (bool)this.Left.Value && (bool)this.Right.Value;
    }
}
public class Or : BinaryBoolean
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    public Or(BooleanExpresion left, BooleanExpresion right)
    {
        this.Left = left;
        this.Right = right;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = (bool)this.Left.Value || (bool)this.Right.Value;
    }
}
public delegate bool BooleanComparer(int a, int b);
public static class BooleanComparers
{
    public static BooleanComparer Equal = (int a, int b) => a == b;
    public static BooleanComparer Major = (int a, int b) => a > b;
    public static BooleanComparer Minor = (int a, int b) => a < b;
    public static BooleanComparer MajorOrEqual = (int a, int b) => a >= b;
    public static BooleanComparer MinorOrEqual = (int a, int b) => a <= b;

}

public class BinaryComparer : BinaryBoolean
{
    public override object Value { get; set; }
    public override ExpressionType Type { get; set; }
    private BooleanComparer Criteria;
    public BinaryComparer(ArithmeticExpressions left, ArithmeticExpressions right, BooleanComparer criteria)
    {
        this.Left = left;
        this.Right = right;
        this.Criteria = criteria;
    }
    public override void Evaluate()
    {
        this.Left.Evaluate();
        this.Right.Evaluate();
        this.Value = Criteria((int)this.Left.Value, (int)this.Right.Value);
    }
}
public class TruePredicate : UnaryBoolean
{
    public override void Evaluate()
    {

    }
    public override object Value { get; set; }
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.boolean;
        }
        set { }
    }
    public TruePredicate()
    {
        this.Value = true;
    }
}
public class FalsePredicate : UnaryBoolean
{
    public override void Evaluate()
    {

    }
    public override object Value { get; set; }
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.boolean;
        }
        set { }
    }
    public FalsePredicate()
    {
        this.Value = false;
    }
}
public class Exist : UnaryBoolean
{
    private string CardName;
    public override object Value { get; set; }
    private object[] Zone = new object[1];
    private List<Cards> list = new List<Cards>();
    MethodInfo Method;

    public Exist(string cardname, MethodInfo method, string zone)
    {
        this.CardName = cardname;
        this.Method = method;
        this.Zone[0] = zone;
    }
    public override void Evaluate()
    {
        GD.Print(this.CardName);
        GD.Print(this.Method.Name);
        GD.Print(this.Zone[0]);
        GD.Print("Evaluando existencia");
        
        if(GameHUD.phase == (int)GameHUD.Phase.EnemyTurn){
            switch(this.Zone[0]){
                
                case "OwnMelee":
                this.Zone[0] = "EnemyMelee";
                break;
                case "OwnMiddle":
                this.Zone[0] = "EnemyMiddle";
                break;
                case "OwnSiege":
                this.Zone[0] = "EnemySiege";
                break;
                case "OwnHand":
                this.Zone[0] = "EnemyHand";
                break;
                case "OwnGraveryard":
                this.Zone[0] = "EnemyGraveryard";
                break;
                case "OwnDeck":
                this.Zone[0] = "EnemyDeck";
                break;
                case "EnemyHand":
                this.Zone[0] = "OwnHand";
                break;
                case "EnemyMelee":
                this.Zone[0] = "OwnMelee";
                break;
                case "EnemyMiddle":
                this.Zone[0] = "OwnMiddle";
                break;
                case "EnemySiege":
                this.Zone[0] = "OwnSiege";
                break;
                case "EnemyGraveryard":
                this.Zone[0] = "OwnGraveryard";
                break;
                case "EnemyDeck":
                this.Zone[0] = "OwnDeck";
                break;
                case "AllOwnCards":
                this.Zone[0] = "AllEnemyCards";
                break;
                case "AllEnemyCards":
                this.Zone[0] = "AllOwnCards";
                break;
            }
        }
        
        this.list = (List<Cards>)this.Method.Invoke(null, this.Zone);
        bool contains()
        {
            foreach (var item in this.list)
            {
                if (item.name == CardName) return true;
            }
            return false;
        }
        this.Value = contains();
    }

}