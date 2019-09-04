using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {

	
	public List<GameObject> allMenus;
	public GameObject mainMenu;
	public GameObject playSettings;
	public GameObject characterSelect;
	public GameObject tutorial;
	public Toggle computerToggle;

	public Button startButton;

	private List<string> characterList;
	private int charPage;

	static public string p1Char;
	static public string p2Char;

	private string playerToSet;

	private GameObject selectedButton;
	static public bool computerPlayer = true;


	private void Start(){
		ReturnToMainMenu();

		characterList = new List<string>();
		charPage = 0;

		p1Char = "";
		p2Char = "";
		playerToSet = "";
		computerPlayer = computerToggle.isOn;



	}

	public void ReturnToMainMenu(){
		ShowMenu(mainMenu);

	}

	public void OpenGameSetup(){
		ShowMenu(playSettings);

		if (p1Char != ""){
			Transform player = playSettings.transform.GetChild(0);
			player.Find("Character Name").GetComponent<TextMeshProUGUI>().SetText(UppercaseFirstChar(p1Char));
			SetPlayerSettingSprites(player.Find("Sprites"), p1Char);

		}

		if (p2Char != ""){
			Transform player = playSettings.transform.GetChild(1);
			player.Find("Character Name").GetComponent<TextMeshProUGUI>().SetText(UppercaseFirstChar(p2Char));
			SetPlayerSettingSprites(player.Find("Sprites"), p2Char);

		}

		startButton.interactable = CheckStart();
		SetBackForwardButtons();


	}

	public void OpenCharacterSelect(){

		playerToSet = EventSystem.current.currentSelectedGameObject.tag;


		ShowMenu(characterSelect);
		characterList.Clear();
		charPage = 0;


		foreach (string file in System.IO.Directory.GetDirectories("Assets\\Resources\\characters")) { 
			characterList.Add(Path.GetFileName(file)); 
 		}

 		LoadCharacterDisplays();
 		



	}

	public void LoadCharacterDisplays(){
		for (int i = 0; i<3; i++){
			//7
			//charpage*3 = 6
			//0 1 2
			Transform currentDisplay = characterSelect.GetComponent<Transform>().GetChild(i);

			//clear out display
			if (charPage*3 +i >= characterList.Count){

	 			SetCharacterDisplays(currentDisplay);

	 		//fill display
 			} else{
				string currentName = characterList[charPage*3 + i];
				SetCharacterDisplays(currentDisplay,currentName);

 			}

 		}
	}

	public void SetCharacterDisplays(Transform characterDisplay, string n){


		if ( (playerToSet == "p1" && p2Char == n) || (playerToSet == "p2" && p1Char == n)  ) 
			characterDisplay.GetComponent<Button>().interactable = false;
		else
			characterDisplay.GetComponent<Button>().interactable = true;

		characterDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(UppercaseFirstChar(n) );
	 			

	 	characterDisplay.Find("Large Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/large" );
	 			
	 	characterDisplay.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/medium" );
	 	characterDisplay.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/small" );

	}

	public void SetCharacterDisplays(Transform characterDisplay){

		characterDisplay.GetComponent<Button>().interactable = false;
		characterDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText("");
	 			

	 	characterDisplay.Find("Large Sprite").GetComponent<SpriteRenderer>().sprite = null;
	 			
	 	characterDisplay.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = null;
	 	characterDisplay.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = null;

	}

	public void NextCharacterPage(){
		charPage+= 1;
		LoadCharacterDisplays();
		SetBackForwardButtons();
	}

	public void PreviousCharacterPage(){
		charPage-= 1;
		LoadCharacterDisplays();
		SetBackForwardButtons();
	}

	public void SetBackForwardButtons(){
		Button forward = characterSelect.transform.Find("Forward").GetComponent<Button>();
		Button back = characterSelect.transform.Find("Backward").GetComponent<Button>();

		if (charPage == 0)
			back.interactable = false;
		else
			back.interactable = true;

		if ( (charPage+1)*3 <= characterList.Count)
			forward.interactable = true;
		else
			forward.interactable = false;
	}


	public void QuitGame(){
		
		Debug.Log("Quit");
		Application.Quit();

	}

	public void ShowMenu(GameObject s){
		foreach (var m in allMenus)
			m.SetActive(false);

		s.SetActive(true);

	}

	public void SelectCharacter(){
		//Debug.Log(EventSystem.current.currentSelectedGameObject.transform.parent.parent.gameObject.name);

		SetCharName(EventSystem.current.currentSelectedGameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text);
		OpenGameSetup();
		//OpenGameSetup();

	}

	public void SetCharName(string name){
		if (playerToSet == "p1")
			p1Char = name;
		else if (playerToSet == "p2")
			p2Char = name;


	}

	public string UppercaseFirstChar(string s){
		if (s == "")
			return "";
		else if (s.Length == 1)
			return char.ToUpper(s[0]) + "";
		else
			return char.ToUpper(s[0]) + s.Substring(1);

	}

	public void SetPlayerSettingSprites(Transform sprites, string character){

		sprites.Find("Large").GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + character + "/large" );
	 			
	 	sprites.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + character + "/medium" );
	 	sprites.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + character + "/small" );

	}

	public void UpdateComputer(){
		computerPlayer= computerToggle.isOn;
	}

	public void StartGame(){
		SceneManager.LoadScene("Main", LoadSceneMode.Single);
	}

	public bool CheckStart(){
		if (p1Char == "" || p2Char == "" || p1Char == p2Char)
			return false;
		else
			return true;  
	}

	public void OpenTutorial(){
		ShowMenu(tutorial);
	}
}
