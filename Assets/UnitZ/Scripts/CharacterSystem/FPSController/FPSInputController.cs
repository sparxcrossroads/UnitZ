//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FPSController))]


public class FPSInputController : MonoBehaviour
{
	public FPSController FPSmotor;
	public CharacterDriver Driver;
	
	void Start ()
	{
		FPSmotor = GetComponent<FPSController> ();	
		Driver = GetComponent<CharacterDriver>();
		Application.targetFrameRate = 60;
	}

	void Awake ()
	{
		MouseLock.MouseLocked = true;
	}

	void Update ()
	{
		if(FPSmotor == null || FPSmotor.character == null)
			return; 
		
		FPSItemEquipment FPSitem = null;
		if (FPSmotor.character.IsMine) {
			
			if ((Driver && Driver.DrivingSeat == null) || Driver == null) {
				
				FPSmotor.Move (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")));
				FPSmotor.Jump (Input.GetButton ("Jump"));
			
			} else {
				if(Driver){
					if (Input.GetKeyDown (KeyCode.F)) {
						Driver.OutVehicle ();
					}
					Driver.Drive (new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical")), Input.GetButton ("Jump"));
				}
			}
			
			if (Input.GetKey (KeyCode.LeftShift)) {
				FPSmotor.Boost (1.4f);	
			}
			
			if (MouseLock.MouseLocked) {
				FPSmotor.Aim (new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y")));

				if (FPSmotor.character.inventory != null && FPSmotor.character.inventory.FPSEquipment != null) {
					FPSitem = FPSmotor.character.inventory.FPSEquipment;
				}
				
				if (FPSitem != null) {
					if (Input.GetButton ("Fire1")) {
						
						FPSitem.Trigger ();
					} else {
						FPSitem.OnTriggerRelease ();
					}
					if (Input.GetButtonDown ("Fire2")) {
						
						FPSitem.Trigger2 ();
					} else {
						FPSitem.OnTrigger2Release ();
					}
				}
			}
			
			if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
				
			}
			if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
				
			}
			if (Input.GetKeyDown (KeyCode.F)) {
				if (FPSmotor.character.inventory != null)
					FPSmotor.character.Interactive (FPSmotor.FPSCamera.transform.position, FPSmotor.FPSCamera.transform.forward);
			}
			if (Input.GetKeyDown (KeyCode.R)) {
				if (FPSmotor.character.inventory != null && FPSmotor.character.inventory.FPSEquipment != null)
					FPSmotor.character.inventory.FPSEquipment.Reload ();
			}
			if (Input.GetKeyDown (KeyCode.Q)) {
				
			}
			
			/*if (Input.GetKeyDown (KeyCode.E)) {
				if (FPSmotor.character.inventory != null) {
					FPSmotor.character.inventory.Toggle = !FPSmotor.character.inventory.Toggle;
					MouseLock.MouseLocked = !FPSmotor.character.inventory.Toggle;
				}
			}*/
		
			FPSmotor.character.Checking (FPSmotor.FPSCamera.transform.position, FPSmotor.FPSCamera.transform.forward);
		}
	}

}
