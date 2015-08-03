//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class LinkActive : ItemData
{

	public string Link = "http://www.hardworkerstudio.com";

	public override void Pickup (CharacterInventory inventory)
	{
		
		if (Application.isWebPlayer) {
			Application.ExternalEval ("window.open('" + Link + "','_blank')");
		} else {
			Application.OpenURL (Link);
		}
		base.Pickup (inventory);
	}
	
}
