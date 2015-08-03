//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class Car : Vehicle
{
	public AudioClip ClashSound;
	public int ClashDamage = 10000;
	public CarControl car;
	private Vector2 inputTemp;
	private bool brakeTemp;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView>();
		Audiosource = this.GetComponent<AudioSource> ();
		car = this.gameObject.GetComponent<CarControl> ();	
	}
	
	public override void Drive (Vector2 input, bool brake)
	{
		if (Network.isClient) {
			if(networkViewer)
				networkViewer.RPC ("driving", RPCMode.Server, new Vector3 (input.x, input.y, 0), brake);
		} else {
			driving (new Vector3 (input.x, input.y, 0), brake);
		}
		
		base.Drive (input, brake);
	}
	
	[RPC]
	public void driving (Vector3 input, bool brake)
	{
		inputTemp = input;
		brakeTemp = brake;
		incontrol = true;
		
		if (input.x == 0 && input.y == 0 && !brake) {
			incontrol = false;	
		}
	}
	
	void Update ()
	{
		if (incontrol) {
			if (car)
				car.Controller (new Vector2 (inputTemp.x, inputTemp.y), brakeTemp);
		}
		UpdateFunction ();
	}
	
	void OnCollisionEnter (Collision collision)
	{
		if(Audiosource && ClashSound && GetComponent<Rigidbody>().velocity.magnitude>3){
			Audiosource.PlayOneShot(ClashSound);
		}
		
		DamagePackage dm = new DamagePackage();
		dm.Damage = (int)(ClashDamage * GetComponent<Rigidbody>().velocity.magnitude);
		dm.Normal = this.GetComponent<Rigidbody>().velocity.normalized;
		dm.Direction = this.GetComponent<Rigidbody>().velocity * 2;
		dm.Position = this.transform.position;
		
		if (Seats.Length > 0 && Seats [0].passenger) {
			dm.ID = Seats [0].passenger.character.ID;
			dm.Team = "car";
		}
		collision.collider.SendMessage ("DirectDamage", dm, SendMessageOptions.DontRequireReceiver);
        
	}
	
}
