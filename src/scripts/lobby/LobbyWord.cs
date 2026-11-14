using Godot;
using System;

public partial class LobbyWord : PanelContainer
{
	
	public override void _Ready()
	{
		var WordRemoveButton = GetNode<Button>("ItemMargin/ItemVBox/HeaderHBox/WordRemoveButton");
		var AddSentenceButton = GetNode<Button>("ItemMargin/ItemVBox/AddSentenceButton");
		
		WordRemoveButton.Pressed += WordRemoveButtonPressed;
		AddSentenceButton.Pressed += AddSentenceButtonPressed;
	}
	
	private void WordRemoveButtonPressed()
	{
		GD.Print("Нажата кнопка: Удалить слово");
		
		//TODO: удаление контейнера
	}
	
	private void AddSentenceButtonPressed()
	{
		GD.Print("Нажата кнопка: Добавить предложение");
		
		//TODO: создание окна или строки для добавления предложения
	}
}
