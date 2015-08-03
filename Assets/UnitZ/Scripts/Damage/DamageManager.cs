//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DamageManager : MonoBehaviour
{
	public int HP = 100;
	public int HPmax = 100;
	public int Armor = 0;
	public int Armormax = 100;
	public GameObject DeadReplacement;
	public float DeadReplaceLifeTime = 180;
	public bool isAlive = true;
	public AudioClip[] SoundPain;
	public AudioSource Audiosource;
	[HideInInspector]
	public bool dieByLifeTime = false;
	[HideInInspector]
	public bool spectreThis = false;
	[HideInInspector]
	public string Team = "";
	[HideInInspector]
	public string ID = "";
	[HideInInspector]
	public string UserID = "";
	[HideInInspector]
	public string UserName = "";
	[HideInInspector]
	public string LastHitByID = "";
	[HideInInspector]
	public bool IsMine;
	[HideInInspector]
	public NetworkView networkViewer;
	private bool initialized = false;
	private Vector3 directionHit;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
		Audiosource = this.GetComponent<AudioSource> ();
		initialized = false;
	}

	private bool isQuitting = false;

	void OnApplicationQuit ()
	{ 
		isQuitting = true; 
	}
	
	void Update ()
	{
		DamageUpdate ();	
	}

	public void DamageUpdate ()
	{
		if (isAlive) {
			if (HP > HPmax)
				HP = HPmax;
			
			if (Armor > Armormax)
				Armor = Armormax;
			
			if (HP <= 0) {
				if (!Network.isServer && !Network.isClient) {
					isAlive = false;
					OnDead ();
				} else {
					if (Network.isServer) {
						isAlive = false;
						OnDead ();
					}	
				}
			}
			
			UpdateData ();
		}
		
		if (UnitZ.gameManager && UnitZ.gameManager.PlayerID == ID) {
			IsMine = true;	
		} else {
			IsMine = false;	
		}
	}

	public void ApplyDamage (int damage, Vector3 direction, string attackerID, string team)
	{
		if ((!Network.isClient && !Network.isServer) || Network.isServer) {
			directionHit = direction;
			LastHitByID = attackerID;
			if (Team != team || team == "") {	
				HP -= damage;
				if (networkViewer && Network.isServer) {
					networkViewer.RPC ("applyDamage", RPCMode.Others, damage, direction, attackerID, team);
				}
			}	
		}
		
		if (Audiosource && SoundPain.Length > 0) {
			Audiosource.PlayOneShot (SoundPain [Random.Range (0, SoundPain.Length)]);	
		}
	}

	[RPC]
	private void applyDamage (int damage, Vector3 direction, string attackerID, string team)
	{
		directionHit = direction;
		LastHitByID = attackerID;
		if (Team != team || team == "") {
			HP -= damage;
		}
	}
	
	public void DirectDamage (DamagePackage pack)
	{
		ApplyDamage ((int)((float)pack.Damage), pack.Direction, pack.ID, pack.Team);
	}
	
	void OnDead ()
	{
		this.gameObject.SendMessage("OnThisThingDead",SendMessageOptions.DontRequireReceiver);
	
		if ((!Network.isServer && !Network.isClient)) {
			GameObject.Destroy (this.gameObject);
		} else {
			Network.RemoveRPCs (networkViewer.viewID);
			Network.Destroy (networkViewer.viewID);
		}
	}

	void OnDestroy ()
	{
		if (!isQuitting && DeadReplacement && !Application.isLoadingLevel) {
			if (networkViewer) {
				Network.RemoveRPCs (networkViewer.viewID);
			}
			GameObject deadbody = (GameObject)GameObject.Instantiate (DeadReplacement, this.transform.position, Quaternion.identity);
			if (spectreThis && deadbody) {
				LookAfterDead (deadbody.gameObject);
			}
			CopyTransformsRecurse (this.transform, deadbody);
			if (dieByLifeTime)
				DeadReplaceLifeTime = 3;
			GameObject.Destroy (deadbody, DeadReplaceLifeTime);
		}
	}
	
	public void CopyTransformsRecurse (Transform src, GameObject dst)
	{
		dst.transform.position = src.position;
		dst.transform.rotation = src.rotation;
		if (dst.GetComponent<Rigidbody>())
			dst.GetComponent<Rigidbody>().AddForce (directionHit, ForceMode.Impulse);
		foreach (Transform child in dst.transform) {
			var curSrc = src.Find (child.name);
			if (curSrc) {
				CopyTransformsRecurse (curSrc, child.gameObject);
			}
		}
	}
	
	void LookAfterDead (GameObject obj)
	{
		SpectreCamera spectre = (SpectreCamera)GameObject.FindObjectOfType (typeof(SpectreCamera));
		if (spectre) {
			spectre.LookingAtObject (obj);
		}
	}
	
	public void InitializeData ()
	{
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("initializeData", RPCMode.Others, HP);	
		}
	}

	[RPC]
	private void initializeData (int hp)
	{
		initialized = true;
		HP = hp;
	}
	
	public void ReceivePlayerID (string id)
	{
		ID = id;
		if (Network.isServer && networkViewer) {
			networkViewer.RPC ("receivePlayerID", RPCMode.Others, ID);	
		}
	}

	[RPC]
	private void receivePlayerID (string id)
	{
		ID = id;
	}

	void UpdateData ()
	{
		if (initialized) {
			if (networkViewer && Network.isServer) {
				networkViewer.RPC ("updateData", RPCMode.Others, ID, Team, HP);	
			}
		}
	}
	
	[RPC]
	void updateData (string id, string team, int hp)
	{
		HP = hp;
		ID = id;
		Team = team;
	}

	
}
