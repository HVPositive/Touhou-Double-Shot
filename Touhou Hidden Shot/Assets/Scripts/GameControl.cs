using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour {

	//List of the grid space buttons
	public List<Button> buttons;

	//Location of the command buttons for highlighting
	public List<Transform> commandLocations;

	//Game object containing all of the command buttons
	public GameObject commandButtons;

	//Status displays for each player
	public Transform p1Status;
	public Transform p2Status;

	//The highlight
	public Transform highlight; //item selection highlight
	public Transform attackHighlight; //highlight to show the location of the last space attacked
	public Transform commandHighlight; //hightlight to show the selected command
	

	//Game object containing log text
	public GameObject lastActionText;

	//Game object containing win text
	public GameObject WinText;

	//The button to display the last command/log
	public GameObject logButton;

	//Max health for items
	private const int maxHealth = 3;

	//Current player's turn
	private string playerSide;

	//if the battle has started
	private bool battleStart;

	//Dictionary containing the location and item for both players
	private Dictionary<string,Item> p1Locations;
	private Dictionary<string,Item> p2Locations;

	//The character names of each player
	private string p1Char;
	private string p2Char;

	//The current command selected
	private string command;

	//Colors for status 
	private Color regular;
	private Color fade; //Used for an item that is destroyed

	//If player 2 should take their turn automatically
	private bool computerPlayer;

	//Turned on if computer is taking their turn - prevents computer from doing their turn multiple times during delay
	private bool computerStop;

	//How long the computer waits to finish its turn
	private const int computerDelay = 5;

	//Ensures playerSide is not swapped to the other player too early
	private bool turnEnded;

	//Keeps track of the button that is highlighted
	private Button highlightedButton;

	private void Start(){

		battleStart = false;
		p1Locations = new Dictionary<string,Item>();
		p2Locations = new Dictionary<string,Item>();

		ResetVariables();

		regular = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		fade = new Color(1.0f, 1.0f, 1.0f, 0.4f);

		computerStop = false;

	}

	private void Update(){
		//function for computer to take actions automatically
		if (computerPlayer)
			ComputerPlayer();

	}

	//Game start/restart/exit functions

	//Reset/reload the game variables
	private void ResetVariables(){
		
		HideAttackHighlight();
		p1Locations.Clear();
		p2Locations.Clear();

		//Load variables from game settings menu or load default values
		if (MenuControl.p1Char == "" || MenuControl.p1Char == null )
			p1Char = "reimu";
		else
			p1Char = MenuControl.p1Char;
		
		if (MenuControl.p2Char == "" || MenuControl.p2Char == null )
			p2Char = "marisa";
		else
			p2Char = MenuControl.p2Char;

		computerPlayer = MenuControl.computerPlayer;

		SetLastActionText();
		HideLastActionText();
		SetCommand("Board Setup");
		playerSide = p1Char;
		TurnOnButtons();
		ClearSprites();
		WinText.SetActive(false);
		ClearPlayerStatus(p1Status);
		ClearPlayerStatus(p2Status);

		ResetLog();
		turnEnded = false;
		DectivateCommandButtons();

	}

	private void ReturnToMenu(){
		SceneManager.LoadScene("Start Menu", LoadSceneMode.Single);
	}


	private void RestartBoard(){
		ChangeBattleState(false);
		ResetVariables();
	}


	//Turn handling functions

	//Handles player side changing
	public void SwapPlayerSide(){

		//The game has ended
		if (playerSide == "end"){
			TurnOffButtons();

		} else if (playerSide == p1Char){

			ClearSprites();
			playerSide = p2Char;
			RestoreSprites(); //Restore sprites for the swapped in player

			//Only highlight the player's items
			if (battleStart) TurnOffButtons(GetPlayerButtons(p2Locations));
		}
		else if (playerSide == p2Char){

			ClearSprites();
			playerSide = p1Char;
			RestoreSprites();

			if (battleStart) TurnOffButtons(GetPlayerButtons(p1Locations));
		}
		

	}

	private void EndTurn(){
		turnEnded = true;
		TurnOffButtons();
		DeactivateHighlight();
		SetCommand("");
		ClearCommandHighlight();

	}

	public bool BattleState(){
		return battleStart;
	}

	//Sprite functions 

	//Clear all sprites
	private void ClearSprites(){
		foreach (var button in buttons){
				button.GetComponentInChildren<SpriteRenderer>().sprite = null;
		}

	}
	//Restore the sprites for the current player (unless they are a computer player)
	private void RestoreSprites(){
		foreach (var button in buttons){
			string currentButton = button.gameObject.name;
			if (!CheckLocation(currentButton)){

				if (playerSide == p1Char || !computerPlayer)
					button.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + playerSide + "/" + GetLocations()[currentButton].GetSize());

			}
		}

	}

	//Given a space and its sprite renderer, add a new item and set the sprite for the current player. Used in board setup
	public bool SetSprite(string space, SpriteRenderer sr){
		string size = GetNextSize();
		//string charName = GetCharName(charNum);
		
		if (CheckLocation(space)){
			AddLocation(space, new Item(size));

			//don't set sprites for p2 if computer player is true
			if (playerSide == p1Char || !computerPlayer )
				sr.sprite = Resources.Load<Sprite>("characters/" + playerSide + "/" + size);
			
			return true;
		} else {

			return false;
		}

	}

	//Given a space and its sprite renderer, add the given item and set the given sprite depending on the item. Used for moving items
	private bool SetSprite(string space, SpriteRenderer sr, Item item){
		
		//if (gameControl.CheckLocation(this.name, charNum))
		if (CheckLocation(space)){
			AddLocation(space, item);

			//don't set sprites for p2 if computer player is true - Should not matter since computer is not currently set to move its items
			if (playerSide == p1Char || !computerPlayer)
				sr.sprite = Resources.Load<Sprite>("characters/" + playerSide + "/" + item.GetSize());

			return true;
		} else {
			return false;
		}



	}

	//Board/battle states

	//Change from board setup to battle
	public void StartBattle(){

		ChangeBattleState(true);

		SetCommand("");
		ClearCommandHighlight();
		SetUpStatus();
		//Do it for first turn
		TurnOffButtons(GetPlayerButtons(p1Locations));
		logButton.SetActive(true);
	}

	private void ChangeBattleState(bool state){
		battleStart = state;
	}


	//Player location functions - These check for the current player's locations unless specified otherwise (usually by opposite)

	//Checks if the given space is free
	private bool CheckLocation(string gridspace){
		return !(GetLocations().ContainsKey(gridspace));

	}

	private void AddLocation(string gridspace, Item newI){
			GetLocations().Add(gridspace, newI);
	}

	private void RemoveLocation(string gridspace){
		GetLocations().Remove(gridspace);
	}

	private Dictionary<string,Item> GetLocations(){
		if (playerSide == p1Char)
			return p1Locations;
		else if (playerSide == p2Char)
			return p2Locations;
		else 
			return new Dictionary<string,Item>(); //empty list
	}

	private Dictionary<string,Item> GetOppositeLocations(){
		if (playerSide == p1Char)
			return p2Locations;
		else if (playerSide == p2Char)
			return p1Locations;
		else 
			return new Dictionary<string,Item>(); //empty list
	}	

	//Given a size and dictionary, return the item with that size
	private Item GetItem(string sz, Dictionary<string,Item> locs){
		foreach(var key in locs.Keys){
			if (locs[key].GetSize() == sz)
				return locs[key];
		}
		return null;
	}	

	//Command functions

	private void SetAttackCommand(){
		SetCommand("attack");
		MoveCommandHighlight();
		TurnOffButtons(GetAttackButtons(highlightedButton));

	}
	private void SetMoveCommand(){
		SetCommand("move");
		MoveCommandHighlight();
		TurnOffButtons(GetMoveButtons(highlightedButton));
	}

	private void BackCommand(){
		DeactivateHighlight();
		TurnOffButtons(GetPlayerButtons(GetLocations()));
		SetCommand("");
		ClearCommandHighlight();
	}


	private void SetCommand(string c){
		command = UppercaseFirstChar(c);
	}

	public string GetCommand(){
		return command;
	}

	//Player status functions

	private void SetUpStatus(){
		SetPlayerStatus(p1Char, p1Status);
		SetPlayerStatus(p2Char, p2Status);
	}

	private void SetPlayerStatus(string charName, Transform playerStatus){
		Transform currentItem;

		//Set up each item
		for (int i =0; i< playerStatus.childCount; i++){
			currentItem = playerStatus.GetChild(i);
			currentItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + charName + "/" + currentItem.name);
			currentItem.GetComponent<SpriteRenderer>().color = regular;
			//Sets health display
			for (int j =0; j< (maxHealth-i); j++){
				 currentItem.GetChild(j).gameObject.SetActive(true);
			}

		}
	}


	private Transform OppositeStatus(){
		if (playerSide == p1Char)
			return p2Status;
		else if (playerSide == p2Char)
			return p1Status;
		else 
			return null; //empty list		
	}

	private void ClearPlayerStatus(Transform playerStatus){
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

	//Update and single status box given the player, their locations and a size
	private void UpdateSingleStatus(Dictionary<string,Item> locs, Transform playerStatus, string size){
		Transform currentItem = playerStatus.Find(size);
		int itemHP;

		//Gets the item hp of the item
		if (GetItem(currentItem.gameObject.name, locs) != null)
			itemHP = GetItem(currentItem.gameObject.name, locs).GetHealth();
		else
			itemHP = 0;

		//If item is destroyed, fade it out
		if (itemHP == 0)
			currentItem.GetComponent<SpriteRenderer>().color = fade;


		//int i = maxHealth-SizeHP(size);
		//Update item hp starts
		for (int j =0; j< SizeHP(size); j++){

			if (j< itemHP)
				currentItem.GetChild(j).gameObject.SetActive(true);
			else
				currentItem.GetChild(j).gameObject.SetActive(false);
		}

		
	}

	//Returns the max HP given a size
	private int SizeHP(string size){
		if (size == "large")
			return 3;
		else if (size == "medium")
			return 2;
		else if (size == "small")
			return 1;
		return 0;
	}


	//Highlight functions

	//Item selected highlight
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
	private void DeactivateHighlight(){
		highlight.gameObject.SetActive(false);
		DectivateCommandButtons();
	}	

	//Command highlight
	private void MoveCommandHighlight(){
		commandHighlight.gameObject.SetActive(true);
		commandHighlight.position = GetCommandLocation();
	}

	private void ClearCommandHighlight(){
		commandHighlight.gameObject.SetActive(false);
	}

	private Vector3 GetCommandLocation(){
		foreach (var x in commandLocations){
			if (x.gameObject.name == command)
				return x.position;
		}
		return new Vector3(0.0f, 0.0f, 0.0f);
	}

	//Attack highlight
	private void MoveAttackHighlight(Vector3 pos){
		attackHighlight.gameObject.SetActive(true);
		attackHighlight.position = pos;
		//highlight.position=pos;
	}

	private void HideAttackHighlight(){
		attackHighlight.gameObject.SetActive(false);
	}	


	//Command buttons functions
	private void ActivateCommandButtons(){
		commandButtons.SetActive(true);
	}

	private void DectivateCommandButtons(){
		commandButtons.SetActive(false);
	}

	//Grid space button functions

	private void TurnOnButtons(){
		foreach (var button in buttons){
			button.interactable = true;
		}
	}

	private void TurnOffButtons(List<Button> excluded){
		//Turn on all buttons before turning off all buttons
		TurnOnButtons();
		foreach (var button in buttons){

			if (!excluded.Contains(button) || (playerSide == p2Char && computerPlayer))
				button.interactable = false;
		}

	}

	private void TurnOffButtons(){

		foreach (var button in buttons){
				button.interactable = false;
		}

	}

	//Returns a list of buttons the players items
	private List<Button> GetPlayerButtons(Dictionary<string,Item> locs){
		List<Button> playerButtons = new List<Button>();
		foreach (var button in buttons){
			if (locs.ContainsKey(button.name))
				playerButtons.Add(button);
				
		}
		return playerButtons;
	}

	//return list of buttons around another button
	private List<Button> GetAttackButtons(Button buttonLoc){
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

	private List<Button> GetMoveButtons(Button buttonLoc){
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

	//Player action functions
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


	public void MoveItem(string newSpace, SpriteRenderer sr){
		string highlightedName = highlightedButton.gameObject.name;
		//Copy highlighted item to newspace

		//Check if moving to that space is possible (if one of your spaces are already on that spot)
		if (SetSprite(newSpace, sr,GetLocations()[highlightedName]) ){

			string size = GetLocations()[highlightedName].GetSize();
		//remove old space from dictionary
			RemoveLocation(highlightedName);
		
		//remove the sprite
			highlightedButton.GetComponentInChildren<SpriteRenderer>().sprite = null;

			//Hides attack highlight
			HideAttackHighlight();

			SetLastActionText(" moved their " + size + " item.");
			EndTurn();

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
		EndTurn();
		MoveAttackHighlight(pos);

	} 	


	private List<string> CheckGraze(Vector3 pos){
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

	private void ItemDeath(string space, SpriteRenderer sr){
		GetOppositeLocations().Remove(space);
		//only remove the space if you have not occupied it
		if (!CheckLocation(space))
			sr.sprite = null;
	}

	private void CheckWin(){
		if (GetOppositeLocations().Count ==0){
			
			WinText.SetActive(true);
			WinText.GetComponentInChildren<Text>().text = UppercaseFirstChar(playerSide) + " Wins!";
			playerSide = "end";
			computerPlayer = false; //Ensures the computer does not take any actions at this point
		}
	}

	private string GetOppositeTurn(){
		if (playerSide == p1Char)
			return p2Char;
		else if (playerSide == p2Char)
			return p1Char;

		return "";
	}



	//Computer player functions


	private void ComputerPlayer(){

		//set board
		if (playerSide == p2Char && !battleStart){
			//Random rand  = new Random();

			int randomNumber = Random.Range(1,buttons.Count+1);

			//if open space is not found, randomize again
			while (!CheckLocation("Grid Space " + randomNumber.ToString() )) {
				randomNumber = Random.Range(1,buttons.Count+1);
			}

			buttons[(randomNumber-1)].GetComponent<GridSpace>().ButtonClick();


			//computer player's turn and has not highlighted a space	
		} else if (playerSide == p2Char && !CheckHighlight() && !computerStop){

			computerStop = true;

			StartCoroutine(computerWait());

		}


	}

	private IEnumerator computerWait(){
		yield return new WaitForSeconds(computerDelay);
		//Choose random item
		buttons[GridSpaceToIndex(RandomItem())].GetComponent<GridSpace>().ButtonClick();


		//Set to attack mode
		SetAttackCommand();

		//Attack a possible space
		RandomItem(GetAttackButtons(highlightedButton)).GetComponent<GridSpace>().ButtonClick();
		//computerStop = false;

	}

	//Returns a random button in current player's item locations
	private string RandomItem(){
		List<string> locs = new List<string>(GetLocations().Keys);

		return locs[Random.Range(0,locs.Count)];
	}

	//Returns a random button given a list of buttons
	private Button RandomItem(List<Button> spaces){
		//List<string> locs = new List<string>(spaces.Values);
		int n = Random.Range(0,spaces.Count);

		return spaces[n];
	}

	//Converts a gridspace name to an index - random item returns the name of the button so the index from that name is parsed here
	private int GridSpaceToIndex(string n){
		return int.Parse( n.Replace("Grid Space ", ""))-1;

	}

	//Log functions

	private void SetLastActionText(string action){
		lastActionText.gameObject.SetActive(true);
		lastActionText.GetComponentInChildren<Text>().text = UppercaseFirstChar(playerSide) + action;
	}

	private void SetLastActionText(){
		lastActionText.GetComponentInChildren<Text>().text = "";
	}

	private void HideLastActionText(){
		lastActionText.gameObject.SetActive(false);
		if (turnEnded){
			SwapPlayerSide();
			turnEnded = false;
			computerStop = false;
		}

	}

	private void ShowLastAction(){
		lastActionText.gameObject.SetActive(true);
	}

	private string ParseGraze(List<string> g){
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


	private void ResetLog(){
		lastActionText.GetComponentInChildren<Text>().text = "No previous action.";
		logButton.SetActive(false);
	}


	private string UppercaseFirstChar(string s){
		if (s == "")
			return "";
		else if (s.Length == 1)
			return char.ToUpper(s[0]) + "";
		else
			return char.ToUpper(s[0]) + s.Substring(1);

	}

}





	// public void UpdatePlayerStatus(Dictionary<string,Item> locs, Transform playerStatus){
	// 	Transform currentItem;
	// 	int itemHP;


	// 	//Set up each item
	// 	for (int i =0; i< playerStatus.childCount; i++){
	// 		currentItem = playerStatus.GetChild(i);
	// 		if (GetItem(currentItem.gameObject.name, locs) != null)
	// 			itemHP = GetItem(currentItem.gameObject.name, locs).GetHealth();
	// 		else
	// 			itemHP = 0;

	// 		if (itemHP == 0 )
	// 			currentItem.GetComponent<SpriteRenderer>().color = fade;
	// 		//maxHealth -i = the hp for that size
	// 		//hp  = ^ - 
	// 		//Sets health display

	// 		//itemhp = 3 2 1
	// 		for (int j =0; j< (maxHealth - i); j++){

	// 			if (j< itemHP)
	// 				currentItem.GetChild(j).gameObject.SetActive(true);
	// 			else
	// 				currentItem.GetChild(j).gameObject.SetActive(false);
	// 		}

	// 	}
	// }



	// //used to check if two buttons are adjacent. Not used but kept for now
	// public bool CheckAdjacent(Button b1, Button b2){

	// 	//Gets angle between buttons and casts ray from the first button in previous angle. Then if the hit object is same as second button, then they are adjacent 
	// 	RaycastHit2D hit;
	// 	hit  = Physics2D.Raycast(b1.GetComponent<Transform>().position, b2.GetComponent<Transform>().position - b1.GetComponent<Transform>().position );

	// 	//Debug.Log( hit.collider.gameObject.name );
	// 	if (hit.collider!=null && (hit.collider.gameObject.name == b2.gameObject.name) )
	// 		return true;
	// 	else
	// 		return false;



	// }