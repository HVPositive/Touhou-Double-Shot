using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

	public Text[] buttonList;
	private string playerSide;
	private int boardCounter;
	private Dictionary<string,string> locations;

	private void Start(){

		boardCounter = 0;
		playerSide = "0";

		locations = new Dictionary<string,string>();
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

	public void AddLocation(string item, string gridspace){
		locations.Add(item, gridspace);
	}
	
}
