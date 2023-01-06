using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

public class Parser
{
    public TokenStream Stream { get; private set; }
    public Parser(TokenStream stream)
    {
        this.Stream = stream;

    }
    public Cards ParseCard()
    {
        Cards answer;
        if (!Stream.CanLookAhead()) throw new Exception("Error");
        switch (Stream.LookAhead().Value)
        {
            case TokenValues.unitcard:
                answer = ParseUnitCard();
                break;
            case TokenValues.leadercard:
                answer = ParseLeaderCard();
                break;
            case TokenValues.effectcard:
                answer = ParseEffectCard();
                break;
            default:
                throw new Exception($"Card identifier expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                break;
        }
        return answer;
    }
    public Power ParsePower()
    {
        string name;
        List<Condition> ConditionSet;
        List<Instruction> InstructionSet;
        if (!(Stream.LookAhead().Value == TokenValues.power))
        {
            throw new Exception($"Power expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        if (this.Stream.MoveNext(TokenType.identifier))
        {
            name = Stream.LookAhead().Value;
        }
        else throw new Exception($"ID expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        ConditionSet = ParseConditionSet().ToList();
        InstructionSet = ParseInstructionSet().ToList();

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Power(name, ConditionSet, InstructionSet);
    }
    public IEnumerable<Condition> ParseConditionSet()
    {
        List<Condition> answer = new List<Condition>();
        if (!Stream.MoveNext(TokenValues.conditionset))
        {
            throw new Exception($"ConditionSet expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        while (Stream.MoveNext(TokenValues.condition))
        {
            answer.Add(ParseCondition());
        }

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        return answer;
    }
    public IEnumerable<Instruction> ParseInstructionSet()
    {
        List<Instruction> answer = new List<Instruction>();
        if (!Stream.MoveNext(TokenValues.instructionset))
        {
            throw new Exception($"InstructionSet expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        while (Stream.MoveNext(TokenValues.instruction))
        {
            answer.Add(ParseInstruction());
        }

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        return answer;
    }
    ArithmeticExpressions ParseArithmeticExpression()
    {
        ArithmeticExpressions answer = null;
        Stream.MoveNext();
        switch (Stream.LookAhead().Value)
        {
            case TokenValues.sum:
                answer = ParseSum();
                break;
            case TokenValues.sub:
                answer = ParseSub();
                break;
            case TokenValues.mult:
                answer = ParseMult();
                break;
            case TokenValues.div:
                answer = ParseDiv();
                break;
            case TokenValues.damage:
                answer = ParseCardDamage();
                break;
            case TokenValues.damagein:
                answer = ParseDamageIn();
                break;
            case TokenValues.numberofcardsin:
                answer = ParseNumberOfCardsIn();
                break;
            default:
                if (Stream.LookAhead().Type == TokenType.number)
                {
                    answer = ParseNumber();
                }
                else throw new Exception($"Boolean Operator expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                break;
        }
        return answer;
    }
    ArithmeticExpressions ParseSum()
    {
        ArithmeticExpressions left;
        ArithmeticExpressions right;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Sum(left, right);
    }
    ArithmeticExpressions ParseSub()
    {
        ArithmeticExpressions left;
        ArithmeticExpressions right;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Sub(left, right);
    }
    ArithmeticExpressions ParseMult()
    {
        ArithmeticExpressions left;
        ArithmeticExpressions right;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Mult(left, right);
    }
    ArithmeticExpressions ParseDiv()
    {
        ArithmeticExpressions left;
        ArithmeticExpressions right;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Div(left, right);
    }
    ArithmeticExpressions ParseNumber()
    {
        return new Number(int.Parse(Stream.LookAhead().Value));
    }
    ArithmeticExpressions ParseCardDamage()
    {
        string name = null;
        MethodInfo Method = null;
        string zone = null;
        string Value = null;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.MoveNext(TokenValues.card))
        {
            if (Stream.MoveNext(TokenType.identifier))
            {
                name = Stream.LookAhead().Value;
            }
        }
        else if (Stream.MoveNext(TokenValues.highestattackin) || Stream.MoveNext(TokenValues.lowestattackin))
        {
            switch (Stream.LookAhead().Value)
            {
                case TokenValues.highestattackin:
                    if (!Stream.MoveNext(TokenValues.OpenBracket))
                    {
                        throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    }
                    if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
                    {
                        Stream.MoveNext();
                        zone = Stream.LookAhead().Value;
                        Type MyType = Type.GetType("FieldZones");
                        Method = MyType.GetMethod("Llama");
                        Value = TokenValues.highestattackin;
                        Stream.MoveForward(1);
                    }
                    else throw new Exception($"Field Zone expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    if (!Stream.MoveNext(TokenValues.ClosedBracket))
                    {
                        throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    }
                    break;

                case TokenValues.lowestattackin:
                    if (!Stream.MoveNext(TokenValues.OpenBracket))
                    {
                        throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    }
                    if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
                    {
                        Stream.MoveNext();
                        zone = Stream.LookAhead().Value;
                        Type MyType = Type.GetType("FieldZones");
                        Method = MyType.GetMethod("Llama");
                        Value = TokenValues.lowestattackin;
                        Stream.MoveForward(1);
                    }
                    else throw new Exception($"Field Zone expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    if (!Stream.MoveNext(TokenValues.ClosedBracket))
                    {
                        throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    }
                    break;
                default:
                    break;
            }
        }
        else throw new Exception($"Card or Card return function expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!(name is null))
        {
            return new Number(name);
        }
        else
        {
            return new Number(Method, Value, zone);
        }

    }
    ArithmeticExpressions ParseDamageIn()
    {
        MethodInfo Method;
        string methodName;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
        {
            methodName = Stream.LookAhead(1).Value;
            Type MyType = Type.GetType("FieldZones");
            Method = MyType.GetMethod("Llama");
            Stream.MoveForward(1);
        }
        else throw new Exception($"Field Zone expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Number(Method, TokenValues.damagein, methodName);
    }
    ArithmeticExpressions ParseNumberOfCardsIn()
    {
        MethodInfo Method;
        string methodName;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
        {
            methodName = Stream.LookAhead(1).Value;
            Type MyType = Type.GetType("FieldZones");
            Method = MyType.GetMethod("Llama");
            Stream.MoveForward(1);
        }
        else throw new Exception($"Field Zone expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Number(Method, TokenValues.numberofcardsin, methodName);
    }
    BooleanExpresion ParseBooleanExpression()
    {
        BooleanExpresion answer = null;
        Stream.MoveNext();
        switch (Stream.LookAhead().Value)
        {
            case TokenValues.not:
                answer = ParseNot();
                break;
            case TokenValues.and:
                answer = ParseAnd();
                break;
            case TokenValues.or:
                answer = ParseOr();
                break;
            case TokenValues.binarycomparer:
                answer = ParseBinaryComparer();
                break;
            case TokenValues.truepredicate:
                answer = ParseTruePredicate();
                break;
            case TokenValues.falsepredicate:
                answer = ParseFalsePredicate();
                break;
            case TokenValues.existcardin:
                answer = ParseExistCardIn();
                break;
            default:
                throw new Exception($"Boolean operator expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                break;
        }
        return answer;
    }
    BooleanExpresion ParseNot()
    {
        BooleanExpresion answer;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        answer = ParseBooleanExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Not(answer);
    }
    BooleanExpresion ParseAnd()
    {
        BooleanExpresion left;
        BooleanExpresion right;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseBooleanExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseBooleanExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new And(left, right);
    }
    BooleanExpresion ParseOr()
    {
        BooleanExpresion left;
        BooleanExpresion right;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseBooleanExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseBooleanExpression();
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Or(left, right);
    }
    BooleanExpresion ParseBinaryComparer()
    {
        ArithmeticExpressions left;
        ArithmeticExpressions right;
        BooleanComparer criteria;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        left = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        right = ParseArithmeticExpression();
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.BooleanComparers.Contains(Stream.LookAhead(1).Value))
        {
            Stream.MoveNext();
            switch (Stream.LookAhead().Value)
            {
                case TokenValues.Equal:
                    criteria = BooleanComparers.Equal;
                    break;
                case TokenValues.Major:
                    criteria = BooleanComparers.Major;
                    break;
                case TokenValues.Minor:
                    criteria = BooleanComparers.Minor;
                    break;
                case TokenValues.MajorOrEqual:
                    criteria = BooleanComparers.MajorOrEqual;
                    break;
                case TokenValues.MinorOrEqual:
                    criteria = BooleanComparers.MinorOrEqual;
                    break;
                default:
                    throw new Exception($"Comparisson criteria expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
            }
        }
        else throw new Exception($"Comparer criteria expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new BinaryComparer(left, right, criteria);
    }
    BooleanExpresion ParseTruePredicate()
    {
        return new TruePredicate();
    }
    BooleanExpresion ParseFalsePredicate()
    {
        return new FalsePredicate();
    }
    BooleanExpresion ParseExistCardIn()
    {
        string cardname = null;
        string zone = null;
        MethodInfo Method = null;
        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName}  in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenValues.card))
        {
            throw new Exception($"Card expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        
        if (!Stream.MoveNext(TokenType.identifier))
        {
            throw new Exception($"Identifier expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        cardname = Stream.LookAhead().Value;
        if (!Stream.MoveNext(TokenValues.ValueSeparator))
        {
            throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
        {
            zone = Stream.LookAhead(1).Value;
            Type MyType = Type.GetType("FieldZones");
            Method = MyType.GetMethod("Llama");
            Stream.MoveForward(1);
        }
        else throw new Exception($"Field Zone expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Exist(cardname, Method, zone);
    }
    public Condition ParseCondition()
    {
        BooleanExpresion answer;
        if (!(Stream.LookAhead().Value == TokenValues.condition))
        {
            throw new Exception($"Condition expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.BooleanOperators.Contains(Stream.LookAhead(1).Value))
        {
            answer = ParseBooleanExpression();
        }
        else throw new Exception($"Boolean Operator expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Condition(answer);
    }
    public Instruction ParseInstruction()
    {
        string name;
        List<object> parameters = new List<object>();
        if (Stream.LookAhead().Value != TokenValues.instruction)
        {
            throw new Exception($"Instruction expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (Stream.ProcessFunctions.Contains(Stream.LookAhead(1).Value))
        {
            name = Stream.LookAhead(1).Value;
            Stream.MoveForward(1);
        }
        else throw new Exception($"Process function expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (!Stream.MoveNext(TokenValues.OpenBracket))
        {
            throw new Exception($"( expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        List<object> ParseDestroy()
        {
            List<object> result = new List<object>();
            if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
            {
                string methodName = Stream.LookAhead(1).Value;
                if (methodName == TokenValues.ownmelee || methodName == TokenValues.ownmiddle || methodName == TokenValues.ownsiege || methodName == TokenValues.enemymelee || methodName == TokenValues.enemymiddle || methodName == TokenValues.enemysiege)
                {
                    Type MyType = Type.GetType("FieldZones");
                    MethodInfo Method = MyType.GetMethod(methodName);
                    result.Add(Method);
                    Stream.MoveForward(1);
                }
                else throw new Exception($"Wrong Field Zone chosen in line:{Stream.LookAhead(1).Location.Line}");
            }
            else if (Stream.LookAhead(1).Type is TokenType.number)
            {
                result.Add(int.Parse(Stream.LookAhead(1).Value));
                Stream.MoveForward(1);
            }
            else if (Stream.LookAhead(1).Value == TokenValues.card)
            {
                List<string> names = new List<string>();
                while (Stream.MoveNext(TokenValues.card))
                {
                    string name0;
                    if (!Stream.MoveNext(TokenType.identifier))
                    {
                        throw new Exception("ID expected in card {Stream.LookAhead().Location.CardName}");
                    }
                    name0 = Stream.LookAhead().Value;
                    if (!Stream.MoveNext(TokenValues.ValueSeparator))
                    {
                        if (Stream.LookAhead(1).Value != TokenValues.ClosedBracket) throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    }
                    names.Add(name0);
                }
                result.Add(names);
            }
            else throw new Exception("Wrong arguments");
            return result;
        }
        List<Object> ParseDraw()
        {
            List<object> result = new List<object>();
            if (Stream.LookAhead(1).Type is TokenType.number)
            {
                result.Add(int.Parse(Stream.LookAhead(1).Value));
                Stream.MoveForward(1);
            }
            else throw new Exception("Number of cards expected in card {Stream.LookAhead().Location.CardName}");
            return result;
        }
        List<Object> ParseReborn()
        {
            List<object> answer = new List<object>();
            if (Stream.LookAhead(1).Value == TokenValues.owngraveryard || Stream.LookAhead(1).Value == TokenValues.enemygraveryard)
            {
                Stream.MoveForward(1);
                string methodName = Stream.LookAhead().Value;
                Type MyType = Type.GetType("FieldZones");
                MethodInfo Method = MyType.GetMethod(methodName);
                answer.Add(Method);
                if (!Stream.MoveNext(TokenValues.ValueSeparator)) throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                if (Stream.MoveNext(TokenType.number))
                {
                    answer.Add(int.Parse(Stream.LookAhead().Value));
                }
                else if (Stream.LookAhead(1).Value == TokenValues.card)
                {
                    List<string> names = new List<string>();
                    while (Stream.MoveNext(TokenValues.card))
                    {
                        string name2;
                        if (!Stream.MoveNext(TokenType.identifier))
                        {
                            throw new Exception("ID expected in card {Stream.LookAhead().Location.CardName}");
                        }
                        name2 = Stream.LookAhead().Value;
                        if (!Stream.MoveNext(TokenValues.ValueSeparator))
                        {
                            if (Stream.LookAhead(1).Value != TokenValues.ClosedBracket) throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                        }
                        names.Add(name2);
                    }
                    answer.Add(names);
                }
                else throw new Exception("Number or cards expected in card {Stream.LookAhead().Location.CardName}");
            }

            else throw new Exception("Graveryard Selection expected in card {Stream.LookAhead().Location.CardName}");
            return answer;
        }
        List<Object> ParseSummon()
        {
            List<object> answer = new List<object>();
            if (Stream.LookAhead(1).Type is TokenType.number)
            {
                answer.Add(int.Parse(Stream.LookAhead(1).Value));
                Stream.MoveForward(1);
            }
            else if (Stream.LookAhead(1).Value == TokenValues.card)
            {
                
                List<string> cardnames = new List<string>();
                while (Stream.MoveNext(TokenValues.card))
                {
                    string name3;
                    if (!Stream.MoveNext(TokenType.identifier))
                    {
                        throw new Exception("ID expected in card {Stream.LookAhead().Location.CardName}");
                    }
                    name3 = Stream.LookAhead().Value;
                    if (!Stream.MoveNext(TokenValues.ValueSeparator))
                    {
                        if (Stream.LookAhead(1).Value != TokenValues.ClosedBracket) throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                    }
                    cardnames.Add(name3);
                }
                answer.Add(cardnames);
            }
            else throw new Exception("Number or cards expected in card {Stream.LookAhead().Location.CardName}");
            return answer;
        }
        List<object> ParseModifyAttack()
        {
            List<object> answer = new List<object>();
            if (Stream.MoveNext(TokenType.number))
            {
                answer.Add(int.Parse(Stream.LookAhead().Value));
                if (!Stream.MoveNext(TokenValues.ValueSeparator))
                {
                    throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
                }
                if (Stream.MoveNext(TokenType.number))
                {
                    answer.Add(int.Parse(Stream.LookAhead().Value));
                }
                else if (Stream.MoveNext(TokenValues.freeelection))
                {
                    answer.Add(Stream.LookAhead().Value);
                }
                else if (Stream.LookAhead(1).Value == TokenValues.card)
                {
                    List<string> names = new List<string>();
                    while (Stream.MoveNext(TokenValues.card))
                    {
                        string name4;
                        if (!Stream.MoveNext(TokenType.identifier))
                        {
                            throw new Exception("ID expected in card {Stream.LookAhead().Location.CardName}");
                        }
                        name4 = Stream.LookAhead().Value;
                        if (!Stream.MoveNext(TokenValues.ValueSeparator))
                        {
                            if (Stream.LookAhead(1).Value != TokenValues.ClosedBracket) throw new Exception($", expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead(1).Location.Line}");
                        }
                        names.Add(name4);
                    }
                    answer.Add(names);
                }
                else if (Stream.FieldZoness.Contains(Stream.LookAhead(1).Value))
                {
                    string methodName = Stream.LookAhead(1).Value;
                    if (methodName == TokenValues.ownmelee || methodName == TokenValues.ownmiddle || methodName == TokenValues.ownsiege || methodName == TokenValues.enemymelee || methodName == TokenValues.enemymiddle || methodName == TokenValues.enemysiege)
                    {
                        Type MyType = Type.GetType("FieldZones");
                        MethodInfo Method = MyType.GetMethod(methodName);
                        answer.Add(Method);
                        Stream.MoveForward(1);
                    }
                    else throw new Exception($"Wrong zone at line {Stream.LookAhead(1).Location.Line}");
                }
                else throw new Exception("Number of cards, cards or field zone expected in card {Stream.LookAhead().Location.CardName}");
            }           
            else throw new Exception($"Increase ammount {Stream.LookAhead(1).Location.Line}");
            return answer;
        }
        List<object> ParseSwitchBand()
        {
            List<object> answer = new List<object>();
            if (!Stream.MoveNext(TokenValues.card))
            {
                throw new Exception($"Card expected in card {Stream.LookAhead().Location.CardName} in line: {Stream.LookAhead().Location.Line}");
            }
            if (!Stream.MoveNext(TokenType.identifier))
            {
                throw new Exception($"Card identifier expected in card {Stream.LookAhead().Location.CardName} in line: {Stream.LookAhead().Location.Line}");
            }
            answer.Add(Stream.LookAhead().Value);

            return answer;
        }
        switch (name)
        {
            case TokenValues.destroy:
                parameters = ParseDestroy();
                break;
            case TokenValues.draw:
                parameters = ParseDraw();
                break;
            case TokenValues.reborn:
                parameters = ParseReborn();
                break;
            case TokenValues.summon:
                parameters = ParseSummon();
                break;
            case TokenValues.modifyAttack:
                parameters = ParseModifyAttack();
                break;
            case TokenValues.switchband:
                parameters = ParseSwitchBand();
                break;
            default:
                break;
        }
        if (!Stream.MoveNext(TokenValues.ClosedBracket))
        {
            throw new Exception($") expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new Instruction(name, parameters);
    }
    public UnitCard ParseUnitCard()
    {
        string Name = "";
        string Type = "";
        string Path = "";
        Power power = null;
        string Phrase = "";
        string Position = "";
        int Damage = 0;
        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        Stream.MoveForward(1);

        if (this.Stream.LookAhead().Value == TokenValues.name)
        {
            Name = ParseName();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Name expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.type)
        {
            Type = ParseType(TokenValues.unitcard);
            Stream.MoveForward(1);
        }
        else throw new Exception($"Type expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.path)
        {
            Path = ParseImage();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Path expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.power)
        {
            power = ParsePower();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Power expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.phrase)
        {
            Phrase = ParsePhrase();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Phrase expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.position)
        {
            Position = ParsePosition(TokenValues.unitcard);
            Stream.MoveForward(1);
        }
        else throw new Exception($"Position expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.attack)
        {
            Damage = ParseDamage();
        }
        else throw new Exception($"Attack expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new UnitCard(Name, Type, Path, power, Phrase, Position, Damage);
    }
    public LeaderCard ParseLeaderCard()
    {
        string Name = "";
        string Type = "";
        string Path = "";
        Power power = null;
        string Phrase = "";
        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        Stream.MoveForward(1);

        if (this.Stream.LookAhead().Value == TokenValues.name)
        {
            Name = ParseName();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Name expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.type)
        {
            Type = ParseType(TokenValues.leadercard);
            Stream.MoveForward(1);
        }
        else throw new Exception($"Type expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.path)
        {
            Path = ParseImage();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Path expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.power)
        {
            power = ParsePower();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Power expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.phrase)
        {
            Phrase = ParsePhrase();
        }
        else throw new Exception($"Phrase expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new LeaderCard(Name, Type, Path, power, Phrase);
    }
    public EffectCard ParseEffectCard()
    {
        string Name = "";
        string Type = "";
        string Path = "";
        Power power = null;
        string Position = "";
        if (!Stream.MoveNext(TokenValues.OpenCurlyBrackets))
        {
            throw new Exception($"Open Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        Stream.MoveForward(1);

        if (this.Stream.LookAhead().Value == TokenValues.name)
        {
            Name = ParseName();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Name expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.type)
        {
            Type = ParseType(TokenValues.effectcard);
            Stream.MoveForward(1);
        }
        else throw new Exception($"Type expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.path)
        {
            Path = ParseImage();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Path expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.power)
        {
            power = ParsePower();
            Stream.MoveForward(1);
        }
        else throw new Exception($"Power expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (this.Stream.LookAhead().Value == TokenValues.position)
        {
            Position = ParsePosition(TokenValues.effectcard);
        }
        else throw new Exception($"Position expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");

        if (!Stream.MoveNext(TokenValues.ClosedCurlyBrackets))
        {
            throw new Exception($"Closed Curly Bracket expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return new EffectCard(Name, Type, Path, power, Position);
    }
    public string ParseName()
    {
        string answer;
        if (!Stream.MoveNext(TokenValues.assign))
        {
            throw new Exception($"= expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenType.identifier))
        {
            throw new Exception($"ID expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        answer = Stream.LookAhead().Value;
        if (!Stream.MoveNext(TokenValues.StatementSeparator))
        {
            throw new Exception($"; expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return answer;
    }
    public string ParseType(string value)
    {
        string answer;
        if (!Stream.MoveNext(TokenValues.assign))
        {
            throw new Exception($"= expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(value))
        {
            throw new Exception($"Wrong Type in line {Stream.LookAhead().Location.Line}");
        }
        answer = Stream.LookAhead().Value;
        if (!Stream.MoveNext(TokenValues.StatementSeparator))
        {
            throw new Exception($"; expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return answer;
    }
    public string ParseImage()
    {
        string answer;
        if (!Stream.MoveNext(TokenValues.assign))
        {
            throw new Exception($"= expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenType.identifier))
        {
            throw new Exception($"ID expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        answer = Stream.LookAhead().Value;
        if (!Stream.MoveNext(TokenValues.StatementSeparator))
        {
            throw new Exception($"; expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return answer;
    }
    public string ParsePhrase()
    {
        string answer;
        if (!Stream.MoveNext(TokenValues.assign))
        {
            throw new Exception($"= expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenType.identifier))
        {
            throw new Exception($"ID expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        answer = Stream.LookAhead().Value;
        if (!Stream.MoveNext(TokenValues.StatementSeparator))
        {
            throw new Exception($"; expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return answer;
    }
    public string ParsePosition(string type)
    {
        string answer = "";
        if (!Stream.MoveNext(TokenValues.assign))
        {
            throw new Exception($"= expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        switch (type)
        {
            case TokenValues.unitcard:
                if (Stream.CanLookAhead())
                {
                    if (Stream.MoveNext(TokenValues.melee) || Stream.MoveNext(TokenValues.middle) || Stream.MoveNext(TokenValues.siege))
                    {
                        answer = Stream.LookAhead().Value;
                    }
                    else throw new Exception($"Wrong Position in line {Stream.LookAhead().Location.Line}");
                }
                break;
            case TokenValues.effectcard:
                if (Stream.CanLookAhead())
                {
                    if (Stream.MoveNext(TokenValues.support) || Stream.MoveNext(TokenValues.weather))
                    {
                        answer = Stream.LookAhead().Value;
                    }
                    else throw new Exception($"Wrong Position in line {Stream.LookAhead().Location.Line}");
                }
                break;
            default:
                break;
        }
        if (!Stream.MoveNext(TokenValues.StatementSeparator))
        {
            throw new Exception($"; expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return answer;
    }
    public int ParseDamage()
    {
        int answer;
        if (!Stream.MoveNext(TokenValues.assign))
        {
            throw new Exception($"= expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        if (!Stream.MoveNext(TokenType.number))
        {
            throw new Exception($"A number is expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        answer = int.Parse(Stream.LookAhead().Value);
        if (!Stream.MoveNext(TokenValues.StatementSeparator))
        {
            throw new Exception($"; expected in card {Stream.LookAhead().Location.CardName} in line {Stream.LookAhead().Location.Line}");
        }
        return answer;
    }
}