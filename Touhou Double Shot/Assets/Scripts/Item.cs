using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Item{

	//current health count
	private int health;

	//size - large, medium, small
	private string size;

		//side/character it belongs to
	//private string side;
	//private string character;


	public Item(string sz){
		size = sz;
		//character = chara;
			
		if (size == "large")
			health = 3;
		else if (size == "medium")
			health = 2;
		else if (size == "small")
			health = 1;


	} 

	public int GetHealth(){
		return health;
	}

	public int HitItem(){
		if (health>0)
			health--;
		return health;
	}
	public string GetSize(){
		return size;
	}

}
