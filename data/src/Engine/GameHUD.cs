using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

public class GameHUD : Node2D
{
	//El num Phase es para identificar 
	public enum Phase { PlayerTurn, EnemyTurn, PlayerWaiting, EnemyWaiting, Results }

#pragma warning disable 649
	[Export]
	public PackedScene CardScene;
	public static List<Cards> playerDeck = new List<Cards>();
	public static List<Cards> enemyDeck = new List<Cards>();
	bool band = false;
	public static SpacePosition Positions;
	public static int currentFrom;
	public static int currentTo;
	public static string currentName;
	public static bool playerPass = false;
	public static bool enemyPass = false;
	public static int PlayerLife = 2;
	public static int EnemyLife = 2;
	public int playerWins = 0;
	public int enemyWins = 0;
	public static bool process;
	public static int phase = -1;
	public static List<PowerData> powerData;
	public static bool startProcessing;
	public static bool remove;
	public static bool ready;
	public int moved;
	public bool shooted;


	public override void _Ready()
	{
		GetNode<CardBase>("ShowCard/Card").show = true;
		Hide();
	}

	//La funcion Process se ejecut a cada milisegundo, en principio 60 veces por segundo.
	public override void _Process(float delta)
	{
		
		//Tiene dentro de si un grupo de condicionales que definen que hacer en cada fase del juego.
		if (phase == (int)Phase.PlayerTurn)
		{
			for (int i = 4; i < 10; i++) UpdateAttacks(i);
			GetNode<Label>("BackGround/PlayerBackDeck/Count").Text = Positions.Places[0].Count.ToString();
			GetNode<Label>("BackGround/EnemyBackDeck/Count").Text = Positions.Places[1].Count.ToString();
			GetNode<Label>("BackGround/PlayerCards").Text = Positions.Places[2].Count.ToString();
			GetNode<Label>("BackGround/EnemyCards").Text = Positions.Places[3].Count.ToString();
			if (!playerPass)
			{
				if (Main.move)
				{
					MoveCard(currentFrom, currentTo, currentName);
					Main.move = false;
				}
			}
			else
			{
				phase = (int)Phase.EnemyTurn;
				if (playerPass && enemyPass)
				{
					shooted = true;
					if (shooted)
					{
						playerPass = false;
						enemyPass = false;
						GetNode<Timer>("ChangeEnemyTimer").Start();
						shooted = false;
						phase = (int)Phase.Results;
					}

				}
			}
		}
		else if (phase == (int)Phase.EnemyTurn)
		{
			for (int i = 4; i < 10; i++) UpdateAttacks(i);
			GetNode<Label>("BackGround/PlayerBackDeck/Count").Text = Positions.Places[0].Count.ToString();
			GetNode<Label>("BackGround/EnemyBackDeck/Count").Text = Positions.Places[1].Count.ToString();
			GetNode<Label>("BackGround/PlayerCards").Text = Positions.Places[2].Count.ToString();
			GetNode<Label>("BackGround/EnemyCards").Text = Positions.Places[3].Count.ToString();
			if (!enemyPass)
			{
				EnemyIA();
			}
			else
			{
				phase = (int)Phase.PlayerTurn;
				if (playerPass && enemyPass)
				{
					shooted = true;
					if (shooted)
					{
						playerPass = false;
						enemyPass = false;
						GetNode<Timer>("ChangeEnemyTimer").Start();
						shooted = false;
						phase = (int)Phase.Results;
					}

				}
			}

		}
		else if (phase == (int)Phase.PlayerWaiting)
		{


			if (Main.move && GetNode<CardBase>("Cards/" + currentName).Ready)
			{
				MoveCard(currentFrom, currentTo, currentName);
				Main.move = false;
			}

			if (remove)
			{
				RemoveFirst();
				remove = false;
			}
			if (powerData.Count == 0)
			{
				startProcessing = false;

			}
			if (startProcessing)
			{

				foreach (var item in powerData)
				{
					if (item is RebornPower)
					{


						if (item.started)
						{
							if (item.processing)
							{
								if (item.cardsCounter == 0)
								{
									item.processing = false;
									List<Cards> cards = Positions.Places[15].Values.ToList();
									foreach (var card in cards)
									{
										MoveCard(15, 10, card.name);
									}
								}
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Reborn();
							break;
						}
					}
					else if (item is SummonPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								if (item.cardsCounter == 0)
								{
									item.processing = false;
									List<Cards> cards = Positions.Places[15].Values.ToList();
									foreach (var card in cards)
									{
										MoveCard(15, 10, card.name);
									}
								}

								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Summon();
							break;
						}
					}
					else if (item is SwitchBandPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							SwitchBand();
							break;
						}
					}
					else if (item is DestroyPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								if (item.cardsCounter == 0)
								{
									item.processing = false;
									RestoreReady();
								}
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Destroy();
							break;
						}
					}
					else if (item is ModifyAttackPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								if (item.cardsCounter == 0)
								{
									item.processing = false;
									RestoreReady();
								}
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							ModifyAttack();
							break;
						}
					}
					else if (item is DrawPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Draw();
							break;
						}
					}

				}

			}
			else
			{
				phase = (int)Phase.EnemyTurn;
			}

		}
		else if (phase == (int)Phase.EnemyWaiting)
		{

			if (remove)
			{
				RemoveFirst();
				remove = false;
			}
			if (powerData.Count == 0)
			{
				startProcessing = false;
			}
			if (startProcessing)
			{

				foreach (var item in powerData)
				{
					if (item is RebornPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Reborn();
							break;
						}
					}
					else if (item is SummonPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Summon();
							break;
						}
					}
					else if (item is SwitchBandPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							SwitchBand();
							break;
						}
					}
					else if (item is DestroyPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Destroy();
							break;
						}
					}
					else if (item is ModifyAttackPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								if (item.cardsCounter == 0)
								{
									item.processing = false;
								}
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							ModifyAttack();
							break;
						}
					}
					else if (item is DrawPower)
					{
						if (item.started)
						{
							if (item.processing)
							{
								break;
							}
							else
							{
								remove = true;
								break;
							}
						}
						else
						{
							item.started = true;
							Draw();
							break;
						}
					}

				}


			}
			else
			{
				phase = (int)Phase.PlayerTurn;
			}


		}
		else if (phase == (int)Phase.Results)
		{
		}
	}

	public void _on_ChangeEnemyTimer_timeout()
	{

		int a = int.Parse(GetNode<Label>("BackGround/PlayerTotal").Text);
		int b = int.Parse(GetNode<Label>("BackGround/EnemyTotal").Text);
		string result = "";
		if (a > b)
		{
			EnemyLife--;
			playerWins++;
			result = "You Won";
			GetNode<AudioStreamPlayer>("VictorySound").Play();
		}
		else if (b > a)
		{
			PlayerLife--;
			enemyWins++;
			result = "You Lose";
			GetNode<AudioStreamPlayer>("DefeatSound").Play();
		}
		else
		{
			PlayerLife--;
			EnemyLife--;
			playerWins++;
			enemyWins++;
			result = "Draw";
			GetNode<AudioStreamPlayer>("DefeatSound").Play();
		}

		DisplayInfo(a, b, result, false);
		UpdateLife();
	}

	public void _on_MeleeSupport_pressed()
	{
		if (ready)
		{
			moved = 4;
			ready = false;
			MoveCard(2, 20, currentName);
		}
	}
	public void _on_MiddleSupport_pressed()
	{
		if (ready)
		{
			moved = 6;
			ready = false;
			MoveCard(2, 18, currentName);
		}
	}
	public void _on_SiegeSupport_pressed()
	{
		if (ready)
		{
			moved = 8;
			ready = false;
			MoveCard(2, 16, currentName);
		}
	}
	public void _on_HideInfoTimer_timeout()
	{
		GetNode<Sprite>("Info").Hide();
	}

	public void DisplayInfo(int a, int b, string result, bool goMenu)
	{

		GetNode<Button>("DisplayInfo/NextButton").Disabled = false;
		GetNode<Label>("DisplayInfo/Title").Text = "Round End";
		GetNode<Label>("DisplayInfo/SubTitle").Text = result;
		GetNode<Label>("DisplayInfo/Results").Text = a + " - " + b;
		if (goMenu) GetNode<Button>("DisplayInfo/GiveUpButton").Text = "Menu";
		else GetNode<Button>("DisplayInfo/GiveUpButton").Text = "Give Up";
		GetNode<Sprite>("DisplayInfo").Show();


	}
	public void _on_GiveUpButton_pressed()
	{
		phase = -1;
		GetNode<AudioStreamPlayer>("ButtonSound").Play();
		GetNode<Sprite>("DisplayInfo").Hide();
		EndGame();
	}
	public void _on_ExitButton_pressed()
	{
		phase = -1;
		GetNode<AudioStreamPlayer>("ButtonSound").Play();
		EndGame();
	}
	public void _on_PassButton_pressed()
	{
		GetNode<AudioStreamPlayer>("ButtonSound").Play();
		playerPass = true;
	}
	public void _on_NextButton_pressed()
	{
		GetNode<AudioStreamPlayer>("ButtonSound").Play();
		GetNode<Sprite>("DisplayInfo").Hide();
		if (PlayerLife == 0 || EnemyLife == 0)
		{
			string result;
			if (playerWins > enemyWins)
			{
				GetNode<AudioStreamPlayer>("VictorySound").Play();
				result = "You Win";
			}
			else if (playerWins < enemyWins)
			{
				GetNode<AudioStreamPlayer>("DefeatSound").Play();
				result = "You Lose";
			}
			else
			{
				GetNode<AudioStreamPlayer>("DefeatSound").Play();
				result = "Draw";
			}

			DisplayInfo(playerWins, enemyWins, result, true);
		}
		else
		{
			phase = (int)Phase.PlayerTurn;
			GoGraves();
		}


	}

	public void _on_Menu_StartGame()
	{
		Show();
	}

	public void _on_Band0Button_pressed()
	{
		GetNode<AudioStreamPlayer>("SelectSound").Play();
		NewGame(true);
	}

	public void _on_Band1Button_pressed()
	{
		GetNode<AudioStreamPlayer>("SelectSound").Play();
		NewGame(false);
	}

	public void NewGame(bool band)
	{

		GetNode<Node2D>("DeckSelection").Hide();
		GenerateDeckInfo(band);

		Positions = new SpacePosition(playerDeck, enemyDeck, GetNode<Position2D>("Hand0").Position, GetNode<Position2D>("Hand1").Position,
		GetNode<Position2D>("PlayerMelee0").Position, GetNode<Position2D>("PlayerMelee1").Position, GetNode<Position2D>("PlayerMiddle0").Position,
		GetNode<Position2D>("PlayerMiddle1").Position, GetNode<Position2D>("PlayerSiege0").Position, GetNode<Position2D>("PlayerSiege1").Position,
		GetNode<Position2D>("EnemyMelee0").Position, GetNode<Position2D>("EnemyMelee1").Position, GetNode<Position2D>("EnemyMiddle0").Position,
		GetNode<Position2D>("EnemyMiddle1").Position, GetNode<Position2D>("EnemySiege0").Position, GetNode<Position2D>("EnemySiege1").Position,
		GetNode<Position2D>("PlayerGrave0").Position, GetNode<Position2D>("PlayerGrave1").Position, GetNode<Position2D>("EnemyGrave0").Position,
		GetNode<Position2D>("EnemyGrave1").Position, GetNode<Position2D>("PlayerLeader0").Position, GetNode<Position2D>("PlayerLeader1").Position,
		GetNode<Position2D>("EnemyLeader0").Position, GetNode<Position2D>("EnemyLeader1").Position, GetNode<Position2D>("Climate0").Position,
		GetNode<Position2D>("Climate1").Position, GetNode<Position2D>("CardSelection0").Position, GetNode<Position2D>("CardSelection1").Position,
		GetNode<Position2D>("SupportPlayerSiege0").Position, GetNode<Position2D>("SupportPlayerSiege1").Position, GetNode<Position2D>("SupportPlayerMiddle0").Position,
		GetNode<Position2D>("SupportPlayerMiddle1").Position, GetNode<Position2D>("SupportPlayerMelee0").Position, GetNode<Position2D>("SupportPlayerMelee1").Position,
		GetNode<Position2D>("SupportEnemyMelee0").Position, GetNode<Position2D>("SupportEnemyMelee1").Position, GetNode<Position2D>("SupportEnemyMiddle0").Position,
		GetNode<Position2D>("SupportEnemyMiddle0").Position, GetNode<Position2D>("SupportEnemySiege0").Position, GetNode<Position2D>("SupportEnemySiege1").Position);

		RandomizeHands();

		Visuals(Positions.Positions[12].Key, Positions.Positions[12].Value, 12);
		Visuals(Positions.Positions[13].Key, Positions.Positions[13].Value, 13);
		Visuals(Positions.Positions[2].Key, Positions.Positions[2].Value, 2);


		UpdateLife();
		Random random = new Random();

		phase = random.Next(0, 1);
	}
	public void EndGame()
	{
		PlayerLife = 2;
		EnemyLife = 2;

		playerWins = 0;
		enemyWins = 0;
		GetNode<Sprite>("DisplayInfo").Hide();
		GetNode<Node2D>("DeckSelection").Show();
		Hide();

		foreach (var card in playerDeck)
		{
			GetNode<Node2D>("Cards").RemoveChild(GetNode<CardBase>("Cards/" + card.name));
		}
		foreach (var card in enemyDeck)
		{
			GetNode<Node2D>("Cards").RemoveChild(GetNode<CardBase>("Cards/" + card.name));
		}
		GetNode<Node2D>("../Menu").Show();

	}
	public void GoGraves()
	{
		var playerMelee = Positions.Places[4].Values.ToList();
		var playerMiddle = Positions.Places[6].Values.ToList();
		var playerSiege = Positions.Places[8].Values.ToList();
		var enemyMelee = Positions.Places[5].Values.ToList();
		var enemyMiddle = Positions.Places[7].Values.ToList();
		var enemySiege = Positions.Places[9].Values.ToList();
		var climate = Positions.Places[14].Values.ToList();
		for (int i = 4; i < 10; i++)
		{
			int to = 0;
			if (i % 2 == 0) to = 10;
			else to = 11;
			foreach (var item in Positions.Places[i].Values.ToList())
			{
				MoveCard(i, to, item.name);
			}
		}
		foreach (var item in climate)
		{
			string name = item.name;
			int to = 0;

			foreach (var card in playerDeck)
			{
				if (card.name == name)
				{
					to = 10;
					break;
				}
				else
				{
					to = 11;
					break;
				}
			}
			MoveCard(14, to, name);
		}
		for (int i = 16; i < 22; i++)
		{
			int to = 0;
			if (i % 2 == 0) to = 10;
			else to = 11;
			foreach (var item in Positions.Places[i].Values.ToList())
			{
				MoveCard(i, to, item.name);
			}
		}
	}
	public void GenerateDeckInfo(bool band)
	{
		string playerAdress = "";
		string enemyAdress = "";

		if (band)
		{
			playerAdress = "../Battle-Card/data/textures/cards/band0";
			enemyAdress = "../Battle-Card/data/textures/cards/band1";
		}
		else
		{
			playerAdress = "../Battle-Card/data/textures/cards/band1";
			enemyAdress = "../Battle-Card/data/textures/cards/band0";
		}

		playerDeck = GetLogicalCards(playerAdress, "player");
		enemyDeck = GetLogicalCards(enemyAdress, "enemy");

		GenerateDeck(playerDeck);
		GenerateDeck(enemyDeck);
	}
	public List<Cards> GetLogicalCards(string adress, string band)
	{
		Tokenizer a = Compiler.Lexical;
		string[] texts = System.IO.Directory.GetFiles(@adress, "*.txt");
		List<Cards> answer = new List<Cards>();
		foreach (var item in texts)
		{
			string text = System.IO.File.ReadAllText(item);
			List<Token> MyList = (List<Token>)a.GetTokens(text, item);
			TokenStream b = new TokenStream(MyList);
			Parser c = new Parser(b);
			Cards card = c.ParseCard();
			card.band = band;
			answer.Add(card);

		}

		return answer;
	}
	public void GenerateDeck(List<Cards> deck)
	{

		foreach (var item in deck)
		{
			GenerateCard(item);
		}

	}
	public void GenerateCard(Cards item)
	{

		if (item is UnitCard)
		{
			var newItem = (UnitCard)item;
			var card = (CardBase)CardScene.Instance();

			card.Name = newItem.name;
			card.Attack = newItem.damage.ToString();
			card.Place = newItem.position;
			card.Phrase = newItem.phrase;
			card.Rute = newItem.imagePath;
			card.Type = newItem.type;
			card.Power = newItem.power.Name;

			card.GenerateCard();

			GetNode<Node2D>("Cards").AddChild(card, true);

			GetNode<MarginContainer>("Cards/CardBase").Name = newItem.name;


		}
		else if (item is LeaderCard)
		{
			var newItem = (LeaderCard)item;
			var card = (CardBase)CardScene.Instance();

			card.Name = newItem.name;
			card.Phrase = newItem.phrase;
			card.Rute = newItem.imagePath;
			card.Type = newItem.type;
			card.Power = newItem.power.Name;

			card.GenerateCard();

			GetNode<Node2D>("Cards").AddChild(card, true);

			GetNode<MarginContainer>("Cards/CardBase").Name = newItem.name;

		}
		else if (item is EffectCard)
		{
			var newItem = (EffectCard)item;
			var card = (CardBase)CardScene.Instance();

			card.Name = newItem.name;
			card.Place = newItem.position;
			card.Rute = newItem.imagePath;
			card.Type = newItem.type;
			card.Power = newItem.power.Name;

			card.GenerateCard();

			GetNode<Node2D>("Cards").AddChild(card, true);

			GetNode<MarginContainer>("Cards/CardBase").Name = newItem.name;

		}
	}
	public void Visuals(Vector2 start, Vector2 end, int place)
	{

		var places = Positions.GetPlaces();
		var count = 0;
		Dictionary<string, Cards> Space = places[place];

		var distance = (end.x - start.x) / Space.Count;

		foreach (var item in Space)
		{

			var position = new Vector2(start.x + (distance * count), start.y);
			count++;
			GetNode<CardBase>("Cards/" + item.Key).Show();
			GetNode<CardBase>("Cards/" + item.Key).RectPosition = position;
		}
	}
	public void UpdateAttacks(int place)
	{

		if (place == 4)
		{
			int amount = 0;
			foreach (var x in Positions.Places[place].Values)
			{
				amount += int.Parse(GetNode<Label>("Cards/" + x.name + "/BackGround/Attack").Text);

			}
			GetNode<Label>("BackGround/PlayerMelee").Text = amount.ToString();
		}
		else if (place == 5)
		{
			int amount = 0;
			foreach (var x in Positions.Places[place].Values)
			{
				amount += int.Parse(GetNode<Label>("Cards/" + x.name + "/BackGround/Attack").Text);

			}
			GetNode<Label>("BackGround/EnemyMelee").Text = amount.ToString();
		}
		else if (place == 6)
		{
			int amount = 0;
			foreach (var x in Positions.Places[place].Values)
			{
				amount += int.Parse(GetNode<Label>("Cards/" + x.name + "/BackGround/Attack").Text);
			}
			GetNode<Label>("BackGround/PlayerMiddle").Text = amount.ToString();
		}
		else if (place == 7)
		{
			int amount = 0;
			foreach (var x in Positions.Places[place].Values)
			{
				amount += int.Parse(GetNode<Label>("Cards/" + x.name + "/BackGround/Attack").Text);

			}
			GetNode<Label>("BackGround/EnemyMiddle").Text = amount.ToString();
		}
		else if (place == 8)
		{
			int amount = 0;
			foreach (var x in Positions.Places[place].Values)
			{
				amount += int.Parse(GetNode<Label>("Cards/" + x.name + "/BackGround/Attack").Text);

			}
			GetNode<Label>("BackGround/PlayerSiege").Text = amount.ToString();
		}
		else if (place == 9)
		{
			int amount = 0;
			foreach (var x in Positions.Places[place].Values)
			{
				amount += int.Parse(GetNode<Label>("Cards/" + x.name + "/BackGround/Attack").Text);

			}
			GetNode<Label>("BackGround/EnemySiege").Text = amount.ToString();
		}

		GetNode<Label>("BackGround/PlayerTotal").Text = (int.Parse(GetNode<Label>("BackGround/PlayerMelee").Text) + int.Parse(GetNode<Label>("BackGround/PlayerMiddle").Text) + int.Parse(GetNode<Label>("BackGround/PlayerSiege").Text)).ToString();
		GetNode<Label>("BackGround/EnemyTotal").Text = (int.Parse(GetNode<Label>("BackGround/EnemyMelee").Text) + int.Parse(GetNode<Label>("BackGround/EnemyMiddle").Text) + int.Parse(GetNode<Label>("BackGround/EnemySiege").Text)).ToString();
	}
	public void MoveCard(int from, int to, string name)
	{

		if (Positions.Places[from].ContainsKey(name))
		{

			var From = Positions.Places[from];
			Positions.Places.Remove(from);

			var card = From[name];
			From.Remove(name);

			var To = Positions.Places[to];
			Positions.Places.Remove(to);

			To.Add(card.name, card);


			Positions.Places.Add(from, From);
			Positions.Places.Add(to, To);


			if (to == 2)
			{
				GetNode<CardBase>("Cards/" + name).OnHand = true;
			}
			else
			{
				GetNode<CardBase>("Cards/" + name).OnHand = false;
			}

			if (to == 15)
			{
				GetNode<CardBase>("Cards/" + name).Ready = true;
			}
			else
			{
				GetNode<CardBase>("Cards/" + name).Ready = false;
			}

			if (((to > 3 && to < 10) || to >= 16 || to == 14) && (from == 2 || from == 3 || from == 10 || from == 11))
			{
				foreach (var item in playerDeck)
				{
					if (name == item.name)
					{
						if (item.power.Name != "None") item.power.Execute();
						else
						{
							if (phase == (int)Phase.PlayerTurn)
							{
								phase = (int)Phase.EnemyTurn;
							}
							else if (phase == (int)Phase.EnemyTurn) phase = (int)Phase.PlayerTurn;
						}
					}
				}
				foreach (var item in enemyDeck)
				{
					if (name == item.name)
					{
						if (item.power.Name != "None") item.power.Execute();
						else
						{
							if (phase == (int)Phase.PlayerTurn) phase = (int)Phase.EnemyTurn;
							else if (phase == (int)Phase.EnemyTurn) phase = (int)Phase.PlayerTurn;

						}
					}
				}
			}

		}
		if (from >= 2 && from != 3) Visuals(Positions.Positions[from].Key, Positions.Positions[from].Value, from);
		if (to >= 2 && to != 3) Visuals(Positions.Positions[to].Key, Positions.Positions[to].Value, to);
	}
	public void RandomizeHands()
	{
		Random random = new Random();
		List<int> nums = new List<int>();

		while (Positions.Places[2].Count < 10)
		{
			int n = random.Next(0, playerDeck.Count);

			for (int i = 0; i < playerDeck.Count; i++)
			{
				if (n == i && !nums.Contains(n))
				{
					MoveCard(0, 2, playerDeck[n].name);
					nums.Add(n);
					break;
				}
			}
		}
		nums = new List<int>();
		while (Positions.Places[3].Count < 10)
		{
			int n = random.Next(0, enemyDeck.Count);

			for (int i = 0; i < enemyDeck.Count; i++)
			{
				if (n == i && !nums.Contains(n))
				{
					MoveCard(1, 3, enemyDeck[n].name);
					nums.Add(n);
					break;
				}
			}
		}
	}
	public void EnemyIA()
	{

		Random random = new Random();
		
		var hand = Positions.Places[3];
		if (hand.Count > 0)
		{

			List<Cards> hands = new List<Cards>();
			foreach (var item in hand.Values)
			{
				hands.Add(item);
			}
			GD.Print("enemi mas q0" + " " + hand.Count);

			int n = random.Next(0, hands.Count - 1);
			int to = 0;

			if (hands[n] is UnitCard)
			{
				GD.Print("es unit");
				UnitCard card = (UnitCard)hands[n];
				switch (card.position)
				{
					case "Melee":
						to = 5;
						break;
					case "Middle":
						to = 7;
						break;
					case "Siege":
						to = 9;
						break;
				}
				MoveCard(3, to, card.name);
			}
			else if (hands[n] is EffectCard)
			{
				GD.Print("es effect");
				EffectCard card = (EffectCard)hands[n];

				switch (card.position)
				{
					case "Weather":
						MoveCard(3, 14, card.name);
						break;
					case "Support":
						int max = 0;
						int i = 0;
						for (int j = 5; j < 10; j = j + 2)
						{
							if (Positions.Places[j].Count > max)
							{
								i = j;
								max = Positions.Places[j].Count;
							}
						}
						GD.Print(card.name);
						if (i == 7)
						{
							MoveCard(3, 19, card.name);

						}
						else if (i == 9)
						{
							MoveCard(3, 17, card.name);

						}
						else
						{
							MoveCard(3, 21, card.name);
							if (i != 5) i = 5;
						}

						moved = i;

						break;
				}

			}
			if (playerPass)
			{
				enemyPass = true;
			}
		}
		else
		{
			foreach (var item in enemyDeck)
			{
				if (item is LeaderCard)
				{
					item.power.Execute();
				}
			}
			enemyPass = true;
		}
	}
	public void UpdateLife()
	{
		GetNode<AnimatedSprite>("BackGround/PlayerLife").Animation = PlayerLife.ToString();
		GetNode<AnimatedSprite>("BackGround/EnemyLife").Animation = EnemyLife.ToString();
	}
	public void Reborn()
	{
		RebornPower rebornData = (RebornPower)powerData[0];
		if (rebornData.select)
		{
			if (phase == (int)Phase.PlayerWaiting)
			{
				rebornData.names = new List<string>();

				foreach (var item in Positions.Places[rebornData.place].Keys)
				{
					rebornData.names.Add(item);
				}
				if (!(rebornData.names.Count >= rebornData.cardsCounter)) rebornData.cardsCounter = rebornData.names.Count;

				foreach (var item in rebornData.names)
				{
					MoveCard(rebornData.place, 15, item);
				}
			}
			else
			{
				Random random = new Random();
				List<Cards> cards = new List<Cards>();
				List<int> nums = new List<int>();
				int n = 0;

				foreach (var item in Positions.Places[rebornData.place].Values)
				{
					cards.Add(item);
				}

				if (!(cards.Count >= rebornData.cardsCounter)) rebornData.cardsCounter = cards.Count;

				while (rebornData.cardsCounter > 0)
				{

					n = random.Next(0, cards.Count - 1);

					if (!nums.Contains(n))
					{

						int to = 0;

						if (cards[n] is UnitCard)
						{
							UnitCard card = (UnitCard)cards[n];
							if (card.position == "Melee") to = 5;
							else if (card.position == "Middle") to = 7;
							else if (card.position == "Siege") to = 9;
						}
						else if (cards[n] is EffectCard)
						{
							to = 3;
						}
						nums.Add(n);
						MoveCard(rebornData.place, to, cards[n].name);
						rebornData.cardsCounter--;

					}
				}
				rebornData.processing = false;

			}
		}
		else
		{
			foreach (var item in rebornData.names)
			{
				if (Positions.Places[rebornData.place].ContainsKey(item))
				{
					List<Cards> cards = new List<Cards>();
					foreach (var card in Positions.Places[rebornData.place].Values)
					{
						cards.Add(card);
					}
					foreach (var card in cards)
					{
						int to = 0;
						if (card is UnitCard)
						{
							UnitCard newCard = (UnitCard)card;
							if (newCard.position == "Melee")
							{
								if (phase == (int)Phase.PlayerWaiting) to = 4;
								else to = 5;
							}
							else if (newCard.position == "Middle")
							{
								if (phase == (int)Phase.PlayerWaiting) to = 6;
								else to = 7;
							}
							else if (newCard.position == "Siege")
							{
								if (phase == (int)Phase.PlayerWaiting) to = 8;
								else to = 9;
							}
						}
						else if (card is EffectCard)
						{
							if (phase == (int)Phase.PlayerWaiting) to = 2;
							else to = 3;
						}

						MoveCard(rebornData.place, to, card.name);
					}
				}
				rebornData.processing = false;
			}
		}
	}
	public void Summon()
	{
		SummonPower summonData = (SummonPower)powerData[0];

		if (summonData.select)
		{
			if (phase == (int)Phase.PlayerWaiting)
			{
				summonData.names = new List<string>();

				foreach (var item in Positions.Places[0].Keys)
				{
					summonData.names.Add(item);
				}

				if (!(summonData.names.Count >= summonData.cardsCounter)) summonData.cardsCounter = summonData.names.Count;

				foreach (var item in summonData.names)
				{
					MoveCard(0, 15, item);
				}
			}
			else
			{
				Random random = new Random();
				List<Cards> cards = new List<Cards>();
				List<int> nums = new List<int>();
				int n = 0;

				foreach (var item in Positions.Places[1].Values)
				{
					cards.Add(item);
				}

				if (!(summonData.cardsCounter <= cards.Count)) summonData.cardsCounter = cards.Count;

				while (summonData.cardsCounter > 0)
				{

					n = random.Next(0, summonData.names.Count - 1);

					if (!nums.Contains(n))
					{

						int to = 0;

						if (cards[n] is UnitCard)
						{
							UnitCard card = (UnitCard)cards[n];
							if (card.position == "Melee") to = 5;
							else if (card.position == "Middle") to = 7;
							else if (card.position == "Siege") to = 9;
						}
						else if (cards[n] is EffectCard)
						{
							to = 3;
						}
						nums.Add(n);
						MoveCard(1, to, cards[n].name);
						summonData.cardsCounter--;

					}
				}
				summonData.processing = false;
			}
		}
		else
		{
			foreach (var item in summonData.names)
			{
				int places = 0;
				List<Cards> cards = new List<Cards>();
				if (phase == (int)Phase.PlayerWaiting) places = 0;
				else if (phase == (int)Phase.EnemyWaiting) places = 1;

				if (Positions.Places[places].ContainsKey(item))
				{
					foreach (var card in Positions.Places[places].Values)
					{
						if (card.name == item)
						{
							cards.Add(card);
						}
					}
				}
				foreach (var card in cards)
				{
					int to = 0;
					if (card is UnitCard)
					{
						UnitCard newCard = (UnitCard)card;
						if (newCard.position == "Melee")
						{
							if (phase == (int)Phase.PlayerWaiting) to = 4;
							else to = 5;
						}
						else if (newCard.position == "Middle")
						{
							if (phase == (int)Phase.PlayerWaiting) to = 6;
							else to = 7;
						}
						else if (newCard.position == "Siege")
						{
							if (phase == (int)Phase.PlayerWaiting) to = 8;
							else to = 9;
						}
					}
					else if (card is EffectCard)
					{
						if (phase == (int)Phase.PlayerWaiting) to = 2;
						else to = 3;
					}

					MoveCard(places, to, card.name);
				}
			}
			summonData.processing = false;
		}
	}
	public void SwitchBand()
	{
		SwitchBandPower switchBandData = (SwitchBandPower)powerData[0];
		int to = 0;
		int from = 0;

		for (int i = 4; i < 10; i++)
		{
			if (Positions.Places[i].ContainsKey(switchBandData.cardName))
			{
				from = i;
				break;
			}
		}
		if (phase == (int)Phase.PlayerWaiting) to = from + 1;
		else if (phase == (int)Phase.EnemyWaiting) to = from - 1;

		MoveCard(from, to, switchBandData.cardName);
		switchBandData.processing = false;
	}
	public void Destroy()
	{
		DestroyPower destroyData = (DestroyPower)powerData[0];
		if (destroyData.identifier == 0)
		{
			List<Cards> cards = new List<Cards>();
			foreach (var item in Positions.Places[destroyData.from].Values)
			{
				cards.Add(item);
			}

			int to = 0;
			if (destroyData.from % 2 == 0) to = 10;
			else to = 11;

			foreach (var item in cards)
			{
				MoveCard(destroyData.from, to, item.name);
			}
			destroyData.processing = false;
		}
		else if (destroyData.identifier == 1)
		{
			int from = 0;
			int to = 0;

			foreach (var item in destroyData.names)
			{
				for (int i = 4; i < 10; i++)
				{
					if (Positions.Places[i].ContainsKey(item))
					{
						from = i;
						if (from % 2 == 0) to = 10;
						else to = 11;
						MoveCard(from, to, item);
						break;
					}
				}
			}
			destroyData.processing = false;
		}
		else if (destroyData.identifier == 2)
		{
			if (phase == (int)Phase.PlayerWaiting)
			{
				List<Cards> cards = FieldZones.AllExistingCards().ToList();
				foreach (var item in cards)
				{
					GetNode<CardBase>("Cards/" + item.name).Ready = true;
				}
				if (!(cards.Count >= destroyData.cardsCounter)) destroyData.cardsCounter = cards.Count;
			}
			else if (phase == (int)Phase.EnemyWaiting)
			{
				List<Cards> playerCards = FieldZones.AllOwnCards().ToList();
				List<Cards> cards = FieldZones.AllExistingCards().ToList();
				int n = 0;
				Random random = new Random();
				List<int> nums = new List<int>();
				if (!(playerCards.Count >= destroyData.cardsCounter))
				{
					destroyData.cardsCounter = playerCards.Count;
				}
				while (destroyData.cardsCounter > 0)
				{

					n = random.Next(0, playerCards.Count - 1);

					if (!nums.Contains(n))
					{

						int from = 0;

						if (playerCards[n] is UnitCard)
						{
							UnitCard card = (UnitCard)cards[n];
							if (card.position == "Melee") from = 5;
							else if (card.position == "Middle") from = 7;
							else if (card.position == "Siege") from = 9;
							nums.Add(n);

							MoveCard(from, 10, cards[n].name);
							destroyData.cardsCounter--;
						}



					}
				}
				destroyData.processing = false;


			}
		}
	}
	public void ModifyAttack()
	{
		ModifyAttackPower modifyAttackData = (ModifyAttackPower)powerData[0];
		if (modifyAttackData.identifier == 0)
		{
			foreach (var item in Positions.Places[modifyAttackData.where].Values)
			{
				GD.Print(item.name);
				GetNode<Label>("Cards/" + item.name + "/BackGround/Attack").Text = (int.Parse(GetNode<Label>("Cards/" + item.name + "/BackGround/Attack").Text) + modifyAttackData.ammount).ToString();
			}
			powerData[0].processing = false;
		}
		else if (modifyAttackData.identifier == 1)
		{
			List<Cards> cards = FieldZones.AllExistingCards().ToList();
			foreach (var item in modifyAttackData.names)
			{
				foreach (var card in cards)
				{
					if (card.name == item)
					{
						GetNode<Label>("Cards/" + item + "/BackGround/Attack").Text = (int.Parse(GetNode<Label>("Cards/" + item + "/BackGround/Attack").Text) + modifyAttackData.ammount).ToString();
					}
				}
			}
			powerData[0].processing = false;
		}
		else if (modifyAttackData.identifier == 2)
		{

			if (phase == (int)Phase.PlayerWaiting)
			{
				List<Cards> cards = (List<Cards>)FieldZones.AllExistingCards();
				foreach (var item in cards)
				{
					GetNode<CardBase>("Cards/" + item.name).Ready = true;
				}
			}
			else if (phase == (int)Phase.EnemyWaiting)
			{
				List<Cards> cards = (List<Cards>)FieldZones.AllEnemyCards();
				if ((cards.Count >= modifyAttackData.cardsCounter)) modifyAttackData.cardsCounter = cards.Count;

				int from = 0;
				List<int> nums = new List<int>();
				while (modifyAttackData.cardsCounter > 0)
				{
					Random random = new Random();
					int n = random.Next(0, cards.Count);
					if (!nums.Contains(n))
					{
						GetNode<Label>("Cards/" + cards[n].name + "/BackGround/Attack").Text = (int.Parse(GetNode<Label>("Cards/" + cards[n].name + "/BackGround/Attack").Text) + modifyAttackData.ammount).ToString();
						nums.Add(n);
						modifyAttackData.cardsCounter--;
					}
				}
			}

		}
		else if (modifyAttackData.identifier == 3)
		{
			GD.Print("identifier3");
			if (phase == (int)Phase.PlayerWaiting)
			{
				foreach (var card in Positions.Places[moved].Values)
				{
					GD.Print(card.name);
					GetNode<Label>("Cards/" + card.name + "/BackGround/Attack").Text = (int.Parse(GetNode<Label>("Cards/" + card.name + "/BackGround/Attack").Text) + modifyAttackData.ammount).ToString();
				}
				powerData[0].processing = false;
			}
			else
			{
				foreach (var card in Positions.Places[moved].Values)
				{
					GetNode<Label>("Cards/" + card.name + "/BackGround/Attack").Text = (int.Parse(GetNode<Label>("Cards/" + card.name + "/BackGround/Attack").Text) + modifyAttackData.ammount).ToString();
				}
				powerData[0].processing = false;
			}
		}
	}
	public void Draw()
	{
		DrawPower drawData = (DrawPower)powerData[0];
		if (phase == (int)Phase.PlayerWaiting)
		{
			for (int i = 0; i < drawData.cardsCounter; i++)
			{
				Random random = new Random();
				List<Cards> cards = new List<Cards>();
				foreach (var item in Positions.Places[0].Values)
				{
					cards.Add(item);
				}
				int n = random.Next(0, cards.Count);
				MoveCard(0, 2, cards[n].name);
			}
		}
		else if (phase == (int)Phase.EnemyWaiting)
		{
			for (int i = 0; i < drawData.cardsCounter; i++)
			{
				Random random = new Random();
				List<Cards> cards = new List<Cards>();
				foreach (var item in Positions.Places[1].Values)
				{
					cards.Add(item);
				}
				int n = random.Next(0, cards.Count);
				MoveCard(1, 3, cards[n].name);
			}
		}

		drawData.processing = false;
	}
	public void RestoreReady()
	{
		List<Cards> cards = FieldZones.AllExistingCards().ToList();

		foreach (var item in cards)
		{
			GetNode<CardBase>("Cards/" + item.name).Ready = false;
		}
	}
	public void RemoveFirst()
	{
		powerData.RemoveAt(0);
	}
	public void ShowCard(Cards card)
	{
		GetNode<Node2D>("ShowCard").Show();
		GetNode<Label>("ShowCard/Card/BackGround/Name").Text = card.name;
		GetNode<Label>("ShowCard/Card/BackGround/Power").Text = card.power.Name;

		var Photo = new ImageTexture();
		Photo.Load(@"../Battle-Card/data/textures/cards/" + card.imagePath);
		GetNode<Sprite>("ShowCard/Card/BackGround/PhotoMark/Photo").Scale = GetNode<MarginContainer>("ShowCard/Card/BackGround/PhotoMark").RectSize / Photo.GetSize();

		if (card is UnitCard)
		{
			UnitCard newCard = (UnitCard)card;
			GetNode<Label>("ShowCard/Card/BackGround/Attack").Text = newCard.damage.ToString();
			GetNode<Label>("ShowCard/Card/BackGround/Phrase").Text = newCard.phrase;

		}
		else if (card is LeaderCard)
		{
			LeaderCard newCard = (LeaderCard)card;
			GetNode<Label>("ShowCard/Card/BackGround/Phrase").Text = newCard.phrase;
		}


	}

}


