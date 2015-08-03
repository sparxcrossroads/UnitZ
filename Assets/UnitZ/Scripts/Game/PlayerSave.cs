//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerSave : MonoBehaviour
{

	public List<PlayerSaveData> LoadedData = new List<PlayerSaveData> ();
	[HideInInspector]
	public CharacterSystem MainCharacter;
	[HideInInspector]
	public NetworkView networkViewer;
	
	void Start ()
	{
		//PlayerPrefs.DeleteAll();
		networkViewer = this.GetComponent<NetworkView> ();
		if (UnitZ.gameManager == null)
			return;
		

		if (UnitZ.gameManager.UserID == "") {
			Debug.Log ("UID is not assigned");
			UnitZ.gameManager.UserID = PlayerPrefs.GetString ("UID");
			if (UnitZ.gameManager.UserID == "") {
				UnitZ.gameManager.UserID = GetUniqueID ();
				PlayerPrefs.SetString ("UID", UnitZ.gameManager.UserID);
				Debug.Log ("UID is generated " + UnitZ.gameManager.UserID);
			} else {
				Debug.Log ("UID is " + UnitZ.gameManager.UserID);
			}
		}
	}

	public void DeleteSave ()
	{
		if (UnitZ.gameManager == null)
			return;
		
		DeleteSave(UnitZ.gameManager.UserID,UnitZ.gameManager.UserName);
	}
	
	public void DeleteSave (string userID,string userName)
	{
		if (userID == "")
			return;
		
		PlayersRegister (userID);
		string hasKey = userID;
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave.UID = hasKey;
		playersave.PlayerName = userName;
		playersave.ItemData = "";
		playersave.EquipData = "";
		playersave.FPSItemIndex = 0;
		playersave.Position = "";
		playersave.LevelName = "";
		playersave.Food = 0;
		playersave.Water = 0;
		playersave.Health = 0;
		
		if ((!Network.isServer && !Network.isClient) || Network.isServer) {
			WriteData (playersave);
		} else {
			if (networkViewer)
				networkViewer.RPC ("SaveToServer", RPCMode.Server, PlayerSaveDataText (playersave));
		}
	}

	public void SavePlayer (CharacterSystem character)
	{
		if (UnitZ.gameManager == null || character == null)
			return;
		
		PlayersRegister (UnitZ.gameManager.UserID);
		string hasKey = UnitZ.gameManager.UserID;
		PlayerSaveData playersave = new PlayerSaveData ();
			
		playersave.UID = hasKey;
		playersave.PlayerName = UnitZ.gameManager.UserName;
		playersave.ItemData = character.inventory.GetItemDataText ();
		playersave.EquipData = character.inventory.GenStickerTextData ();
		playersave.FPSItemIndex = character.inventory.GetCollectorFPSindex ();
		playersave.Position = character.transform.position.x + "," + character.transform.position.y + "," + character.transform.position.z;
		playersave.LevelName = Application.loadedLevelName;
		playersave.Health = character.HP;
		
		CharacterLiving living = character.GetComponent<CharacterLiving> ();
		if (living) {
			playersave.Food = living.Hungry;
			playersave.Water = living.Water;
		}
		
		if ((!Network.isServer && !Network.isClient) || Network.isServer) {
			WriteData (playersave);
		} else {
			if (networkViewer)
				networkViewer.RPC ("SaveToServer", RPCMode.Server, PlayerSaveDataText (playersave));
		}
	}
	
	public void LoadPlayer (CharacterSystem character)
	{
		if (UnitZ.gameManager == null || character == null)
			return;
		
		MainCharacter = character;
		string hasKey = UnitZ.gameManager.UserID + "_" + Application.loadedLevelName + "_" + UnitZ.gameManager.UserName;
		//Debug.Log("LOAD : HAS KEY "+hasKey);
		if ((!Network.isServer && !Network.isClient) || Network.isServer) {
			PlayerSaveData playersave = new PlayerSaveData ();
			playersave = ReadData (hasKey);
			ApplyPlayerData (playersave);
		} else {
			if (networkViewer)
				networkViewer.RPC ("RequestLoadFromServer", RPCMode.Server, hasKey);
		}
	}
	
	public void InstantiateCharacter (CharacterSystem character, string hasKey)
	{
		Debug.Log ("Instantiat Character save");
		if (character == null)
			return;
		
		MainCharacter = character;
		if ((!Network.isServer && !Network.isClient) || Network.isServer) {
			PlayerSaveData playersave = new PlayerSaveData ();
			playersave = ReadData (hasKey);
			
			// Apply data to host
			ApplyPlayerData (playersave);
			
			// apply data to clients
			if (Network.isServer) {
				if (MainCharacter.networkViewer)
					MainCharacter.networkViewer.RPC ("ApplyData", RPCMode.Others, PlayerSaveDataText (playersave));
			}
		}
		
		MainCharacter = null;
	}
	
	public void WriteData (PlayerSaveData playersave)
	{
		string hasKey = playersave.UID + "_" + Application.loadedLevelName + "_" + playersave.PlayerName;
		//Debug.Log("SAVE : HAS KEY "+hasKey);
		PlayerPrefs.SetString ("PLAYER_" + hasKey, playersave.UID);
		PlayerPrefs.SetString ("NAME_" + hasKey, playersave.PlayerName);
		PlayerPrefs.SetString ("ITEM_" + hasKey, playersave.ItemData);
		PlayerPrefs.SetString ("EQUIP_" + hasKey, playersave.EquipData);
		PlayerPrefs.SetInt ("FPSINDEX" + hasKey, playersave.FPSItemIndex);
		PlayerPrefs.SetString ("POS" + hasKey, playersave.Position);
		PlayerPrefs.SetString ("LEVELNAME" + hasKey, playersave.LevelName);
		PlayerPrefs.SetInt ("FOOD" + hasKey, playersave.Food);
		PlayerPrefs.SetInt ("WATER" + hasKey, playersave.Water);
		PlayerPrefs.SetInt ("HEALTH" + hasKey, playersave.Health);
		//Debug.Log ("Write Data " + playersave.UID);
		//Debug.Log (PlayerSaveDataText (playersave));
	}
	
	public PlayerSaveData ReadData (string hasKey)
	{
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave.UID = PlayerPrefs.GetString ("PLAYER_" + hasKey);
		playersave.PlayerName = PlayerPrefs.GetString ("NAME_" + hasKey);
		playersave.ItemData = PlayerPrefs.GetString ("ITEM_" + hasKey);
		playersave.EquipData = PlayerPrefs.GetString ("EQUIP_" + hasKey);
		playersave.FPSItemIndex = PlayerPrefs.GetInt ("FPSINDEX" + hasKey);
		playersave.Position = PlayerPrefs.GetString ("POS" + hasKey);
		playersave.LevelName = PlayerPrefs.GetString ("LEVELNAME" + hasKey);
		playersave.Food = PlayerPrefs.GetInt ("FOOD" + hasKey);
		playersave.Water = PlayerPrefs.GetInt ("WATER" + hasKey);
		playersave.Health = PlayerPrefs.GetInt ("HEALTH" + hasKey);
		return playersave;
	}
	
	public string PlayerSaveDataText (PlayerSaveData playersave)
	{
		return playersave.UID + 
			"^" + playersave.PlayerName + 
			"^" + playersave.ItemData + 
			"^" + playersave.EquipData + 
			"^" + playersave.FPSItemIndex +
			"^" + playersave.Position + 
			"^" + playersave.LevelName +
			"^" + playersave.Food +
			"^" + playersave.Water +
			"^" + playersave.Health;	
	}
	
	public PlayerSaveData GetSaveDataFromText (string dataText)
	{
		string[] raw = dataText.Split ("^" [0]);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave.UID = raw [0];
		playersave.PlayerName = raw [1];
		playersave.ItemData = raw [2];
		playersave.EquipData = raw [3];
		int.TryParse (raw [4], out playersave.FPSItemIndex);
		playersave.Position = raw [5];
		playersave.LevelName = raw [6];
		int.TryParse (raw [7], out playersave.Food);
		int.TryParse (raw [8], out playersave.Water);
		int.TryParse (raw [9], out playersave.Health);
		
		return playersave;
	}
	
	void ApplyPlayerData (PlayerSaveData playersave)
	{
		ApplyPlayerData (playersave, MainCharacter);
	}
	
	void ApplyPlayerData (PlayerSaveData playersave, CharacterSystem character)
	{
		if (character && character.inventory) {
			//Debug.Log ("Applying ");
			//Debug.Log ("with " + PlayerSaveDataText (playersave));
		
			character.inventory.SetItemsFromText (playersave.ItemData);
			character.inventory.UpdateOtherInventory (playersave.EquipData);
			if (character.inventory.Items.Count > playersave.FPSItemIndex)
				character.inventory.EquipItemByCollector (character.inventory.Items [playersave.FPSItemIndex]);
			string[] positionraw = playersave.Position.Split ("," [0]);
			if (positionraw.Length > 2) {
				Vector3 position = Vector3.zero;
				float.TryParse (positionraw [0], out position.x);
				float.TryParse (positionraw [1], out position.y);
				float.TryParse (positionraw [2], out position.z);
				character.transform.position = position;
			}

			character.HP = playersave.Health;
			CharacterLiving living = character.GetComponent<CharacterLiving> ();
			if (living) {
				living.Hungry = playersave.Food;
				living.Water = playersave.Water;
			}
			// if new save, all parameter must be set by default.
			if (playersave.Food <= 0 && playersave.Water <= 0 && playersave.Health <= 0) {
				character.HP = character.HPmax;
				if (living) {
					living.Hungry = living.HungryMax;
					living.Water = living.WaterMax;	
				}
			}
			
			character.InitializeData ();
		}
	}
	
	[RPC]
	public void SaveToServer (string dataText)
	{
		//Debug.Log ("Server received : " + dataText);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = GetSaveDataFromText (dataText);
		WriteData (playersave);
	}
	
	[RPC]
	public void ReceiveFromServer (string datatext)
	{
		//Debug.Log ("Received From Server " + datatext);
		
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = GetSaveDataFromText (datatext);

		ApplyPlayerData (playersave);
		
	}
	
	public void ReceiveDataAndApply (string datatext, CharacterSystem character)
	{
		//Debug.Log ("Received From Server " + datatext);
		
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = GetSaveDataFromText (datatext);

		ApplyPlayerData (playersave, character);
		
	}

	[RPC]
	public void RequestLoadFromServer (string hasKey, NetworkMessageInfo info)
	{
		//Debug.Log ("Request Loading " + hasKey);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = ReadData (hasKey);
		if (networkViewer)
			networkViewer.RPC ("ReceiveFromServer", info.sender, PlayerSaveDataText (playersave));
	}
	
	public bool PlayersRegister (string uid)
	{
		string allplayer = PlayerPrefs.GetString ("PLAYERS");
		//Debug.Log ("Load all UID " + allplayer);
		string[] playerlist = allplayer.Split ("," [0]);
		for (int i=0; i<playerlist.Length; i++) {
			if (playerlist [i] != "" && uid == playerlist [i]) {
				//Debug.Log (uid + ": is alrady exists");
				return false;	
			}
		}
		allplayer += uid + ",";
		PlayerPrefs.SetString ("PLAYERS", allplayer);
		
		//Debug.Log ("Registered " + allplayer);
		
		return true;
	}
	
	void Update ()
	{
	
	}
	
	public string GetUniqueID ()
	{
		var random = new System.Random ();   
		DateTime epochStart = new System.DateTime (1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		string uniqueID = String.Format ("{0:X}", Convert.ToInt32 (timestamp))
                 + "-" + String.Format ("{0:X}", Convert.ToInt32 (Time.time * 1000000))
                 + "-" + String.Format ("{0:X}", random.Next (1000000000));
         
		//Debug.Log ("Generated Unique ID: " + uniqueID);
		return uniqueID;
	}
	
	public bool SaveCharacter (CharacterSaveData character)
	{
		string hasKey = character.PlayerName;
		//Debug.Log ("Save Character " + hasKey + " " + PlayerPrefs.HasKey ("CHARACTER_" + hasKey));
		if (!PlayerPrefs.HasKey ("CHARACTER_" + hasKey)) {
			PlayerPrefs.SetString ("CHARACTER_" + hasKey, character.PlayerName);
			PlayerPrefs.SetInt ("CharacterIndex_" + hasKey, character.CharacterIndex);
			return true;
		}
		
		return false;
	}
	
	public bool CreateCharacter (CharacterSaveData character)
	{
		string charactersList = PlayerPrefs.GetString ("CHARACTERS");
		
		if (SaveCharacter (character)) {
			charactersList += "," + character.PlayerName;
			PlayerPrefs.SetString ("CHARACTERS", charactersList);
			//Debug.Log (" character list " + charactersList);
			return true;
		}
		return false;
	}
	
	public void RemoveCharacter (CharacterSaveData character)
	{
		string[] charactersList = PlayerPrefs.GetString ("CHARACTERS").Split ("," [0]);
		string characters = "";
		string hasKey = character.PlayerName;
		
		
		if (charactersList.Length > 0) {
			for (int i=0; i<charactersList.Length; i++) {
				if (charactersList [i] != "" && charactersList [i] != hasKey) {
					characters += "," + charactersList [i];
				}
			}
			PlayerPrefs.SetString ("CHARACTERS", characters);
			PlayerPrefs.DeleteKey ("CHARACTER_" + hasKey);
			PlayerPrefs.DeleteKey ("CharacterIndex_" + hasKey);
			
			if(UnitZ.gameManager)
			DeleteSave(UnitZ.gameManager.UserID,character.PlayerName);
		}
	}
	
	public CharacterSaveData LoadCharacter (string name)
	{
		//Debug.Log ("Load " + name);
		CharacterSaveData character = new CharacterSaveData ();
		string hasKey = name;
		
		if (PlayerPrefs.HasKey ("CHARACTER_" + hasKey)) {
			character.PlayerName = PlayerPrefs.GetString ("CHARACTER_" + hasKey);
			character.CharacterIndex = PlayerPrefs.GetInt ("CharacterIndex_" + hasKey);
		}
		return character;
	}
	
	public CharacterSaveData[] LoadAllCharacters ()
	{
		string[] charactersList = PlayerPrefs.GetString ("CHARACTERS").Split ("," [0]);
		List<CharacterSaveData> chars = new List<CharacterSaveData> ();
		
		if (charactersList.Length > 0) {
			for (int i=0; i<charactersList.Length; i++) {
				if (charactersList [i] != "") {
					CharacterSaveData ch = LoadCharacter (charactersList [i]);
					if (ch.PlayerName != "") {
						chars.Add (ch);
					}
				}
			}
			
			CharacterSaveData[] allCharacters = chars.ToArray ();
			
			return allCharacters;
		}
		return null;
	}
	
}

public struct PlayerSaveData
{
	public string UID;
	public string ItemData;
	public string EquipData;
	public int FPSItemIndex;
	public string PlayerName;
	public string Position;
	public string LevelName;
	public int Food;
	public int Water;
	public int Health;
}

public struct CharacterSaveData
{
	public string PlayerName;
	public int CharacterIndex;
}

[System.Serializable]
public class CharacterPreset
{
	public CharacterSystem CharacterPrefab;
	public Texture2D Icon;
}
