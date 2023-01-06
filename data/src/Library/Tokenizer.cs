using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using Godot;
public class Tokenizer
{

    Dictionary<string, string> operators = new Dictionary<string, string>();
    Dictionary<string, string> keywords = new Dictionary<string, string>();
    Dictionary<string, string> texts = new Dictionary<string, string>();
    public List<string> Keywords { get { return keywords.Keys.ToList(); } }

    public void RegisterOperator(string op, string tokenValue)
    {
        this.operators[op] = tokenValue;
    }
    public void RegisterKeyword(string keyword, string tokenValue)
    {
        this.keywords[keyword] = tokenValue;
    }
    public void RegisterText(string start, string end)
    {
        this.texts[start] = end;
    }
    private bool MatchSymbol(TokenReader stream, List<Token> tokens)
    {
        foreach (var op in operators.Keys.OrderByDescending(k => k.Length))
            if (stream.Matches(op))
            {
                tokens.Add(new Token(operators[op], TokenType.symbols, stream.Location));
                return true;
            }
        return false;
    }
    private bool MatchText(TokenReader stream, List<Token> tokens)
    {
        foreach (var start in texts.Keys.OrderByDescending(k => k.Length))
        {
            string text;
            if (stream.Matches(start))
            {
                if (!stream.ReadUntil(texts[start], out text))
                    //errors.Add(new CompilingError(stream.Location, ErrorCode.Expected, texts[start]));
                    tokens.Add(new Token(text, TokenType.text, stream.Location));
                return true;
            }
        }
        return false;
    }
    public IEnumerable<Token> GetTokens(string code, string cardname)
    {
        List<Token> tokens = new List<Token>();

        TokenReader stream = new TokenReader(code, cardname);

        while (!stream.EndOfFile)
        {
            string value;

            if (stream.ReadWhiteSpace())
                continue;
           
            if (MatchSymbol(stream, tokens))
                continue;
           
            if(stream.ReadKeyWord(out value)){
                
                if(Keywords.Contains(value)) 
                {
                    tokens.Add(new Token(keywords[value], TokenType.keyword, stream.Location));
                    continue;
                }
                throw new Exception($"Unknown Token in card {stream.Location.CardName} at line: {stream.Location.Line} , comillas expected");
            }
           
            if (stream.ReadID(out value))
            {   
                value = value.Substring(1, value.Length-2);      
                tokens.Add(new Token(value, TokenType.identifier, stream.Location));
                continue;
            }
         
            if (stream.ReadNumber(out value))
            {
                double d;
                if (!double.TryParse(value, out d))                    
                    throw new Exception($"Wrong number format at line: {stream.Location.Line}");
                tokens.Add(new Token(value, TokenType.number, stream.Location));
                continue;
            }
            if (MatchText(stream, tokens))
                continue;
            
            var unkOp = stream.ReadAny();
            throw new Exception($"Unknown Token at line: {stream.Location.Line} , character:{unkOp}");
        }

        return tokens;
    }
}
class TokenReader
{
    string Code;
    int position;
    int line;
    string cardName;
    public CodeLocation Location
    {
        get
        {
            return new CodeLocation
            {
                Line = this.line,
                CardName = this.cardName
            };
        }
    }
    public TokenReader(string code, string cardname)
    {
        this.Code = code;
        this.position = 0;
        this.line = 1;
        this.cardName = cardname;
    }
    public char PeekNext()
    {
        if (position < 0 || position >= this.Code.Length)
        {
            throw new InvalidOperationException();
        }
        return Code[position];
    }
    public bool EndOfFile { get { return this.position >= Code.Length; } }
    public bool EndOfLine { get { return EndOfFile || this.Code[position] == '\n'; } }
    public bool ContinuesWith(string prefix)
    {
        if (this.position + prefix.Length > this.Code.Length)
            return false;
        for (int i = 0; i < prefix.Length; i++)
            if (this.Code[this.position + i] != prefix[i])
                return false;
        return true;
    }
    public bool Matches(string prefix)
    {
        if (ContinuesWith(prefix))
        {
            this.position += prefix.Length;
            return true;
        }

        return false;
    }
    public bool IsValidIdKeyWordCharacter(char c, bool begining)
    {
        return (begining ? char.IsLetter(c) : char.IsLetterOrDigit(c));
    }
    public bool ReadKeyWord(out string KeyWord)
    {
        KeyWord = "";
        while (!EndOfLine && IsValidIdKeyWordCharacter(PeekNext(), KeyWord.Length == 0))
            KeyWord += ReadAny();
        return KeyWord.Length > 0;
    }
    public bool ReadID(out string id)
    {
        id = "";        
        while (!EndOfLine && IsValidIdCharacter(PeekNext(), id.Length == 0))
        {
            id+=ReadAny();
        }
        if(id.Length>0) id += ReadAny();
        return id.Length>0;
    }
    public bool IsValidIdCharacter(char c, bool begining)
    {
        if (begining)
        {
            return c== '"';
        }
        else if(c=='"') return false;
        else return true;
    }
    public bool ReadWhiteSpace()
    {
        if (char.IsWhiteSpace(PeekNext()))
        {
            ReadAny();
            return true;
        }
        return false;
    }
    public bool ReadUntil(string end, out string text)
    {
        text = "";
        while (!Matches(end))
        {
            if (EndOfFile || EndOfLine)
                return false;
            text += ReadAny();
        }
        return true;
    }
    public bool ReadNumber(out string number)
    {
        number = "";
        if(ContinuesWith("-"))
        number+=ReadAny();
        while (!EndOfLine && char.IsDigit(PeekNext()))
            number += ReadAny();
        if (!EndOfLine && Matches("."))
        {
            number += '.';
            while (!EndOfLine && char.IsDigit(PeekNext()))
                number += ReadAny();
        }

        if (number.Length == 0)
            return false;

        return number.Length > 0;
    }
    public char ReadAny()
    {
        if (EndOfFile)
            throw new InvalidOperationException();
        if (EndOfLine) line += 1;

        return this.Code[this.position++];
    }
}