//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterDriver : MonoBehaviour
{

	[HideInInspector]
	public Seat DrivingSeat;
	[HideInInspector]
	public string LastSeat;
	[HideInInspector]
	public NetworkView networkViewer;
	[HideInInspector]
	public CharacterSystem character;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		character = this.GetComponent<CharacterSystem> ();
	}
	
	public void Drive (Vector2 input, bool brake)
	{
		if (DrivingSeat && DrivingSeat.IsDriver) {
			if (DrivingSeat.VehicleRoot)
				DrivingSeat.VehicleRoot.Drive (new Vector2 (input.x, input.y), brake);
		}
	}
	
	public void OutVehicle ()
	{
		if (character != null)
			DrivingSeat.GetOut (this);
	}
	
	public void OnVehicle (string vehicleID, string seatID, bool isSit)
	{
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("onVehicle", RPCMode.Others, vehicleID, seatID, isSit);
		} 
	}
	
	[RPC]
	private void onVehicle (string vehicleID, string seatID, bool isSit)
	{
		if (character == null)
			return;
		
		Vehicle[] vehicles = (Vehicle[])GameObject.FindObjectsOfType (typeof(Vehicle));	
		foreach (Vehicle vehicle in vehicles) {
			if (vehicle.VehicleID == vehicleID) {
				foreach (Seat seat in vehicle.Seats) {
					if (seat.SeatID == seatID) {
						seat.OnSeat (this, isSit);
						break;
					}
				}
				break;
			}
		}
	}
}
