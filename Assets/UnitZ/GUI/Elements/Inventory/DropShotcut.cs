//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropShotcut : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int ShortcutIndex;
	public KeyCode Key;
	private GUIItemCollector guiItem;
	private PlayerManager player;
	
	public void Start ()
	{
		guiItem = this.GetComponent<GUIItemCollector> ();
		player = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));
	}
	
	public void OnDrop (PointerEventData data)
	{
		if (player == null || player.playingCharacter == null || guiItem == null || data == null)
			return;
		
		DragItem dragitem = GetDropSprite (data);
		if (dragitem != null) {
			ItemCollector itemDrag = dragitem.Item;
			if (itemDrag != null) {
				player.playingCharacter.inventory.DeleteShortcut (itemDrag, ShortcutIndex);
				itemDrag.Shortcut = ShortcutIndex;
				guiItem.SetItemCollector (itemDrag);
			}
		}
		
	}

	public void OnPointerEnter (PointerEventData data)
	{

	}

	public void OnPointerExit (PointerEventData data)
	{

	}
	
	public void UseItem ()
	{
		if (guiItem != null && guiItem.Item != null && player != null) {
			player.playingCharacter.inventory.EquipItemByCollector (guiItem.Item);
		}	
	}
	
	public void Update ()
	{
		
		if (Input.GetKeyDown (Key)) {
			UseItem ();
		}
		
		if (player == null || player.playingCharacter == null)
			return;
		
		ItemCollector item = player.playingCharacter.inventory.GetItemByShortCutIndex (ShortcutIndex);
		guiItem.SetItemCollector (item);
		
	}
	
	private DragItem GetDropSprite (PointerEventData data)
	{
		var originalObj = data.pointerDrag;
		if (originalObj == null)
			return null;

		var srcImage = originalObj.GetComponent<DragItem> ();
		if (srcImage == null)
			return null;
		
		return srcImage;
	}
}