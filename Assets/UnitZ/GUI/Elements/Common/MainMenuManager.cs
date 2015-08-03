//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : PanelsManager
{
	public string SceneStart = "zombieland";
	[HideInInspector]
	public GameManager gameManage;
	public CharacterCreatorCanvas characterCreator;
	public Text CharacterName;
	public GameObject Preloader;

	void Start ()
	{
		Application.targetFrameRate = 140;
		gameManage = (GameManager)GameObject.FindObjectOfType (typeof(GameManager));
		characterCreator = (CharacterCreatorCanvas)GameObject.FindObjectOfType (typeof(CharacterCreatorCanvas));
		
		if (PlayerPrefs.GetString ("StartScene") != "") {
			SceneStart = PlayerPrefs.GetString ("StartScene");
		}
	}
	
	void Update ()
	{
		if (CharacterName && gameManage) {
			CharacterName.text = gameManage.UserName;
		}
		if (UnitZ.gameClient) {
			if (UnitZ.gameClient.isConnecting) {
				if (Preloader) {
					Preloader.SetActive (UnitZ.gameClient.isConnecting);
				}
			}
		}
	}

	public void LevelSelected (string name)
	{
		SceneStart = name;
		PlayerPrefs.SetString ("StartScene", SceneStart);
	}
	
	public void ConnectIP ()
	{
		OpenPanelByName ("LoadCharacter");
	}
	
	public void HostGame ()
	{
		if (gameManage) {
			gameManage.CreateGame (SceneStart,true);
		}
	}
	
	public void UseMasterServer (bool masterserver)
	{
		if (UnitZ.gameServer)
			UnitZ.gameServer.LanOnly = !masterserver;
	}
	
	public void StartSinglePlayer ()
	{
		if (gameManage) {
			gameManage.CreateGame (SceneStart,false);
			OpenPanelByName ("LoadCharacter");
		}
	}
	
	public void StartNetworkGame ()
	{
		if (gameManage) {
			gameManage.CreateGame (SceneStart,true);
			OpenPanelByName ("LoadCharacter");
		}		
	}

	public void EnterWorld ()
	{
		if (gameManage) {
			if (characterCreator){
				characterCreator.SetCharacter ();
			}
			gameManage.StartGame (SceneStart);
		}
		OpenPanelByName ("Connecting");
	}
	
	public void ConnectingDeny ()
	{
		gameManage.ConnectingDeny ();
	}
	
	public void ExitGame ()
	{
		Application.Quit ();	
	}
}
