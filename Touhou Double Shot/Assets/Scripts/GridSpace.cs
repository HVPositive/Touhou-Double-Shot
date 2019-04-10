using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GridSpace : MonoBehaviour {

	private Button button;
	private SpriteRenderer spriteRend;


	public GameControl gameControl;


	public void Start(){
		button = GetComponent<Button>();
		spriteRend = GetComponentInChildren<SpriteRenderer>();
		button.onClick.AddListener(SetSpace);

	}
	public void  SetGameControlReference(GameControl control){
		gameControl = control;
	}


	public void SetSpace(){

		if (gameControl.GetPlayerSide() == "1"){


			spriteRend.sprite = Resources.Load<Sprite>("reimu/large");
			gameControl.SetPlayerSide("2");
		}
		else{ 
			spriteRend.sprite = Resources.Load<Sprite>("marisa/large");
			gameControl.SetPlayerSide("1");
		}

		button.interactable = false;
	}
	
}
