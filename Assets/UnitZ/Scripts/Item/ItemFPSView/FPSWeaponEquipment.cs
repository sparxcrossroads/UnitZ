//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]


public class FPSWeaponEquipment : FPSItemEquipment
{
	
	private CharacterSystem character;
	private FPSController fpsController;
	private float timeTemp;
	private AudioSource audioSource;
	private Animator animator;
	public bool HoldFire = true;
	public float FireRate = 0.09f;
	public float Spread = 20;
	public int BulletNum = 1;
	public int Damage = 10;
	public float Force = 10;
	public Vector2 KickPower = Vector2.zero;
	public int MaxPenetrate = 1;
	public float Distance = 100;
	public int Ammo = 30;
	public int AmmoMax = 30;
	public int ClipSize = 30;
	public int AmmoHave = 0;
	public AudioClip SoundFire;
	public AudioClip SoundReload;
	public AudioClip SoundReloadComplete;
	public AudioClip[] DamageSound;
	public GameObject MuzzleFX;
	public Transform MuzzlePoint;
	public ItemData ItemUsed;
	public int UsingType = 0;
	public bool InfinityAmmo;
	public bool OnAnimationEvent;
	public float AnimationSpeed = 1;
	private float animationSpeedTemp = 1;
	public float panicfire = 0;
	public float PanicFireMult = 0.1f;
	public float FOVZoom = 65;
	public float SpreadZoomMult = 1;
	public bool HideWhenZoom = false;
	public GameObject ProjectileFX;
	public GUISkin Skin;
	
	void Start ()
	{
		reloading = false;
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
		
		if (character)
			character.DamageSound = DamageSound;
		
		if (fpsController)
			fpsController.zooming = false;
		
		Hide (true);
		timeTemp = Time.time;
		
		StyleManager style = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		if (style && !Skin) {
			Skin = style.GetSkin (0);	
		}
		if (animator) {
			animationSpeedTemp = animator.speed;
		}
	}
	
	public override void Trigger ()
	{
		if (!HoldFire && OnFire1)
			return;
		
		if (character && fpsController) {
			if (Time.time > timeTemp + FireRate) {
				Shoot ();
				timeTemp = Time.time;
			}
		}
		base.Trigger ();
	}

	public override void OnTriggerRelease ()
	{
		base.OnTriggerRelease ();
	}

	public override void Trigger2 ()
	{
		fpsController.Zoom (FOVZoom);
		base.Trigger2 ();
	}

	public override void OnTrigger2Release ()
	{
		base.OnTrigger2Release ();
	}
	
	public void Shoot ()
	{
		if (animator)
			animator.speed = AnimationSpeed;
		if (Ammo <= 0 && !InfinityAmmo)
			return;
		
		if (!OnAnimationEvent) {
			OnAction ();
		} 
		if (character != null) {
			character.AttackAnimation (UsingType);
		}
		if (animator) {
			animator.SetTrigger ("shoot");
		}
		
	}
	
	private bool reloading;

	public override void Reload ()
	{
		if (Ammo >= AmmoMax || Ammo <= 0 && AmmoHave <= 0)
			return;
		
		if (!reloading) {
			if (audioSource && SoundReload) {
				audioSource.PlayOneShot (SoundReload);
			}
		}
		
		if (ItemUsed != null) {
			int ammo = character.inventory.GetItemNum (ItemUsed);
			if (!InfinityAmmo) {
				if (character != null && character.inventory != null && ammo <= 0) {
					return;	
				}
			}
		}
		if (animator)
			animator.SetTrigger ("reloading");	
		reloading = true;
		
		base.Reload ();
	}
	
	public override void ReloadComplete ()
	{
		int ammoused = ClipSize;
		if (AmmoMax - Ammo < ammoused)
			ammoused = AmmoMax - Ammo;
		
		if (character != null && character.inventory != null && ItemUsed != null) {
			if (AmmoHave <= 0) {
				reloading = false;
				return;
			}
			if (AmmoHave < ammoused) {
				ammoused = AmmoHave;
			}
			character.inventory.RemoveItem (ItemUsed, ammoused);
		}
		Ammo += ammoused;
		
		if (Ammo >= AmmoMax) {
			reloading = false;
		} else {
			Reload ();	
		}
		if (audioSource && SoundReloadComplete) {
			audioSource.PlayOneShot (SoundReloadComplete);
		}	
		base.ReloadComplete ();
	}
	
