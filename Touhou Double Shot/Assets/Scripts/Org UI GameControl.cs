using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using UnityEngine.UI;


public class OrgGameControl : MonoBehaviour {

	public List<Button> buttons;
	//public Text[] buttonList;
	public Text currentPlayerText;
	public Text currentCommandText;
	public Text lastActionText;
	public Transform p1Status;
	public Transform p2Status;
	public Transform highlight; // the transform of the highlighter
	public Transform attackHighlight;
	public GameObject commandButtons;

	public GameObject WinText;

	private int maxHealth;
	private string playerSide;
	private bool battleStart;
	private Dictionary<string,Item> p1Locations;
	private Dictionary<string,Item> p2Locations;

	private string p1Char;
	private string p2Char;

	//private string command;

	private Color regular;
	private Color fade;

	public bool computerPlayer;
	private bool computerDelay;


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

		//computerPlayer = true;
		computerDelay = false;

	}

	private void Update(){
		//Handle Player Turn Text
		
		if (computerPlayer)
			ComputerPlayer();


	}

	public string GetPlayerSide(){
		return playerSide;
	}


	public void SwapPlayerSide(){
		if (playerSide == "end"){
			TurnOffButtons();

		} else if (playerSide == p1Char){

			ClearSprites();
			playerSide = p2Char;
			RestoreSprites();

			if (battleStart) TurnOffButtons(GetPlayerButtons(p2Locations));
		}
		else if (playerSide == p2Char){

			ClearSprites();
			playerSide = p1Char;
			RestoreSprites();

			if (battleStart) TurnOffButtons(GetPlayerButtons(p1Locations));
		}
		DeactivateHighlight();
		SetPlayerTurnText();
	}


	public bool BattleState(){
		return battleStart;
	}

	public void StartBattle(){

		ToggleBattle();

		SetCommandText("");
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
	//return true - if it is a free space
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


	public void SetAttackCommand(){
		SetCommandText("attack");
		//TurnOnButtons();
		TurnOffButtons(GetAttackButtons(highlightedButton));

	}
	public void SetMoveCommand(){
		SetCommandText("move");
		TurnOffButtons(GetMoveButtons(highlightedButton));
	}

	public void BackCommand(){
		DeactivateHighlight();
		TurnOffButtons(GetPlayerButtons(GetLocations()));
		SetCommandText("");
	}


	public void SetCommandText(string c){
		currentCommandText.text = UppercaseFirstChar(c);
	}

	public string GetCommand(){
		return currentCommandText.text;
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
			currentItem.GetComponent<SpriteRenderer>().color = regular;
			
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

			if (!excluded.Contains(button) || (playerSide == p2Char && computerPlayer))
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

			string size = GetLocations()[highlightedName].GetSize();
		//remove old space from dictionary
			RemoveLocation(highlightedName);
		
		//remove the sprite
			highlightedButton.GetComponentInChildren<SpriteRenderer>().sprite = null;

			//Hides attack highlight
			HideAttackHighlight();

			SetLastActionText(" moved their " + size + " item.");
			SwapPlayerSide();
			SetCommandText("");

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


	//Given a space and its sprite renderer, add a new item and set the sprite for the current player. Used in board setup
	public bool SetSprite(string space, SpriteRenderer sr){
		string size = GetNextSize();
		//string charName = GetCharName(charNum);
		
		if (CheckLocation(space)){
			AddLocation(space, new Item(size));

			//don't set sprites for p2 if computer player is true
			if (playerSide == p1Char || !computerPlayer )
				sr.sprite = Resources.Load<Sprite>(playerSide + "/" + size);
			
			return true;
		} else {
			Debug.Log("Space already occupied.");
			return false;
		}

	}

	//Given a space and its sprite renderer, add the given item and set the given sprite depending on the item. Used for moving items
	public bool SetSprite(string space, SpriteRenderer sr, Item item){

		//string charName = GetCharName(charNum);
		
		//gameControl.AddLocation(character + " " + size, this.name);
		
		//if (gameControl.CheckLocation(this.name, charNum))
		if (CheckLocation(space)){
			AddLocation(space, item);

			//don't set sprites for p2 if computer player is true - Should not matter since computer is not currently set to move its items
			if (playerSide == p1Char || !computerPlayer)
				sr.sprite = Resources.Load<Sprite>(playerSide + "/" + item.GetSize());

			return true;
		} else {

			Debug.Log("Space already occupied.");
			return false;
		}



	}


	public void CheckHit(string space, Vector3 pos, SpriteRenderer sr){

		string action = " attacked and ";


		if (GetOppositeLocations().ContainsKey(space) ){
			
			string size = GetOppositeLocations()[space].GetSize();

			action = action + "hit a " + size + " item.";

			if (GetOppositeLocations()[space].HitItem() == 0)
				ItemDeath(space,sr);
				
				//we want to remove it from the dictionary, change opacity in, and remove from board
			//UpdateStatusWindows(); (Dictionary<string,Item> locs, Transform playerStatus, string size
			UpdateSingleStatus(GetOppositeLocations(),OppositeStatus(), size );
			CheckWin();

		} else if (CheckGraze(pos).Count != 0){
			action = action + "grazed " + ParseGraze(CheckGraze(pos));
		} else{
			action = action + "missed.";
		}

		SetLastActionText(action);
		SwapPlayerSide();
		SetCommandText("");

		MoveAttackHighlight(pos);

	} 	


	public List<string> CheckGraze(Vector3 pos){
		List<string> hitsFound = new List<string>();

		RaycastHit2D hit;
		Vector2[] grazeVectors = new Vector2[] { Vector2.up, new Vector2(1,1), Vector2.right,
		 										 new Vector2(1,-1), Vector2.down, new Vector2(-1,-1),
		 										 Vector2.left, new Vector2(-1,1)};

		for (int i =0; i < grazeVectors.Length; i++){
			hit = Physics2D.Raycast(pos, grazeVectors[i]);

			if (hit.collider!=null && GetOppositeLocations().ContainsKey(hit.collider.gameObject.name)){
				hitsFound.Add(GetOppositeLocations()[hit.collider.gameObject.name].GetSize());

			}

		}

		return hitsFound;


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
		//only remove the space if you have not occupied it
		if (!CheckLocation(space))
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
		if (s == "")
			return "";
		else if (s.Length == 1)
			return char.ToUpper(s[0]) + "";
		else
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
		
		HideAttackHighlight();
		p1Locations.Clear();
		p2Locations.Clear();

		p1Char = "reimu";
		p2Char = "marisa";
		SetLastActionText();
		SetCommandText("Board Setup");
		playerSide = p1Char;
		TurnOnButtons();
		foreach (var b in buttons){
			b.GetComponentInChildren<SpriteRenderer>().sprite = null;
		}
		WinText.SetActive(false);
		ClearPlayerStatus(p1Status);
		ClearPlayerStatus(p2Status);
		SetPlayerTurnText();

	}

	public void ComputerPlayer(){

		//set board
		if (playerSide == p2Char && !battleStart){
			//Random rand  = new Random();

			int randomNumber = Random.Range(1,buttons.Count+1);

			//if open space is not found, randomize again
			while (!CheckLocation("Grid Space " + randomNumber.ToString() )) {
				randomNumber = Random.Range(1,buttons.Count+1);
			}

			buttons[(randomNumber-1)].GetComponent<GridSpace>().SetSpace();


			//computer player's turn and has not highlighted a space	
		} else if (playerSide == p2Char && !CheckHighlight() && !computerDelay){

			computerDelay = true;

			StartCoroutine(computerWait());
			//Highlight a space


			
		}


	}

	IEnumerator computerWait(){
		yield return new WaitForSeconds(5);
		buttons[GridSpaceToIndex(RandomItem())].GetComponent<GridSpace>().ButtonClick();


		//Set to attack mode
		SetAttackCommand();

		//Attack a possible space
		//buttons[GridSpaceToIndex(RandomItem( GetAttackButtons(highlightedButton) ) )]
		RandomItem(GetAttackButtons(highlightedButton)).GetComponent<GridSpace>().ButtonClick();
		computerDelay = false;

	}

	//Returns a random button in current player's item locations
	public string RandomItem(){
		List<string> locs = new List<string>(GetLocations().Keys);

		return locs[Random.Range(0,locs.Count)];
	}

	//Returns a random button in current player's item locations
	public Button RandomItem(List<Button> spaces){
		//List<string> locs = new List<string>(spaces.Values);
		int n = Random.Range(0,spaces.Count);

		return spaces[n];
	}

	//Converts a gridspace name to an index
	public int GridSpaceToIndex(string n){
		return int.Parse( n.Replace("Grid Space ", ""))-1;

	}

	public void MoveAttackHighlight(Vector3 pos){
		attackHighlight.gameObject.SetActive(true);
		attackHighlight.position = pos;
		//highlight.position=pos;
	}

	public void HideAttackHighlight(){
		attackHighlight.gameObject.SetActive(false);
	}

	public void SetPlayerTurnText(){
		currentPlayerText.text = UppercaseFirstChar(playerSide);
	}

	public void SetLastActionText(string action){
		lastActionText.text = UppercaseFirstChar(playerSide) + action;
	}

	public void SetLastActionText(){
		lastActionText.text = "";
	}

	public string ParseGraze(List<string> g){
		//string text = "";

		//Debug.Log(temp.Count);
		if (g.Count == 1)
			return "a " + g[0] + " item.";
		else if (g.Count == 2)
			return g[0] + " and " + g[1] + " items.";
		else if (g.Count == 3)
			return g[0] + ", " + g[1] + ", and " + g[2] + " items.";
		else 
			return "";
	}

	public void ClearSprites(){
		foreach (var button in buttons){
				button.GetComponentInChildren<SpriteRenderer>().sprite = null;
		}

	}

	public void RestoreSprites(){
		foreach (var button in buttons){
			string currentButton = button.gameObject.name;
			if (!CheckLocation(currentButton)){


				if (playerSide == p1Char || !computerPlayer)
					button.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(playerSide + "/" + GetLocations()[currentButton].GetSize());

			}
		}

	}

}



