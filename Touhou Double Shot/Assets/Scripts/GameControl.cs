using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class GameControl : MonoBehaviour {

	public List<Button> buttons;
	//public Text[] buttonList;
	public Text currentPlayerText;
	public Text currentCommandText;
	public Transform p1Status;
	public Transform p2Status;
	public Transform highlight;

	private int maxHealth;
	private string playerSide;
	private int boardCounter;
	private bool battleStart;
	private Dictionary<string,Item> p1Locations;
	private Dictionary<string,Item> p2Locations;

	private string p1Char;
	private string p2Char;

	private string command;

	private Vector3 selectedPos;

	private void Start(){

		maxHealth = 3;
		boardCounter = 0;
		playerSide = "0";

		battleStart = false;

		p1Locations = new Dictionary<string,Item>();
		p2Locations = new Dictionary<string,Item>();

		p1Char = "reimu";
		p2Char = "marisa";

		command = "Board Setup";

	}

	private void Update(){

		if (battleStart){


			//Handle Player Turn Text
			if (boardCounter%2 == 0)
				currentPlayerText.text = p1Char;
			else
				currentPlayerText.text = p2Char;

			//Handle Current Command Text
			currentCommandText.text = command;

		} else{
			currentCommandText.text = command;
		}

	}

	public string GetPlayerSide(){
		return playerSide;
	}

	public void SetPlayerSide(string newPlayer){
		playerSide = newPlayer;
	}

	public void IncBoardCounter(){
		boardCounter++;
	}

	public void DecBoardCounter(){
		boardCounter--;
	}


	public int GetBoardCounter(){
		return boardCounter;
	}

	public void ResetBoardCounter(){
		boardCounter = 0;
	}

	public bool BattleState(){
		return battleStart;
	}

	public void StartBattle(){

		ToggleBattle();
		ResetBoardCounter();
		command = "";
		SetUpStatus();

		TurnOffButtons(GetPlayerButtons(p1Locations));
	}

	public void ToggleBattle(){
		battleStart = !battleStart;
	}

	//If space is already occupied, it is true and returns false
	//Else, does not contain key, then returns true;
	public bool CheckLocation(string gridspace, int playerNum){
		return !(GetLocations(playerNum).ContainsKey(gridspace));

	}
	public void AddLocation(string gridspace, Item newI, int playerNum){
			GetLocations(playerNum).Add(gridspace, newI);
		
	}

	public Dictionary<string,Item> GetLocations(int playerNum){
		if (playerNum == 1)
			return p1Locations;
		else if (playerNum == 2)
			return p2Locations;
		else 
			return new Dictionary<string,Item>(); //empty list
	}

	public string GetP1Char(){
		return p1Char;
	}

	public string GetP2Char(){
		return p2Char;
	}

	public string GetCommand(){
		return command;
	}
	public void SetAttackCommand(){
		command = "attack";
	}
	public void SetMoveCommand(){
		command = "move";
	}
	public void ResetCommand(){
		command = "";
	}

	public void SetUpStatus(){
		Transform currentItem;


		SetPlayerStatus(p1Char, p1Status);
		SetPlayerStatus(p2Char, p2Status);


	}

	public void UpdateStatusWindows(){

		UpdatePlayerStatus(p1Locations, p1Status);
		UpdatePlayerStatus(p2Locations, p2Status);
	}

	public void SetPlayerStatus(string charName, Transform playerStatus){
		Transform currentItem;

		//Set up each item
		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);
			currentItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(charName + "/" + currentItem.name);
			
			//Sets health display
			for (int j =0; j< (maxHealth-i); j++){
				 currentItem.GetChild(j).gameObject.SetActive(true);
			}

		}
	}

	public void UpdatePlayerStatus(Dictionary<string,Item> locs, Transform playerStatus){
		Transform currentItem;
		int itemHP;


		//Set up each item
		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);

			itemHP = GetItem(currentItem.gameObject.name, locs).GetHealth();

			//maxHealth -i = the hp for that size
			//hp  = ^ - 
			//Sets health display

			//itemhp = 3 2 1
			for (int j =0; j< (maxHealth - i); j++){

				if (j< itemHP)
					currentItem.GetChild(j).gameObject.SetActive(true);
				else
					currentItem.GetChild(j).gameObject.SetActive(false);
			}

		}
	}


	public Item GetItem(string sz, Dictionary<string,Item> locs){
		foreach(var key in locs.Keys){
			if (locs[key].GetSize() == sz)
				return locs[key];
		}
		return null;
	}	

	public void MoveHighlight(){
		highlight.position = selectedPos;
	}

	public void SetSelectedPos(Vector3 pos){
		selectedPos = pos;
	}

	public Vector3 GetSelectedPos(){
		return selectedPos;
	}

	public bool CheckHighlight(){
		return highlight.gameObject.activeSelf;

	}

	public void ToggleHighlight(){
		highlight.gameObject.SetActive(!highlight.gameObject.activeSelf);
	}


	public void TurnOnButtons(){
		foreach (var button in buttons){
			button.interactable = true;
		}
	}

	public void TurnOffButtons(List<Button> excluded){
		
		foreach (var button in buttons){

			if (!excluded.Contains(button))
				button.interactable = false;
		}

	}

	public List<Button> GetPlayerButtons(Dictionary<string,Item> locs){
		List<Button> playerButtons = new List<Button>();
		foreach (var button in buttons){
			if (locs.ContainsKey(button.name))
				playerButtons.Add(button);
				
		}
		return playerButtons;
	}
}

