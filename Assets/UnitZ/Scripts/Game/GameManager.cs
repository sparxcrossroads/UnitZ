//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(ChatLog))]
[RequireComponent(typeof(GameServer))]
[RequireComponent(typeof(GameClient))]

public class GameManager : MonoBehaviour
{

	public string UserName = "";
	public string Team = "";
	public string UserID = "";
	[HideInInspector]
	public HostData[] gameList;
	[HideInInspector]
	public bool OfflineMode;
	[HideInInspector]
	public string PlayingLevel;
	[HideInInspector]
	public string CurrentLevel;
	[HideInInspector]
	public bool IsRefreshing = false;
	[HideInInspector]
	public bool IsPlaying = false;
	[HideInInspector]
	public int lastLevelPrefix;
	[HideInInspector]
	public string PlayerID = "";
	private NetworkView networkViewer;
	private static GameManager gameManager;
	
	void Awake ()
	{
		DontDestroyOnLoad (this.gameObject);
		networkViewer = this.GetComponent<NetworkView> ();
		if (networkViewer)
			networkViewer.group = 1;
	}
	
	void Start ()
	{
		PlayerPrefs.SetString ("landingpage", Application.loadedLevelName);
		UserName = PlayerPrefs.GetString ("user_name");
		gameList = null;
	}
	
	void Update ()
	{
		if (IsRefreshing) {
			if (MasterServer.PollHostList ().Length > 0) {
				IsRefreshing = false;
				gameList = MasterServer.PollHostList ();
				Debug.Log ("Get data from master server " + gameList.Length);
			}
		}
		
		CurrentLevel = Application.loadedLevelName;
	}
		
	public void CreateGame (string startlevel, bool multiplayer)
	{
		// Create game.
		PlayerID = "0";
		IsPlaying = false;
		PlayingLevel = startlevel;
		OfflineMode = !multiplayer;	
		RestartGame ();
		
		if (multiplayer) {
			if (UnitZ.gameServer) {
				UnitZ.gameServer.StartServer ();
			}
		}
		
	}

	public void StartGame (string level)
	{
		if (OfflineMode || Network.isServer) {
			StartSinglePlayerGame (level);
		} else {
			StartMultiplayerGame ();
		}
	}

	public void StartMultiplayerGame ()
	{
		if (!Network.isServer && !OfflineMode) {
			UnitZ.gameClient.AttemptConnectToServer ();
			IsPlaying = false;
		}
		PlayerPrefs.SetString ("user_name", UserName);
	}
	
	public void StartSinglePlayerGame (string level)
	{
		StartLoadLevel (level, lastLevelPrefix+1);
		if(UnitZ.playersManager != null)
			UnitZ.playersManager.UpdatePlayerInfo ("0", 0, 0, UserName, Team, UnitZ.GameKeyVersion, true);
		
		PlayerID = "0";
		IsPlaying = true;
		PlayerPrefs.SetString ("user_name", UserName);
	}
	
	public void RestartGame ()
	{
		if (UnitZ.playersManager != null) {
			UnitZ.playersManager.ClearPlayers ();
			UnitZ.playersManager.AddPlayer ("0");
		}
		if (UnitZ.playerManager != null)
			UnitZ.playerManager.Reset ();
	}
	
	public void QuitGame ()
	{
		if (UnitZ.playersManager != null) {
			UnitZ.playersManager.ClearPlayers ();
		}
		
		if (UnitZ.chatLog != null)
			UnitZ.chatLog.Clear ();
		
		if (!Network.isClient && !Network.isServer) {
			ClearNetworkGameObject();
			if (Application.loadedLevelName != PlayerPrefs.GetString ("landingpage")) {
				Application.LoadLevel (PlayerPrefs.GetString ("landingpage"));
				GameObject.Destroy (this.gameObject);
			}
		
		} else {
			if (Network.isServer) {
				UnitZ.gameServer.KillServer ();	
			}
			if (Network.isClient) {
				UnitZ.gameClient.Disconnect ();
			}
		}
	}
	
	public void ConnectingDeny ()
	{
		Network.Disconnect ();	
	}

	public void ClearNetworkGameObject ()
	{
		Debug.Log("Clear all object");
		foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject))) {
			if (go.GetComponent<NetworkView> () && go.gameObject != this.gameObject) {
				GameObject.Destroy (go.gameObject);
			} 
		}
		if (UnitZ.playerManager != null)
			UnitZ.playerManager.Reset ();
	}

	public void Refresh ()
	{
		MasterServer.RequestHostList (UnitZ.gameServer.ServerName);
		gameList = null;
		IsRefreshing = true;
	}

	void OnGUI ()
	{
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		string gametype = "Local game";
		
		if (UnitZ.gameServer && !UnitZ.gameServer.LanOnly)
			gametype = "Online";
		
		if (Network.isServer) {
			if (networkViewer)
				GUI.Label (new Rect (0, 0, 800, 100), IsPlaying + " - " + gametype + "  Server : " + UnitZ.gameServer.IPServer + " (" + (Network.connections.Length + 1) + ") Players : ID is :" + networkViewer.owner.ToString () + " Team : " + Team);
		} else {
			if (Network.isClient)
				GUI.Label (new Rect (0, 0, 800, 30), IsPlaying + " - " + gametype + "  Client : Your ID is :" + PlayerID + " Team : " + Team);
		}
	}

	void OnLevelWasLoaded ()
	{
		Debug.Log (Application.loadedLevelName + " was loaded");
		
		if (IsPlaying) {
			if (Application.loadedLevelName == PlayingLevel) {
				
				if (UnitZ.playerManager != null) {
					if (!UnitZ.playerManager.ManualSpawn)
						UnitZ.playerManager.RequestSpawnPlayer();
				}
				
				if (Network.isServer) {
					UnitZ.gameServer.isActive = true;	
				}
				
				if (UnitZ.sceneSave != null) {
					UnitZ.sceneSave.LevelLoaded ();
				}
				
				PlayerPrefs.SetString ("StartScene", PlayingLevel);
			}
		}
	}

	public void StartLoadLevel (string level, int levelPrefix)
	{
		PlayingLevel = level;
		if (Network.isServer) {
			Debug.Log ("Server Load level " + level);
			StartCoroutine (SceneLoadLevel (PlayingLevel, levelPrefix));
		}
		
		if (Network.isClient) {
			Debug.Log ("Client Load level " + level);
			StartCoroutine (SceneLoadLevel (PlayingLevel, levelPrefix));
		}
		
		if (!Network.isServer && !Network.isClient) {
			Debug.Log ("Single player Load level " + PlayingLevel);
			Application.LoadLevel (PlayingLevel);
		}
	}

	[RPC]
	public IEnumerator SceneLoadLevel (string level, int levelPrefix)
	{
		if (Network.isServer || Network.isClient) {
			
			if(Network.isServer){
				Network.RemoveRPCsInGroup (0);
				Network.RemoveRPCsInGroup (1);
			}
			
			Debug.Log ("Network : Loading " + level);
		
			lastLevelPrefix = levelPrefix;
		
			Network.SetSendingEnabled (0, false);    
			Network.isMessageQueueRunning = false;
			Network.SetLevelPrefix (lastLevelPrefix);
		
			Application.LoadLevel (level);
		
			yield return null;
			yield return null;

			Network.isMessageQueueRunning = true;
			Network.SetSendingEnabled (0, true); 
			
			GameObject[] gameObjects = (GameObject[])GameObject.FindObjectsOfType (typeof(GameObject));
			foreach (var go in gameObjects)
				go.SendMessage ("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
		}

	}
}
