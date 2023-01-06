using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
public class Compiler
{
    private static Tokenizer tokenizer;

    public static Tokenizer Lexical
    {
        get
        {
            if (tokenizer == null)
            {
                tokenizer = new Tokenizer();


                tokenizer.RegisterOperator("Add", TokenValues.sum);
                tokenizer.RegisterOperator("Mult", TokenValues.mult);
                tokenizer.RegisterOperator("Sub", TokenValues.sub);
                tokenizer.RegisterOperator("Div", TokenValues.div);
                tokenizer.RegisterOperator("=", TokenValues.assign);
                tokenizer.RegisterOperator("==", TokenValues.Equal);
                tokenizer.RegisterOperator(">", TokenValues.Major);
                tokenizer.RegisterOperator("<", TokenValues.Minor);
                tokenizer.RegisterOperator(">=", TokenValues.MajorOrEqual);
                tokenizer.RegisterOperator("<=", TokenValues.MinorOrEqual);
                tokenizer.RegisterOperator("And", TokenValues.and);
                tokenizer.RegisterOperator("Or", TokenValues.or);
                tokenizer.RegisterOperator("Not", TokenValues.not);
                tokenizer.RegisterOperator("Compare", TokenValues.binarycomparer);
                tokenizer.RegisterOperator("True", TokenValues.truepredicate);
                tokenizer.RegisterOperator("False", TokenValues.falsepredicate);

                tokenizer.RegisterOperator(",", TokenValues.ValueSeparator);
                tokenizer.RegisterOperator(";", TokenValues.StatementSeparator);
                tokenizer.RegisterOperator("(", TokenValues.OpenBracket);
                tokenizer.RegisterOperator(")", TokenValues.ClosedBracket);
                tokenizer.RegisterOperator("{", TokenValues.OpenCurlyBrackets);
                tokenizer.RegisterOperator("}", TokenValues.ClosedCurlyBrackets);

                tokenizer.RegisterKeyword("ConditionSet", TokenValues.conditionset);
                tokenizer.RegisterKeyword("InstructionSet", TokenValues.instructionset);
                tokenizer.RegisterKeyword("Condition", TokenValues.condition);
                tokenizer.RegisterKeyword("Instruction", TokenValues.instruction);
                tokenizer.RegisterKeyword("Card", TokenValues.card);
                tokenizer.RegisterKeyword("UnitCard", TokenValues.unitcard);
                tokenizer.RegisterKeyword("LeaderCard", TokenValues.leadercard);
                tokenizer.RegisterKeyword("EffectCard", TokenValues.effectcard);
                tokenizer.RegisterKeyword("Name", TokenValues.name);
                tokenizer.RegisterKeyword("Attack", TokenValues.attack);
                tokenizer.RegisterKeyword("Power", TokenValues.power);
                tokenizer.RegisterKeyword("Position", TokenValues.position);
                tokenizer.RegisterKeyword("Phrase", TokenValues.phrase);
                tokenizer.RegisterKeyword("Path", TokenValues.path);
                tokenizer.RegisterKeyword("Type", TokenValues.type);
                tokenizer.RegisterKeyword("Melee", TokenValues.melee);
                tokenizer.RegisterKeyword("Middle", TokenValues.middle);
                tokenizer.RegisterKeyword("Siege", TokenValues.siege);
                tokenizer.RegisterKeyword("Weather", TokenValues.weather);
                tokenizer.RegisterKeyword("Support", TokenValues.support);
                tokenizer.RegisterKeyword("Destroy", TokenValues.destroy);
                tokenizer.RegisterKeyword("Summon", TokenValues.summon);
                tokenizer.RegisterKeyword("Reborn", TokenValues.reborn);
                tokenizer.RegisterKeyword("Draw", TokenValues.draw);
                tokenizer.RegisterKeyword("ModifyAttack", TokenValues.modifyAttack);
                tokenizer.RegisterKeyword("AllEnemyCards", TokenValues.allenemycards);
                tokenizer.RegisterKeyword("AllOwnCards", TokenValues.allowncards);
                tokenizer.RegisterKeyword("HighestAttackIn", TokenValues.highestattackin);
                tokenizer.RegisterKeyword("LowestAttackIn", TokenValues.lowestattackin);
                tokenizer.RegisterKeyword("NumberOfCardsIn", TokenValues.numberofcardsin);
                tokenizer.RegisterKeyword("Damage", TokenValues.damage);
                tokenizer.RegisterKeyword("DamageIn", TokenValues.damagein);
                tokenizer.RegisterKeyword("OwnHand", TokenValues.ownhand);
                tokenizer.RegisterKeyword("OwnMelee", TokenValues.ownmelee);
                tokenizer.RegisterKeyword("OwnMiddle", TokenValues.ownmiddle);
                tokenizer.RegisterKeyword("OwnSiege", TokenValues.ownsiege);
                tokenizer.RegisterKeyword("OwnGraveryard", TokenValues.owngraveryard);
                tokenizer.RegisterKeyword("OwnDeck", TokenValues.owndeck);
                tokenizer.RegisterKeyword("EnemyHand", TokenValues.enemyhand);
                tokenizer.RegisterKeyword("EnemyMelee", TokenValues.enemymelee);
                tokenizer.RegisterKeyword("EnemyMiddle", TokenValues.enemymiddle);
                tokenizer.RegisterKeyword("EnemySiege", TokenValues.enemysiege);
                tokenizer.RegisterKeyword("EnemyGraveryard", TokenValues.enemygraveryard);
                tokenizer.RegisterKeyword("EnemyDeck", TokenValues.enemydeck);
                tokenizer.RegisterKeyword("AllExistingCards", TokenValues.allexistingcards);
                tokenizer.RegisterKeyword("SwitchBand", TokenValues.switchband);
                tokenizer.RegisterKeyword("ExistCardIn", TokenValues.existcardin);
                tokenizer.RegisterKeyword("FreeElection", TokenValues.freeelection);
                /*  */
                tokenizer.RegisterText("\"", "\"");
            }

            return tokenizer;
        }
    }
}