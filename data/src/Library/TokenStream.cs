using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
public class TokenStream
{
    private List<Token> Tokens;
    private int pos;
    public int Position { get { return this.pos; } }
    public List<string> ProcessFunctions { get; private set; }
    public List<string> FieldZoness { get; private set; }
    public List<string> FieldZonesConsult { get; private set; }
    public List<string> BooleanOperators { get; private set; }
    public List<string> BooleanComparers { get; private set; }
    private void SetBooleanOperators()
    {
        BooleanOperators = new List<string>();
        BooleanOperators.Add(TokenValues.and);
        BooleanOperators.Add(TokenValues.or);
        BooleanOperators.Add(TokenValues.not);
        BooleanOperators.Add(TokenValues.binarycomparer);
        BooleanOperators.Add(TokenValues.truepredicate);
        BooleanOperators.Add(TokenValues.falsepredicate);
        BooleanOperators.Add(TokenValues.existcardin);
    }
    private void SetBooleanComparers()
    {
        BooleanComparers = new List<string>();
        BooleanComparers.Add(TokenValues.Equal);
        BooleanComparers.Add(TokenValues.Major);
        BooleanComparers.Add(TokenValues.Minor);
        BooleanComparers.Add(TokenValues.MajorOrEqual);
        BooleanComparers.Add(TokenValues.MinorOrEqual);
    }
    private void SetProcessFunctions()
    {
        ProcessFunctions = new List<string>();
        ProcessFunctions.Add(TokenValues.destroy);
        ProcessFunctions.Add(TokenValues.draw);
        ProcessFunctions.Add(TokenValues.modifyAttack);
        ProcessFunctions.Add(TokenValues.reborn);
        ProcessFunctions.Add(TokenValues.summon);
        ProcessFunctions.Add(TokenValues.switchband);
    }
    private void SetFieldZones()
    {
        FieldZoness = new List<string>();
        FieldZoness.Add(TokenValues.allowncards);
        FieldZoness.Add(TokenValues.allenemycards);
        FieldZoness.Add(TokenValues.allexistingcards);
        FieldZoness.Add(TokenValues.owndeck);
        FieldZoness.Add(TokenValues.owngraveryard);
        FieldZoness.Add(TokenValues.ownhand);
        FieldZoness.Add(TokenValues.ownmelee);
        FieldZoness.Add(TokenValues.ownmiddle);
        FieldZoness.Add(TokenValues.ownsiege);
        FieldZoness.Add(TokenValues.enemydeck);
        FieldZoness.Add(TokenValues.enemygraveryard);
        FieldZoness.Add(TokenValues.enemyhand);
        FieldZoness.Add(TokenValues.enemymelee);
        FieldZoness.Add(TokenValues.enemymiddle);
        FieldZoness.Add(TokenValues.enemysiege);
    }
    private void SetFieldZonesConsult()
    {
        FieldZonesConsult = new List<string>();
        FieldZonesConsult.Add(TokenValues.highestattackin);
        FieldZonesConsult.Add(TokenValues.lowestattackin);
        FieldZonesConsult.Add(TokenValues.numberofcardsin);
        FieldZonesConsult.Add(TokenValues.damagein);
        FieldZonesConsult.Add(TokenValues.damage);
    }
    public TokenStream(IEnumerable<Token> tokens)
    {
        this.Tokens = new List<Token>(tokens);
        this.pos = 0;
        SetProcessFunctions();
        SetFieldZones();
        SetFieldZonesConsult();
        SetBooleanOperators();
        SetBooleanComparers();
    }
    public bool ReachedEnd => Position == this.Tokens.Count - 1;
    public void MoveForward(int cant)
    {
        pos += cant;
    }
    public void MoveBackWard(int cant)
    {
        pos -= cant;
    }
    public bool MoveNext()
    {
        if (this.pos < Tokens.Count - 1)
        {
            this.pos++;
        }
        return this.pos < Tokens.Count;
    }
    public bool MoveNext(TokenType Type)
    {
        if (this.pos < Tokens.Count - 1 && LookAhead(1).Type == Type)
        {
            this.pos++;
            return true;
        }
        return false;
    }
    public bool MoveNext(string value)
    {
        if (this.pos < Tokens.Count - 1 && LookAhead(1).Value == value)
        {
            this.pos++;
            return true;
        }
        return false;
    }
    public bool CanLookAhead(int k = 0)
    {
        return Tokens.Count - this.pos > k;
    }
    public Token LookAhead(int k = 0)
    {
        return Tokens[this.pos + k];
    }
}