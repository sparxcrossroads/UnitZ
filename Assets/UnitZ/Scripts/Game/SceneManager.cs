//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour
{
	public LevelPreset[] LevelPresets;
	private DayNightCycle dayNight;
	private TreesManager Trees;
	private NetworkView networkViewer;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		Trees = (TreesManager)GameObject.FindObjectOfType (typeof(TreesManager));
	}

	void Update ()
	{
		if (dayNight == null) {
			dayNight = (DayNightCycle)GameObject.FindObjectOfType (typeof(DayNightCycle));
		}
		if (Network.isServer) {
			if (dayNight) {
				if (networkViewer)
					networkViewer.RPC ("dayTimeUpdate", RPCMode.Others, dayNight.Timer);
			}
		}
	}
	
	[RPC]
	void dayTimeUpdate (float time)
	{
		if (dayNight) {
			dayNight.Timer = time;
		}
		
	}

	public void ResetAllTrees ()
	{
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("resetAllTrees", RPCMode.All, null);
		}
	}
	
	public void GetInitializeeScene ()
	{
		if (Network.isClient && networkViewer) {
			networkViewer.RPC ("UpdateRemovedTrees", RPCMode.Server, null);
		}
	}
	
	public void SendRemovedTreeIndex (int index)
	{
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("sendRemovedTreeIndex", RPCMode.Others, index);
		}
	}
	
	[RPC]
	private void resetAllTrees ()
	{
		if (Trees == null)
			Trees = (TreesManager)GameObject.FindObjectOfType (typeof(TreesManager));
		
		if (Trees == null)
			return;
		
		Trees.ResetTrees ();
	}
	
	[RPC]
	private void sendRemovedTreeIndex (int index)
	{
		if (Trees == null)
			Trees = (TreesManager)GameObject.FindObjectOfType (typeof(TreesManager));
		
		if (Trees == null)
			return;
		
		Trees.RemoveATrees (index);
	}
	
	[RPC]
	public void UpdateRemovedTrees ()
	{
		if (Trees == null)
			Trees = (TreesManager)GameObject.FindObjectOfType (typeof(TreesManager));
		
		if (Trees == null)
			return;
		
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("getRemovedIndex", RPCMode.Others, Trees.GetRemovedTrees ());	
		}
	}
	
	[RPC]
	private void getRemovedIndex (string indexremoved)
	{
		if (Trees == null)
			Trees = (TreesManager)GameObject.FindObjectOfType (typeof(TreesManager));
		
		if (Trees == null)
			return;
		
		Trees.UpdateRemovedTrees (indexremoved);
	}


}
