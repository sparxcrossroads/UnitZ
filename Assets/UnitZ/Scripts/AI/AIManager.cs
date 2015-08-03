//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

/// <summary>
// This class made for recucing call of FindGameObjectsWithTag function in every AI
// FindGameObjectsWithTag is too much cost in update loop.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AIManager : MonoBehaviour
{
	
	public List<AITarget> TargetObject;
	private float timeTemp;
	public float LoopInterval = 0.1f;
	
	void Start ()
	{
		TargetObject = new List<AITarget>();
		timeTemp = Time.time;
	}
	
	public void Searching (string[] tags)
	{
		for (int i=0; i<tags.Length; i++) {
			if (!checking (tags [i])) {
				Debug.Log("Add new target");
				AITarget target = new AITarget();
				target.Tag = tags[i];
				target.Targets = null;
				TargetObject.Add (target);
			}
		}
	}
	bool checking(string tag){
		foreach(AITarget target in TargetObject){
			if(target.Tag == tag)
				return true;
		}
		return false;
	}
	
	public GameObject[] GetTargets (string targettag)
	{
		foreach(AITarget target in TargetObject){
			if(target.Tag == targettag)
				return target.Targets;
		}
		return null;
	}
	
	public void Loop ()
	{
		foreach(AITarget target in TargetObject){	
			GameObject[] targets = (GameObject[])GameObject.FindGameObjectsWithTag (target.Tag);
			if (targets != null && targets.Length > 0) {
				target.Targets = targets;
			}
		}
	}
	
	void Update ()
	{
		if (Time.time >= timeTemp + LoopInterval) {
			Loop ();
			timeTemp = Time.time;	
		}
	}

}
public class AITarget{
	public string Tag;
	public GameObject[] Targets;
}