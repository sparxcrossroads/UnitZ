//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSwapItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public DragItem dragitem;
	
	public void Start ()
	{
		
	}
	
	public void OnDrop (PointerEventData data)
	{
		PlayerManager player = (PlayerManager)GameObject.FindObjectOfType (typeof(PlayerManager));
		if (GetDropSprite (data) != null)
			player.playingCharacter.inventory.SwarpCollector (dragitem.Item, GetDropSprite (data).Item);
	}

	public void OnPointerEnter (PointerEventData data)
	{

	}

	public void OnPointerExit (PointerEventData data)
	{

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