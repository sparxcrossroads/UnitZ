//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CharacterBadge : MonoBehaviour {

	public RawImage GUIImage;
	public Text GUIName;
	public Text GUIType;
	public CharacterSaveData CharacterData;

	public int Index;
	[HideInInspector]
	public CharacterCreatorCanvas CharacterCreatorS;

	
	void Start () {

	}
	
	public void Delete(){
		if(CharacterCreatorS)
			CharacterCreatorS.RemoveCharacter(Index);
	}
	
	public void PlayThisCharacter(){
		if(CharacterCreatorS && CharacterData.PlayerName!="")
			CharacterCreatorS.SelectCharacter(CharacterData);
	}


}
