//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public enum EquipType{
	None,Weapon,Head,Armor,BackPack,Foot,FPSItemView
}

public class ItemSticker : MonoBehaviour {
	public EquipType equipType;
	public bool Primary;
	public int ItemIndex;
	public Vector3 RotationOffset;
	void Start () {
	
	}
	
	void Update () {
	
	}
}
