using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameControl : MonoBehaviour {

	public Text[] buttonList;
	public Text currentPlayerText;
	private string playerSide;
	private int boardCounter;
	private bool battleStart;
	private Dictionary<string,Items.Item> p1Locations;
	private Dictionary<string,Items.Item> p2Locations;

	private string p1Char;
	private string p2Char;

	private void Start(){

		boardCounter = 0;
		playerSide = "0";

		battleStart = false;

		p1Locations = new Dictionary<string,Items.Item>();
		p2Locations = new Dictionary<string,Items.Item>();

		p1Char = "reimu";
		p2Char = "marisa";

	}

	private void Update(){

		if (battleStart){
			if (boardCounter%2 == 0)
				currentPlayerText.text = p1Char;
			else
				currentPlayerText.text = p2Char;

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

	public void ToggleBattle(){
		battleStart = !battleStart;
	}


	public void AddLocation(string gridspace, Items.Item newI, int playerNum){
		GetLocations(playerNum).Add(gridspace, newI);
	}
	public Dictionary<string,Items.Item> GetLocations(int playerNum){
		if (playerNum == 1)
			return p1Locations;
		else if (playerNum == 2)
			return p2Locations;
		else 
			return new Dictionary<string,Items.Item>(); //empty list
	}

	public string GetP1Char(){
		return p1Char;
	}

	public string GetP2Char(){
		return p2Char;
	}

}
