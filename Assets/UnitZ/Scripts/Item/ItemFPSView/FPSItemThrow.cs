//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class FPSItemThrow : FPSItemEquipment
{
	public bool HoldFire = false;
	public GameObject Item;
	public float FireRate = 0.1f;
	public int UsingType = 0;
	public ItemData ItemUsed;
	public bool InfinityAmmo;
	public bool OnAnimationEvent;
	public float Force1 = 15;
	public float Force2 = 5;
	public AudioClip SoundThrow;
	private CharacterSystem character;
	private FPSController fpsController;
	private float timeTemp;
	private AudioSource audioSource;
	private Animator animator;
	private int throwType = 0;
	
	void Start ()
	{
		animator = this.GetComponent<Animator> ();
		audioSource = this.GetComponent<AudioSource> ();
		if (this.transform.root) {
			character = this.transform.root.GetComponent<CharacterSystem> ();
			fpsController = this.transform.root.GetComponent<FPSController> ();
			if (character == null)
				character = this.transform.root.GetComponentInChildren<CharacterSystem> ();
			if (fpsController == null)
				fpsController = this.transform.root.GetComponentInChildren<FPSController> ();
		} else {
			character = this.transform.GetComponent<CharacterSystem> ();
			fpsController = this.transform.GetComponent<FPSController> ();
		}
		timeTemp = Time.time;
	}

	void Update ()
	{
	
	}

	void Use ()
	{
		if (ItemUsed != null) {
			if (!InfinityAmmo) {
				if (character != null && character.inventory != null && !character.inventory.CheckItem (ItemUsed, 1)) {
					return;	
				}
			}
		}
		
		if (!OnAnimationEvent) {
			OnAction ();
		}
				
		if (animator) {
			animator.SetInteger ("shoot_type", UsingType);
			animator.SetTrigger ("shoot");
		}
		if (character != null) {
			character.AttackAnimation (UsingType);
		}
	}

	public override void Trigger ()
	{
		if(!HoldFire && OnFire1)
			return;
		
		if (character && fpsController) {
			if (Time.time > timeTemp + FireRate) {
				throwType = 0;
				Use ();
				timeTemp = Time.time;
			}
		}
		base.Trigger ();
	}

	public override void Trigger2 ()
	{
		if(!HoldFire && OnFire2)
			return;
		
		if (character && fpsController) {
			if (Time.time > timeTemp + FireRate) {
				throwType = 1;
				Use ();
				timeTemp = Time.time;
			}
		}
		base.Trigger2 ();
	}
	
	public override void OnAction ()
	{
		
		if (Item) {
			if (Network.isServer || Network.isClient) {
				GameObject item = (GameObject)Network.Instantiate (Item, this.transform.position, this.transform.rotation, 3);
				DamageBase dm = item.GetComponent<DamageBase>();
				if(dm){
					if(character && UnitZ.gameManager){
						dm.OwnerID = character.ID;
						dm.OwnerTeam = character.Team;
					}
				}
				if (throwType == 0) {
					if (item.GetComponent<Rigidbody>())
						item.GetComponent<Rigidbody>().AddForce(this.transform.forward * Force1,ForceMode.Impulse);
				} else {
					if (item.GetComponent<Rigidbody>())
						item.GetComponent<Rigidbody>().AddForce(this.transform.forward * Force2,ForceMode.Impulse);
				}
			} else {
				GameObject item = (GameObject)GameObject.Instantiate (Item, this.transform.position, this.transform.rotation);
				DamageBase dm = item.GetComponent<DamageBase>();
				if(dm){
					if(character && UnitZ.gameManager){
						dm.OwnerID = character.ID;
						dm.OwnerTeam = character.Team;

					}
				}
				
				if (throwType == 0) {
					if (item.GetComponent<Rigidbody>())
						item.GetComponent<Rigidbody>().AddForce(this.transform.forward * Force1,ForceMode.Impulse);
				} else {
					if (item.GetComponent<Rigidbody>())
						item.GetComponent<Rigidbody>().AddForce(this.transform.forward * Force2,ForceMode.Impulse);
				}
			}
			if(SoundThrow && audioSource)
				if(audioSource.enabled)
					audioSource.PlayOneShot(SoundThrow);
		}
		if (ItemUsed != null) {
			if (!InfinityAmmo) {
				if (character != null && character.inventory != null && !character.inventory.RemoveItem (ItemUsed, 1)) {
					return;	
				}
			}
		}
		base.OnAction ();
	}
	
}
