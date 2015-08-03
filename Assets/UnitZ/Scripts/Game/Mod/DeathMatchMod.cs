//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class DeathMatchMod : MonoBehaviour
{
	public Texture2D TeamA,TeamB;
	public GUISkin skin;
	
	void Awake ()
	{
		if (UnitZ.playerManager) {
			UnitZ.playerManager.ManualSpawn = true;
			UnitZ.playerManager.DisabledSpawn = true;
			UnitZ.playerManager.SavePlayer = false;
		}
	}
	
	void Start ()
	{
		StyleManager Styles = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		if (!skin && Styles)
			skin = Styles.GetSkin (0);
	}

	void Update ()
	{
		if (UnitZ.playerManager == null)
			return;
		
		if(UnitZ.playerManager.playingCharacter==null){
			MouseLock.MouseLocked = false;	
		}
	}
	
	void OnGUI ()
	{
		if (UnitZ.playerManager == null)
			return;
		
		if (skin)
			GUI.skin = skin;
		
		if (!UnitZ.playerManager.playingCharacter) {
			GUI.BeginGroup (new Rect ((Screen.width / 2) - 400, (Screen.height / 2) - 200, 800, 400));
		
			if (GUI.Button (new Rect (50, 0, 300, 400), TeamA)) {
				UnitZ.playerManager.RequestSpawnWithTeamSpawner ("PUPPY", 0);	
			}
		
			if (GUI.Button (new Rect (450, 0, 300, 400), TeamB)) {
				UnitZ.playerManager.RequestSpawnWithTeamSpawner ("KITTY", 1);	
			}
		
			GUI.EndGroup ();
		}
			
	}
}
