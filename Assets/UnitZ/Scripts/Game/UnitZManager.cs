using UnityEngine;
using System.Collections;

public class UnitZManager : MonoBehaviour {
	
	public string GameKeyVersion = "first_build";
	void Awake () {
		UnitZ.gameManager = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));
		UnitZ.gameServer = (GameServer)GameObject.FindObjectOfType(typeof(GameServer));
		UnitZ.gameClient = (GameClient)GameObject.FindObjectOfType(typeof(GameClient));
		UnitZ.characterManager = (CharacterManager)GameObject.FindObjectOfType(typeof(CharacterManager));
		UnitZ.itemManager = (ItemManager)GameObject.FindObjectOfType(typeof(ItemManager));
		UnitZ.itemCraftManager = (ItemCrafterManager)GameObject.FindObjectOfType(typeof(ItemCrafterManager));
		UnitZ.playerManager = (PlayerManager)GameObject.FindObjectOfType(typeof(PlayerManager));
		UnitZ.playersManager = (PlayersManager)GameObject.FindObjectOfType(typeof(PlayersManager));
		UnitZ.playerSave = (PlayerSave)GameObject.FindObjectOfType(typeof(PlayerSave));
		UnitZ.scoreManager = (ScoreManager)GameObject.FindObjectOfType(typeof(ScoreManager));
		UnitZ.sceneSave = (SceneSave)GameObject.FindObjectOfType(typeof(SceneSave));
		UnitZ.chatLog = (ChatLog)GameObject.FindObjectOfType(typeof(ChatLog));
		UnitZ.styleManager = (StyleManager)GameObject.FindObjectOfType(typeof(StyleManager));
		UnitZ.popup = (Popup)GameObject.FindObjectOfType(typeof(Popup));
		UnitZ.sceneManager = (SceneManager)GameObject.FindObjectOfType(typeof(SceneManager));
		UnitZ.GameKeyVersion = GameKeyVersion;
	}
	

}
public static class UnitZ{

	public static GameManager gameManager;
	public static GameServer gameServer;
	public static GameClient gameClient;
	public static CharacterManager characterManager;
	public static ItemManager itemManager;
	public static ItemCrafterManager itemCraftManager;
	public static PlayerManager playerManager;
	public static PlayersManager playersManager;
	public static PlayerSave playerSave;
	public static ScoreManager scoreManager;
	public static SceneSave sceneSave;
	public static ChatLog chatLog;
	public static StyleManager styleManager;
	public static Popup popup;
	public static SceneManager sceneManager;
	public static string GameKeyVersion = "";
	public static bool IsOnline = false;

}