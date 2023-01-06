using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

    //SpacePosition es una abstraccion para representar los lugares logicos y ademas tiene la funcion de devolver las posiciones que debe tener cada
    //carta en la parte visual. 
public class SpacePosition : Node
{   
    //Places, es un diccionario que tendra por llave un entero que representa una posicion logica, ademas de la visual, como valor tiene otro
    //diccionario, el cual almacenara las cartas que se encuentren en la posicion que representa la llave, como llave este diccionario tiene
    //el nombre de la carta y como valor, la propia carta.
    public Dictionary<int, Dictionary<string, Cards>> Places = new Dictionary<int, Dictionary<string, Cards>>();

    //Positions es un diccionario, donde su llave al igual que en Places, representara una posicion logica, ademas de la visual, sus valores
    //son un tanto especiales, pues es una dupla, con el inicio y el final de el lugar geometrico donde deben ser colocadas las cartas en la
    //parte visual.
    //Los Vector2 no son mas que un tipo de objeto de Godot(motor) que contienen una coordenada X y una coordenada Y.
    public Dictionary<int, KeyValuePair<Vector2, Vector2>> Positions = new Dictionary<int, KeyValuePair<Vector2, Vector2>>();

    //Estas variables representan las posiciones en el juego, mas que nada son para usarse como guia para el propio programador, no tener que
    //estar recordando cual era cual.
    public int playerDeck = 0;
    public int enemyDeck = 1;
    public int playerHand = 2;
    public int enemyHand = 3;
    public int playerMelee = 4;
    public int enemyMelee = 5;
    public int playerMiddle = 6; 
    public int enemyMiddle = 7;
    public int playerSiege = 8;
    public int enemySiege = 9;
    public int playerGrave = 10;
    public int enemyGrave = 11;
    public int playerLeader = 12;
    public int enemyLeader = 13;
    public int climate = 14;
    public int cardSelection = 15;
    public int supportPlayerSiege = 16;
    public int supportEnemySiege = 17;
    public int supportPlayerMiddle = 18;
    public int supportEnemyMiddle = 19;
    public int supportPlayerMelee = 20;
    public int supportEnemyMelee = 21;


    //Constructor por defecto.    
    public SpacePosition(){}

