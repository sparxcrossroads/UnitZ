//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterMotor))]
public class CharacterFootStep : MonoBehaviour {
	
	private AudioSource audios;
	private CharacterMotor motor;
	private float delay = 0;
	public AudioClip[] FoodSteps;
	public float Delay = 3;
	
	void Start () {
		motor = this.gameObject.GetComponent<CharacterMotor>();
		audios = this.gameObject.GetComponent<AudioSource>();
	}
	
	void PlaySound(){
		if(FoodSteps.Length>0){
			audios.PlayOneShot(FoodSteps[Random.Range(0,FoodSteps.Length)]);	
		}
	}

	void Update () {
		if(!motor)
			return;
		
		float magnitude = motor.movement.velocity.magnitude;
		if(motor.grounded && motor.IsActive){
			if(delay >= Delay){
				PlaySound();
				delay = 0;
			}
		}
		if(delay < Delay){
			delay+= magnitude * Time.deltaTime;	
		}
	}
}
