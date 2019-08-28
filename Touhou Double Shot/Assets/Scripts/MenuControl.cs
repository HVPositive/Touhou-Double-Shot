using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour {

	
	public List<GameObject> allMenus;
	public GameObject mainMenu;
	public GameObject playSettings;


	private void Start(){
		ReturnToMainMenu();
	}

	public void ReturnToMainMenu(){
		ShowMenu(mainMenu);
		Debug.Log("test?");
	}

	public void OpenGameSetup(){
		ShowMenu(playSettings);
		Debug.Log("test2");
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
