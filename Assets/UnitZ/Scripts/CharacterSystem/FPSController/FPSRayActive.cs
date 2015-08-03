//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct casthit
{
	public int index;
	public float distance;
	public string name;
}

public class FPSRayActive : MonoBehaviour
{

	public bool Sorting;
	public string[] IgnoreTag = {"Player"};
	public string[] DestroyerTag = {"scene"};

	void Start ()
	{
		
	}

	void Update ()
	{
		
	}

	public void ShootRayOnce (Vector3 origin, Vector3 direction, string id, string team)
	{
		RaycastHit hit;
		if (Physics.Raycast (origin, direction, out hit, 100.0f)) {
			Debug.Log (hit.collider.name);
			if (hit.collider.gameObject != this.gameObject) {
				
				DamagePackage dm;
				dm.Damage = 50;
				dm.Normal = hit.normal;
				dm.Direction = direction;
				dm.Position = hit.point;
				dm.ID = id;
				dm.Team = team;
				hit.collider.SendMessage ("OnHit", dm, SendMessageOptions.DontRequireReceiver);
				
			}
		}
	}

	public void CheckingRay (Vector3 origin, Vector3 direction)
	{
		float raySize = 3;

		
		RaycastHit[] casterhits = Physics.RaycastAll (origin, direction, raySize);
		for (int i=0; i<casterhits.Length; i++) {
			if (casterhits [i].collider) {
				
				RaycastHit hit = casterhits [i];
				ItemData item = hit.collider.GetComponent<ItemData> ();
				if (item) {
					item.ShowInfo = true;
				}
				
				Vehicle vehicle = hit.collider.GetComponent<Vehicle> ();
				if (vehicle) {
					vehicle.ShowInfo = true;
				}
				
			}
		}

	}
	
	public void ActiveRay (Vector3 origin, Vector3 direction)
	{
		float raySize = 3;
		RaycastHit[] casterhits = Physics.RaycastAll (origin, direction, raySize);
		for (int i=0; i<casterhits.Length; i++) {
			if (casterhits [i].collider) {
				ItemData item = casterhits [i].collider.GetComponent<ItemData> ();
				if (item) {
					item.Pickup (this.GetComponent<CharacterInventory> ());
				}
				
				Vehicle vehicle = casterhits [i].collider.GetComponent<Vehicle> ();
				if (vehicle) {
					vehicle.Pickup (this.GetComponent<CharacterSystem> ());
				}
			}
		}
	}
	
	public bool ShootRay (Vector3 origin, Vector3[] direction, int damage, float size, int hitmax, string id, string team)
	{
		bool res = false;
		for (int b=0; b<direction.Length; b++) {
			int damages = damage;
			int hitcount = 0;
			int castcount = 0;
			RaycastHit[] hits;
			List<casthit> castHits = new List<casthit> ();
			float raySize = size;

		
			RaycastHit[] casterhits = Physics.RaycastAll (origin, direction [b], raySize);
			for (int i=0; i<casterhits.Length; i++) {
				if (casterhits [i].collider) {
					if (tagCheck (casterhits [i].collider.tag) && 
					casterhits [i].collider.gameObject != this.gameObject && 
					((casterhits [i].collider.transform.root && 
						casterhits [i].collider.transform.root != this.gameObject.transform.root &&
						casterhits [i].collider.transform.root.gameObject != this.gameObject) || 
						casterhits [i].collider.transform.root == null)) {
						castcount++;
						casthit casted = new casthit ();
						casted.distance = Vector3.Distance (origin, casterhits [i].point);
						casted.index = i;
						casted.name = casterhits [i].collider.name;
						castHits.Add (casted);
					}
				}
				
			}

			hits = new RaycastHit[castcount];
			castHits.Sort ((x,y) => x.distance.CompareTo (y.distance));

			for (int i=0; i<castHits.Count; i++) {
				hits [i] = casterhits [castHits [i].index];
			}

			for (var i = 0; i < hits.Length && hitcount < hitmax; i++) {
				RaycastHit hit = hits [i];
				
				DamagePackage dm;
				dm.Damage = damage;
				dm.Normal = hit.normal;
				dm.Direction = direction [b];
				dm.Position = hit.point;
				dm.ID = id;
				dm.Team = team;
				hit.collider.SendMessage ("OnHit", dm, SendMessageOptions.DontRequireReceiver);
				
				res = true;
				hitcount++;
				if (hitcount >= hitmax || tagDestroyerCheck (hit.collider.tag)) {
					break;
				}
				int damageReduce = (int)((float)damages * 0.75f);
				damages = damageReduce;
			}
		}
		return res;
	}
	
