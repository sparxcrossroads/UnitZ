//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour
{
	public float VersionCheckDelay = 5;
	public List<PlayerData> PlayerList = new List<PlayerData> ();
	public List<string> TeamList = new List<string> ();
	private NetworkView networkViewer;
	
	void Start ()
	{
		ClearPlayers ();
		networkViewer = this.GetComponent<NetworkView> ();
	}
	
	public void ClearPlayers ()
	{
		PlayerList.Clear ();
		TeamList.Clear ();
	}

	void Update ()
	{
		if (networkViewer == null)
			return;
		
		if (Network.isServer && UnitZ.gameServer.isActive) {
			foreach (PlayerData player in PlayerList) {
				if (player != null) {
					networkViewer.RPC ("UpdatePlayerInfo", RPCMode.Others, player.ID, player.Score, player.Dead, player.Name, player.Team, player.GameKey, player.IsConnected);
				}
			}
			
			UpdateMyInfo ("0", UnitZ.gameManager.UserName, UnitZ.gameManager.Team, true);

			foreach (PlayerData player in PlayerList) {
				if (player.ID != "0") {
					player.IsConnected = false;
				} else {
					player.IsConnected = true;
				}
			}
			
			foreach (NetworkPlayer connect in Network.connections) {
				foreach (PlayerData player in UnitZ.playersManager.PlayerList) {
					if (player.ID == connect.ToString () || player.ID == "0") {
						player.IsConnected = true;
						// checking client version, kick if not match 
						if (player.GameKey != UnitZ.GameKeyVersion) {
							Network.CloseConnection (connect, true);	
							player.IsConnected = false;
						}
					}
				}
			}
		}
	}
	
	[RPC]
	public void AddPlayer (string id)
	{
		PlayerData play = new PlayerData ();
		play.Dead = 0;
		play.ID = id;
		play.Name = "";
		play.Team = "";
		play.IsConnected = true;
		PlayerList.Add (play);
	}
	
	private void AddTeam (string team)
	{
		foreach (string tm in TeamList) {
			if (team == tm)
				return;
		}
		TeamList.Add (team);
	}
	
	[RPC]
	public void UpdateMyInfo (string id, string name, string team, bool isconnected)
	{
		foreach (PlayerData pp in PlayerList) {
			if (id == pp.ID) {
				pp.Name = name;
				pp.Team = team;
				pp.IsConnected = isconnected;
				AddTeam (team);
				return;
			}
		}
	}

	[RPC]
	public void UpdatePlayerInfo (string id, int score, int dead, string name, string team, string gameKey, bool isconnected)
	{
		bool have = false;
		foreach (PlayerData pp in PlayerList) {
			if (pp.ID == id) {
				have = true;
			}
		}
		if (!have) {
			AddPlayer (id);
		}
		foreach (PlayerData pp in PlayerList) {
			if (id == pp.ID) {
				pp.Score = score;
				pp.Dead = dead;
				pp.Name = name;
				pp.Team = team;
				pp.GameKey = gameKey;
				pp.ConnectedTime = Time.time;
				pp.IsConnected = isconnected;
				AddTeam (team);
				return;
			}
		}
	}
}

public class PlayerData
{
	public string ID;
	public int Score;
	public string Team;
	public int Dead;
	public string Name;
	public bool IsConnected;
	public string GameKey = "";
	public float ConnectedTime;
	public float Delay = 0;
}

