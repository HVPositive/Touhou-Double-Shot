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
	public GameObject commandButtons;

	private int maxHealth;
	private string playerSide;
	private int boardCounter;
	private bool battleStart;
	private Dictionary<string,Item> p1Locations;
	private Dictionary<string,Item> p2Locations;

	private string p1Char;
	private string p2Char;

	private string command;


	private Button highlightedButton;

	private void Start(){

		maxHealth = 3;
		boardCounter = 0;
		//playerSide = p1Char;

		battleStart = false;

		p1Locations = new Dictionary<string,Item>();
		p2Locations = new Dictionary<string,Item>();

		p1Char = "reimu";
		p2Char = "marisa";

		command = "Board Setup";
		playerSide = p1Char;

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

	public void SwapPlayerSide(){
		if (playerSide == p1Char){
			playerSide = p2Char;


			TurnOffButtons(GetPlayerButtons(p2Locations));
		}
		else if (playerSide == p2Char){
			playerSide = p1Char;


			TurnOffButtons(GetPlayerButtons(p1Locations));
		}
		DeactivateHighlight();
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
		//Do it for first turn
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
	public Dictionary<string,Item> GetLocations(){
		if (playerSide == p1Char)
			return p1Locations;
		else if (playerSide == p2Char)
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
		//TurnOnButtons();
		TurnOffButtons(GetAttackButtons(highlightedButton));
	}
	public void SetMoveCommand(){
		command = "move";
		TurnOffButtons(GetMoveButtons(highlightedButton));
	}

	public void BackCommand(){
		DeactivateHighlight();
		TurnOffButtons(GetPlayerButtons(GetLocations()));
		ResetCommand();
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
		highlight.position = highlightedButton.GetComponent<Transform>().position;
	}

	public void SetHighlight(Button b){
		highlightedButton = b;
	}
	/*
	public void SetSelectedPos(Vector3 pos){
		selectedPos = pos;
	}

	public Vector3 GetSelectedPos(){
		return selectedPos;
	}
*/
	public bool CheckHighlight(){
		return highlight.gameObject.activeSelf;

	}

	public void ActivateHighlight(){
		highlight.gameObject.SetActive(true);
		ActivateCommandButtons();
	}
	public void DeactivateHighlight(){
		highlight.gameObject.SetActive(false);
		DectivateCommandButtons();
	}

	public void ActivateCommandButtons(){
		commandButtons.SetActive(true);
	}

	public void DectivateCommandButtons(){
		commandButtons.SetActive(false);
	}



	public void TurnOnButtons(){
		foreach (var button in buttons){
			button.interactable = true;
		}
	}

	public void TurnOffButtons(List<Button> excluded){
		//Turn on all buttons before turning off all buttons
		TurnOnButtons();
		foreach (var button in buttons){

			if (!excluded.Contains(button))
				button.interactable = false;
		}

	}
	//locs - location of player's items
	//Returns a list of buttons the players items
	public List<Button> GetPlayerButtons(Dictionary<string,Item> locs){
		List<Button> playerButtons = new List<Button>();
		foreach (var button in buttons){
			if (locs.ContainsKey(button.name))
				playerButtons.Add(button);
				
		}
		return playerButtons;
	}

	//
	//return list of buttons around another button
	public List<Button> GetAttackButtons(Button buttonLoc){
		List<Button> attackButtons = new List<Button>();

		RaycastHit2D hit;
		Vector2[] attackVectors = new Vector2[] { Vector2.up, new Vector2(1,1), Vector2.right,
		 										 new Vector2(1,-1), Vector2.down, new Vector2(-1,-1),
		 										 Vector2.left, new Vector2(-1,1)};

		for (int i =0; i < attackVectors.Length; i++){
			hit = Physics2D.Raycast(buttonLoc.GetComponent<Transform>().position, attackVectors[i]);

			if (hit.collider!=null)
				attackButtons.Add(hit.collider.gameObject.GetComponent<Button>());

		}


		return attackButtons;
	}

	public List<Button> GetMoveButtons(Button buttonLoc){
		List<Button> moveButtons = new List<Button>();

		RaycastHit2D hit;
		Vector2[] moveVectors = new Vector2[] { Vector2.up, Vector2.right,
		 										Vector2.down, 
		 										Vector2.left};

		for (int i =0; i < moveVectors.Length; i++){
			hit = Physics2D.Raycast(buttonLoc.GetComponent<Transform>().position, moveVectors[i]);

			if (hit.collider!=null)
				moveButtons.Add(hit.collider.gameObject.GetComponent<Button>());

			//keep on raycasting in direction and add buttons
			while (hit.collider!=null){
				hit = Physics2D.Raycast(hit.collider.gameObject.GetComponent<Transform>().position, moveVectors[i]);
				
				if (hit.collider!=null)
					moveButtons.Add(hit.collider.gameObject.GetComponent<Button>());

			}

		}

		return moveButtons;
	}


	//used to check if two buttons are adjacent. NOt used but kept for now
	public bool CheckAdjacent(Button b1, Button b2){

		//Gets angle between buttons and casts ray from the first button in previous angle. Then if the hit object is same as second button, then they are adjacent 
		RaycastHit2D hit;
		hit  = Physics2D.Raycast(b1.GetComponent<Transform>().position, b2.GetComponent<Transform>().position - b1.GetComponent<Transform>().position );

		//Debug.Log( hit.collider.gameObject.name );
		if (hit.collider!=null && (hit.collider.gameObject.name == b2.gameObject.name) )
			return true;
		else
			return false;



	}

}

