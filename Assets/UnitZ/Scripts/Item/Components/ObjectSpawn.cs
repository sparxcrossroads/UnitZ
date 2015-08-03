//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class ObjectSpawn : MonoBehaviour
{
	public string ItemID = "";
	public GameObject Item;
	public int Group = 2;
	
	void Start ()
	{
		if (Item) {
			if (Network.isServer || Network.isClient) {
				if(Network.isServer){
					GameObject item = (GameObject)Network.Instantiate (Item, this.transform.position, this.transform.rotation, Group);
					item.SendMessage("SetItemID",ItemID,SendMessageOptions.DontRequireReceiver);
				}
			} else {
				GameObject item = (GameObject)GameObject.Instantiate (Item, this.transform.position, this.transform.rotation);
				item.SendMessage("SetItemID",ItemID,SendMessageOptions.DontRequireReceiver);
				
			}
		}
		GameObject.Destroy(this.gameObject);
	}
	
	public void SetItemID(string id){
		ItemID = id;
	}
	
	void Update ()
	{
	
	}
}
