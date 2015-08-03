//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class GameMenuCanvas : MonoBehaviour
{
	[HideInInspector]
	public GameManager gameManage;
	
	void Start ()
	{
		gameManage = (GameManager)GameObject.FindObjectOfType (typeof(GameManager));
	}
	
	public void Resume ()
	{
		MouseLock.MouseLocked = true;
	}
	
	public void Disconnect ()
	{
		if (gameManage)
			gameManage.QuitGame ();
		
		MouseLock.MouseLocked = false;
	}

}