	private float spreadmult;

	void Update ()
	{
		if (!reloading && Ammo <= 0) {
			Reload ();
		}
		
		Hide (true);
		spreadmult = 1;
		if (HideWhenZoom) {
			if (fpsController && fpsController.zooming) {
				Hide (false);
				spreadmult = SpreadZoomMult;
			}
		}
		
		if (panicfire < 0.01f)
			panicfire = 0;
		panicfire += (0 - panicfire) * 5 * Time.deltaTime;
		
		if (animator)
			animator.SetInteger ("shoot_type", UsingType);
		
		if (character != null && character.inventory != null && ItemUsed != null) {
			AmmoHave = character.inventory.GetItemNum (ItemUsed);
		}
		if (CollectorSlot != null) {
			CollectorSlot.NumTag = Ammo;	
		}
	}

	public void SetCollectorSlot (ItemCollector collector)
	{
		CollectorSlot = collector;
		if (collector.NumTag != -1)
			Ammo = collector.NumTag;
	}
		
	void OnGUI ()
	{
		if (InfinityAmmo)
			return;
		
		if (Skin)
			GUI.skin = Skin;
		
		GUI.skin.label.alignment = TextAnchor.LowerRight;
		GUI.skin.label.fontSize = 35;
		GUI.Label (new Rect (Screen.width - 230, Screen.height - 90, 200, 60), Ammo + "/" + AmmoHave);	
	}
	
	public override void OnAction ()
	{
		if (animator)
			animator.speed = animationSpeedTemp;
		if (Ammo > 0 || InfinityAmmo) {
			if (!InfinityAmmo)
				Ammo -= 1;
		
			if (audioSource && SoundFire) {
				if(audioSource.enabled)
					audioSource.PlayOneShot (SoundFire);
			}	
			if (BulletNum <= 0)
				BulletNum = 1;
		
			Vector3[] dirs = new Vector3[BulletNum];
			if (!MuzzlePoint) {
				MuzzlePoint = this.transform;
			}
			for (int i=0; i<dirs.Length; i++) {
			
			
				if (dirs.Length <= 1)
					panicfire = 1;
				if (fpsController)
					dirs [i] = (fpsController.FPSCamera.transform.forward + (new Vector3 (Random.Range (-Spread + (int)panicfire, Spread + (int)panicfire) * 0.001f, Random.Range (-Spread + (int)panicfire, Spread + (int)panicfire) * 0.001f, Random.Range (-Spread + (int)panicfire, Spread + (int)panicfire) * 0.001f) * spreadmult));
		
				if (ProjectileFX && fpsController) {
					if (!Network.isClient && !Network.isServer) {
						GameObject.Instantiate (ProjectileFX, fpsController.FPSCamera.transform.position + (dirs [i] * 5), Quaternion.LookRotation (dirs [i]));	
					} else {
						Network.Instantiate (ProjectileFX, fpsController.FPSCamera.transform.position + (dirs [i] * 5), Quaternion.LookRotation (dirs [i]), 0);	
					}
				}
				dirs [i] *= Force;
			}
		
			if (fpsController)
				fpsController.Kick (KickPower);
		
			
			if (character != null) {
				character.DoDamage (fpsController.FPSCamera.transform.position, dirs, Damage, Distance, MaxPenetrate, character.ID, character.Team);
			}
		
		
			if (MuzzleFX) {
				GameObject fx = (GameObject)GameObject.Instantiate (MuzzleFX, MuzzlePoint.position, MuzzlePoint.rotation);	
				fx.transform.parent = this.transform;
				GameObject.Destroy (fx, 2);
			}	
		
			
		
		
			panicfire += PanicFireMult;
		}
		base.OnAction ();
		
	}
}
