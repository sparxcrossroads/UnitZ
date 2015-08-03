//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Seat : MonoBehaviour
{
	public Vehicle VehicleRoot;
	public string SeatID = "S1";
	public string CharacterID = "";
	public bool IsDriver = false;
	public bool Active = false;
	public CharacterDriver passenger;
	
	void Start ()
	{
		if (VehicleRoot == null) {
			if (this.transform.root)
				VehicleRoot = this.transform.root.GetComponent<Vehicle> ();
		}
	}
	
	public void GetIn (CharacterDriver driver)
	{
		if (driver) {
			Active = true;
			driver.transform.position = this.transform.position;
			driver.transform.parent = this.transform;
			driver.character.controller.enabled = false;
			driver.DrivingSeat = this;
			driver.LastSeat = SeatID;
			passenger = driver;
			if(VehicleRoot!=null)
				VehicleRoot.UpdateSeat (driver, this, true);
		}
	}
	
	public void GetOut (CharacterDriver driver)
	{
		if (driver) {
			Active = false;
			driver.transform.parent = null;
			driver.character.controller.enabled = true;
			driver.DrivingSeat = null;
			passenger = null;
			if(VehicleRoot!=null)
				VehicleRoot.UpdateSeat (driver, this, false);
		}
	}
	
	public void OnSeat (CharacterDriver driver, bool sit)
	{
		if (driver) {
			if (sit) {
				Active = true;
				passenger = driver;
				driver.character.controller.enabled = false;
				driver.DrivingSeat = this;
			} else {
				driver.transform.parent = null;
				driver.character.controller.enabled = true;
				driver.DrivingSeat = null;	
				passenger = null;
			}
		}
		
	}

	public void CleanSeat ()
	{
		CharacterDriver[] pasengers = (CharacterDriver[])this.transform.GetComponentsInChildren<CharacterDriver> ();
		for (int i=0; i<pasengers.Length; i++) {
			if (pasengers [i]) {
				pasengers [i].transform.parent = null;	
			}
		}
		Active = false;
		passenger = null;
		Debug.Log(" Clean Seat! "+SeatID);
	}
	public void CheckSeat ()
	{
		if(this.transform.childCount <=0){
			Active = false;
			passenger = null;
		}
	}
	
	void Update ()
	{
		if (passenger && passenger.networkViewer.isMine) {
			CharacterID = passenger.character.ID;
			passenger.OnVehicle (VehicleRoot.VehicleID, SeatID, true);	
		}else{
			Active = false;
		}
		
		CheckSeat ();
	}
	
	void FixedUpdate ()
	{
		if (passenger) {
			passenger.transform.position = this.transform.position;
			passenger.transform.parent = this.transform;
		} 
	}
	
	void OnDrawGizmos ()
	{
		#if UNITY_EDITOR
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawWireCube (transform.position, transform.localScale);
		Handles.Label(transform.position, "Seat");
		#endif
	}
}
