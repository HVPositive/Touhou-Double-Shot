using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GridSpace : MonoBehaviour {

	private Button button;
	private SpriteRenderer spriteRend;

	public GameControl gameControl;

	private void Start(){
		button = GetComponent<Button>();
		spriteRend = GetComponentInChildren<SpriteRenderer>();
		button.onClick.AddListener(ButtonClick); //Handles actions after the board has been setup

	}

	public void ButtonClick(){
		//If battle is not started, button clicking sets up the board
		if (!gameControl.BattleState()){
			
			//Sets the sprite on the gameboard
			gameControl.SetSprite(this.name,spriteRend);

			//If the player has set all its sprites then swap the player side
			if (gameControl.GetNextSize() == "swap"){
				gameControl.SwapPlayerSide();

				//after swapping sides and it says to swap again, then both players have setup their board - so start battle
				if (gameControl.GetNextSize() == "swap")
					gameControl.StartBattle();

			}

		} else{

			//If the item selected highlight is off, turn it on
			if (!gameControl.CheckHighlight()){
				gameControl.ActivateHighlight();
			}

			//If no command is chosen, then player is selecting an item - Set this button as highlighted and move it to this button
			if (gameControl.GetCommand() == ""){
				gameControl.SetHighlight(GetComponent<Button>());
				gameControl.MoveHighlight();
			} else if (gameControl.GetCommand() == "Attack"){

				gameControl.CheckHit(this.name, transform.position, spriteRend);

			} else if (gameControl.GetCommand() == "Move"){

				gameControl.MoveItem(this.name, spriteRend);

			}
		}

	}


}
