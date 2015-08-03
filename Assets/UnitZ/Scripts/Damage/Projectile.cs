//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class Projectile : DamageBase
{
	
	public float Duration = 3;
	public GameObject Spawn;
	private float timeTemp;
	private NetworkView networkViewer;
	
	[RPC]
	void PositionUpdate (Vector3 position, Quaternion rotation, string id)
	{
		OwnerID = id;
		this.transform.position = Vector3.Lerp (this.transform.position, position, 1);
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rotation, 1);
	}

	private bool isQuitting = false;
	void OnApplicationQuit ()
	{ 
		isQuitting = true; 
	}
	
	void OnDestroy ()
	{
		if (!isQuitting && Spawn && !Application.isLoadingLevel) {
			GameObject fx = (GameObject)GameObject.Instantiate (Spawn, this.transform.position, this.transform.rotation);
			DamageBase dm = fx.GetComponent<DamageBase> ();
			if (dm) {
				dm.OwnerID = OwnerID;
				dm.OwnerTeam = OwnerTeam;
			}
		}
	}
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView>();
		timeTemp = Time.time;
	}
	
	void Update ()
	{
		
		if((networkViewer && networkViewer.isMine) || (!Network.isServer && !Network.isClient)){
			if (Time.time >= timeTemp + Duration) {
				OnDead ();
			}
		}
		
		if (networkViewer && networkViewer.isMine && (Network.isServer || Network.isClient)) {
			networkViewer.RPC ("PositionUpdate", RPCMode.Others, this.transform.position, this.transform.rotation, OwnerID);
		}
	}
	
	[RPC]
	void OnDead ()
	{
		if (Network.isServer || Network.isClient) {
			Network.Destroy (this.gameObject);
			if(networkViewer)
			Network.RemoveRPCs (networkViewer.viewID);
		} else {
			GameObject.Destroy (this.gameObject);	
		}
	}
}
