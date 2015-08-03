//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIItemLoader : MonoBehaviour
{
	public PlayerManager PlayerManage;
	public RectTransform Canvas;
	public RectTransform Item;
	public int Size = 5;
	public float Spacing = 3;
	
	private int updateTmp;
	private int numRaw;
	private int numItem;

	void Awake ()
	{
		PlayerManage = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));	
	}
	
	void Start ()
	{
	}
	
	void OnEnable ()
	{
		UpdateGUIInventory ();
	}

	void AddItemToRaw (ItemCollector item)
	{
		// adding item collector to GUI object
		GameObject obj = (GameObject)GameObject.Instantiate (Item.gameObject, Vector3.zero, Quaternion.identity);
		DragItem drag = obj.GetComponent<DragItem> ();
		
		if (drag) {
			drag.Item = item;
			if (item.Item.ImageSprite)
				drag.Icon.sprite = item.Item.ImageSprite;
		
			drag.Num.text = item.Num.ToString ();
		}
		
		obj.transform.SetParent (Canvas.gameObject.transform);
		RectTransform rect = obj.GetComponent<RectTransform> ();
		rect.anchoredPosition = new Vector2 (((rect.sizeDelta.x + Spacing) * numItem) + Spacing, -(((rect.sizeDelta.y + Spacing) * numRaw) + Spacing));
		rect.localScale = Item.gameObject.transform.localScale;
		numItem++;
		
		if (numItem >= Size) {
			numItem = 0;
			numRaw += 1;
		}
		Canvas.sizeDelta = new Vector2 (Canvas.sizeDelta.x, (Item.sizeDelta.y + Spacing) * (numRaw + 1));
	}
	
	void UpdateGUIInventory ()
	{
		if (PlayerManage == null || PlayerManage.playingCharacter == null)
			return;
		Clear ();
		List<ItemCollector> getitems = PlayerManage.playingCharacter.inventory.Items;
		for (int i=0; i<getitems.Count; i++) {
			if (getitems [i].Num > 0 && getitems [i].Item != null && getitems [i].Index > -1) {
				AddItemToRaw (getitems [i]);
			}
		}
	}
	
	void Clear ()
	{
		if (Canvas == null)
			return;
		
		numItem = 0;
		numRaw = 0;
		
		foreach (Transform child in Canvas.transform)
		{
 		 	GameObject.Destroy (child.gameObject);
		}
	}
	
	void Update ()
	{
		if (PlayerManage == null || PlayerManage.playingCharacter == null)
			return;
		
		// GUI inventory  will update only when ChracterSystem.inventory has been modified
		// In every items has changed ChracterSystem.inventory.UpdateCount has been plus + 1
		// so updateTmp using for marking a latest point of UpdateCount.
		
		if (PlayerManage.playingCharacter.inventory.UpdateCount != updateTmp) {
			updateTmp = PlayerManage.playingCharacter.inventory.UpdateCount;
			UpdateGUIInventory ();
		}
	
	}
}
