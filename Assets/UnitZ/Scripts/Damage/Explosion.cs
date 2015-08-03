//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : DamageBase
{
	
	public float Duration = 3;
	public float Force = 100;
	public float Radius = 30;
	public int Damage = 100;
	public string[] BlockerTag = {"Scene"};

	void Start ()
	{
		Collider[] colliders = Physics.OverlapSphere (this.transform.position, Radius);
		foreach (Collider hit in colliders) {
			float distance = Vector3.Distance (this.transform.position, hit.transform.position);
			float dmMult = 1 - ((1.0f / Radius) * distance);
			if (dmMult < 0)
				dmMult = 0;

			int castcount = 0;
			RaycastHit[] hits;
			List<casthit> castHits = new List<casthit> ();
			RaycastHit[] casterhits = Physics.RaycastAll (this.transform.position, (hit.transform.position - this.transform.position).normalized, distance + 0.2f);
			for (int i=0; i<casterhits.Length; i++) {
				if (casterhits [i].collider) {
					if (casterhits [i].collider.gameObject != this.gameObject) {
						castcount++;
						casthit casted = new casthit ();
						casted.distance = Vector3.Distance (this.transform.position, casterhits [i].point);
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
			
			int hitcount = 0;
			bool pass = true;
			for (var i = 0; i < hits.Length && hitcount < 10; i++) {
				if (hits [i].collider.gameObject == hit.GetComponent<Collider>().gameObject) {
					pass = true;
					break;
				}
				if (tagDestroyerCheck(hits[i].collider.tag)) {
					pass = false;
					break;
				}
				hitcount++;
			}
			
			if (pass) {
				if (hit && hit.GetComponent<Rigidbody>())
					hit.GetComponent<Rigidbody>().AddExplosionForce (Force, this.transform.position, Radius, 3.0F);
			

				DamagePackage dm;
				dm.Damage = (int)((float)Damage * dmMult);
				dm.Normal = hit.transform.forward;
				dm.Direction = (hit.transform.position - this.transform.position).normalized * Force;
				dm.Position = hit.transform.position;
				dm.ID = OwnerID;
				dm.Team = OwnerTeam;
				hit.GetComponent<Collider>().SendMessage ("OnHit", dm, SendMessageOptions.DontRequireReceiver);
			}
			
		}
		Destroy(this.gameObject,Duration);
	}
	
	
	private bool tagDestroyerCheck (string tag)
	{
		for (int i=0; i<BlockerTag.Length; i++) {
			if (BlockerTag [i] == tag) {
				return true;	
			}
		}
		return false;
	}
	

}