	public bool ShootRayTest (Vector3 origin, Vector3[] direction, int damage, float size, int hitmax, string id, string team)
	{
		bool res = false;
		for (int b=0; b<direction.Length; b++) {
			int hitcount = 0;
			int castcount = 0;
			RaycastHit[] hits;
			List<casthit> castHits = new List<casthit> ();
			float raySize = size;

		
			RaycastHit[] casterhits = Physics.RaycastAll (origin, direction [b], raySize);
			for (int i=0; i<casterhits.Length; i++) {
				if (casterhits [i].collider) {
					if (tagCheck (casterhits [i].collider.tag) && 
					casterhits [i].collider.gameObject != this.gameObject && 
					((casterhits [i].collider.transform.root && 
						casterhits [i].collider.transform.root.gameObject != this.gameObject) || 
						casterhits [i].collider.transform.root == null)) {
						castcount++;
						casthit casted = new casthit ();
						casted.distance = Vector3.Distance (origin, casterhits [i].point);
						casted.index = i;
						casted.name = casterhits [i].collider.name;
						castHits.Add (casted);
					}
				}
				
			}

			hits = new RaycastHit[castcount];
			castHits.Sort ((x,y) => x.distance.CompareTo (y.distance));

			for (int i=0; i<castHits.Count; i++) {
				hits [i] = casterhits [castHits [i].index];
			}

			for (var i = 0; i < hits.Length && hitcount < hitmax; i++) {
				RaycastHit hit = hits [i];
				
				res = true;
				hitcount++;
				if (hitcount >= hitmax || tagDestroyerCheck (hit.collider.tag)) {
					break;
				}
				
			}
		}
		return res;
	}
	
	public bool Overlap (Vector3 origin, Vector3 forward, int damage, float size, float dot, string id, string team)
	{

		bool res = false;
		var colliders = Physics.OverlapSphere (origin, size);
		
		foreach (var hit in colliders) {
			if (hit && hit.gameObject != this.gameObject && hit.gameObject.transform.root != this.gameObject.transform) {
				Debug.Log (hit.gameObject.transform.root.name);
				var dir = (hit.transform.position - origin).normalized;
				var direction = Vector3.Dot (dir, forward);	
			
				if (direction >= dot) {
					DamagePackage dm;
					dm.Damage = damage;
					dm.Normal = dir;
					dm.Direction = forward;
					dm.Position = hit.gameObject.transform.position;
					dm.ID = id;
					dm.Team = team;
					hit.GetComponent<Collider>().SendMessage ("OnHit", dm, SendMessageOptions.DontRequireReceiver);
					
					res = true;
				}
			}
		}
		return res;
	}

	public bool OverlapTest (Vector3 origin, Vector3 forward, int damage, float size, float dot, string id, string team)
	{
		bool res = false;
		var colliders = Physics.OverlapSphere (origin, size);
		
		foreach (var hit in colliders) {
			if (hit && hit.gameObject != this.gameObject && hit.gameObject.transform.root != this.gameObject) {
				var dir = (hit.transform.position - origin).normalized;
				var direction = Vector3.Dot (dir, forward);	
			
				if (direction >= dot) {
					res = true;
				}
			}
		}
		
		return res;
	}
	
	private bool tagDestroyerCheck (string tag)
	{
		for (int i=0; i<DestroyerTag.Length; i++) {
			if (DestroyerTag [i] == tag) {
				return true;	
			}
		}
		return false;
	}

	private bool tagCheck (string tag)
	{
		for (int i=0; i<IgnoreTag.Length; i++) {
			if (IgnoreTag [i] == tag) {
				return false;	
			}
		}
		return true;
	}
}
