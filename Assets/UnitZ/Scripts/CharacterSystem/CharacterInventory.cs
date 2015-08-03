//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterInventory : MonoBehaviour
{
	[HideInInspector]
	public ItemManager itemManager;
	[HideInInspector]
	public CharacterSystem character;
	public ItemSticker[] itemStickers;
	public List<ItemCollector> Items = new List<ItemCollector> ();
	public ItemSticker FPSItemView;
	[HideInInspector]
	public FPSItemEquipment FPSEquipment;
	[HideInInspector]
	public ItemEquipment TDEquipment;
	[HideInInspector]
	public string StickerTextData;
	[HideInInspector]
	public bool Toggle = false;
	public ItemDataPackage[] StarterItems;
	[HideInInspector]
	public int UpdateCount = 0;
	private NetworkView networkViewer;
	
	void Awake ()
	{
		networkViewer = this.gameObject.GetComponent<NetworkView> ();
		character = this.gameObject.GetComponent<CharacterSystem> ();
		itemManager = (ItemManager)GameObject.FindObjectOfType (typeof(ItemManager));
		if (this.transform.GetComponentsInChildren (typeof(ItemSticker)).Length > 0) {
			var items = this.transform.GetComponentsInChildren (typeof(ItemSticker));
			itemStickers = new ItemSticker[items.Length];
			for (int i=0; i<items.Length; i++) {
				itemStickers [i] = items [i].GetComponent<ItemSticker> ();
				itemStickers [i].ItemIndex = -1;
			}
		}	
		
		PlayerView pv = gameObject.GetComponent<PlayerView> ();
		if (pv && FPSItemView == null) {
			FPSItemView = pv.FPScamera.FPSItemView.GetComponent<ItemSticker> ();
		}
		
		int shortkey = 0;
		for (int i=0; i< StarterItems.Length; i++) {
			if (StarterItems [i].item != null) {
				AddItemByIdemData (StarterItems [i].item, StarterItems [i].Num, -1,shortkey);
				shortkey++;
			}
		}
		if (Items.Count > 0 && Items [0] != null)
			EquipItemByCollector (Items [0]);
	}
	
	void Start ()
	{	
	
	}
	

	
	public string GenStickerTextData ()
	{
		string res = "";
		
		for (int i=0; i<itemStickers.Length; i++) {
			if (itemStickers [i].transform.childCount <= 0) {
				itemStickers [i].ItemIndex = -1;
			}
			res += itemStickers [i].ItemIndex + ",";
		}
		return res;
	}
	
	public string GetItemDataText ()
	{
		string itemdata = "";
		string indexdata = "";
		string numdata = "";
		string numtagdata = "";
		string shortcut = "";
		
		foreach (var item in Items) {
			if (item.Item != null) {
				indexdata += item.Item.ItemID + ",";
				numdata += item.Num + ",";
				numtagdata += item.NumTag + ",";
				shortcut += item.Shortcut + ",";
			}
		}
		itemdata = indexdata + "|" + numdata + "|" + numtagdata + "|" + shortcut;
		//Debug.Log ("items data: " + itemdata);
		
		return itemdata;
	}
	
	public void SetItemsFromText (string itemdatatext)
	{
		ItemManager item = (ItemManager)GameObject.FindObjectOfType (typeof(ItemManager));
		if (item) {
			string[] data = itemdatatext.Split ("|" [0]);
			//Debug.Log ("Get itemlist : " + itemdatatext + data.Length);
			if (data.Length >= 4) {
				RemoveAllItem ();
				string[] indexList = data [0].Split ("," [0]);
				string[] numList = data [1].Split ("," [0]);
				string[] numtagList = data [2].Split ("," [0]);
				string[] shortcutList = data [3].Split ("," [0]);
				
				for (int i=0; i<indexList.Length; i++) {
					if (indexList [i] != "") {
						ItemCollector itemCol = new ItemCollector ();
						itemCol.Item = item.GetItemDataByID (indexList [i]);
						if (itemCol.Item != null) {
							//Debug.Log ("Item " + itemCol.Item.name + " x " + itemCol.Num);
							int.TryParse (numList [i], out itemCol.Num);
							int.TryParse (numtagList [i], out itemCol.NumTag);
							int.TryParse (shortcutList [i], out itemCol.Shortcut);
							
							itemCol.Active = true;
							AddItemByIdemData (itemCol.Item, itemCol.Num, itemCol.NumTag,itemCol.Shortcut);	
						}
					}
				}
			}
		}
	}
	
	public void AddItemByIndex (int index, int num, int numtag)
	{
		UpdateCount+=1;
		if (itemManager != null) {
			var itemdata = itemManager.GetItem (index);
			if (itemdata != null && num > 0) {
				ItemCollector itemc = new ItemCollector ();
				itemc.Index = index;
				itemc.Item = itemdata;
				itemc.NumTag = numtag;
				for (int i=0; i<Items.Count; i++) {
					if (Items [i].Index == index && itemdata.Stack) {
						Items [i].Num += num;
						return;
					}
				}
				itemc.Num += num;
				Items.Add (itemc);
			}
		}
		
	}
	
	public void AddItemByIdemData (ItemData item, int num, int numtag,int shortcut)
	{
		UpdateCount+=1;
		if (itemManager != null) {
			if (item != null && num > 0) {
				ItemData itemdata = itemManager.CloneItemData (item);
				if (itemdata != null) {
					ItemCollector itemc = new ItemCollector ();
					itemc.Index = itemManager.GetIndexByID (itemdata.ItemID);
					itemc.Item = itemdata;
					itemc.NumTag = numtag;
					itemc.Shortcut = shortcut;
					for (int i=0; i<Items.Count; i++) {
						if (Items [i].Item != null && Items [i].Item.ItemID == itemdata.ItemID && itemdata.Stack) {
							Items [i].Num += num;
							return;
						}
					}
					itemc.Num += num;
					Items.Add (itemc);
				
					if (itemc.Index == -1) {
						Debug.Log (itemdata.name + " Is not registered in Item Manager! this item will not save");	
					}
				}
			}
		}
		
	}

	public bool RemoveItem (ItemData itemdata, int num)
	{
		UpdateCount+=1;
		if (itemManager != null) {
			//Debug.Log ("Removing " + itemdata + " > " + num);
			if (itemdata != null && num > 0) {
				
				for (int i=0; i<Items.Count; i++) {
					if (Items [i] != null && Items [i].Item.ItemID == itemdata.ItemID) {
						
						if (Items [i].Num <= 0) {
							Debug.Log (Items [i].Item.ItemName + " Is no more");
							return false;	
						}
						if (Items [i].Num < num) {
							if (Items [i].Num > 0) {
								Items [i].Num -= Items [i].Num;
							}
						} else {
							Items [i].Num -= num;
						}
						
						if (Items [i].Num <= 0) {
							
							RemoveEquipItemByIndex (Items [i].Index);
							Items.RemoveAt (i);
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool RemoveItemByCollector (ItemCollector itemcollector, int num)
	{
		UpdateCount+=1;
		ItemData itemdata = itemcollector.Item;
		if (itemManager != null) {
			
			if (itemdata != null && num > 0) {
				//Debug.Log ("Removing " + itemdata + " > " + num);
				for (int i=0; i<Items.Count; i++) {
					if (Items [i] != null && Items [i] == itemcollector) {
						
						if (Items [i].Num <= 0) {
							Debug.Log (Items [i].Item.ItemName + " Is no more");
							return false;	
						}
						if (Items [i].Num < num) {
							if (Items [i].Num > 0) {
								Items [i].Num -= Items [i].Num;
							}
						} else {
							Items [i].Num -= num;
						}
						
						if (Items [i].Num <= 0) {
							
							if (collectorAttachedTemp == itemcollector) {
								RemoveEquipItemByIndex (Items [i].Index);
								Debug.Log ("Remove " + collectorAttachedTemp + " Num " + Items [i].Num + "/" + itemcollector.Num);
							}
							Items.Remove (itemcollector);
						}
						return true;
					}
				}
			}
		}
		return false;
	}
	
	public bool RemoveItemByIndex (int index, int num)
	{
		UpdateCount+=1;
		if (itemManager != null) {
			var itemdata = itemManager.GetItem (index);
			if (itemdata != null && num > 0) {
				for (int i=0; i<Items.Count; i++) {
					if (Items [i] != null && Items [i].Index == index) {
						if (Items [i].Num <= 0) {
							return false;	
						}
						if (Items [i].Num < num) {
							if (Items [i].Num > 0) {
								Items [i].Num -= Items [i].Num;
							}
						} else {
							Items [i].Num -= num;
						}
						if (Items [i].Num <= 0) {
							RemoveEquipItemByIndex (Items [i].Index);
							Items.RemoveAt (i);
							
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	public void RemoveAllItem ()
	{
		UpdateCount+=1;
		if (itemManager != null) {
			for (int i=0; i<Items.Count; i++) {
				if (Items [i] != null) {	
					Items [i].Num = 0;
					RemoveEquipItemByIndex (Items [i].Index);
					Items.RemoveAt (i);
				}	
			}
		}	
		Items.Clear ();
	}
		
	public int GetItemNum (ItemData itemdata)
	{
		if (itemManager != null) {
			if (itemdata != null) {
				for (int i=0; i<Items.Count; i++) {
					if (Items [i].Item.ItemID == itemdata.ItemID) {
						return Items [i].Num;
					}
				}
			}
		}
		return 0;
	}
	
	public int GetItemNumByIndex (int index)
	{
		if (itemManager != null) {
			var itemdata = itemManager.GetItem (index);
			if (itemdata != null) {
				for (int i=0; i<Items.Count; i++) {
					if (Items [i].Index == index) {
						return Items [i].Num;
					}
				}
			}
		}
		return 0;
	}
	
	public bool CheckItem (ItemData itemdata, int num)
	{
		if (itemManager != null) {
			if (itemdata != null && num > 0) {
				for (int i=0; i<Items.Count; i++) {
					if (Items [i].Item.ItemID == itemdata.ItemID) {
						if (Items [i].Num >= num) {
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public bool CheckItemByIndex (int index, int num)
	{
		if (itemManager != null) {
			var itemdata = itemManager.GetItem (index);
			if (itemdata != null && num > 0) {
				for (int i=0; i<Items.Count; i++) {
					if (Items [i].Index == index) {
						if (Items [i].Num >= num) {

							return true;
						}
					}
				}
			}
		}
		return false;
	}
	
	public void DropItem (ItemData itemdata, int num, int numtag)
	{
		UpdateCount+=1;
		Debug.Log (itemdata);
		if (RemoveItem (itemdata, num)) {
			GameObject droped = null;
			if (Network.isServer || Network.isClient) {
				droped = (GameObject)Network.Instantiate (itemdata.gameObject, this.transform.position, itemdata.gameObject.transform.rotation, 2);
				ItemData data = (ItemData)droped.GetComponent<ItemData> ();
				data.NumTag = numtag;
			} else {
				droped = (GameObject)GameObject.Instantiate (itemdata.gameObject, this.transform.position, itemdata.gameObject.transform.rotation);
				ItemData data = (ItemData)droped.GetComponent<ItemData> ();
				data.NumTag = numtag;
			}
		}
	}
	
	public void DropItemByCollector (ItemCollector item, int num)
	{
		UpdateCount+=1;
		if (RemoveItemByCollector (item, num)) {
			GameObject droped = null;
			if (Network.isServer || Network.isClient) {
				droped = (GameObject)Network.Instantiate (item.Item.gameObject, this.transform.position, item.Item.gameObject.transform.rotation, 2);
				ItemData data = (ItemData)droped.GetComponent<ItemData> ();
				data.NumTag = item.NumTag;
			} else {
				droped = (GameObject)GameObject.Instantiate (item.Item.gameObject, this.transform.position, item.Item.gameObject.transform.rotation);
				ItemData data = (ItemData)droped.GetComponent<ItemData> ();
				data.NumTag = item.NumTag;
			}
		}
	}
	
	public void EquipItemByItemIndex (int index)
	{
		UpdateCount+=1;
		if (itemManager != null) {
			var itemdata = itemManager.GetItem (index);
			if (itemdata != null) {
				EquipItem (itemdata.ItemEquip, index);
				if (itemdata.ItemFPS) {
					AttachFPSItemView (itemdata.ItemFPS);
				}
			}
		}
	}
	
	public void EquipItemByCollector (ItemCollector item)
	{
		Debug.Log("Equip Item "+item.Index);
		UpdateCount+=1;
		if (item.Index != -1 && item.Num > 0) {
			if (itemManager != null) {
				var itemdata = itemManager.GetItem (item.Index);
				if (itemdata != null) {
					EquipItem (itemdata.ItemEquip, item.Index);
					if (itemdata.ItemFPS) {
						AttachFPSItemViewAndCollector (itemdata.ItemFPS, item);
					}
					character.UpdateAnimationState ();
				}
			}
		}
	}

	public void RemoveEquipItemByIndex (int index)
	{
		UpdateCount+=1;
		if (itemManager != null) {
			for (int i=0; i<itemStickers.Length; i++) {
				if (itemStickers [i].ItemIndex == index) {
					RemoveEquipItem (itemStickers [i]);
					break;
				}
			}
		}
	}

	public void RemoveStickerItem (ItemSticker sticker)
	{
		UpdateCount+=1;
		if (sticker != null) {
			sticker.ItemIndex = -1;
			var items = sticker.transform.GetComponentsInChildren (typeof(ItemEquipment));
			for (int i= 0; i< items.Length; i++)
				Destroy (items [i].gameObject);
		}
	}

	public void RemoveEquipItem (ItemSticker sticker)
	{
		UpdateCount+=1;
		if (sticker != null) {
			sticker.ItemIndex = -1;
			var items = sticker.transform.GetComponentsInChildren (typeof(ItemEquipment));
			for (int i= 0; i< items.Length; i++)
				Destroy (items [i].gameObject);
			
			var fpsitems = FPSItemView.transform.GetComponentsInChildren (typeof(FPSItemEquipment));
			for (int v=0; v<fpsitems.Length; v++)
				Destroy (fpsitems [v].gameObject);
		}
	}

	public void AttachItem (ItemSticker sticker, ItemEquipment item)
	{
		UpdateCount+=1;
		if (sticker != null && item != null) {
			RemoveEquipItem (sticker);
			Quaternion rotationTemp = sticker.transform.rotation;
			rotationTemp.eulerAngles += sticker.RotationOffset;
			GameObject newitem = (GameObject)GameObject.Instantiate (item.gameObject, sticker.transform.position, rotationTemp);
			newitem.transform.parent = sticker.gameObject.transform;
			if (sticker.equipType == EquipType.Weapon) {
				TDEquipment = newitem.GetComponent<ItemEquipment> ();
			}
		}		
	}
	
	[HideInInspector]
	public ItemCollector collectorAttachedTemp;
	
	public int GetCollectorFPSindex ()
	{
		
		for (int i=0; i<Items.Count; i++) {
			if (Items [i] != null) {	
				if (Items [i] == collectorAttachedTemp) {
					return i;
				}
			}	
		}
		return 0;	
		
	}

	public void SaveDataToItemCollector (FPSWeaponEquipment fpsprefab, ItemCollector item)
	{
		if (fpsprefab != null) {
			fpsprefab.SetCollectorSlot (item);
		}
		
	}
	
	public void AttachFPSItemView (FPSItemEquipment item)
	{
		UpdateCount+=1;
		if (item != null && FPSItemView != null) {
			RemoveEquipItem (FPSItemView);
			Quaternion rotationTemp = FPSItemView.transform.rotation;
			rotationTemp.eulerAngles += FPSItemView.RotationOffset;
			//Debug.Log (item.gameObject.name);
			GameObject newitem = (GameObject)GameObject.Instantiate (item.gameObject, FPSItemView.transform.position, rotationTemp);
			newitem.transform.parent = FPSItemView.gameObject.transform;
			FPSEquipment = newitem.GetComponent<FPSItemEquipment> ();
		}			
	}

	public void AttachFPSItemViewAndCollector (FPSItemEquipment item, ItemCollector itemcollector)
	{
		UpdateCount+=1;
		if (item != null && FPSItemView != null) {
			RemoveEquipItem (FPSItemView);
			Quaternion rotationTemp = FPSItemView.transform.rotation;
			rotationTemp.eulerAngles += FPSItemView.RotationOffset;
			GameObject newitem = (GameObject)GameObject.Instantiate (item.gameObject, FPSItemView.transform.position, rotationTemp);
			newitem.transform.parent = FPSItemView.gameObject.transform;
			FPSEquipment = newitem.GetComponent<FPSItemEquipment> ();
			collectorAttachedTemp = itemcollector;
			SaveDataToItemCollector ((FPSWeaponEquipment)FPSEquipment.GetComponent<FPSWeaponEquipment> (), itemcollector);
		}			
	}
	
	public void EquipItem (ItemEquipment item, int index)
	{
		UpdateCount+=1;
		for (int i=0; i<itemStickers.Length; i++) {
			if (itemStickers [i] != null && item != null) {
				if (itemStickers [i].equipType == item.itemType && itemStickers [i].Primary) {
					AttachItem (itemStickers [i], item);
					itemStickers [i].ItemIndex = index;
					return;
				}
			}
		}
		for (int i=0; i<itemStickers.Length; i++) {
			if (itemStickers [i] != null && item != null) {
				if (itemStickers [i].equipType == item.itemType) {
					
					AttachItem (itemStickers [i], item);
					itemStickers [i].ItemIndex = index;
					return;
				}
			}
		}
	}

	[RPC]
	public void UpdateOtherInventory (string text)
	{
		if (GenStickerTextData () != text) {
			string[] stickers = text.Split ("," [0]);
			for (int i=0; i<stickers.Length; i++) {
				if (stickers [i] != "") {
					int indexget = 2;
					if (int.TryParse (stickers [i], out indexget)) {
						if (itemManager != null && i < itemStickers.Length && i >= 0) {
							var itemdata = itemManager.GetItem (indexget);
							
							if (itemdata != null) {
								
								if (character.IsMine || (!Network.isClient && !Network.isServer)) {
									AttachFPSItemView (itemdata.ItemFPS);
								}
								AttachItem (itemStickers [i], itemdata.ItemEquip);
								AttachFPSItemView (itemdata.ItemFPS);
								itemStickers [i].ItemIndex = indexget;
							}
							
							if (indexget == -1) {
								RemoveStickerItem (itemStickers [i]);
							}
						}
					}
				}
			}
		}
		StickerTextData = text;
	}
	
	public void SwarpCollector(ItemCollector item1,ItemCollector item2){
		Debug.Log("Swap");
		
		ItemCollector tmp = new ItemCollector();
		CopyCollector(tmp,item1);
		CopyCollector(item1,item2);
		CopyCollector(item2,tmp);
		UpdateCount+=1;
	}
	
	public void CopyCollector(ItemCollector item,ItemCollector source){
		item.Active = source.Active;
		item.Index = source.Index;
		item.Item = source.Item;
		item.Num = source.Num;
		item.NumTag = source.NumTag;
		item.Shortcut = source.Shortcut;
	}
	
	public ItemCollector GetItemByShortCutIndex (int index)
	{
		if (itemManager != null) {
			foreach(ItemCollector item in Items){
				if (item != null) {	
					if(item.Shortcut == index){
						return item;	
					}
				}	
			}
		}
		return null;
	}
	
	public void DeleteShortcut (ItemCollector itemCollector ,int shortcut)
	{
		if (itemManager != null) {
			foreach(ItemCollector item in Items){
				if (item != null) {	
					if(itemCollector != item){
						if(item.Shortcut == shortcut){
							item.Shortcut=-1;
						}
					}
					if(itemCollector == item){
						item.Shortcut = shortcut;
					}
				}	
			}
		}
	}

	void Update ()
	{
		if(character == null)
			return;
		
		if ((Network.isServer || Network.isClient) && character.IsMine) {
			if(networkViewer)
				networkViewer.RPC ("UpdateOtherInventory", RPCMode.Others, GenStickerTextData ());
		}
	}
	
	public void EquipmentOnAction ()
	{
		
		if (TDEquipment) {
			TDEquipment.Action ();	
		}
	}
}

[System.Serializable]
public class ItemDataPackage
{
	public ItemData item;
	public int Num = 1;
}
