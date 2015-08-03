//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class CharacterCreator : MonoBehaviour
{
	
	public GUISkin skin;
	public int PlayerIndex = 0;
	public CharacterSystem[] CharacterBase;
	[HideInInspector]
	public GameManager gameManage;
	[HideInInspector]
	public PlayerSave Save;
	[HideInInspector]
	public PlayerManager playerManage;
	[HideInInspector]
	public int State = 0;
	public Texture2D[] CharactersIcon;
	
	private Vector2 scrollPosition;
	private CharacterSaveData[] Characters;
	private CharacterSaveData currentCharacter;
	private CharacterSaveData characterCreate;
	private int indexCharacter = 0;
	private int indexDelete;
	
	void Start ()
	{
		indexDelete =-1;
		Save = (PlayerSave)GameObject.FindObjectOfType (typeof(PlayerSave));
		gameManage = (GameManager)GameObject.FindObjectOfType (typeof(GameManager));
		playerManage = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));
		StyleManager Styles = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		
		if (!skin && Styles)
			skin = Styles.GetSkin (0);
		
		Characters = Save.LoadAllCharacters ();
		indexCharacter = PlayerPrefs.GetInt ("INDEX_CRE_CHAR");

	}
	
	float delta;

	void Update ()
	{
		delta += (0 - delta) / 10f;
	}
	
	public void OpenCharacter ()
	{
		State = 0;
		delta = 1;
		indexDelete = -1;
		if (Save) {
			Characters = Save.LoadAllCharacters ();
			
			if (Characters.Length > 0) {
				if (indexCharacter >= Characters.Length) 
					indexCharacter = Characters.Length - 1;
			
				if (indexCharacter < 0) 
					indexCharacter = 0;
			
				currentCharacter = Characters [indexCharacter];
				PlayerIndex = currentCharacter.CharacterIndex;
				SetCharacter ();
			}
		}
	}
	
	public void SetCharacter ()
	{
		indexDelete = -1;
		PlayerIndex = currentCharacter.CharacterIndex;
		if (CharacterBase.Length > 0) {
			if (PlayerIndex >= CharacterBase.Length) 
				PlayerIndex = CharacterBase.Length - 1;
			
			if (PlayerIndex < 0) 
				PlayerIndex = 0;
			
			if (playerManage)
				playerManage.characterBase = CharacterBase [PlayerIndex];
			
			if (gameManage)
				gameManage.UserName = currentCharacter.PlayerName;
			
			
		}
	}

	public void DrawCharacterCreator ()
	{
		switch (State) {
		case 0:
			GUI.skin.label.fontSize = 25;
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.Label (new Rect (Screen.width / 2 - 130, Screen.height / 2 - 50 + (-100 * delta), 260, 50), gameManage.UserName);
			
			if (GUI.Button (new Rect ((Screen.width - 210) + (300 * delta), 50, 160, 50), "Create")) {
				State = 1;
				delta = 1;
			}
			if (GUI.Button (new Rect ((Screen.width - 210) + (300 * delta), 105, 160, 50), "Characters")) {
				State = 3;
				delta = 1;
				Characters = Save.LoadAllCharacters ();
				
			}
			if (Characters.Length <= 0) {
				State = 1;
			}
			break;
		case 1:
			if(Characters.Length > 0){
				if (GUI.Button (new Rect ((Screen.width - 210) + (300 * delta), 50, 160, 50), "Cancel")) {
					State = 0;
					delta = 1;
				}
			}
			
			GUI.BeginGroup (new Rect ((Screen.width / 2) - 520, (Screen.height / 2) - 125 + (-100 * delta), 1040, 300), "");
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.skin.label.fontSize = 20;
			
			if (GUI.Button (new Rect (0, 0, 200, 300), CharactersIcon[0])) {
				characterCreate = new CharacterSaveData ();
				characterCreate.CharacterIndex = 0;
				State = 2;
			}
			GUI.Label (new Rect (0, 20, 200, 300), "Policeman");
			if (GUI.Button (new Rect (210, 0, 200, 300), CharactersIcon[1])) {
				characterCreate = new CharacterSaveData ();
				characterCreate.CharacterIndex = 1;
				State = 2;
			}
			
			GUI.Label (new Rect (210, 20, 200, 300), "Farmer");
			if (GUI.Button (new Rect (420, 0, 200, 300), CharactersIcon[2])) {
				characterCreate = new CharacterSaveData ();
				characterCreate.CharacterIndex = 2;
				State = 2;
			}
			
			GUI.Label (new Rect (420, 20, 200, 300), "Salaryman");
			if (GUI.Button (new Rect (630, 0, 200, 300), CharactersIcon[3])) {
				characterCreate = new CharacterSaveData ();
				characterCreate.CharacterIndex = 3;
				State = 2;
			}
			
			GUI.Label (new Rect (630, 20, 200, 300), "Nu New");
			if (GUI.Button (new Rect (840, 0, 200, 300), CharactersIcon[4])) {
				characterCreate = new CharacterSaveData ();
				characterCreate.CharacterIndex = 4;
				State = 2;
			}
			GUI.Label (new Rect (840, 20, 200, 300), "Doctor");
			
			
			GUI.EndGroup ();
			
			break;
		case 2:
			
			if (GUI.Button (new Rect ((Screen.width - 210) + (300 * delta), 50, 160, 50), "Cancel")) {
				State = 0;
				delta = 1;
			}
			gameManage.UserName = GUI.TextField (new Rect (Screen.width / 2 - 130, Screen.height / 2 - 50 + (-100 * delta), 260, 50), gameManage.UserName);
			
			if (GUI.Button (new Rect (Screen.width / 2 - 130, Screen.height / 2 + 80 + (-100 * delta), 260, 50), "Create")) {
				characterCreate.PlayerName = gameManage.UserName;
				if (Save.CreateCharacter (characterCreate)) {
					OpenCharacter ();
					State = 3;	
				}
			}

			break;
		case 3:
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.skin.label.fontSize = 25;
			
			if (GUI.Button (new Rect ((Screen.width - 210) + (300 * delta), 50, 160, 50), "Close")) {
				State = 0;
				delta = 1;
			}
			
			
			scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2 - 210, 50 + (300 * delta), 420, Screen.height - 200), scrollPosition, new Rect (0, 0, 400, 160 * Characters.Length));

			for (int i=0; i<Characters.Length; i++) {	
				if (GUI.Button (new Rect (0, i * 160, 340, 150),"")) {
					currentCharacter = Characters [i];
					indexCharacter = i;
					PlayerPrefs.SetInt ("INDEX_CRE_CHAR", indexCharacter);
					SetCharacter ();
					State = 0;
				}
				
				GUI.DrawTexture (new Rect (0, i * 160, 100, 150), CharactersIcon[Characters [i].CharacterIndex]);
				GUI.Label (new Rect (120, i * 160, 370, 150), Characters [i].PlayerName);
				if(indexDelete == i){
					if (GUI.Button (new Rect (350, i * 160, 50, 50),"No")) {
						indexDelete = -1;
					}
					if (GUI.Button (new Rect (350, (i * 160) + 60, 50, 50),"Yes")) {
						Save.RemoveCharacter(Characters [i]);
						OpenCharacter();
						State = 3;
					}
				}else{
					if (GUI.Button (new Rect (350, i * 160, 50, 50),"X")) {
						indexDelete = i;
					}
				}
			}
			GUI.EndScrollView ();
			break;
		}
	}
	
}
