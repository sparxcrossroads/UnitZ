//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

/// <summary>
/// AI character controller.
/// Just A basic AI Character controller 
/// will looking for a Target and moving to and Attacking
/// </summary>
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterSystem))]

public class AICharacterController : MonoBehaviour
{
	public string[] TargetTag = {"Player"};
	public GameObject ObjectTarget;
	[HideInInspector]
	public Vector3 PositionTarget;
	[HideInInspector]
	public CharacterSystem character;
	[HideInInspector]
	public float DistanceAttack = 2;
	public float DistanceMoveTo = 20;
	public float TurnSpeed = 10.0f;
	public bool BrutalMode;
	public bool RushMode;
	public float PatrolRange = 10;
	[HideInInspector]
	public Vector3 positionTemp;
	[HideInInspector]
	public int aiTime = 0;
	[HideInInspector]
	public int aiState = 0;
	private float attackTemp = 0;
	public float AttackDelay = 0.5f;
	public float LifeTime = 30;
	public float IdleSoundDelay = 10;
	private float lifeTimeTemp = 0;
	private float jumpTemp, jumpTime, soundTime, soundTimeDuration;
	public int JumpRate = 20;
	private AIManager AImange;
	
	void Start ()
	{
		//AImange = (AIManager)GameObject.FindObjectOfType (typeof(AIManager));
		character = gameObject.GetComponent<CharacterSystem> ();
		positionTemp = this.transform.position;
		aiState = 0;
		attackTemp = Time.time;
		lifeTimeTemp = Time.time;
		jumpTemp = Time.time;
		soundTime = Time.time;
		soundTimeDuration = Random.Range (0, IdleSoundDelay);
		character.ID = "";
	}
	
	public void Attack (Vector3 targetDirectiom)
	{
		if (Time.time > attackTemp + AttackDelay) {
			Vector3[] dirs = new Vector3[1];
			dirs [0] = targetDirectiom.normalized;
			character.DoDamage (this.transform.position + character.DamageOffset, dirs, character.Damage, character.DamageLength, character.Penetrate, "", character.Team);
			character.AttackAnimation ();
			attackTemp = Time.time;	
		}
	}
	
	void FrontObstacleChecker ()
	{
		if (Time.time >= jumpTemp + jumpTime) {
			character.Motor.inputJump = true;
			jumpTime = Random.Range (0, JumpRate) * 0.1f;
			jumpTemp = Time.time;
		} else {
			character.Motor.inputJump = false;	
		}
	}

	void Update ()
	{
		if (character == null)
			return;
		
		
		if (Time.time > soundTime + soundTimeDuration) {
			character.PlayIdleSound ();	
			soundTimeDuration = Random.Range (0, IdleSoundDelay);
			soundTime = Time.time;
		}
		if (Time.time > lifeTimeTemp + LifeTime) {
			character.HP = 0;
			character.dieByLifeTime = true;
		}
		
		//if (AImange == null)
		//return;
		
		if (!character.OnUpdate)
			return;
		
		bool offlineMode = (!Network.isServer && !Network.isClient);
		
		if (Network.isServer || offlineMode) {

			DistanceAttack = character.PrimaryWeaponDistance;	

			character.Motor.inputJump = true;
			float distance = Vector3.Distance (PositionTarget, this.gameObject.transform.position);
			Vector3 targetDirectiom = (PositionTarget - this.transform.position);
			Quaternion targetRotation = this.transform.rotation;
			float str = TurnSpeed * Time.time;
			
			if (targetDirectiom != Vector3.zero) {
				targetRotation = Quaternion.LookRotation (targetDirectiom);
				targetRotation.x = 0;
				targetRotation.z = 0;
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
			}
			
			FrontObstacleChecker ();
			
		
			if (ObjectTarget != null) {
				lifeTimeTemp = Time.time;
				PositionTarget = ObjectTarget.transform.position;
				if (aiTime <= 0) {
					aiState = Random.Range (0, 4);
					aiTime = Random.Range (10, 100);
				} else {
					aiTime--;
				}
			
				if (distance <= DistanceAttack) {
					if (aiState == 0 || BrutalMode) {
						Attack (targetDirectiom);
					}
				} else {
					if (distance <= DistanceMoveTo) {
						transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);
					
					} else {
						ObjectTarget = null;
						if (aiState == 0) {
							aiState = 1;
							aiTime = Random.Range (10, 500);
							PositionTarget = positionTemp + new Vector3 (Random.Range (-PatrolRange, PatrolRange), 0, Random.Range (-PatrolRange, PatrolRange));
						}
					}
				}

			} else {
	
				float length = float.MaxValue;
				//AImange.Searching (TargetTag);
				for (int t=0; t< TargetTag.Length; t++) {
					//GameObject[] targets = AImange.GetTargets (TargetTag [t]);
					GameObject[] targets = (GameObject[])GameObject.FindGameObjectsWithTag (TargetTag [t]);
					if (targets != null && targets.Length > 0) {
						for (int i=0; i<targets.Length; i++) {
							float distancetargets = Vector3.Distance (targets [i].gameObject.transform.position, this.gameObject.transform.position);
							if ((distancetargets <= length && (distancetargets <= DistanceMoveTo || distancetargets <= DistanceAttack || RushMode)) && ObjectTarget != targets [i].gameObject) {
								length = distancetargets;
								ObjectTarget = targets [i].gameObject;
							}
						}
					}
				}
				if (aiState == 0) {
					aiState = 1;
					aiTime = Random.Range (10, 200);
					PositionTarget = positionTemp + new Vector3 (Random.Range (-PatrolRange, PatrolRange), 0, Random.Range (-PatrolRange, PatrolRange));
				}
				if (aiTime <= 0) {
					aiState = Random.Range (0, 4);
					aiTime = Random.Range (10, 200);
				} else {
					aiTime--;
				}
			
			
			}
			if (!offlineMode) {
				character.networkViewer.RPC ("MoveToPosition", RPCMode.All, PositionTarget);
			} else {
				character.MoveToPosition (PositionTarget);
			}
		}
	}
}
