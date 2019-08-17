using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class GameControl : MonoBehaviour {

	public List<Button> buttons;
	//public Text[] buttonList;
	public Text currentPlayerText;
	public Text currentCommandText;
	public Transform p1Status;
	public Transform p2Status;
	public Transform highlight; // the transform of the highlighter
	public GameObject commandButtons;

	public GameObject WinText;

	private int maxHealth;
	private string playerSide;
	private bool battleStart;
	private Dictionary<string,Item> p1Locations;
	private Dictionary<string,Item> p2Locations;

	private string p1Char;
	private string p2Char;

	private string command;

	private Color regular;
	private Color fade;


	private Button highlightedButton;

	private void Start(){

		maxHealth = 3;
		//playerSide = p1Char;

		battleStart = false;
		p1Locations = new Dictionary<string,Item>();
		p2Locations = new Dictionary<string,Item>();

		ResetVariables();

		regular = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		fade = new Color(1.0f, 1.0f, 1.0f, 0.4f);

	}

	private void Update(){
		//Handle Player Turn Text

		currentPlayerText.text = UppercaseFirstChar(playerSide);

		if (battleStart){




			//Handle Current Command Text
			currentCommandText.text = command;

		} else{
			currentCommandText.text = command;
		}

	}

	public string GetPlayerSide(){
		return playerSide;
	}


	public void SwapPlayerSide(){
		if (playerSide == "end"){
			TurnOffButtons();

		} else if (playerSide == p1Char){
			playerSide = p2Char;


			if (battleStart) TurnOffButtons(GetPlayerButtons(p2Locations));
		}
		else if (playerSide == p2Char){
			playerSide = p1Char;


			if (battleStart) TurnOffButtons(GetPlayerButtons(p1Locations));
		}
		DeactivateHighlight();
	}


	public bool BattleState(){
		return battleStart;
	}

	public void StartBattle(){

		ToggleBattle();

		command = "";
		SetUpStatus();
		//Do it for first turn
		TurnOffButtons(GetPlayerButtons(p1Locations));
	}

	public void RestartBoard(){
		ToggleBattle();


		ResetVariables();


	}

	public void ToggleBattle(){
		battleStart = !battleStart;
	}

	//return false - if it contains a key
	public bool CheckLocation(string gridspace){
		return !(GetLocations().ContainsKey(gridspace));

	}

	public void AddLocation(string gridspace, Item newI){
			GetLocations().Add(gridspace, newI);
	}

	public void RemoveLocation(string gridspace){
		GetLocations().Remove(gridspace);
	}

	public Dictionary<string,Item> GetOppositeLocations(){
		if (playerSide == p1Char)
			return p2Locations;
		else if (playerSide == p2Char)
			return p1Locations;
		else 
			return new Dictionary<string,Item>(); //empty list
	}	

	public Dictionary<string,Item> GetLocations(string playerNum){
		if (playerNum == p1Char)
			return p1Locations;
		else if (playerNum == p2Char)
			return p2Locations;
		else 
			return new Dictionary<string,Item>(); //empty list
	}
	public Dictionary<string,Item> GetLocations(){
		if (playerSide == p1Char)
			return p1Locations;
		else if (playerSide == p2Char)
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
		//TurnOnButtons();
		TurnOffButtons(GetAttackButtons(highlightedButton));
	}
	public void SetMoveCommand(){
		command = "move";
		TurnOffButtons(GetMoveButtons(highlightedButton));
	}

	public void BackCommand(){
		DeactivateHighlight();
		TurnOffButtons(GetPlayerButtons(GetLocations()));
		ResetCommand();
	}
	public void ResetCommand(){
		command = "";
	}


	public void SetUpStatus(){
		SetPlayerStatus(p1Char, p1Status);
		SetPlayerStatus(p2Char, p2Status);


	}

	public void UpdateStatusWindows(){

		UpdatePlayerStatus(p1Locations, p1Status);
		UpdatePlayerStatus(p2Locations, p2Status);
	}

	public Transform OppositeStatus(){
		if (playerSide == p1Char)
			return p2Status;
		else if (playerSide == p2Char)
			return p1Status;
		else 
			return null; //empty list		
	}

	public void ClearPlayerStatus(Transform playerStatus){
		Transform currentItem;

		//Set up each item
		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);
			currentItem.GetComponent<SpriteRenderer>().sprite = null;
			
			//Sets health display
			for (int j =0; j< maxHealth; j++){
				 currentItem.GetChild(j).gameObject.SetActive(false);
			}

		}
	}

	public void SetPlayerStatus(string charName, Transform playerStatus){
		Transform currentItem;

		//Set up each item
		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);
			currentItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(charName + "/" + currentItem.name);
			
			//Sets health display
			for (int j =0; j< (maxHealth-i); j++){
				 currentItem.GetChild(j).gameObject.SetActive(true);
			}

		}
	}

	public void UpdatePlayerStatus(Dictionary<string,Item> locs, Transform playerStatus){
		Transform currentItem;
		int itemHP;


		//Set up each item
		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);
			if (GetItem(currentItem.gameObject.name, locs) != null)
				itemHP = GetItem(currentItem.gameObject.name, locs).GetHealth();
			else
				itemHP = 0;

			if (itemHP == 0 )
				currentItem.GetComponent<SpriteRenderer>().color = fade;
			//maxHealth -i = the hp for that size
			//hp  = ^ - 
			//Sets health display

			//itemhp = 3 2 1
			for (int j =0; j< (maxHealth - i); j++){

				if (j< itemHP)
					currentItem.GetChild(j).gameObject.SetActive(true);
				else
					currentItem.GetChild(j).gameObject.SetActive(false);
			}

		}
	}

	public void UpdateSingleStatus(Dictionary<string,Item> locs, Transform playerStatus, string size){
		Transform currentItem = playerStatus.Find(size);
		int itemHP;

		if (GetItem(currentItem.gameObject.name, locs) != null)
			itemHP = GetItem(currentItem.gameObject.name, locs).GetHealth();
		else
			itemHP = 0;

		if (itemHP == 0 )
			currentItem.GetComponent<SpriteRenderer>().color = fade;
		//maxHealth -i = the hp for that size
		//hp  = ^ - 
		//Sets health display

		int i = 3-SizeHP(size);
		//itemhp = 3 2 1
		for (int j =0; j< (maxHealth - i); j++){

			if (j< itemHP)
				currentItem.GetChild(j).gameObject.SetActive(true);
			else
				currentItem.GetChild(j).gameObject.SetActive(false);
		}

		
	}

	public Item GetItem(string sz, Dictionary<string,Item> locs){
		foreach(var key in locs.Keys){
			if (locs[key].GetSize() == sz)
				return locs[key];
		}
		return null;
	}	

	public void MoveHighlight(){
		highlight.position = highlightedButton.GetComponent<Transform>().position;
	}

	public void SetHighlight(Button b){
		highlightedButton = b;
	}

	public bool CheckHighlight(){
		return highlight.gameObject.activeSelf;

	}

	public void ActivateHighlight(){
		highlight.gameObject.SetActive(true);
		ActivateCommandButtons();
	}
	public void DeactivateHighlight(){
		highlight.gameObject.SetActive(false);
		DectivateCommandButtons();
	}

	public void ActivateCommandButtons(){
		commandButtons.SetActive(true);
	}

	public void DectivateCommandButtons(){
		commandButtons.SetActive(false);
	}



	public void TurnOnButtons(){
		foreach (var button in buttons){
			button.interactable = true;
		}
	}

	public void TurnOffButtons(List<Button> excluded){
		//Turn on all buttons before turning off all buttons
		TurnOnButtons();
		foreach (var button in buttons){

			if (!excluded.Contains(button))
				button.interactable = false;
		}

	}


	public void TurnOffButtons(){

		foreach (var button in buttons){
				button.interactable = false;
		}

	}
	//locs - location of player's items
	//Returns a list of buttons the players items
	public List<Button> GetPlayerButtons(Dictionary<string,Item> locs){
		List<Button> playerButtons = new List<Button>();
		foreach (var button in buttons){
			if (locs.ContainsKey(button.name))
				playerButtons.Add(button);
				
		}
		return playerButtons;
	}

	//
	//return list of buttons around another button
	public List<Button> GetAttackButtons(Button buttonLoc){
		List<Button> attackButtons = new List<Button>();

		RaycastHit2D hit;
		Vector2[] attackVectors = new Vector2[] { Vector2.up, new Vector2(1,1), Vector2.right,
		 										 new Vector2(1,-1), Vector2.down, new Vector2(-1,-1),
		 										 Vector2.left, new Vector2(-1,1)};

		for (int i =0; i < attackVectors.Length; i++){
			hit = Physics2D.Raycast(buttonLoc.GetComponent<Transform>().position, attackVectors[i]);

			if (hit.collider!=null)
				attackButtons.Add(hit.collider.gameObject.GetComponent<Button>());

		}


		return attackButtons;
	}

	public List<Button> GetMoveButtons(Button buttonLoc){
		List<Button> moveButtons = new List<Button>();

		RaycastHit2D hit;
		Vector2[] moveVectors = new Vector2[] { Vector2.up, Vector2.right,
		 										Vector2.down, 
		 										Vector2.left};

		for (int i =0; i < moveVectors.Length; i++){
			hit = Physics2D.Raycast(buttonLoc.GetComponent<Transform>().position, moveVectors[i]);

			if (hit.collider!=null)
				moveButtons.Add(hit.collider.gameObject.GetComponent<Button>());

			//keep on raycasting in direction and add buttons
			while (hit.collider!=null){
				hit = Physics2D.Raycast(hit.collider.gameObject.GetComponent<Transform>().position, moveVectors[i]);
				
				if (hit.collider!=null)
					moveButtons.Add(hit.collider.gameObject.GetComponent<Button>());

			}

		}

		return moveButtons;
	}


	//used to check if two buttons are adjacent. Not used but kept for now
	public bool CheckAdjacent(Button b1, Button b2){

		//Gets angle between buttons and casts ray from the first button in previous angle. Then if the hit object is same as second button, then they are adjacent 
		RaycastHit2D hit;
		hit  = Physics2D.Raycast(b1.GetComponent<Transform>().position, b2.GetComponent<Transform>().position - b1.GetComponent<Transform>().position );

		//Debug.Log( hit.collider.gameObject.name );
		if (hit.collider!=null && (hit.collider.gameObject.name == b2.gameObject.name) )
			return true;
		else
			return false;



	}


	public void MoveItem(string newSpace, SpriteRenderer sr){
		string highlightedName = highlightedButton.gameObject.name;
		//Copy highlighted item to newspace
		if (SetSprite(newSpace, sr,GetLocations()[highlightedName]) ){

		//remove old space from dictionary
			RemoveLocation(highlightedName);
		
		//remove the sprite
			highlightedButton.GetComponentInChildren<SpriteRenderer>().sprite = null;


			SwapPlayerSide();
			ResetCommand();
		}
	}



	public string GetNextSize(){
		Dictionary<string,Item> locs = GetLocations();
		
		if (GetItem("large", locs) ==null)
			return "large";
		else if (GetItem("medium", locs) == null)
			return "medium";
		else if (GetItem("small", locs) == null)
			return "small";

		return "swap";
	}

	public bool SetSprite(string space, SpriteRenderer sr){
		string size = GetNextSize();
		//string charName = GetCharName(charNum);
		
		if (CheckLocation(space)){
			AddLocation(space, new Item(size));
			sr.sprite = Resources.Load<Sprite>(playerSide + "/" + size);
			return true;
		} else {
			Debug.Log("Space already occupied.");
			return false;
		}




	}
	public bool SetSprite(string space, SpriteRenderer sr, Item item){

		//string charName = GetCharName(charNum);
		
		//gameControl.AddLocation(character + " " + size, this.name);
		
		//if (gameControl.CheckLocation(this.name, charNum))
		if (CheckLocation(space)){
			AddLocation(space, item);
			sr.sprite = Resources.Load<Sprite>(playerSide + "/" + item.GetSize());
			return true;
		} else {

			Debug.Log("Space already occupied.");
			return false;
		}



	}


	public void CheckHit(string space, Vector2 pos, SpriteRenderer sr){




		if (GetOppositeLocations().ContainsKey(space) ){
			Debug.Log("nice hit");
			string size = GetOppositeLocations()[space].GetSize();
			if (GetOppositeLocations()[space].HitItem() == 0)
				ItemDeath(space,sr);
				
				//we want to remove it from the dictionary, change opacity in, and remove from board
			//UpdateStatusWindows(); (Dictionary<string,Item> locs, Transform playerStatus, string size
			UpdateSingleStatus(GetOppositeLocations(),OppositeStatus(), size );
			CheckWin();

		} else if (CheckGraze(pos)){
			Debug.Log("nice graze");
		} else{
			Debug.Log("nice miss");
		}
		SwapPlayerSide();
		ResetCommand();

	} 	


	public bool CheckGraze(Vector2 pos){
		bool hitFound = false;

		RaycastHit2D hit;
		Vector2[] grazeVectors = new Vector2[] { Vector2.up, new Vector2(1,1), Vector2.right,
		 										 new Vector2(1,-1), Vector2.down, new Vector2(-1,-1),
		 										 Vector2.left, new Vector2(-1,1)};

		for (int i =0; i < grazeVectors.Length; i++){
			hit = Physics2D.Raycast(pos, grazeVectors[i]);

			if (hit.collider!=null && GetOppositeLocations().ContainsKey(hit.collider.gameObject.name))
				hitFound = true;

		}

		return hitFound;


	}

	public string GetOppositeTurn(){
		if (playerSide == p1Char)
			return p2Char;
		else if (playerSide == p2Char)
			return p1Char;

		return "";
	}

	public void ItemDeath(string space, SpriteRenderer sr){
		GetOppositeLocations().Remove(space);
		sr.sprite = null;
	}

	public int SizeHP(string size){
		if (size == "large")
			return 3;
		else if (size == "medium")
			return 2;
		else if (size == "small")
			return 1;
		return 0;
	}


	public string UppercaseFirstChar(string s){
		return char.ToUpper(s[0]) + s.Substring(1);

	}

	public void CheckWin(){
		if (GetOppositeLocations().Count ==0){
			
			WinText.SetActive(true);
			WinText.GetComponentInChildren<Text>().text = UppercaseFirstChar(playerSide) + " Wins!";
			playerSide = "end";
		}
	}

	public void ResetVariables(){

		p1Locations.Clear();
		p2Locations.Clear();

		p1Char = "reimu";
		p2Char = "marisa";

		command = "Board Setup";
		playerSide = p1Char;
		TurnOnButtons();
		foreach (var b in buttons){
			b.GetComponentInChildren<SpriteRenderer>().sprite = null;
		}
		WinText.SetActive(false);
		ClearPlayerStatus(p1Status);
		ClearPlayerStatus(p2Status);

	}

}



