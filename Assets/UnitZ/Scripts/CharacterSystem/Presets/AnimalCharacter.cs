//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class AnimalCharacter : CharacterSystem
{

	public float DamageDirection = 0.5f;
	public bool OnBattle = false;
	public float Force = 70;
	
	void Awake ()
	{	
		SetupAwake ();	
	}
	
	void Start ()
	{	
		this.transform.position += Vector3.up;
	}

	void Update ()
	{
		UpdateFunction ();
		Motor.inputJump = false;
		
		if (Motor.controller.velocity.y < -20)
			ApplyDamage (10000, Motor.controller.velocity, "", "");
		
		if (Motor.inputJump)
			Motor.inputJump = false;

	}
	
	public void Leap ()
	{	

	}
	
	public void AfterAttack ()
	{

	}
	
	public void DoAttack ()
	{
		DoOverlapDamage (this.transform.position + DamageOffset, this.transform.forward * Force, Damage, DamageLength, DamageDirection, "", Team);
	}

	public override void PlayMoveAnimation (float magnitude)
	{
		if (animator) {
			if (magnitude > 0.4f) {
				animator.SetInteger ("StateAnimation", 1);
			} else {
				animator.SetInteger ("StateAnimation", 0);
			}
		}

		base.PlayMoveAnimation (magnitude);
	}

	public override void PlayAttackAnimation (bool attacking, int attacktype)
	{
		if (animator) {
			if (attacking) {

				animator.SetTrigger ("Attacking");
				spdMovAtkMult = 0;
			}
		}
		base.PlayAttackAnimation (attacking, attacktype);
	}

}
