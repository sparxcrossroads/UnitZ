//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(CharacterFootStep))]
[RequireComponent(typeof(FPSRayActive))]
[RequireComponent(typeof(NetworkView))]

public class CharacterSystem : DamageManager
{
	[HideInInspector]
	public CharacterInventory inventory;
	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public FPSRayActive rayActive;
	[HideInInspector]
	public CharacterController controller;
	[HideInInspector]
	public bool OnUpdate;
	[HideInInspector]
	public CharacterMotor Motor;
	[HideInInspector]
	public bool Sprint;
	public float MoveSpeed = 0.7f;
	public float MoveSpeedMax = 5;
	public float TurnSpeed = 5;
	public float PrimaryWeaponDistance = 1;
	public int PrimaryItemType;
	public int AttackType = 0;
	public int Damage = 2;
	public float DamageLength = 1;
	public int Penetrate = 1;
	public Vector3 DamageOffset = Vector3.up;
	public AudioClip[] DamageSound;
	public AudioClip[] SoundIdle;
	[HideInInspector]
	public float spdMovAtkMult = 1;
	
	void Awake ()
	{
		SetupAwake ();
	}
	
	public void SetupAwake ()
	{
		
		DontDestroyOnLoad (this.gameObject);
		networkViewer = this.GetComponent<NetworkView> ();
		Motor = this.GetComponent<CharacterMotor> ();
		controller = this.GetComponent<CharacterController> ();
		Audiosource = this.GetComponent<AudioSource> ();
		animator = this.GetComponent<Animator> ();
		rayActive = this.GetComponent<FPSRayActive> ();
		inventory = this.GetComponent<CharacterInventory> ();
		spdMovAtkMult = 1;
	}

	void Update ()
	{
		UpdateFunction ();
	}
	
	public void UpdateFunction ()
	{
		Motor.ID = ID;
		Motor.OnUpdate = OnUpdate;
		animator.enabled = OnUpdate;
		
		if (OnUpdate) 
			UpdatePosition ();
		
		
		DamageUpdate ();
	}
		
	public virtual void PlayAttackAnimation (bool attacking, int attacktype)
	{
		/*if (animator) {
			if (attacking) {
				animator.SetTrigger ("Shoot");
			}
			animator.SetInteger ("Shoot_Type", attacktype);
		}*/
	}
	
	public virtual void PlayMoveAnimation (float magnitude)
	{
		/*if (animator) {
			if (magnitude > 0.4f) {
				animator.SetInteger ("LowerState", 1);
			} else {
				animator.SetInteger ("LowerState", 0);
			}
		}*/
	}
	
	public void MoveAnimation ()
	{
		PlayMoveAnimation (Motor.OjectVelocity.magnitude);
	}
	
	[RPC]
	public void MoveTo (Vector3 dir)
	{
		float speed = MoveSpeed;
		if (Sprint)
			speed = MoveSpeedMax;
		
		Move (dir * speed * spdMovAtkMult);
		MoveAnimation ();
	}

	[RPC]
	public void MoveToPosition (Vector3 position)
	{
		float speed = MoveSpeed;
		if (Sprint)
			speed = MoveSpeedMax;
		Vector3 direction = (position - transform.position);
		direction = Vector3.ClampMagnitude (direction, 1);
		direction.y = 0;
		Move (direction.normalized * speed * direction.magnitude * spdMovAtkMult);
		if (direction != Vector3.zero) {
			Quaternion newrotation = Quaternion.LookRotation (direction);
			transform.rotation = Quaternion.Slerp (transform.rotation, newrotation, Time.deltaTime * TurnSpeed * direction.magnitude);
		}
		MoveAnimation ();
	}
	
	[RPC]
	public void UpdateTransform (Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		if (this.transform.parent == null) {
			this.transform.position = Vector3.Lerp (this.transform.position, position, 0.5f);
		}
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rotation, 0.5f);
		Motor.OjectVelocity = velocity;
		MoveAnimation ();
		
	}
	
	public void UpdatePosition ()
	{
		if (networkViewer && (IsMine || (ID == "" && networkViewer.isMine)) && (Network.isClient || Network.isServer)) {
			networkViewer.RPC ("UpdateTransform", RPCMode.Others, this.transform.position, this.transform.rotation, Motor.OjectVelocity);
		}
	}

	public void AttackAnimation (int attacktype)
	{
		AttackType = attacktype;
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("attackAnimation", RPCMode.All, attacktype);
		} else {
			attackAnimation (attacktype);
		}
	}
	
	public void AttackTo (Vector3 direction, int attacktype)
	{
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("attackTo", RPCMode.All, direction, attacktype);
		} else {
			attackTo (direction, attacktype);
		}
	}

	[RPC]
	private void attackAnimation (int attacktype)
	{
		PlayAttackAnimation (true, attacktype);
	}
	
	[RPC]
	private void attackTo (Vector3 direction, int attacktype)
	{
		PlayAttackAnimation (true, attacktype);
	}
	
	public void AttackAnimation ()
	{
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("attackAnimation", RPCMode.All, AttackType);
		} else {
			attackAnimation (AttackType);
		}
	}
	
	public void UpdateAnimationState ()
	{
		if (networkViewer && (Network.isClient || Network.isServer))
			networkViewer.RPC ("updateAnimation", RPCMode.Others, AttackType);	
	}
	
	[RPC]
	private void updateAnimation (int attacktype)
	{
		PlayAttackAnimation (false, attacktype);
	}
	
	public string ConvertArrayToString (Vector3[] vector3s)
	{
		string res = "";
		for (int i=0; i<vector3s.Length; i++) {
			res += vector3s [i].x + "," + vector3s [i].y + "," + vector3s [i].z;
			if (i < vector3s.Length - 1)
				res += "|";
		}
		return res;
	}
	
	public Vector3[] ReadArrayFromString (string vector3stext)
	{
		string[] vector3s = vector3stext.Split ("|" [0]);
		Vector3[] res = new Vector3[vector3s.Length];
		for (int i=0; i<vector3s.Length; i++) {
			string[] vecs = vector3s [i].Split ("," [0]);
			res [i].x = float.Parse (vecs [0]);
			res [i].y = float.Parse (vecs [1]);
			res [i].z = float.Parse (vecs [2]);
				
		}
		return res;
	}
