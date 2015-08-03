//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUICraft : MonoBehaviour
{

	public Text Name;
	public Image Icon;
	public ItemCrafter Crafter;
	public int Index;
	public GUICraftListLoader CrafterLoader;
	public ItemCrafterManager CrafterManager;
	
	void Start ()
	{
		if (Icon)
			Icon.enabled = false;
		if (Name)
			Name.enabled = false;
	}

	void Update ()
	{
			
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
	
	public	void Craft ()
	{
		if (CrafterLoader) {
			CrafterLoader.SelectCraft (Index, Crafter);
		}
	}
}
