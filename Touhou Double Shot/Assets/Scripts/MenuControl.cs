using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class MenuControl : MonoBehaviour {

	
	public List<GameObject> allMenus;
	public GameObject mainMenu;
	public GameObject playSettings;
	public GameObject characterSelect;

	private List<string> characterList;
	private int charPage;

	private void Start(){
		ReturnToMainMenu();

		characterList = new List<string>();
		charPage = 0;
	}

	public void ReturnToMainMenu(){
		ShowMenu(mainMenu);

	}

	public void OpenGameSetup(){
		ShowMenu(playSettings);

	}

	public void OpenCharacterSelect(){
		ShowMenu(characterSelect);
		characterList.Clear();
		charPage = 0;


		foreach (string file in System.IO.Directory.GetDirectories("Assets\\Resources\\characters")) { 
			characterList.Add(Path.GetFileName(file)); 
 		}

 		LoadCharacterDisplays();
 		SetBackForwardButtons();



	}

	public void LoadCharacterDisplays(){
		for (int i = 0; i<3; i++){
			//7
			//charpage*3 = 6
			//0 1 2
			Transform currentChar = characterSelect.GetComponent<Transform>().GetChild(i);

			if (charPage*3 +i >= characterList.Count){

	 			SetCharacterDisplays(currentChar);

 			} else{
				string currentName = characterList[charPage*3 + i];
				SetCharacterDisplays(currentChar,currentName);

 			}

 		}
	}

	public void SetCharacterDisplays(Transform characterDisplay, string n){

		characterDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(n);
	 			

	 	characterDisplay.Find("Large Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/large" );
	 			
	 	characterDisplay.Find("Medium Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/medium" );
	 	characterDisplay.Find("Small Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("characters/" + n + "/small" );

	}

	public void SetCharacterDisplays(Transform characterDisplay){

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
}
