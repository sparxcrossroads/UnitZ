//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public enum MortorType
{
	Rear,
	Front,
	FourWheel
}

public class CarControl : MonoBehaviour
{
	public MortorType Type;
	public WheelCollider Wheel_FL;
	public WheelCollider Wheel_FR;
	public WheelCollider Wheel_RL;
	public WheelCollider Wheel_RR;
	public float[] GearRatio;
	public int CurrentGear = 0;
	public float EngineTorque = 600.0f;
	public float MaxEngineRPM = 3000.0f;
	public float MinEngineRPM = 1000.0f;
	public float SteerAngle = 10;
	public Transform COM;
	public float Speed;
	public float maxSpeed = 150;
	public AudioSource skidAudio;
	public float EngineRPM = 0.0f;
	private float motorInput;
	public NetworkView networkViewer;
	
	void Awake(){
		networkViewer = this.GetComponent<NetworkView>();	
	}
	
	void Start ()
	{
		GetComponent<Rigidbody>().centerOfMass = new Vector3 (COM.localPosition.x * transform.localScale.x, COM.localPosition.y * transform.localScale.y, COM.localPosition.z * transform.localScale.z);
	}
	
	private Vector2 inputAxis;
	private bool inputBrake;
	public float steerAngleTemp;
	
	public void Controller (Vector2 input, bool brake)
	{
		inputAxis = input;
		inputBrake = brake;
	}
	
	void Update ()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer)) {
			if (GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().isKinematic = false;
			
			Speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
			GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 100;
			EngineRPM = (Wheel_FL.rpm + Wheel_FR.rpm) / 2 * GearRatio [CurrentGear];
	
			ShiftGears ();
	
			//Input For MotorInput.
			motorInput = inputAxis.y;
	
			//Audio
			GetComponent<AudioSource>().pitch = Mathf.Abs (EngineRPM / MaxEngineRPM) + 1.0f;
			if (GetComponent<AudioSource>().pitch > 2.0f) {
				GetComponent<AudioSource>().pitch = 2.0f;
			}
	
			//Steering
			steerAngleTemp = SteerAngle * inputAxis.x;
			Wheel_FL.steerAngle = steerAngleTemp;
			Wheel_FR.steerAngle = steerAngleTemp;
	
			//Speed Limiter.
			if (Speed > maxSpeed) {
				Wheel_RL.motorTorque = 0;
				Wheel_RR.motorTorque = 0;
				Wheel_FL.motorTorque = 0;
				Wheel_FR.motorTorque = 0;
			} else {
				switch (Type) {
				case MortorType.FourWheel:
					Wheel_FL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_FR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_RL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_RR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					break;
				case MortorType.Front:
					Wheel_FL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_FR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					break;
				case MortorType.Rear:
					Wheel_RL.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					Wheel_RR.motorTorque = (EngineTorque / GearRatio [CurrentGear]) * inputAxis.y;
					break;
				}
			}
	
			//Input.
			if (motorInput <= 0) {
				Wheel_RL.brakeTorque = 30;
				Wheel_RR.brakeTorque = 30;
			} else if (motorInput >= 0) {
				Wheel_RL.brakeTorque = 0;
				Wheel_RR.brakeTorque = 0;
			}
	
			//SkidAudio.
			WheelHit CorrespondingGroundHit;
			Wheel_RR.GetGroundHit (out CorrespondingGroundHit);
			if (Mathf.Abs (CorrespondingGroundHit.sidewaysSlip) > 10) {
				skidAudio.enabled = true;
			} else {
				skidAudio.enabled = false;
			}
	
			//HandBrake
			if (inputBrake) {
				Wheel_FL.brakeTorque = 100;
				Wheel_FR.brakeTorque = 100;
			}
			if (!inputBrake) {
				Wheel_FL.brakeTorque = 0;
				Wheel_FR.brakeTorque = 0;
			}
		
			inputAxis = Vector2.zero;
			inputBrake = false;
			
			
			if (networkViewer && Network.isServer)
				networkViewer.RPC ("engineUpdate", RPCMode.Others, GetComponent<AudioSource>().pitch, steerAngleTemp, Wheel_FL.motorTorque, Wheel_FR.motorTorque, Wheel_RL.motorTorque, Wheel_RR.motorTorque);
		
		}
		if (Network.isClient) {
			if (GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().isKinematic = true;	
			
			Wheel_FL.steerAngle = steerAngleTemp;
			Wheel_FR.steerAngle = steerAngleTemp;
			
		}
	}
	
	[RPC]
	public void engineUpdate (float pitch, float steerAngle, float w_FL, float w_FR, float w_RL, float w_RR)
	{
		GetComponent<AudioSource>().pitch = pitch;
		if (GetComponent<AudioSource>().pitch > 2.0f) {
			GetComponent<AudioSource>().pitch = 2.0f;
		}

		Wheel_FL.motorTorque = w_FL;
		Wheel_FR.motorTorque = w_FR;
		Wheel_RL.motorTorque = w_RL;
		Wheel_RR.motorTorque = w_RR;
		
		steerAngleTemp = steerAngle;
	}
	
	void ShiftGears ()
	{
		int AppropriateGear = CurrentGear;
		if (EngineRPM >= MaxEngineRPM) {
			
			for (int i = 0; i < GearRatio.Length; i ++) {
				if (Wheel_FL.rpm * GearRatio [i] < MaxEngineRPM) {
					AppropriateGear = i;
					break;
				}
			}
			CurrentGear = AppropriateGear;
		}
	
		if (EngineRPM <= MinEngineRPM) {
			AppropriateGear = CurrentGear;
			for (int j = GearRatio.Length-1; j >= 0; j --) {
				if (Wheel_FL.rpm * GearRatio [j] > MinEngineRPM) {
					AppropriateGear = j;
					break;
				}
			}
			CurrentGear = AppropriateGear;
		}
	}
}
