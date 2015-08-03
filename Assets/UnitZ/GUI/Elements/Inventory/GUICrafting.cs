//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUICrafting : GUICraft
{

	public ValueBar Process;
	public Button CraftButton;
	
	void Start ()
	{
		if (Icon)
			Icon.enabled = false;
		if (Name)
			Name.enabled = false;
		if (Process)
			Process.gameObject.SetActive (false);
		if (CraftButton)
			CraftButton.gameObject.SetActive (false);
	}

	void Update ()
	{
		if (CrafterManager) {
			if (Process) {
				if (CrafterManager.crafting && CrafterManager.ItemSelected == Crafter) {
					Process.gameObject.SetActive (true);
					Process.ValueMax = 1;
					Process.Value = 1 - CrafterManager.CraftingDurationNormalize;
					Process.CustomText = Mathf.Floor (CrafterManager.CraftingDuration).ToString () + " SEC.";
				} else {
					Process.gameObject.SetActive (false);
				}
			}
			
			if (CraftButton) {
				
				if (CrafterManager.crafting) {
					CraftButton.gameObject.SetActive (false);	
				} else {
					if (CrafterLoader.Player && CrafterLoader.Player.playingCharacter) {
						if (CrafterManager.CheckNeeds (Crafter, CrafterLoader.Player.playingCharacter.inventory)) {
							CraftButton.gameObject.SetActive (true);	
						} else {
							CraftButton.gameObject.SetActive (false);
						}
					}	
				}
			}
		
			if (Crafter != null && Crafter.ItemResult != null) {
			
				if (Icon != null && Crafter.ItemResult.ImageSprite != null) {
					Icon.sprite = Crafter.ItemResult.ImageSprite;
					Icon.enabled = true;
				}
			
				if (Name != null) {
					Name.text = Crafter.ItemResult.ItemName;
					Name.enabled = true;
				}
			}
		}
	}

	public void CancelCraft ()
	{
		if (CrafterManager && CrafterManager.ItemSelected == Crafter)
			CrafterManager.CancelCraft ();
		
		if (CrafterLoader)
			CrafterLoader.SelectCraft (Index, null);
	}
	
	public void ConfirmCraft ()
	{
		if (CrafterManager) {
			if (CrafterLoader.Player && CrafterLoader.Player.playingCharacter) {
				CrafterManager.CraftSelected (Crafter);
				CrafterManager.Craft (CrafterLoader.Player.playingCharacter.inventory);
			}
		}
	}

}
