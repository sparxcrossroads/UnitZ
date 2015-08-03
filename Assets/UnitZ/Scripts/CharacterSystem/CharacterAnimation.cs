//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	private Animator animator;
	public Transform upperSpine;
	public Transform headCamera;
	public Quaternion CameraRotation;
	private CharacterSystem character;
	
	void Start () {
		animator = this.GetComponent<Animator>();
		character = this.GetComponent<CharacterSystem>();
	}
	

	void Update () {
		if(animator == null || character == null)
			return;
		
		if(upperSpine){
			
			if(character.IsMine){
				CameraRotation = upperSpine.localRotation;
				CameraRotation.eulerAngles = new Vector3(upperSpine.localRotation.eulerAngles.x,upperSpine.localRotation.eulerAngles.y,-headCamera.transform.rotation.eulerAngles.x);
			
				if(Network.isServer || Network.isClient){
					GetComponent<NetworkView>().RPC ("UpdateHeadRotation", RPCMode.Others, CameraRotation);
				}
			}
			
			upperSpine.transform.localRotation = CameraRotation;
			
			if(animator.GetComponent<Animation>() && animator.GetComponent<Animation>()[animator.GetComponent<Animation>().clip.name])
				animator.GetComponent<Animation>()[animator.GetComponent<Animation>().clip.name].AddMixingTransform(upperSpine);

		}
		
	}
	[RPC]
	void UpdateHeadRotation(Quaternion rotation){
		CameraRotation = rotation;
	}

}
