using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GridSpace : MonoBehaviour {

	public Button button;
	public Text buttonText;


	private GameControl gameControl;


	public void  SetGameControlReference(GameControl control){
		gameControl = control;
	}


	public void SetSpace(){

		if (gameControl.GetPlayerSide() == "1"){


			buttonText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("reimu/large");
			gameControl.SetPlayerSide("2");
		}
		else{ 
			buttonText.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("marisa/large");
			gameControl.SetPlayerSide("1");
		}

		button.interactable = false;
	}
}
