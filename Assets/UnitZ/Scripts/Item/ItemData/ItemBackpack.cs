//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemBackpack : ItemData
{

	public List<ItemCollector> Items = new List<ItemCollector> ();

	
	void Start ()
	{
		
		if (networkViewer && networkViewer.isMine && (Network.isServer || Network.isClient)) {
			string itemdata = "";
			string indexdata = "";
			string numdata = "";
			string numtagdata = "";
			
			foreach (var item in Items) {
				if (item.Item != null) {
					indexdata += item.Item.ItemID + ",";
					numdata += item.Num + ",";
					numtagdata += item.NumTag + ",";
				}
			}
			itemdata = indexdata + "|" + numdata + "|" + numtagdata;
			Debug.Log("Copy "+itemdata);
			networkViewer.RPC ("GetItem", RPCMode.Others, itemdata);
		}
	}
	
	[RPC]
	void GetItem (string itemdatatext)
	{
		ItemManager item = (ItemManager)GameObject.FindObjectOfType (typeof(ItemManager));
		if (item) {
			Debug.Log ("Get itemlist : " + itemdatatext);
			string[] data = itemdatatext.Split ("|" [0]);
			string[] indexList = data [0].Split ("," [0]);
			string[] numList = data [1].Split ("," [0]);
			string[] numtagList = data [2].Split ("," [0]);
			
			for (int i=0; i<indexList.Length; i++) {
				if (indexList [i] != "") {
					ItemCollector itemCol = new ItemCollector ();
					itemCol.Item = item.GetItemDataByID (indexList [i]);
					
					int.TryParse(numList [i],out itemCol.Num);
					int.TryParse(numtagList [i],out itemCol.NumTag);
					
					itemCol.Active = true;
					AddItem (itemCol);
				}
			}
		}
	}
	
	public void AddItem (ItemCollector item)
	{
		if (item.Active){
			Items.Add (item);
		}
	}
	
	public override void Pickup (CharacterInventory inventory)
	{
		if (inventory != null && Items != null) {
			
			foreach (var item in Items) {
				if (item.Item != null) {
					Debug.Log ("Pick up " + item.Item + "Num tag "+item.NumTag);
					inventory.AddItemByIdemData (item.Item, item.Num,item.NumTag,-1);	
				}
			}
		}
		
		RemoveItem();
		
	}

}
