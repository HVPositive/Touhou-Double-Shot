using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {

	//Menu screens
	public List<GameObject> allMenus;
	public GameObject mainMenu;
	public GameObject playSettings;
	public GameObject characterSelect;
	public GameObject tutorial;


	//computer toggle in game settings
	public Toggle computerToggle;

	//start button in game settings
	public Button startButton;

	//Game setting variables
	static public string p1Char;
	static public string p2Char;
	static public bool computerPlayer = true;

	private string playerToSet;


	//List of character names
	private List<string> characterList;

	//Page tracking
	private int charPage;
	private int tutPage;

	//Tutorial Text
	private List<string>  tutorialText;

	//Constant
	private const int maxChars = 3; //Max number of characters per page in character select





	private void Start(){
		ReturnToMainMenu();

		//Reset variables
		characterList = new List<string>();
		charPage = 0;
		tutPage = 0;
		p1Char = "";
		p2Char = "";
		playerToSet = "";
		computerPlayer = computerToggle.isOn;

		//Load list of characters
 		characterList = new List<string>(Resources.Load<TextAsset>("characters/characterList").ToString().Split(new string[]{"\r\n"}, System.StringSplitOptions.None));

		//Load tutorial text
		tutorialText = new List<string>(Resources.Load<TextAsset>("tutorial/tutorial").ToString().Split(new string[]{"||"}, System.StringSplitOptions.None));


	}

	//Shows the given menu and turns off all other menus
	private void ShowMenu(GameObject s){
		foreach (var m in allMenus)
			m.SetActive(false);
		s.SetActive(true);
	}

	// Menu Navigation  //

	private void ReturnToMainMenu(){
		ShowMenu(mainMenu);
	}

	private void OpenGameSetup(){
		ShowMenu(playSettings);

		//Update game setup according to current variables
		SetPlayerDisplay(playSettings.transform.Find("Player 1"), p1Char);
		SetPlayerDisplay(playSettings.transform.Find("Player 2"), p2Char);
		startButton.interactable = CheckStart();

	}

	private void OpenCharacterSelect(){

		//Reset character select variables
		charPage = 0;
		SetBackForwardButtons(characterSelect);

		//Get the tag of the clicked button to get which player is choosing their character
		playerToSet = EventSystem.current.currentSelectedGameObject.tag;

		ShowMenu(characterSelect);

 		LoadCharacterSelectDisplays();
 		
	}

	private void OpenTutorial(){
		tutPage = 0;
		ShowMenu(tutorial);
		SetTutorialText();
		SetBackForwardButtons(tutorial);
	}

	private void QuitGame(){
		Application.Quit();

	}

	//Game Setting functions

	private void SetPlayerDisplay(Transform player, string characterName){
		player.Find("Character Name").GetComponent<TextMeshProUGUI>().SetText(UppercaseFirstChar(characterName));
		SetPlayerSettingSprites(player.Find("Sprites"), characterName);
	}
	private void SetPlayerSettingSprites(Transform sprites, string character){

		if (character == ""){
			sprites.Find("Large").GetComponentInChildren<SpriteRenderer>().sprite = null;
	 		sprites.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = null;
	 		sprites.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = null;
	 	} else{
			sprites.Find("Large").GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + character + "/large" );
		 	sprites.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + character + "/medium" );
		 	sprites.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + character + "/small" );
		}

	}
	//Checks if variables are valid to start a game
	private bool CheckStart(){
		if (p1Char == "" || p2Char == "" || p1Char == p2Char)
			return false;
		else
			return true;  
	}

	private void UpdateComputer(){
		computerPlayer= computerToggle.isOn;
	}

	private void StartGame(){
		SceneManager.LoadScene("Main", LoadSceneMode.Single);
	}


	//Character select functions

	private void LoadCharacterSelectDisplays(){

		//Go through each character display and set them up
		for (int i = 0; i<maxChars; i++){

			Transform currentDisplay = characterSelect.GetComponent<Transform>().GetChild(i);
			int currentCharIndex = charPage*maxChars +i;

			//clear out display
			if (currentCharIndex >= characterList.Count){

	 			SetCharacterDisplays(currentDisplay);

	 		//fill display
 			} else{
				string currentName = characterList[currentCharIndex];
				SetCharacterDisplays(currentDisplay,currentName);

 			}

 		}
	}
	//Set button interactability and set display 
	private void SetCharacterDisplays(Transform characterDisplay, string n){

		//Checks if character has already been chosen to disable choosing it
		if ((playerToSet == "p1" && p2Char == n) || (playerToSet == "p2" && p1Char == n)) 
			characterDisplay.GetComponent<Button>().interactable = false;
		else
			characterDisplay.GetComponent<Button>().interactable = true;

		characterDisplay.GetComponentInChildren<TextMeshProUGUI>().text = UppercaseFirstChar(n); //SetText(UppercaseFirstChar(n) );
	 	characterDisplay.Find("Large Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/large" );
	 	characterDisplay.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/medium" );
	 	characterDisplay.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/small" );

	}

	//No string name given, turn off button and clear display
	private void SetCharacterDisplays(Transform characterDisplay){

		characterDisplay.GetComponent<Button>().interactable = false;
		characterDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";  //SetText("");
	 			
	 	characterDisplay.Find("Large Sprite").GetComponent<SpriteRenderer>().sprite = null;
	 	characterDisplay.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = null;
	 	characterDisplay.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = null;

	}

	//Go to next page of characters
	private void NextCharacterPage(){
		charPage+= 1;
		LoadCharacterSelectDisplays();
		SetBackForwardButtons(characterSelect);
	}

	//Go to previous page of characters
	private void PreviousCharacterPage(){
		charPage-= 1;
		LoadCharacterSelectDisplays();
		SetBackForwardButtons(characterSelect);
	}

	//Select and set character then return to game setup
	private void SelectCharacter(){

		SetCharName(EventSystem.current.currentSelectedGameObject.transform.Find("Character Name").GetComponent<TextMeshProUGUI>().text.ToLower());
		OpenGameSetup();


	}

	private void SetCharName(string n){
		if (playerToSet == "p1")
			p1Char = n;
		else if (playerToSet == "p2")
			p2Char = n;
	}



	//Tutorial Functions


	private void SetTutorialText(){

		tutorial.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(tutorialText[tutPage]);
	}

	private void NextTutorialPage(){
		tutPage+= 1;
		SetTutorialText();
		SetBackForwardButtons(tutorial);
	}

	private void PreviousTutorialPage(){
		tutPage-= 1;
		SetTutorialText();
		SetBackForwardButtons(tutorial);
	}




	//Given a menu and the current page, set the page navigation button
	private void SetBackForwardButtons(GameObject menu){
		Button forward = menu.transform.Find("Forward").GetComponent<Button>();
		Button back = menu.transform.Find("Backward").GetComponent<Button>();

		int page =0;
		int maxPage = 0;

		//set page variables depending on menu's tag
		if (menu.tag == "character select"){
			page = charPage;
			maxPage = (characterList.Count-1)/3; 
		} else if (menu.tag == "tutorial"){
			page = tutPage;
			maxPage = tutorialText.Count-1;
		}

		//Set button interactability
		if (page == 0)
			back.interactable = false;
		else
			back.interactable = true;

		if (page < maxPage)
			forward.interactable = true;
		else
			forward.interactable = false;
	}

	//Returns a string with the first letter capitalized
	private string UppercaseFirstChar(string s){
		if (s == "")
			return "";
		else if (s.Length == 1)
			return char.ToUpper(s[0]) + "";
		else
			return char.ToUpper(s[0]) + s.Substring(1);

	}
}
