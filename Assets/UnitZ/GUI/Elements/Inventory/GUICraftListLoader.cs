//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUICraftListLoader : MonoBehaviour
{

	public RectTransform Canvas;
	public RectTransform GUICraftPrefab;
	public RectTransform GUICraftingPrefab;
	public RectTransform GUICraftNeed;
	public ItemCrafterManager CrafterManager;
	public PlayerManager Player;
	private int indexSelected = -1;
	
	void Start ()
	{
		CrafterManager = (ItemCrafterManager)GameObject.FindObjectOfType (typeof(ItemCrafterManager));
		Player = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));
		SetupCrafterList ();
	}
	
	void OnEnable ()
	{
		SetupCrafterList ();
	}
	
	void SetupCrafterList ()
	{
		if (CrafterManager == null || Canvas == null || GUICraftPrefab == null)
			return;
		
		Clear ();
		float nextMark = 0;
		
		for (int i=0; i<CrafterManager.ItemCraftList.Length; i++) {
			GameObject craftObj = GUICraftPrefab.gameObject;
			if (i == indexSelected) {
				craftObj = GUICraftingPrefab.gameObject;
			}
			GameObject obj = (GameObject)GameObject.Instantiate (craftObj, Vector3.zero, Quaternion.identity);
			RectTransform rect = obj.GetComponent<RectTransform> ();
			
			GUICraft craf = obj.GetComponent<GUICraft> ();
			if (craf) {
				craf.Crafter = CrafterManager.ItemCraftList [i];
				craf.CrafterLoader = this;
				craf.CrafterManager = CrafterManager;
				craf.Index = i;
			}

			
			float nextmarkoffset = 0;
			if (i == indexSelected) {
				nextmarkoffset = DrawNeeds (CrafterManager.ItemCraftList [i], rect.sizeDelta.y + nextMark);
			}
			
			obj.transform.SetParent (Canvas.gameObject.transform);
			rect.anchoredPosition = new Vector2 (5, -nextMark);
			rect.localScale = craftObj.transform.localScale;
			nextMark += (rect.sizeDelta.y + nextmarkoffset);
	
		}
		Canvas.sizeDelta = new Vector2 (Canvas.sizeDelta.x, nextMark);
		
	}

	float DrawNeeds (ItemCrafter crafter, float offset)
	{

		float res = 0;
		
		for (int i=0; i<crafter.ItemNeeds.Length; i++) {
			GameObject obj = (GameObject)GameObject.Instantiate (GUICraftNeed.gameObject, Vector3.zero, Quaternion.identity);
			
			obj.transform.SetParent (Canvas.gameObject.transform);
			GUIItemDataNeed itemdata = obj.GetComponent<GUIItemDataNeed> ();
			
			if (itemdata) {
				itemdata.Item = crafter.ItemNeeds [i].Item;
				itemdata.Need = crafter.ItemNeeds [i].Num;
				if (Player != null && Player.playingCharacter != null) {
					itemdata.Inventory = Player.playingCharacter.inventory;
				}
			}
			RectTransform rect = obj.GetComponent<RectTransform> ();
			rect.anchoredPosition = new Vector2 (5, -((rect.sizeDelta.y * i) + offset));
			rect.localScale = GUICraftNeed.gameObject.transform.localScale;
			res += rect.sizeDelta.y;
		}
		return res;
	}

	void Clear ()
	{
		if (Canvas == null)
			return;
		
		foreach (Transform child in Canvas.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}
	
	public void SelectCraft (int index, ItemCrafter crafter)
	{
		if (indexSelected == index) {
			indexSelected = -1;
			
		} else {
			indexSelected = index;
		}
		SetupCrafterList ();
	}

	void Update ()
	{
	
	}
}
