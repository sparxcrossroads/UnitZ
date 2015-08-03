//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class ProjectileMove : MonoBehaviour {

	public float Speed = 100;
	void Start () {
		GameObject.Destroy(this.gameObject,2);
	}
	
	void FixedUpdate () {
		this.transform.position += this.transform.forward * Speed * Time.fixedDeltaTime;
	}
}
