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
		//if (gameControl.GetBoardCounter() ==6){
			

		//}


	}
	public void  SetGameControlReference(GameControl control){
		gameControl = control;
	}


	public void SetSpace(){

		if (gameControl.BattleState())
			button.onClick.RemoveListener(SetSpace);
		else{

			//Sets up the board depending on selected space.
			if (gameControl.GetBoardCounter() == 0){
				SetSprite(1, "large");
			}
			else if (gameControl.GetBoardCounter() == 1)
				SetSprite(1, "medium");
			else if (gameControl.GetBoardCounter() == 2)
				SetSprite(1, "small");
			else if (gameControl.GetBoardCounter() == 3)
				SetSprite(2, "large");			
			else if (gameControl.GetBoardCounter() == 4)
				SetSprite(2, "medium");
			else if (gameControl.GetBoardCounter() == 5)
				SetSprite(2, "small");

			gameControl.IncBoardCounter();

			//only increment if all set. Afterwards, this listener is usless and should be removed if activated.
			if (gameControl.GetBoardCounter() >= 6){
				gameControl.StartBattle();
			}

		}
		//button.interactable = false;
	}

	
	public void SetSprite(int charNum, string size){
		string charName = GetCharName(charNum);
		
		//gameControl.AddLocation(character + " " + size, this.name);
		
		//if (gameControl.CheckLocation(this.name, charNum))
		if (gameControl.CheckLocation(this.name, charNum)){
			gameControl.AddLocation(this.name, new Item(size,charName), charNum);
			spriteRend.sprite = Resources.Load<Sprite>(charName + "/" + size);
		} else {
			gameControl.DecBoardCounter();
			Debug.Log("Space already occupied.");
		}



	}


	public void ButtonClick(){
		if (!gameControl.BattleState()){
			return;
		}

		if (!gameControl.CheckHighlight()){
			gameControl.ActivateHighlight();
		}
		gameControl.SetHighlight(GetComponent<Button>());
		gameControl.MoveHighlight();
		

		if (gameControl.GetCommand() == "attack"){
			//TODO 
			CheckHit();
			gameControl.SwapPlayerSide();
		} else if (gameControl.GetCommand() == "move"){
			//TODO
			//Move
			MoveItem();
			gameControl.SwapPlayerSide();
		}

	}
	//even is p1, odd is p2 turn

	public void CheckHit(){

		int pNum = 0;


		if (gameControl.GetPlayerSide() == gameControl.GetP1Char() )
			pNum = 2; //If even, then p1's turn and check p2
		else if (gameControl.GetPlayerSide() == gameControl.GetP2Char())
			pNum = 1; 

		if (gameControl.GetLocations(pNum).ContainsKey(this.name) ){
			Debug.Log("nice hit");
			gameControl.GetLocations(pNum)[this.name].HitItem();
			gameControl.UpdateStatusWindows();

		} else if (CheckGraze(pNum)){
			Debug.Log("nice graze");
		} else{
			Debug.Log("nice miss");
		}

		gameControl.IncBoardCounter();
		gameControl.ResetCommand();

	} 	


	public bool CheckGraze(int pNum){
		bool hitFound = false;

		RaycastHit2D hit;
		Vector2[] grazeVectors = new Vector2[] { Vector2.up, new Vector2(1,1), Vector2.right,
		 										 new Vector2(1,-1), Vector2.down, new Vector2(-1,-1),
		 										 Vector2.left, new Vector2(-1,1)};

		for (int i =0; i < grazeVectors.Length; i++){
			hit = Physics2D.Raycast(transform.position, grazeVectors[i]);

			if (hit.collider!=null && gameControl.GetLocations(pNum).ContainsKey(hit.collider.gameObject.name))
				hitFound = true;

		}

		return hitFound;


	}

	public void MoveItem(){

		gameControl.IncBoardCounter();
		gameControl.ResetCommand();

	}

	public string GetCharName(int num){
		if (num ==1)
			return gameControl.GetP1Char();
		else if (num ==2)
			return gameControl.GetP2Char();
		else
			return "reimu"; //default char?
	}


}
