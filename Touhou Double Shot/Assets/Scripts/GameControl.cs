using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

	//public Text[] buttonList;
	private string playerSide;

	void Awake(){

		playerSide = "1";
	}

	public string GetPlayerSide(){
		return "1";
	}

	public void SetPlayerSide(String newPlayer){
		return newPlayer;
	}
}
