//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterLiving : MonoBehaviour
{
	
	
	public int Hungry = 100;
	public int HungryMax = 100;
	public int Water = 100;
	public int WaterMax = 100;
	[HideInInspector]
	public CharacterSystem character;
	[HideInInspector]
	public NetworkView networkViewer;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		character = this.GetComponent<CharacterSystem> ();
		
		InvokeRepeating ("stomachUpdate", 1.0f, 1.0f);
		InvokeRepeating ("hungryUpdate", 1.0f, 15.0f);
		InvokeRepeating ("thirstilyUpdate", 1.0f, 10.0f);
		
	}

	void Update ()
	{
		if (Hungry < 0)
			Hungry = 0;
			
		if (Hungry > HungryMax)
			Hungry = HungryMax;
		
		if (Water < 0)
			Water = 0;
			
		if (Water > WaterMax)
			Water = WaterMax;
	}
	
	public void stomachUpdate ()
	{
		if (character == null)
			return;
		
		if (Water <= 0) {	
			if (Network.isServer && !Network.isClient) {
				if(networkViewer)
					networkViewer.RPC ("applyDamage", RPCMode.All, 2, Vector3.up, character.ID, "");
			} else {
				character.ApplyDamage (2, Vector3.up, character.ID, "");	
			}
		}
		if (Hungry <= 0) {
			if (Network.isServer && !Network.isClient) {
				if(networkViewer)
					networkViewer.RPC ("applyDamage", RPCMode.All, 2, Vector3.up, character.ID, "");
			} else {
				character.ApplyDamage (1, Vector3.up, character.ID, "");		
			}
		}
	}
	
	public void hungryUpdate ()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer)) {
			Eating (-1);
		}
	}
	
	public void thirstilyUpdate ()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer)) {
			Drinking (-1);
		}
	}
	
	[RPC]
	void eatUpdate (int num)
	{
		Hungry = num;
	}
	[RPC]
	public void Eating (int num)
	{
		if (!Network.isServer && !Network.isClient) {
			Hungry += num;
		} else {
			if (Network.isClient) {
				if (networkViewer)
					networkViewer.RPC ("Eating", RPCMode.Server, num);
			}
			if (Network.isServer) {
				Hungry += num;
				if (networkViewer) {
					networkViewer.RPC ("eatUpdate", RPCMode.Others, Hungry);
				}
			}
		}
		
	}
	
	[RPC]
	void drinkUpdate (int num)
	{
		Water = num;
	}
	[RPC]
	public void Drinking (int num)
	{
		if (!Network.isServer && !Network.isClient) {
			Water += num;
		} else {
			if (Network.isClient) {
				if (networkViewer)
					networkViewer.RPC ("Drinking", RPCMode.Server, num);
			}
			if (Network.isServer) {
				Water += num;
				if (networkViewer) {
					networkViewer.RPC ("drinkUpdate", RPCMode.Others, Water);
				}
			}
		}
	}
}
