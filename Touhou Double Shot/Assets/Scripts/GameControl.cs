using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameControl : MonoBehaviour {

	public Text[] buttonList;
	public Text currentPlayerText;
	public Text currentCommandText;
	public Transform p1Status;
	public Transform p2Status;


	private string playerSide;
	private int boardCounter;
	private bool battleStart;
	private Dictionary<string,Item> p1Locations;
	private Dictionary<string,Item> p2Locations;

	private string p1Char;
	private string p2Char;

	private string command;

	private void Start(){

		boardCounter = 0;
		playerSide = "0";

		battleStart = false;

		p1Locations = new Dictionary<string,Item>();
		p2Locations = new Dictionary<string,Item>();

		p1Char = "reimu";
		p2Char = "marisa";

		command = "";

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
		SetUpStatus();
	}

	public void ToggleBattle(){
		battleStart = !battleStart;
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

	public void SetPlayerStatus(string charName, Transform playerStatus){
		Transform currentItem;


		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);
			currentItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(charName + "/" + currentItem.name);

		}
	}
}

