//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(FPSInputController))]
[RequireComponent(typeof(CharacterInventory))]
[RequireComponent(typeof(CharacterHUD))]
[RequireComponent(typeof(UIInventory))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSController : MonoBehaviour
{
	[HideInInspector]
	public CharacterSystem character;
	[HideInInspector]
	public CharacterMotor motor;
	[HideInInspector]
	public Vector3 inputDirection;
	private Vector2 mouseDirection;
	public Transform FPSViewPart;
	public Camera FPSCamera;
	[HideInInspector]
	public float sensitivityXMult = 1;
	[HideInInspector]
	public float sensitivityYMult = 1;
	public float sensitivityX = 15;
	public float sensitivityY = 15;
	public float minimumX = -360;
	public float maximumX = 360;
	public float minimumY = -60;
	public float maximumY = 60;
	public float delayMouse = 0.5f;

	private float rotationX = 0;
	private float rotationY = 0;
	private float rotationXtemp = 0;
	private float rotationYtemp = 0;
	private Quaternion originalRotation;
	private Vector2 kickPower;
	private float fovTemp = 40;
	private float fovTarget;
	[HideInInspector]
	public bool zooming = false;

	
	void Start ()
	{
		character = gameObject.GetComponent<CharacterSystem> ();
		PlayerView pv = gameObject.GetComponent<PlayerView>();
		if(pv && FPSCamera == null){
			FPSCamera = pv.FPScamera.MainCamera;
			FPSViewPart = pv.FPScamera.transform;
		}
		
		motor = GetComponent<CharacterMotor> ();
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		
		originalRotation = transform.localRotation;
		if(FPSCamera){
			fovTemp = FPSCamera.fieldOfView;
			fovTarget = fovTemp;
		}
	}
	
	public void Zoom(float zoom){
		fovTarget = zoom;
		zooming = !zooming;
	}
	public void Kick(Vector2 power){
		kickPower = power;
	}
	
	public void HideGun (bool visible)
	{
		if (FPSCamera && FPSCamera.GetComponent<Camera>()) {
			FPSCamera.GetComponent<Camera>().enabled = visible;	
		}
	}

	public void Boost (float mult)
	{
		motor.boostMults = mult;
	}
	
	float climbDirection;
	public void Climb(float speed){
		motor.Climb(speed);
		
	}
	
	public void Move (Vector3 directionVector)
	{
		if(character == null)
			return;
		
		inputDirection = directionVector;
		if (directionVector != Vector3.zero) {
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			directionLength = Mathf.Min (1, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		
		Quaternion rotation = transform.rotation;
		
		if (FPSViewPart) {
			rotation = FPSViewPart.transform.rotation;
		}
		Vector3 angle = rotation.eulerAngles; 
		angle.x = 0;
		angle.z = 0;
		rotation.eulerAngles = angle;
		character.MoveTo(rotation * directionVector);
	}

	public void Jump (bool jump)
	{
		motor.inputJump = jump;
	}
	
	public void Aim (Vector2 direction)
	{
		mouseDirection = direction;	
		
	}
	
	void Update ()
	{
		if(!MouseLock.MouseLocked || character == null)
			return;
		
		if(character.IsMine){
			if(!zooming){
				sensitivityXMult = 1;
				sensitivityYMult = sensitivityXMult;
				fovTarget = fovTemp;	
			}else{
				sensitivityXMult = fovTarget/fovTemp;
				sensitivityYMult = sensitivityXMult;
			}
			if(FPSCamera)
				FPSCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(FPSCamera.GetComponent<Camera>().fieldOfView,fovTarget,0.5f);

			motor.boostMults += (1 - motor.boostMults) * Time.deltaTime;
			kickPower.y += (0 - kickPower.y) / 20f;
			kickPower.x += (0 - kickPower.x) / 20f;
			
			rotationXtemp += (mouseDirection.x * sensitivityX * sensitivityXMult);
			rotationYtemp += (mouseDirection.y * sensitivityY * sensitivityYMult);
			rotationX = Mathf.Lerp(rotationX ,rotationXtemp, delayMouse);
			rotationY = Mathf.Lerp(rotationY ,rotationYtemp, delayMouse);


			if (rotationX >= 360) {
				rotationX = 0;
				rotationXtemp = 0;
			}
			if (rotationX <= -360) {
				rotationX = 0;
				rotationXtemp = 0;
			}
		
			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);
			rotationYtemp = ClampAngle (rotationYtemp, minimumY, maximumY);
      
			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX + kickPower.x, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY + kickPower.y, Vector3.left);
		
			if (FPSViewPart) {
				FPSViewPart.transform.localRotation = originalRotation * Quaternion.AngleAxis (0, Vector3.up) * yQuaternion;
				this.transform.localRotation = originalRotation * xQuaternion * Quaternion.AngleAxis (0, Vector3.left);
			} else {
				this.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			}
		}
	}

	public void Stun (float val)
	{
		kickPower.y = val;
	}

	static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360.0f)
			angle += 360.0f;

		if (angle > 360.0f)
			angle -= 360.0f;

		return Mathf.Clamp (angle, min, max);
	}
}
