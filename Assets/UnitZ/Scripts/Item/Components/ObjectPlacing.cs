//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectPlacing : MonoBehaviour {
	
	public string ItemID = "";
	
	void Awake(){
		DontDestroyOnLoad (this.gameObject);
	}
	
	public void SetItemID(string id){
		ItemID = id;
	}
	
	void Start () {
	
	}	

}
