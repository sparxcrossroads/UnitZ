//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class CharactersBadgesRefresher : MonoBehaviour {

	public CharacterCreatorCanvas characterCreator;

	void Start () {
	
	}
	
	void OnEnable(){
		if(characterCreator)
			StartCoroutine (characterCreator.LoadCharacters());
	}

}
