//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterCreatorCanvas : MonoBehaviour
{
	
	public RectTransform CharacterBadgePrefab;
	public RectTransform Canvas;
	[HideInInspector]
	public CharacterSaveData[] Characters;
	private Vector2 scrollPosition;
	private int indexCharacter = 0;

	public void Setup ()
	{
		ClearCanvas ();
		indexCharacter = PlayerPrefs.GetInt ("INDEX_CRE_CHAR");
	}

	void ClearCanvas ()
	{
		if (Canvas == null)
			return;
		
		foreach (Transform child in Canvas.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}

	void Start ()
	{
		Setup ();
	}

	public IEnumerator LoadCharacters ()
	{
		if (UnitZ.playerSave) {
			Characters = UnitZ.playerSave.LoadAllCharacters ();
		
			while ((Characters !=null && Characters.Length <= 0) || Characters == null) {
				yield return new WaitForEndOfFrame();
			}
			if (Characters.Length > 0) {
				if (indexCharacter >= Characters.Length) 
					indexCharacter = Characters.Length - 1;
			
				if (indexCharacter < 0) 
					indexCharacter = 0;
			
				if(UnitZ.characterManager)
					UnitZ.characterManager.SetupCharacter(Characters [indexCharacter]);
				
				DrawCharactersToCanvas ();
			}
		}
	}

	public void DrawCharactersToCanvas ()
	{
		if (Canvas == null || CharacterBadgePrefab == null || Characters == null)
			return;
		
		ClearCanvas ();
		for (int i=0; i<Characters.Length; i++) {
			
			GameObject obj = (GameObject)GameObject.Instantiate (CharacterBadgePrefab.gameObject, Vector3.zero, Quaternion.identity);
			obj.transform.SetParent (Canvas.transform);
			CharacterBadge charbloger = obj.GetComponent<CharacterBadge> ();
			RectTransform rect = obj.GetComponent<RectTransform> ();
			
			if (rect) {
				rect.anchoredPosition = new Vector2 (5, -(((CharacterBadgePrefab.sizeDelta.y + 5) * i)));
				rect.localScale = CharacterBadgePrefab.gameObject.transform.localScale;
			}
			
			if (charbloger) {
				
				charbloger.Index = i;
				charbloger.CharacterData = Characters [i];
				if(UnitZ.characterManager)
					charbloger.GUIImage.texture = UnitZ.characterManager.CharacterPresets [Characters [i].CharacterIndex].Icon;
				charbloger.GUIName.text = Characters [i].PlayerName;
				charbloger.CharacterCreatorS = this;
				charbloger.name = Characters [i].PlayerName;
			}
		}
		
		Canvas.sizeDelta = new Vector2 (Canvas.sizeDelta.x, (CharacterBadgePrefab.sizeDelta.y + 5) * Characters.Length);
	}
	
	public void CreateCharacter (Text textName)
	{
		if (UnitZ.characterManager && textName) {
			
			if (UnitZ.characterManager.CreateCharacter (textName.text)) {
				Setup ();
				StartCoroutine (LoadCharacters ());
				MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType (typeof(MainMenuManager));
				if (menu)
					menu.OpenPanelByNameNoPreviousSave ("LoadCharacter");
					
			}
		}
	}
	
	public void SelectCharacter (CharacterSaveData character)
	{
		if(UnitZ.characterManager)
			UnitZ.characterManager.SetupCharacter(character);
		
		MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType (typeof(MainMenuManager));
		if (menu)
			menu.OpenPanelByName ("EnterWorld");
	}
	
	public void SetCharacter ()
	{
		if(UnitZ.characterManager)
			UnitZ.characterManager.SetCharacter();
	}

	public void SelectCreateCharacter (int index)
	{
		if(UnitZ.characterManager)
			UnitZ.characterManager.SelectCreateCharacter (index);
	}

	
	public void RemoveCharacter (int index)
	{
		if (UnitZ.characterManager) {
			UnitZ.characterManager.RemoveCharacter (Characters [index]);
			StartCoroutine (LoadCharacters ());
		}
	}
	
	
}
