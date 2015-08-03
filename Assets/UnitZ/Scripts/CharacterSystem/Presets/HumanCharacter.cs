//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

public class HumanCharacter : CharacterSystem
{
	[HideInInspector]
	public bool ToggleFlashlight = false;
	
	void Awake ()
	{
		SetupAwake ();	
	}
	
	void Start ()
	{	
		if (this.GetComponent<CharacterDriver> () == null) {
			this.gameObject.AddComponent<CharacterDriver> ();
		}
		if (this.GetComponent<CharacterLiving> () == null) {
			this.gameObject.AddComponent<CharacterLiving> ();
		}
		
		animator.SetInteger ("Shoot_Type", AttackType);
	}

	void Update ()
	{
		animator.SetInteger ("UpperState", 1);
		UpdateFunction ();
		
		if (Motor.controller.velocity.y < -20)
			ApplyDamage (10000, Motor.controller.velocity, "", "");
		
	}
	
	public override void PlayMoveAnimation (float magnitude)
	{
		if (animator) {
			if (magnitude > 0.4f) {
				animator.SetInteger ("LowerState", 1);
			} else {
				animator.SetInteger ("LowerState", 0);
			}
		}
	
		base.PlayMoveAnimation (magnitude);
	}

	public override void PlayAttackAnimation (bool attacking, int attacktype)
	{
		if (animator) {
			if (attacking) {
				animator.SetTrigger ("Shoot");
			}
			animator.SetInteger ("Shoot_Type", attacktype);
		}
		base.PlayAttackAnimation (attacking, attacktype);
	}
	
	
	[RPC]
	public void toggleFlash (bool active)
	{
		ToggleFlashlight = active;
	}
	
	public void ToggleFlash ()
	{
		if (networkViewer && networkViewer.isMine && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("toggleFlash", RPCMode.Others, ToggleFlashlight);
		}
	}
	
	public override void OnThisThingDead ()
	{
		if (Network.isServer || (!Network.isServer && !Network.isClient)) {
			if (UnitZ.playerSave)
				UnitZ.playerSave.DeleteSave (UserID, UserName);
		}
		
		if (UnitZ.scoreManager) {
			UnitZ.scoreManager.AddDead (1, ID);
			if (ID != LastHitByID)
				UnitZ.scoreManager.AddScore (1, LastHitByID);
		}
		
		
		CharacterItemDroper itemdrop = this.GetComponent<CharacterItemDroper> ();
		if (itemdrop)
			itemdrop.DropItem ();
		base.OnThisThingDead ();
	}
}