    //Constructor que utilizaremos, recibe 38 argumentos, los cuales son dos listas de Cards:playerDeck y enemyDeck, y el resto
    //serian las duplas de Vector2 que representan los espacios geometricos en el juego. 
    public SpacePosition(List<Cards> PlayerDeck, List<Cards> EnemyDeck, Vector2 Hand1, Vector2 Hand2,
     Vector2 PlayerMelee1, Vector2 PlayerMelee2, Vector2 PlayerMiddle1, Vector2 PlayerMiddle2,
     Vector2 PlayerSiege1, Vector2 PlayerSiege2, Vector2 EnemyMelee1, Vector2 EnemyMelee2,
     Vector2 EnemyMiddle1, Vector2 EnemyMiddle2, Vector2 EnemySiege1, Vector2 EnemySiege2, Vector2 PlayerGrave1,
     Vector2 PlayerGrave2, Vector2 EnemyGrave1, Vector2 EnemyGrave2, Vector2 PlayerLeader1, Vector2 PlayerLeader2,
     Vector2 EnemyLeader1, Vector2 EnemyLeader2, Vector2 Climate1, Vector2 Climate2, Vector2 CardSelection1, Vector2 CardSelection2,
     Vector2 SupportPlayerSiege0, Vector2 SupportPlayerSiege1, Vector2 SupportPlayerMiddle0, Vector2 SupportPlayerMiddle1, 
     Vector2 SupportPlayerMelee0, Vector2 SupportPlayerMelee1, Vector2 SupportEnemyMelee0, Vector2 SupportEnemyMelee1,
     Vector2 SupportEnemyMiddle0, Vector2 SupportEnemyMiddle1, Vector2 SupportEnemySiege0, Vector2 SupportEnemySiege1){
        
        //Se inicializan las posiciones de Places.
        Places.Add(this.playerDeck, GenDecks(PlayerDeck));
        Places.Add(this.enemyDeck, GenDecks(EnemyDeck));
        Places.Add(this.playerMelee, new Dictionary<string, Cards>());
        Places.Add(this.playerMiddle, new Dictionary<string, Cards>());
        Places.Add(this.playerSiege, new Dictionary<string, Cards>());
        Places.Add(this.enemyMelee, new Dictionary<string, Cards>());
        Places.Add(this.enemyMiddle, new Dictionary<string, Cards>());
        Places.Add(this.enemySiege, new Dictionary<string, Cards>());
        Places.Add(this.playerHand, new Dictionary<string, Cards>());
        Places.Add(this.enemyHand, new Dictionary<string, Cards>());
        Places.Add(this.playerGrave, new Dictionary<string, Cards>());
        Places.Add(this.enemyGrave, new Dictionary<string, Cards>());
        Places.Add(this.playerLeader, new Dictionary<string, Cards>());
        Places.Add(this.enemyLeader, new Dictionary<string, Cards>());
        Places.Add(this.climate, new Dictionary<string, Cards>());
        Places.Add(this.cardSelection, new Dictionary<string, Cards>());
        Places.Add(this.supportPlayerSiege, new Dictionary<string, Cards>());
        Places.Add(this.supportEnemySiege, new Dictionary<string, Cards>());
        Places.Add(this.supportPlayerMiddle, new Dictionary<string, Cards>());
        Places.Add(this.supportEnemyMiddle, new Dictionary<string, Cards>());
        Places.Add(this.supportPlayerMelee, new Dictionary<string, Cards>());
        Places.Add(this.supportEnemyMelee, new Dictionary<string, Cards>());


        //Se agregan las Duplas a Positions.
        AddDupla(Hand1, Hand2, this.playerHand);
        AddDupla(PlayerLeader1, PlayerLeader2, this.playerLeader);
        AddDupla(PlayerMelee1, PlayerMelee2, this.playerMelee);
        AddDupla(PlayerMiddle1, PlayerMiddle2, this.playerMiddle);
        AddDupla(PlayerSiege1, PlayerSiege2, this.playerSiege);
        AddDupla(PlayerGrave1, PlayerGrave2, this.playerGrave);
        AddDupla(SupportPlayerSiege0, SupportPlayerSiege1, this.supportPlayerSiege);
        AddDupla(SupportPlayerMiddle0, SupportPlayerMiddle1, this.supportPlayerMiddle);
        AddDupla(SupportPlayerMelee0, SupportPlayerMelee1, this.supportPlayerMelee);

        AddDupla(EnemyMelee1, EnemyMelee2, this.enemyMelee);
        AddDupla(EnemyLeader1, EnemyLeader2, this.enemyLeader);
        AddDupla(EnemyMiddle1, EnemyMiddle2, this.enemyMiddle);
        AddDupla(EnemySiege1, EnemySiege2, this.enemySiege);
        AddDupla(EnemyGrave1, EnemyGrave2, this.enemyGrave);
        AddDupla(SupportEnemySiege0, SupportEnemySiege1, this.supportEnemySiege);
        AddDupla(SupportEnemyMiddle0, SupportEnemyMiddle1, this.supportEnemyMiddle);
        AddDupla(SupportEnemyMelee0, SupportEnemyMelee1, this.supportEnemyMelee);

        AddDupla(Climate1, Climate2, this.climate);
        AddDupla(CardSelection1, CardSelection2, this.cardSelection);
        
        //Se llama al metodo GetReady.
        GetReady(PlayerDeck, EnemyDeck);
        
        

    }

    //Se encarga de posicionar los leaderCards de cada bando en su posicion especial.
    private void GetReady(List<Cards> playerDeck, List<Cards> enemyDeck){

        List<Cards> newPlayerDeck = new List<Cards>();
        List<Cards> newEnemyDeck = new List<Cards>();

        foreach(var item in playerDeck){
            if(item is LeaderCard){
                this.Places[12].Add(item.name, item);
                this.Places[0].Remove(item.name);
            }else{
                newPlayerDeck.Add(item);
            }
        }

        foreach(var item in enemyDeck){
            if(item is LeaderCard){
                this.Places[13].Add(item.name, item);
            }else{
                newEnemyDeck.Add(item);
            }
        }
    }

    //Se encarga de tomar una de las listas de carta y ponerlas en su posicion logica.
    private Dictionary<string, Cards> GenDecks(List<Cards> Deck){

        Dictionary<string, Cards> deck = new Dictionary<string, Cards>();
        
        foreach(var item in Deck){
            deck.Add(item.name, item);
        }
        return deck;
    }

    //Esto devuelve la estructura Places.
     public Dictionary<int, Dictionary<string, Cards>> GetPlaces(){
        return Places;
    }

    //Este metodo agrega las duplas de Vector2 a Positions
    private void AddDupla(Vector2 start, Vector2 end, int place){
        KeyValuePair<Vector2, Vector2> current = new KeyValuePair<Vector2, Vector2>(start, end);

        this.Positions.Add(place, current);
    }
}
    


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }ss