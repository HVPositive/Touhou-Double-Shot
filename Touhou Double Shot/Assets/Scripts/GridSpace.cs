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

		if (gameControl.GetBoardCounter() == 0){
			SetSprite("reimu", "large");
		}
		else if (gameControl.GetBoardCounter() == 1)
			SetSprite("reimu", "medium");
		else if (gameControl.GetBoardCounter() == 2)
			SetSprite("reimu", "small");
		else if (gameControl.GetBoardCounter() == 3)
			SetSprite("marisa", "large");			
		else if (gameControl.GetBoardCounter() == 4)
			SetSprite("marisa", "medium");
		else if (gameControl.GetBoardCounter() == 5)
			SetSprite("marisa", "small");

		gameControl.IncBoardCounter();
		//button.interactable = false;
	}
	
	public void SetSprite(string character, string size){
		spriteRend.sprite = Resources.Load<Sprite>(character + "/" + size);
		gameControl.AddLocation(character + " " + size, this.name);



	}	
}
