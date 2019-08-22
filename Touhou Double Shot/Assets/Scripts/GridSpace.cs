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
		button.onClick.AddListener(ButtonClick);
		button.onClick.AddListener(SetSpace);


	}

	public void Update(){


	}
	public void  SetGameControlReference(GameControl control){
		gameControl = control;
	}


	public void SetSpace(){

		if (gameControl.BattleState())
			return;
		else{
			//make this done by turn count rather than hardcoded number
			//Sets up the board depending on selected space.

			gameControl.SetSprite(this.name,spriteRend);
			//SetSprite(gameControl.GetPlayerSide(), gameControl.GetNextSize());

			if (gameControl.GetNextSize() == "swap"){
				gameControl.SwapPlayerSide();

				//after swapping sides and it says to swap again, start battle
				if (gameControl.GetNextSize() == "swap")
					gameControl.StartBattle();

			}

		}

	}

	public void ButtonClick(){
		if (!gameControl.BattleState()){
			return;
		}

		if (!gameControl.CheckHighlight()){
			gameControl.ActivateHighlight();
		}

		if (gameControl.GetCommand() == "Attack"){
			gameControl.CheckHit(this.name, transform.position, spriteRend);

		} else if (gameControl.GetCommand() == "Move"){

			gameControl.MoveItem(this.name, spriteRend);

		} else{
			gameControl.SetHighlight(GetComponent<Button>());
			gameControl.MoveHighlight();
		}

	}


}
