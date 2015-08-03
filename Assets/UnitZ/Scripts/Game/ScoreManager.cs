//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
	public bool DrawGUI = true;
	public GUISkin skin;
	public bool Toggle;
	public NetworkView networkViewer;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		StyleManager Styles = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		
		if (!skin && Styles)
			skin = Styles.GetSkin (0);
		
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (UnitZ.gameManager && UnitZ.gameManager.IsPlaying)
				Toggle = !Toggle;
		}
	}
	
	[RPC]
	public void UpdatePlayerScore (string id, int score, int dead)
	{
		foreach (PlayerData pp in UnitZ.playersManager.PlayerList) {
			if (id == pp.ID) {
				pp.Score += score;
				pp.Dead += dead;
				return;
			}
		}
	}
	
	public void AddScore (int score, string id)
	{
		if (Network.isServer || Network.isClient) {
			if (Network.isServer) {
				UpdatePlayerScore (id, score, 0);
			} else {
				if (networkViewer)
					networkViewer.RPC ("UpdatePlayerScore", RPCMode.Server, id, score, 0);
			}
		} else {
			UpdatePlayerScore (id, score, 0);	
		}

	}
	
	public void AddDead (int dead, string id)
	{
		if (Network.isServer || Network.isClient) {
			if (Network.isServer) {
				UpdatePlayerScore (id, 0, dead);
			} else {
				if (networkViewer)
					networkViewer.RPC ("UpdatePlayerScore", RPCMode.Server, id, 0, dead);
			}
		} else {
			UpdatePlayerScore (id, 0, dead);
		}
	}
	
	private Vector2 scrollPosition;
	private int playerCount = 0;
	
	void OnGUI ()
	{
		if (!Toggle || !DrawGUI)
			return;
		
		if (skin)
			GUI.skin = skin;
		
		Vector2 size = new Vector2 (450, 350);
		
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.skin.label.fontSize = 35;

		GUI.Box (new Rect ((Screen.width / 2 - size.x / 2) - 10, (Screen.height / 2 - size.y / 2) - 10, size.x + 20, size.y + 20), "");
		GUI.BeginGroup (new Rect (Screen.width / 2 - size.x / 2, Screen.height / 2 - size.y / 2, size.x, size.y), "");
		GUI.skin.label.fontSize = 19;
			
		
		GUI.Label (new Rect (20, 0, 200, 30), "Player Name");
		GUI.Label (new Rect (250, 0, 100, 30), "Kills");	
		GUI.Label (new Rect (360, 0, 100, 30), "Death");
		
		scrollPosition = GUI.BeginScrollView (new Rect (0, 0, size.x, size.y), scrollPosition, new Rect (0, 0, size.x - 20, playerCount * 35));
		playerCount = 1;
		
		foreach (string tm in UnitZ.playersManager.TeamList) {
			if (tm != "") {
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.Label (new Rect (20, playerCount * 35, 200, 30), "TEAM " + tm);
				playerCount++;
			}
			foreach (PlayerData player in UnitZ.playersManager.PlayerList) {
				if (player.IsConnected && tm == player.Team) {
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUI.Label (new Rect (20, playerCount * 35, 200, 30), "<color=lime>" + player.Name.ToString () + "</color>");
					GUI.skin.label.alignment = TextAnchor.MiddleCenter;
					GUI.Label (new Rect (250, playerCount * 35, 50, 30), player.Score.ToString ());	
					GUI.Label (new Rect (360, playerCount * 35, 30, 30), player.Dead.ToString ());
					playerCount++;
				}
			}
		}

		GUI.EndScrollView ();
		GUI.EndGroup ();
	}
}
