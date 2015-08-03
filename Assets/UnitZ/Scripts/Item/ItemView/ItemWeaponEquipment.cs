//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]

public class ItemWeaponEquipment : ItemEquipment {
	
	private CharacterSystem character;
	public GameObject MuzzleFX;
	private AudioSource audioSource;
	public AudioClip SoundFire;
	public AudioClip[] DamageSound;
	
	void Start () {
		audioSource = this.GetComponent<AudioSource>();
		if (this.transform.root) {
			character = this.transform.root.GetComponent<CharacterSystem> ();
		} else {
			character = this.transform.GetComponent<CharacterSystem> ();
		}
		if(character)
			character.DamageSound = DamageSound;
	}

	public override void Action ()
	{
		if(audioSource && SoundFire){
			if(audioSource.enabled)
				audioSource.PlayOneShot(SoundFire);
		}
		if(MuzzleFX){
			GameObject fx = (GameObject)GameObject.Instantiate(MuzzleFX,this.transform.position,this.transform.rotation);	
			fx.transform.parent = this.transform;
			GameObject.Destroy(fx,2);
		}
		base.Action ();
	}
}
