//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class PlayerView : MonoBehaviour
{

	public FPSCamera FPScamera;
	public GameObject[] PlayerObjects;
	private CharacterSystem character;

	void Start ()
	{
		SetActive ();
	}

	void Awake ()
	{
		character = this.GetComponent<CharacterSystem>();
		GameObject fpscam = this.transform.FindChild ("FPScamera").gameObject;
		if (fpscam)
			FPScamera = fpscam.GetComponent<FPSCamera> ();
	}	
	
	void Update ()
	{
		SetActive ();
	}
	
	public void SetActive ()
	{
		if (character && character.IsMine) {
			foreach (var go in PlayerObjects) {
				go.SetActive (false);
			}
			FPScamera.gameObject.SetActive (true);
		} else {
			FPScamera.gameObject.SetActive (false);
			foreach (var go in PlayerObjects) {
				go.SetActive (true);
			}
		}
	}
}
