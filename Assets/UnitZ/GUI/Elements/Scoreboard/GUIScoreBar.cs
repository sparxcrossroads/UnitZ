﻿//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIScoreBar : MonoBehaviour {

	public Text Name;
	public Text Kills;
	public Text Deads;
	public PlayerData Player;
	
	void Start () {
		if(Name)
			Name.enabled = false;
		
		if(Kills)
			Kills.enabled = false;
		
		if(Deads)
			Deads.enabled = false;
	}
	
	void Update () {
		if(Player==null)
			return;
		
		if(Name){
			Name.text = Player.Name;
			Name.enabled = true;
		}
		
		if(Kills){
			Kills.text = Player.Score.ToString();
			Kills.enabled = true;
		}
		
		if(Deads){
			Deads.text = Player.Dead.ToString();
			Deads.enabled = true;
		}
	}
}