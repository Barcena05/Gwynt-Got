using Godot;
using System;
using System.IO;
public class Manage : Node2D
{

	public static string adress = "../Battle-Card/data/textures/cards/band0";
	public static bool go;
	public static string[] names = new string[0];
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		go = false;
	}


	public void _on_Menu_Manage()
	{
		Show();
		go = true;
	}
	public void _on_Menu_pressed()
	{
		Hide();
		go = false;
		GetNode<Node2D>("../Menu").Show();
	}

	public void _on_DeleteButton_pressed()
	{
		string name = GetNode<TextEdit>("Name").Text;
		bool exist = false;
		if (name != string.Empty)
		{
			foreach (var item in names)
			{
				string txtName = item.Substring(adress.Length + 1, item.Length - adress.Length - 5);
				if (name == txtName) exist = true;
			}
			if (exist)
			{
				System.IO.File.Delete(@adress + "/" + name + ".txt");
			}
		}
	}
	public void _on_OpenButton_pressed()
	{
		string name = GetNode<TextEdit>("Name").Text;
		bool exist = false;
		string text;

		if (name != string.Empty)
		{
			foreach (var item in names)
			{
				string txtName = item.Substring(adress.Length + 1, item.Length - adress.Length - 5);
				if (name == txtName) exist = true;
			}
			if (exist)
			{
				text = System.IO.File.ReadAllText(@adress + "/" + name + ".txt");
				GetNode<TextEdit>("TextEdit").Text = text;
			}
		}
	}
	public void _on_SaveButton_pressed()
	{
		string text = GetNode<TextEdit>("TextEdit").Text;
		string name = GetNode<TextEdit>("Name").Text;

		if (name != string.Empty)
		{
			System.IO.File.WriteAllText(@adress + "/" + name + ".txt", text);
		}
	}
	public void _on_Band_pressed()
	{
		if (GetNode<Button>("Band").Text == "Daenerys")
		{
			GetNode<Button>("Band").Text = "JonSnow";
			adress = "../Battle-Card/data/textures/cards/band1";

		}
		else
		{
			GetNode<Button>("Band").Text = "Daenerys";
			adress = "../Battle-Card/data/textures/cards/band0";
		}
	}
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (go)
		{
			if (Input.IsActionPressed("save"))
			{
				string text = GetNode<TextEdit>("TextEdit").Text;
				string name = GetNode<TextEdit>("Name").Text;

				if (name != string.Empty)
				{
					System.IO.File.WriteAllText(@adress + "/" + name + ".txt", text);
				}
			}
			names = System.IO.Directory.GetFiles(@adress, "*.txt");
			GetNode<TextEdit>("NameList").Text = string.Empty;
			foreach (var item in names)
			{
				string name = item.Substring(adress.Length + 1, item.Length - adress.Length - 5);
				GetNode<TextEdit>("NameList").Text += name + "\n";
			}
		}
	}
}


