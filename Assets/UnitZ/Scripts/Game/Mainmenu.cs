//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

/// <summary>
/// Mainmenu. Just Main menu GUI
/// </summary>
using UnityEngine;
using System.Collections;

public class Mainmenu : MonoBehaviour
{

	public Texture2D LogoGame;
	[HideInInspector]
	public int PageState = 0;
	public string SceneStart = "sandbox";
	[HideInInspector]
	public GameManager gameManage;
	[HideInInspector]
	public CharacterCreator characterCreate;
	public GUISkin skin;
	private float delta;
	private int pageTemp;
	
	void Start ()
	{
		delta = 1;	
		Application.targetFrameRate = 140;
		gameManage = (GameManager)GameObject.FindObjectOfType (typeof(GameManager));
		characterCreate = (CharacterCreator)GameObject.FindObjectOfType (typeof(CharacterCreator));
		StyleManager Styles = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		if (PlayerPrefs.GetString ("StartScene") != "") {
			SceneStart = PlayerPrefs.GetString ("StartScene");
		}
		if (!skin && Styles)
			skin = Styles.GetSkin (0);
		
		
	}
	
	Vector2 scrollPosition;

	void DrawGameLobby ()
	{
		if (gameManage) {
			if (!gameManage.IsRefreshing) {
				if (gameManage.gameList != null) {
					scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2 - 275, 50, 550, Screen.height - 200), scrollPosition, new Rect (0, 0, 500, 60 * gameManage.gameList.Length));

					for (int i=0; i<gameManage.gameList.Length; i++) {
						HostData hostgame = gameManage.gameList [i];
						string ips = "";
						foreach (string ip in hostgame.ip) {
							ips += ip;
						}
						if (GUI.Button (new Rect (0, i * 60, 550, 50), hostgame.gameName + " " + ips)) {
							UnitZ.gameClient.GameSelected (hostgame);
							characterCreate.OpenCharacter ();
							PageState = 1;
						}
					}
					GUI.EndScrollView ();
				} else {
					GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2, 200, 50), "No game Found");
				}
			}
		}
	}

	void OnGUI ()
	{
		Screen.lockCursor = false;
		
		if (skin)
			GUI.skin = skin;

		GUI.skin.button.fontSize = 17;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		
		if (SceneStart != "zombieland") {
			if (GUI.Button (new Rect (10, 10, 150, 30), "Zombie Land")) {
				SceneStart = "zombieland";
			}
		} else {
			GUI.Box (new Rect (10, 10, 150, 30), "Zombie Land");
		}
		
		if (SceneStart != "sandbox") {
			if (GUI.Button (new Rect (170, 10, 150, 30), "Sandbox")) {
				SceneStart = "sandbox";
			}
		} else {
			GUI.Box (new Rect (170, 10, 150, 30), "Sandbox");
		}
		
		if (SceneStart != "training") {
			if (GUI.Button (new Rect (330, 10, 150, 30), "Death Match")) {
				SceneStart = "training";
			}
		} else {
			GUI.Box (new Rect (330, 10, 150, 30), "Death Match");
		}
		
		switch (PageState) {
			
		case 0:
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 20 + (-100 * delta), 260, 50), "Single Player")) {
				if (gameManage) {
					gameManage.OfflineMode = true;
					characterCreate.OpenCharacter ();
				}

				PageState = 1;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (-100 * delta), 260, 50), "Network")) {
				if (gameManage)
					gameManage.OfflineMode = false;
				
				PageState = 2;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 140 + (-100 * delta), 260, 50), "Exit")) {
				Application.Quit ();
			}

			GUI.DrawTexture (new Rect (Screen.width / 2 - (LogoGame.width * 0.5f) / 2, Screen.height / 2 - 200 + (-300 * delta), LogoGame.width * 0.5f, LogoGame.height * 0.5f), LogoGame);	
		
			break;
		case 1:
	
			if (GUI.Button (new Rect (50 + (-300 * delta), 50, 160, 50), "Back")) {
				PageState = 0;
				if(UnitZ.gameServer)
					UnitZ.gameServer.KillServer();
			}	
			
			
			if (!UnitZ.gameClient.isConnecting) {
				
				characterCreate.DrawCharacterCreator ();
				if (characterCreate.State == 0) {
					if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (-100 * delta), 260, 50), "Enter world")) {
						if (gameManage) {
							characterCreate.SetCharacter ();
							if (gameManage.OfflineMode) {
								gameManage.StartSinglePlayerGame (SceneStart);
							} else {
								gameManage.StartMultiplayerGame ();
							}
						}
					}
				}
				
			} else {		
				if (UnitZ.gameClient.isConnecting) {
					GUI.skin.label.alignment = TextAnchor.MiddleCenter;
					GUI.Box (new Rect ((Screen.width / 2) - 130, (Screen.height / 2) - 25, 260, 50), "");
					GUI.Label (new Rect ((Screen.width / 2) - 130, (Screen.height / 2) - 25, 260, 50), "Connecting to server..");
				}	
			}
			break;
			
		case 2:
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 20 + (50 * delta), 260, 50), "Host Game")) {

				PageState = 7;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (50 * delta), 260, 50), "Find Game")) {
				PageState = 4;
				if (gameManage)
					gameManage.Refresh ();
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 140 + (50 * delta), 260, 50), "Connect IP")) {
				PageState = 3;
			}

			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 200 + (50 * delta), 260, 50), "Back")) {
				PageState = 0;
			}

			GUI.DrawTexture (new Rect (Screen.width / 2 - (LogoGame.width * 0.5f) / 2, Screen.height / 2 - 200, LogoGame.width * 0.5f, LogoGame.height * 0.5f), LogoGame);	

			break;
			
		case 3:
			
			UnitZ.gameServer.Port = int.Parse (GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 20, 180, 50), UnitZ.gameServer.Port.ToString ()));
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 20, 100, 50), "Port");

			UnitZ.gameServer.IPServer = GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 80, 180, 50), UnitZ.gameServer.IPServer);
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80, 100, 50), "IP");

			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 140 + (50 * delta), 260, 50), "Connect")) {
				characterCreate.OpenCharacter ();
				PageState = 1;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 200 + (50 * delta), 260, 50), "Back")) {
				PageState = 2;
			}
			GUI.DrawTexture (new Rect (Screen.width / 2 - (LogoGame.width * 0.5f) / 2, Screen.height / 2 - 200, LogoGame.width * 0.5f, LogoGame.height * 0.5f), LogoGame);	
			break;
			
		case 4:
			
			GUI.Box (new Rect (Screen.width / 2 - 275, 50, 550, Screen.height - 200), "");
			DrawGameLobby ();

			if (GUI.Button (new Rect (Screen.width / 2 - 130 + (-300 * delta), Screen.height - 120, 260, 50), "Back")) {
				PageState = 2;
			}
			
			if (gameManage) {
				if (!gameManage.IsRefreshing) {
					if (GUI.Button (new Rect (Screen.width / 2 + 170, Screen.height - 120, 105, 50), "Refresh")) {
						gameManage.Refresh ();
					}
				} else {
					GUI.Label (new Rect (Screen.width / 2 + 170, Screen.height - 120, 105, 50), "Refreshing..");
					
				}
			}
			break;
		case 5:
			UnitZ.gameServer.Port = int.Parse (GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 20, 180, 50), UnitZ.gameServer.Port.ToString ()));
			SceneStart = GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 30, 180, 50), SceneStart);
			GUI.Label (new Rect (Screen.width / 2 - 150, Screen.height / 2 - 70, 300, 50), "sandbox , zombieland ,training");
			
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 - 30, 100, 50), "Level");
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 20, 100, 50), "Port");
			
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (50 * delta), 260, 50), "Host Game")) {
				if (gameManage) {
					gameManage.CreateGame (SceneStart,true);
				}
				characterCreate.OpenCharacter ();
				PageState = 1;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 140 + (50 * delta), 260, 50), "Back")) {
				PageState = 7;
			}
			GUI.DrawTexture (new Rect (Screen.width / 2 - (LogoGame.width * 0.5f) / 2, Screen.height / 2 - 200, LogoGame.width * 0.5f, LogoGame.height * 0.5f), LogoGame);	
			break;
		case 6:
			GUI.Label (new Rect (Screen.width / 2 - 150, Screen.height / 2 - 70, 300, 50), "sandbox , zombieland ,training");
			SceneStart = GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 30, 180, 50), SceneStart);
			UnitZ.gameServer.Port = int.Parse (GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height / 2 + 20, 180, 50), UnitZ.gameServer.Port.ToString ()));
			
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 - 30, 100, 50), "Level");
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 20, 100, 50), "Port");
			
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (50 * delta), 260, 50), "Host Local Game")) {
				if (gameManage) {
					UnitZ.gameServer.LanOnly = true;
					gameManage.CreateGame (SceneStart,true);
					
				}
				characterCreate.OpenCharacter ();
				PageState = 1;
			}
			
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 140 + (50 * delta), 260, 50), "Back")) {
				PageState = 7;
			}
			GUI.DrawTexture (new Rect (Screen.width / 2 - (LogoGame.width * 0.5f) / 2, Screen.height / 2 - 200, LogoGame.width * 0.5f, LogoGame.height * 0.5f), LogoGame);	
			break;
		case 7:
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 20 + (50 * delta), 260, 50), "Master Server")) {
				PageState = 5;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (50 * delta), 260, 50), "Local Game")) {
				PageState = 6;
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 140 + (50 * delta), 260, 50), "Back")) {
				PageState = 2;
			}
			
			GUI.DrawTexture (new Rect (Screen.width / 2 - (LogoGame.width * 0.5f) / 2, Screen.height / 2 - 200, LogoGame.width * 0.5f, LogoGame.height * 0.5f), LogoGame);	
			
			break;
		}
		
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label (new Rect (0, Screen.height - 50, Screen.width, 30), "UnitZ beta | www.hardworkerstudio.com");
	}

	void Update ()
	{
		delta += (0 - delta) / 10f;
		if (pageTemp != PageState) {
			delta = 1;
			pageTemp = PageState;
		}
	}
}
