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
	private bool computerDelay;

	//Ensures playerSide is not swapped to the other player too early
	private bool turnEnded;

	//Keeps track of the button that is highlighted
	private Button highlightedButton;

	private void Start(){

		
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
	//Reset/reload the game variables
	public void ResetVariables(){
		
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
		foreach (var b in buttons){
			b.GetComponentInChildren<SpriteRenderer>().sprite = null;
		}
		WinText.SetActive(false);
		ClearPlayerStatus(p1Status);
		ClearPlayerStatus(p2Status);

		ResetLog();
		turnEnded = false;
		DectivateCommandButtons();

	}

	private void Update(){
		//function for computer to take actions automatically
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
		

	}


	public bool BattleState(){
		return battleStart;
	}

	public void StartBattle(){

		ToggleBattle();

		SetCommand("");
		ClearCommandHighlight();
		SetUpStatus();
		//Do it for first turn
		TurnOffButtons(GetPlayerButtons(p1Locations));
		logButton.SetActive(true);
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
		SetCommand("attack");
		//TurnOnButtons();
		MoveCommandHighlight();
		TurnOffButtons(GetAttackButtons(highlightedButton));

	}
	public void SetMoveCommand(){
		SetCommand("move");
		MoveCommandHighlight();
		TurnOffButtons(GetMoveButtons(highlightedButton));
	}

	public void BackCommand(){
		DeactivateHighlight();
		TurnOffButtons(GetPlayerButtons(GetLocations()));
		SetCommand("");
		ClearCommandHighlight();
	}


	public void SetCommand(string c){
		command = UppercaseFirstChar(c);
	}

	public string GetCommand(){
		return command;
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
			currentItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + charName + "/" + currentItem.name);
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

	public void MoveCommandHighlight(){
		commandHighlight.gameObject.SetActive(true);
		commandHighlight.position = GetCommandLocation();
	}

	public void ClearCommandHighlight(){
		commandHighlight.gameObject.SetActive(false);
	}

	public Vector3 GetCommandLocation(){
		foreach (var x in commandLocations){
			if (x.gameObject.name == command)
				return x.position;
		}
		return new Vector3(0.0f, 0.0f, 0.0f);
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
				sr.sprite = Resources.Load<Sprite>("characters/" + playerSide + "/" + size);
			
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
				sr.sprite = Resources.Load<Sprite>("characters/" + playerSide + "/" + item.GetSize());

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
		EndTurn();
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
			computerPlayer = false; //Ensures the computer does not take any actions at this point
		}
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

			buttons[(randomNumber-1)].GetComponent<GridSpace>().ButtonClick();


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
		//computerDelay = false;

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


	public void SetLastActionText(string action){
		lastActionText.gameObject.SetActive(true);
		lastActionText.GetComponentInChildren<Text>().text = UppercaseFirstChar(playerSide) + action;
	}

	public void SetLastActionText(){
		lastActionText.GetComponentInChildren<Text>().text = "";
	}

	public void HideLastActionText(){
		lastActionText.gameObject.SetActive(false);
		if (turnEnded){
			SwapPlayerSide();
			turnEnded = false;
			computerDelay = false;
		}

	}

	public void ShowLastAction(){
		lastActionText.gameObject.SetActive(true);
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
					button.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + playerSide + "/" + GetLocations()[currentButton].GetSize());

			}
		}

	}

	public void ResetLog(){
		lastActionText.GetComponentInChildren<Text>().text = "No previous action.";
		logButton.SetActive(false);
	}

	public void EndTurn(){
		turnEnded = true;
		TurnOffButtons();
		DeactivateHighlight();

		//SwapPlayerSide();
		SetCommand("");
		ClearCommandHighlight();

	}

	public void ReturnToMenu(){
		SceneManager.LoadScene("Start Menu", LoadSceneMode.Single);
	}

}



