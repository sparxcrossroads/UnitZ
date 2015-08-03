//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {

	public int CharacterIndex = 0;
	public CharacterPreset[] CharacterPresets;
	private CharacterSaveData characterCreate;
	public CharacterSaveData SelectedCharacter;
	
	void Start () {
	
	}
	
	void Update () {
	
	}
	
	public bool CreateCharacter (string characterName)
	{
		if (UnitZ.gameManager) {
			if (characterName!="") {
				if (SaveNewCharacter (characterName)) {
					UnitZ.gameManager.UserName = characterName;
					return true;
				}
			}
		}
		return false;
	}
	
	public void SetCharacter ()
	{
		CharacterIndex = SelectedCharacter.CharacterIndex;
		
		if (CharacterPresets.Length > 0) {
			
			if (CharacterIndex >= CharacterPresets.Length) 
				CharacterIndex = CharacterPresets.Length - 1;
			
			if (CharacterIndex < 0) 
				CharacterIndex = 0;
			
			if (UnitZ.playerManager) {
				UnitZ.playerManager.characterBase = CharacterPresets [CharacterIndex].CharacterPrefab;
			}
			
			if (UnitZ.gameManager)
				UnitZ.gameManager.UserName = SelectedCharacter.PlayerName;
		}
	}
	
	public void SetupCharacter (CharacterSaveData character)
	{
		SelectedCharacter = character;
		CharacterIndex = SelectedCharacter.CharacterIndex;
		
		if (CharacterPresets.Length > 0) {
			
			if (CharacterIndex >= CharacterPresets.Length) 
				CharacterIndex = CharacterPresets.Length - 1;
			
			if (CharacterIndex < 0) 
				CharacterIndex = 0;
			
			if (UnitZ.playerManager) {
				UnitZ.playerManager.characterBase = CharacterPresets [CharacterIndex].CharacterPrefab;
			}
			
			if (UnitZ.gameManager)
				UnitZ.gameManager.UserName = SelectedCharacter.PlayerName;
		}
	}

	public void SelectCreateCharacter (int index)
	{
		characterCreate = new CharacterSaveData ();
		characterCreate.CharacterIndex = index;
	}

	public bool SaveNewCharacter (string characterName)
	{
		if (characterName != "" && UnitZ.gameManager && UnitZ.playerSave) {
			UnitZ.gameManager.UserName = characterName;
			characterCreate.PlayerName = UnitZ.gameManager.UserName;
			if (UnitZ.playerSave.CreateCharacter (characterCreate)) {
				return true;
			}	
		}
		return false;
	}
	
	public void RemoveCharacter (CharacterSaveData character)
	{
		if (UnitZ.playerSave) {
			UnitZ.playerSave.RemoveCharacter (character);
		}
	}
}
