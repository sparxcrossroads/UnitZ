//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterItemDroper : MonoBehaviour
{
	
	public GameObject Backpack;
	CharacterSystem character;
	
	void Start ()
	{
		character = this.GetComponent<CharacterSystem> ();
	}
	
	void Update ()
	{
	
	}

	public void DropItem ()
	{
		if (character && Backpack) {
			ItemBackpack backpack = null;
			if(Network.isClient || Network.isServer){
				GameObject deadbody = (GameObject)Network.Instantiate (Backpack.gameObject, this.transform.position, Quaternion.identity, 2);
				backpack = deadbody.GetComponent<ItemBackpack> ();
			}else{
				GameObject deadbody = (GameObject)GameObject.Instantiate (Backpack.gameObject, this.transform.position, Quaternion.identity);
				backpack = deadbody.GetComponent<ItemBackpack> ();
			}

			if (backpack) {
				foreach (var item in character.inventory.Items) {
					if (character != null) {
						ItemData itemClone = character.inventory.itemManager.CloneItemData (item.Item);
						if (itemClone != null) {
	
							ItemCollector itemCol = new ItemCollector ();
							itemCol.Item = itemClone;
							itemCol.Num = item.Num;
							itemCol.NumTag = item.NumTag;
							itemCol.Active = true;
							backpack.AddItem (itemCol);
						}
					
					}
				}
			}
		}
	}
}
