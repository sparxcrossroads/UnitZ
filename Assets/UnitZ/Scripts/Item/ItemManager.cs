//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour
{

	public ItemData[] ItemsList;
	public string Suffix = "UZ";
	public NetworkView networkViewer;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView>();
	}
	
	void Awake ()
	{
		for (int i=0; i<ItemsList.Length; i++) {
			ItemsList [i].ItemID = Suffix + i;
			if (ItemsList [i].ItemFPS) {
				if (ItemsList [i].ItemFPS.GetComponent<FPSItemEquipment> ())
					ItemsList [i].ItemFPS.GetComponent<FPSItemEquipment> ().ItemID = ItemsList [i].ItemID;
			}
		}
	}
	
	public ItemData GetItem (int index)
	{
		if (index < ItemsList.Length && index >= 0) {
			return ItemsList [index];
		} else {
			return null;	
		}
	}
	
	public int GetIndexByID (string itemid)
	{
		for (int i=0; i<ItemsList.Length; i++) {
			if (itemid == ItemsList [i].ItemID) {
				return i;
			}
		}
		return -1;
	}

	public int GetIndexByName (string itemname)
	{
		for (int i=0; i<ItemsList.Length; i++) {
			if (itemname == ItemsList [i].ItemName) {
				return i;
			}
		}
		return -1;
	}
	
	public ItemData CloneItemData (ItemData item)
	{
		for (int i=0; i<ItemsList.Length; i++) {
			if (item.ItemID == ItemsList [i].ItemID) {
				return ItemsList [i];
			}
		}
		return null;
	}
	
	public int GetItemIndexByItemData (ItemData item)
	{
		for (int i=0; i<ItemsList.Length; i++) {
			if (item.ItemID == ItemsList [i].ItemID) {
				return i;
			}
		}
		return -1;
	}
	
	public ItemData GetItemDataByID (string itemid)
	{
		for (int i=0; i<ItemsList.Length; i++) {
			if (itemid == ItemsList [i].ItemID) {
				return ItemsList [i];
			}
		}
		return null;
	}
	
	public ItemData GetItemDataByName (string itemname)
	{
		for (int i=0; i<ItemsList.Length; i++) {
			if (itemname == ItemsList [i].ItemName) {
				return ItemsList [i];
			}
		}
		return null;
	}
	
	[RPC]
	private void placingObject (string itemid, Vector3 position, Vector3 normal)
	{
		ItemData itemdata = GetItemDataByID (itemid);
		if (itemdata.ItemFPS) {
			FPSItemPlacing fpsplacing = itemdata.ItemFPS.GetComponent<FPSItemPlacing> ();
			if (fpsplacing) {
				if (fpsplacing.Item) {
					GameObject placeditem = (GameObject)Network.Instantiate (fpsplacing.Item, normal, fpsplacing.Item.gameObject.transform.rotation, 2);
					placeditem.transform.forward = normal;
				}
			}
		}
	}
	
	public void PlacingObject (string itemid, Vector3 position, Vector3 normal)
	{
		if (Network.isServer || Network.isClient) {
			if(networkViewer)
				networkViewer.RPC ("placingObject", RPCMode.Server, itemid, position, normal);
		} else {
			placingObject (itemid, position, normal);
		}
	}
	
	public void DirectPlacingObject (string itemid, Vector3 position, Quaternion rotation)
	{
		ItemData itemdata = GetItemDataByID (itemid);
		if (itemdata.ItemFPS) {
			FPSItemPlacing fpsplacing = itemdata.ItemFPS.GetComponent<FPSItemPlacing> ();
			if (fpsplacing) {
				if (fpsplacing.Item) {
					if (Network.isServer || Network.isClient) {
						GameObject placeditem = (GameObject)Network.Instantiate (fpsplacing.Item, position, rotation, 2);
						placeditem.SendMessage("SetItemID",itemid,SendMessageOptions.DontRequireReceiver);
					} else {
						GameObject placeditem = (GameObject)GameObject.Instantiate (fpsplacing.Item, position, rotation);
						placeditem.SendMessage("SetItemID",itemid,SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}
	
	void Update ()
	{
	
	}
}