#if UNITY_4_6_3 || UNITY_4_6_4 || UNITY_4_6_5 
	[RPC]
	public void doDamage (Vector3 origin, string direction, int damage, float distance, int penetrate, string id, string team)
	{
		//Debug.Log(direction);
		if (rayActive) {
			if (rayActive.ShootRay (origin, ReadArrayFromString (direction), damage, distance, penetrate, id, team))
				PlayDamageSound ();
		}
				
		if (inventory) {
			inventory.EquipmentOnAction ();	
		}
	}
	
	public void DoDamage (Vector3 origin, Vector3[] direction, int damage, float distance, int penetrate, string id, string team)
	{
		
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("doDamage", RPCMode.All, origin, ConvertArrayToString (direction), damage, distance, penetrate, id, team);
			if (rayActive) {
				if (rayActive.ShootRayTest (origin, direction, damage, distance, penetrate, id, team))
					PlayDamageSound ();
			}
		} else {
			doDamage (origin, ConvertArrayToString (direction), damage, distance, penetrate, id, team);
		}
	}
	
	public void DoDamage ()
	{

		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("doDamage", RPCMode.All, this.transform.position + DamageOffset, this.transform.forward, Damage, DamageLength, Penetrate, ID, Team);
		} else {
			Vector3[] direction = {this.transform.forward};
			doDamage (this.transform.position + DamageOffset, ConvertArrayToString (direction), Damage, DamageLength, Penetrate, ID, Team);
		}
	}
#else
	[RPC]
	public void doDamage (Vector3 origin, Vector3[] direction, int damage, float distance, int penetrate, string id, string team)
	{
		if (rayActive) {
			if (rayActive.ShootRay (origin, direction, damage, distance, penetrate, id, team))
				PlayDamageSound ();
		}
				
		if (inventory) {
			inventory.EquipmentOnAction ();	
		}
	}
	
	public void DoDamage (Vector3 origin, Vector3[] direction, int damage, float distance, int penetrate, string id, string team)
	{
		
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("doDamage", RPCMode.All, origin, direction, damage, distance, penetrate, id, team);
			if (rayActive) {
				if (rayActive.ShootRayTest (origin, direction, damage, distance, penetrate, id, team))
					PlayDamageSound ();
			}
		} else {
			doDamage (origin, direction, damage, distance, penetrate, id, team);
		}
	}
	
	public void DoDamage ()
	{

		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("doDamage", RPCMode.All, this.transform.position + DamageOffset, this.transform.forward, Damage, DamageLength, Penetrate, ID, Team);
		} else {
			Vector3[] direction = {this.transform.forward};
			doDamage (this.transform.position + DamageOffset, direction, Damage, DamageLength, Penetrate, ID, Team);
		}
	}
#endif
	[RPC]
	public void doOverlapDamage (Vector3 origin, Vector3 direction, int damage, float distance, float dot, string id, string team)
	{
		if (rayActive) {
			if (rayActive.Overlap (origin, direction, damage, distance, dot, id, team))
				PlayDamageSound ();
		}
				
		if (inventory) {
			inventory.EquipmentOnAction ();	
		}
	}
	
	public void DoOverlapDamage (Vector3 origin, Vector3 direction, int damage, float distance, float dot, string id, string team)
	{
		if (networkViewer && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("doOverlapDamage", RPCMode.All, origin, direction, damage, distance, dot, id, team);
			if (rayActive) {
				if (rayActive.OverlapTest (origin, direction, damage, distance, dot, id, team))
					PlayDamageSound ();
			}
		} else {
			doOverlapDamage (origin, direction, damage, distance, dot, id, team);
		}
	}
	
	public void Interactive (Vector3 origin, Vector3 direction)
	{
		interactive (origin, direction);
	}
	
	public void Checking (Vector3 origin, Vector3 direction)
	{
		if (rayActive) {
			rayActive.CheckingRay (origin, direction);
		}	
	}
	
	[RPC]
	private void interactive (Vector3 origin, Vector3 direction)
	{
		if (rayActive) {
			rayActive.ActiveRay (origin, direction);
		}
	}

	public void Move (Vector3 directionVector)
	{
		if (Motor) {
			Motor.inputMoveDirection = directionVector;
		}
	}
	
	public void PlayIdleSound ()
	{
		if (Audiosource && SoundIdle.Length > 0) {
			Audiosource.PlayOneShot (SoundIdle [Random.Range (0, SoundIdle.Length)]);	
		}
	}
	
	public void PlayDamageSound ()
	{
		if (Audiosource && DamageSound.Length > 0) {
			Audiosource.PlayOneShot (DamageSound [Random.Range (0, DamageSound.Length)]);	
		}
	}
	
	[RPC]
	public void ApplyData (string saveData)
	{
		if (UnitZ.playerSave)
			UnitZ.playerSave.ReceiveDataAndApply (saveData, this);
	}
	
	public virtual void OnThisThingDead ()
	{
		// Do something when dying
	}

}
