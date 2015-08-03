//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
	[HideInInspector]
	public CharacterInventory inventory;
	[HideInInspector]
	public ItemCrafterManager crafterManager;
	public GUISkin Skin;
	private int page;
	
	void Start ()
	{
		page = 0;
		inventory = this.GetComponent<CharacterInventory> ();
		crafterManager = (ItemCrafterManager)GameObject.FindObjectOfType (typeof(ItemCrafterManager));
		StyleManager Styles = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		if(!Skin && Styles)
			Skin = Styles.GetSkin(0);
	}
	
	void Update ()
	{
	
	}
	
	Vector2 scrollPosition;
	public Vector2 size = new Vector2 (450, 300);
	
	void OnGUI ()
	{
		size = new Vector2 (450, 350);
		if (Skin)
			GUI.skin = Skin;
	
		
		if (((!Network.isServer && !Network.isClient) || GetComponent<NetworkView>().isMine) && inventory != null) {
			if (!inventory.Toggle)
				return;
			
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.skin.label.fontSize = 35;
			
			if (GUI.Button (new Rect ((Screen.width / 2 - size.x / 2) - 10, (Screen.height / 2 - size.y / 2) - 50, 100, 35), "Items")) {
				page = 0;	
			}
			if (GUI.Button (new Rect ((Screen.width / 2 - size.x / 2) + 95, (Screen.height / 2 - size.y / 2) - 50, 100, 35), "Craft")) {
				if(crafterManager){
					if(crafterManager.crafting){
						page = 2;
					}else{
						page = 1;
					}
				}
			}
			
			GUI.Box (new Rect ((Screen.width / 2 - size.x / 2) - 10, (Screen.height / 2 - size.y / 2) - 10, size.x + 20, size.y + 20), "");
			GUI.BeginGroup (new Rect (Screen.width / 2 - size.x / 2, Screen.height / 2 - size.y / 2, size.x, size.y), "");
			GUI.skin.label.fontSize = 19;
			int itemCount = 0;
			
			switch (page) {
			case 0:
			
				List<ItemCollector> items = inventory.Items;
				for (int i=0; i<items.Count; i++) {
					if (items [i].Index != -1 && items [i].Num > 0) {
						itemCount++;
					}
				}
			
				scrollPosition = GUI.BeginScrollView (new Rect (0, 0, size.x, size.y), scrollPosition, new Rect (0, 0, size.x - 20, itemCount * 35));
			
				for (int i=0; i<items.Count; i++) {
					if (items [i].Item != null && items [i].Index != -1 && items [i].Num > 0) {
						GUI.Label (new Rect (20, i * 35, 200, 30), items [i].Item.ItemName + " <color=lime>x " + items [i].Num+"</color>");
			
						if (GUI.Button (new Rect (340, i * 35, 50, 30), "Use")) {
							inventory.EquipItemByCollector (items [i]);
						}
						if (GUI.Button (new Rect (400, i * 35, 30, 30), "X")) {
							Debug.Log (items [i] + "  " + items [i].Item);
							inventory.DropItemByCollector (items [i], items [i].Item.Quantity);
						}
					}
				}
				GUI.EndScrollView ();
				
			
				break;
			case 1:
				if (crafterManager) {
					scrollPosition = GUI.BeginScrollView (new Rect (0, 0, size.x, size.y), scrollPosition, new Rect (0, 0, size.x - 20, crafterManager.ItemCraftList.Length * 35));
			
					for (int i=0; i<crafterManager.ItemCraftList.Length; i++) {
						if (crafterManager.ItemCraftList [i].ItemResult) {
							GUI.Label (new Rect (20, i * 35, 200, 30), crafterManager.ItemCraftList [i].ItemResult.ItemName);
			
							if (GUI.Button (new Rect (340, i * 35, 100, 30), "Craft")) {
								crafterManager.CraftSelected (crafterManager.ItemCraftList [i]);
								page = 2;
							}
						}
					}
					GUI.EndScrollView ();
				}
				break;
			case 2:
				if (crafterManager != null && crafterManager.ItemSelected != null) {
					scrollPosition = GUI.BeginScrollView (new Rect (0, 0, size.x, size.y - 60), scrollPosition, new Rect (0, 0, size.x - 20, crafterManager.ItemSelected.ItemNeeds.Length * 35));
					bool cancraftit = true;
					for (int i=0; i<crafterManager.ItemSelected.ItemNeeds.Length; i++) {
						if (crafterManager.ItemSelected.ItemNeeds [i].Item) {
							int youhave = inventory.GetItemNum (crafterManager.ItemSelected.ItemNeeds [i].Item);
							int need = crafterManager.ItemSelected.ItemNeeds [i].Num;
							GUI.skin.label.alignment = TextAnchor.MiddleLeft;
							GUI.Label (new Rect (20, i * 35, 200, 30), crafterManager.ItemSelected.ItemNeeds [i].Item.ItemName + "  <color=silver>X " + need+"</color>");
							GUI.skin.label.alignment = TextAnchor.MiddleRight;
							if (youhave >= crafterManager.ItemSelected.ItemNeeds [i].Num) {
								GUI.Label (new Rect (340, i * 35, 100, 30), "<color=lime>Ready</color>");
							} else {
								cancraftit = false;
								int left = need - youhave;
								GUI.Label (new Rect (340, i * 35, 100, 30), "<color=red>Need " + left + "</color>");
							}
						}
					}
					GUI.EndScrollView ();
				
					if (crafterManager.crafting) {
						GUI.skin.label.alignment = TextAnchor.MiddleLeft;
						GUI.Label (new Rect (10, size.y - 50, size.x - 110, 40), "Crafting.. " + Mathf.Floor(crafterManager.CraftingDuration) + " s.");
						
						
						if (GUI.Button (new Rect (size.x - 110, size.y - 50, 100, 40), "Cancel")) {
							crafterManager.CancelCraft ();
						}
					
					} else {
						
						if (GUI.Button (new Rect (10, size.y - 50, size.x - 20, 40), "Craft")) {
							if (cancraftit) {
								crafterManager.Craft (inventory);
							}
						}
					}
				}
				break;
			}
			
			GUI.EndGroup ();
		}
	}
}
