using UnityEngine;
using System.Collections;

public class TreasureBox : DamageManager
{

	public ItemData[] Item;
	public Vector3 Offset = new Vector3 (0, 0.1f, 0);
	
	public void OnThisThingDead ()
	{
		Spawn ();	
	}
	
	void Spawn ()
	{
		if (Item.Length > 0) {
			ItemData itemPick = Item [Random.Range (0, Item.Length)];

			Vector3 spawnPoint = DetectGround (transform.position + Vector3.up);
			if ((Network.isServer || Network.isClient)) {
				// Only server can spawn an items.
				if (Network.isServer) { 
					Network.Instantiate (itemPick.gameObject, spawnPoint, Quaternion.identity, 2);
				
				}
			} else {
				// Spawn in offline mode.
				GameObject.Instantiate (itemPick.gameObject, spawnPoint, Quaternion.identity);
				
			}

		}
	}
	
	Vector3 DetectGround (Vector3 position)
	{
		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit, 1000.0f)) {
			return hit.point + Offset;
		}
		return position;
	}
}
