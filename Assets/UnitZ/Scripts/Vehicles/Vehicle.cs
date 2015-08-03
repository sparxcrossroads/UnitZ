//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CarControl))]
public class Vehicle : DamageManager
{

	public Seat[] Seats;
	public string VehicleName;
	public string VehicleID;
	[HideInInspector]
	public bool incontrol;

	void Awake ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		if (Network.isServer) {
			VehicleID = "V_" + this.transform.GetInstanceID ();
		}
		
		DontDestroyOnLoad (this.gameObject);
		
		if (Seats.Length <= 0) {
			var seat = this.GetComponentsInChildren (typeof(Seat));
			Seats = new Seat[seat.Length];
			for (int i=0; i<seat.Length; i++) {
				Seats [i] = seat [i].GetComponent<Seat> ();	
			}
		}
	}
	
	void Start ()
	{
	}
	
	public virtual void Pickup (CharacterSystem character)
	{
		if (character && (character.IsMine || (!Network.isClient && !Network.isServer))) {
			CharacterDriver driver = character.GetComponent<CharacterDriver> ();
			if (driver) {
				for (int i=0; i<Seats.Length; i++) {
					if (Seats [i].passenger == null) {
						Debug.Log ("Pick up " + character.name);
						Seats [i].GetIn (driver);
						break;
					} 
				}
			}
		}
	}

	private Seat FindOpenSeat ()
	{
		for (int i=0; i<Seats.Length; i++) {
			if (!Seats [i].Active) {
				return Seats [i];
			} 
		}
		return null;
	}

	public virtual void Drive (Vector2 input, bool brake)
	{
	}

	public virtual void Ejected ()
	{
	}
	
	public void UpdateFunction ()
	{
		UpdateTrasform ();
		DamageUpdate();
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("UpdateVehicle", RPCMode.Others, VehicleID);
		}
	}
	
	void Update ()
	{
		
		UpdateFunction ();
		UpdateDriver ();
	
	}
	
	public void UpdateDriver ()
	{
		for (int i=0; i<Seats.Length; i++) {
			if (Seats [i].IsDriver && Seats [i].passenger != null) {
				return;
			} 
		}
		incontrol = false;	
	}
	
	public void UpdateTrasform ()
	{
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("UpdateTransform", RPCMode.Others, this.transform.position, this.transform.rotation);
		}
	}
	
	public void FixedUpdate ()
	{
		ShowInfo = false;	
	}
	
	public bool ShowInfo;
	
	void OnGUI ()
	{
		if (ShowInfo) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint (this.gameObject.transform.position);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label (new Rect (screenPos.x, Screen.height - screenPos.y, 200, 60), "Get in\n" + VehicleName);
		}
	}
	
	[RPC]
	public void UpdateTransform (Vector3 position, Quaternion rotation)
	{
		this.transform.position = Vector3.Lerp (this.transform.position, position, 0.5f);
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rotation, 0.5f);
	}
	
	[RPC]
	public void UpdateVehicle (string id)
	{
		VehicleID = id;	
	}
	
	public void UpdateSeat (CharacterDriver driver, Seat seat, bool sit)
	{
		driver.OnVehicle (VehicleID, seat.SeatID, sit);
	}

}
