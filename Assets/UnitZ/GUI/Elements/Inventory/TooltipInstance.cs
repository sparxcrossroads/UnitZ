//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TooltipInstance : MonoBehaviour
{   
	public Text Header;
	public Text Content;
	public PlayerManager playerManager;
	private ItemCollector Item;

	
	void Start ()
	{
		playerManager = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));	
	}
	
	void Awake ()
	{
		instance = this;
		HideTooltip ();
	}
 
	public void Delete ()
	{
		if (playerManager != null && playerManager.playingCharacter != null && Item != null) {
			playerManager.playingCharacter.inventory.DropItemByCollector (Item, Item.Num);
		}
		HideTooltip ();
	}
	
	public void Use ()
	{
		if (playerManager != null && playerManager.playingCharacter != null && Item != null) {
			playerManager.playingCharacter.inventory.EquipItemByCollector (Item);
		}
		HideTooltip ();
	}
	
	public void ShowTooltip (ItemCollector itemCol, Vector3 pos)
	{
		if (itemCol == null)
			return;
		
		Item = itemCol;
		if (Header)
			Header.text = itemCol.Item.ItemName;
		if (Content)
			Content.text = itemCol.Item.Description;
		
		transform.position = pos;
		gameObject.SetActive (true);
	}
         
	public void HideTooltip ()
	{
		gameObject.SetActive (false);
	}

	private static TooltipInstance instance;

	public static TooltipInstance Instance {
		get {
			if (instance == null)
				instance = GameObject.FindObjectOfType<TooltipInstance> ();
			return instance;
		}
	}


}
