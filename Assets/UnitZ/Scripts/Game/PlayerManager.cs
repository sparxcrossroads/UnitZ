//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	[HideInInspector]
	public int characterBaseIndex;
	[HideInInspector]
	public CharacterSystem characterBase;
	public GUISkin skin;
	[HideInInspector]
	public bool isPlaying;
	[HideInInspector]
	public CharacterSystem playingCharacter;
	public float SaveInterval = 5;
	public bool SavePlayer = true;
	[HideInInspector]
	public bool ManualSpawn;
	[HideInInspector]
	public bool DisabledSpawn;
	private SpectreCamera Spectre;
	private float timeTemp = 0;
	private bool characterLoaded = false;
	private bool savePlayerTemp;
	private NetworkView networkViewer;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		savePlayerTemp = SavePlayer;
		if (!skin && UnitZ.styleManager)
			skin = UnitZ.styleManager.GetSkin (0);
		
	}
	
	public void Reset ()
	{
		ManualSpawn = false;
		DisabledSpawn = false;
		SavePlayer = savePlayerTemp;
	}
	
	void Update ()
	{
		if (UnitZ.gameManager == null || !UnitZ.gameManager.IsPlaying)
			return;
		
		OnPlaying ();
	}
	
	private void findPlayerCharacter ()
	{
		CharacterSystem[] characters = (CharacterSystem[])GameObject.FindObjectsOfType (typeof(CharacterSystem));
		for (int i=0; i<characters.Length; i++) {
			if (characters [i].IsMine) {
				playingCharacter = characters [i];
				return;	
			}
		}
	}

	public void RemovePlayerCharacter (string id)
	{
		if (Network.isServer) {
			CharacterSystem[] characters = (CharacterSystem[])GameObject.FindObjectsOfType (typeof(CharacterSystem));
			for (int i=0; i<characters.Length; i++) {
				if (characters [i].ID == id) {
					NetworkView networkviewer = characters [i].GetComponent<NetworkView> ();
					if (networkviewer) {
						Network.RemoveRPCs (networkviewer.viewID);
						Network.Destroy (characters [i].gameObject);
					}
				}
			}
		}
	}
	
	public void OnPlaying ()
	{
		if (playingCharacter) {
			characterLoaded = true;
			if (UnitZ.playerSave) {
				if (Time.time >= timeTemp + SaveInterval) {
					timeTemp = Time.time;
					if (SavePlayer)
						UnitZ.playerSave.SavePlayer (playingCharacter);
				}
			}
		} else {
			findPlayerCharacter ();
		}
		
		if (Spectre != null) {
			if (playingCharacter == null) {
				Spectre.Active (true);
			} else {
				Spectre.Active (false);	
				Spectre.LookingAt (playingCharacter.gameObject.transform.position);
				playingCharacter.spectreThis = true;
			}
		} else {
			Spectre = (SpectreCamera)GameObject.FindObjectOfType (typeof(SpectreCamera));	
		}
	}
	
	public void RequestSpawnWithTeam (string team)
	{
		
		Debug.Log ("Request Spawn with team :" + team);
		UnitZ.gameManager.Team = team;
		if (Network.isClient) {
			if (networkViewer) {
				networkViewer.RPC ("RequestSpawn", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
			}
		} else {
			InstantiatePlayerObject (UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
		}
	}

	public void RequestSpawnWithTeamSpawner (string team, int spawner)
	{
		Debug.Log ("Request spawn with team spawner :" + team + " at " + spawner);
		UnitZ.gameManager.Team = team;
		if (Network.isClient) {
			if (networkViewer) {
				networkViewer.RPC ("RequestSpawn", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, spawner);
			}
		} else {
			InstantiatePlayerObject (UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, spawner);
		}
	}
	
	public void RequestSpawnPlayer ()
	{
		if (Network.isClient) {
			if (networkViewer) {
				networkViewer.RPC ("RequestSpawn", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
			}
		} else {
			InstantiatePlayerObject (UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
		}
	}
	
	bool InstantiatePlayerObject (string playerID, string userID, string userName, int index, string team, int spawner)
	{
		if (UnitZ.characterManager == null || UnitZ.characterManager.CharacterPresets.Length < index || index < 0)
			return false;
		
		characterBase = UnitZ.characterManager.CharacterPresets [index].CharacterPrefab;
		PlayerSpawner[] spawnPoint = (PlayerSpawner[])GameObject.FindObjectsOfType (typeof(PlayerSpawner));
		
		if (spawner < 0 || spawner >= spawnPoint.Length) {
			spawner = Random.Range (0, spawnPoint.Length);
		}
		
		if (characterBase && spawnPoint.Length > 0) {
			CharacterSystem character = spawnPoint [spawner].Spawn (characterBase.gameObject).GetComponent<CharacterSystem> ();
			character.Team = team;
			character.UserID = userID;
			character.UserName = userName;
			character.ReceivePlayerID (playerID);
			
			if (UnitZ.playerSave != null && character != null) {
				string haskey = userID + "_" + Application.loadedLevelName + "_" + userName;
				UnitZ.playerSave.InstantiateCharacter (character, haskey);
			}
			
			if (playerID == UnitZ.gameManager.PlayerID) {
				playingCharacter = character;
			}
			characterLoaded = false;
			MouseLock.MouseLocked = true;
			return true;
		}
		return false;
	}
	
	[RPC]
	public void RequestSpawn (string playerID, string userID, string userName, int index, string team, int spawner, NetworkMessageInfo info)
	{
		if (InstantiatePlayerObject (playerID, userID, userName, index, team, spawner)) {
			if (networkViewer) {
				networkViewer.RPC ("spawnedCallback", info.sender);
			}	
		}
	}
	
	[RPC]
	void spawnedCallback ()
	{
		Debug.Log ("your character has been spawn on a server side");
		findPlayerCharacter ();
	}
	
	void OnGUI ()
	{
		if (skin)
			GUI.skin = skin;
		
		if (DisabledSpawn)
			return;
		
		if (UnitZ.gameManager && UnitZ.gameManager.IsPlaying && characterLoaded) {
			if (playingCharacter == null) {
				MouseLock.MouseLocked = false;
				if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2, 200, 40), "Re Spawn")) {
					RequestSpawnPlayer ();
				}
			}
		}
	}

}
