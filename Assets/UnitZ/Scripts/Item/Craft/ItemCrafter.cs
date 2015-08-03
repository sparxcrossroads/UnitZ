//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemCrafter {

	public ItemData ItemResult;
	public int NumResult = 1;
	public ItemNeeded[] ItemNeeds;
	public float CraftTime = 2;
	
}

[System.Serializable]
public class ItemNeeded{
	public ItemData Item;
	public int Num;
}