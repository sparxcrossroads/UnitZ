//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
 
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler,IPointerClickHandler
{
	public ItemCollector Item;
	private Vector3 pointerPosition;
	
	public void OnPointerClick (PointerEventData eventData)
	{
		DragItem dragitem = this.GetComponent<DragItem> ();
		if (dragitem)
			Item = dragitem.Item;
		pointerPosition = new Vector3 (eventData.position.x, eventData.position.y - 18f, 0f);
		StartHover (pointerPosition);
		
		if(eventData.pointerDrag!=this.gameObject)
			StopHover();
	}
	
	public void OnPointerEnter (PointerEventData eventData)
	{
		
	}

	public void OnSelect (BaseEventData eventData)
	{
		StartHover (transform.position);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		
	}

	public void OnDeselect (BaseEventData eventData)
	{
		
	}
 
	void StartHover (Vector3 position)
	{
		if (Item != null) {
			TooltipInstance.Instance.ShowTooltip (Item, position);
		}
	}

	void StopHover ()
	{
		TooltipInstance.Instance.HideTooltip ();
	}
	

}
