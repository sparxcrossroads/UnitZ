//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

public class CharacterHUDCanvas : PanelsManager
{
	
	private PlayerManager PlayerManage;
	private CharacterSystem mainCharacter;
	private CharacterLiving living;
	public GameObject Canvas;
	public ValueBar HPbar, FoodBar, WaterBar;
	
	void Start ()
	{
		PlayerManage = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));
		
	}

	void Awake ()
	{
		if (Pages.Length > 0)
			ClosePanel (Pages [0]);
	}
	
	void InputController ()
	{
		
		if (Input.GetKeyDown (KeyCode.E)) {
			MouseLock.MouseLocked = !TogglePanelByName ("Inventory");	
		}
		
		if (Input.GetKeyDown (KeyCode.Escape)) {
			MouseLock.MouseLocked = !TogglePanelByName ("InGameMenu");	
		}
		
		if (Input.GetKeyDown (KeyCode.Tab)) {
			TogglePanelByName ("Scoreboard");	
		}

		if (IsPanelOpened ("InGameMenu") || IsPanelOpened ("Inventory") || IsPanelOpened ("Craft")) {
			MouseLock.MouseLocked = false;
		} else {
			MouseLock.MouseLocked = true;
		}

	}
	
	void Update ()
	{
		if (PlayerManage == null || Canvas == null)
			return;
		
		mainCharacter = PlayerManage.playingCharacter;

		if (PlayerManage.playingCharacter != null) {
			Canvas.gameObject.SetActive (true); 
		} else {
			Canvas.gameObject.SetActive (false); 
		}
		
		if (mainCharacter) {
			
			InputController ();
			
			if (HPbar) {
				HPbar.Value = mainCharacter.HP;	
				HPbar.ValueMax = mainCharacter.HPmax;	
			}
			if (FoodBar && living) {
				FoodBar.Value = living.Hungry;	
				FoodBar.ValueMax = living.HungryMax;	
			}
			if (WaterBar && living) {
				WaterBar.Value = living.Water;	
				WaterBar.ValueMax = living.WaterMax;	
			}
			
			if (living == null)
				living = mainCharacter.GetComponent<CharacterLiving> ();
			
		} else {
			living = null;
		}
	
	}
}
