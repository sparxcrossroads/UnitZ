//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class MassiveAIController : MonoBehaviour
{
	public string PlayerTag = "Player";
	public AICharacterController[] ai;

	void Start ()
	{
	
	}
	
	void Update ()
	{
		GameObject[] playersaround = GameObject.FindGameObjectsWithTag (PlayerTag);
		bool offlineMode = (!Network.isServer && !Network.isClient);
		
		if (Network.isServer || offlineMode) {
			ai = (AICharacterController[])GameObject.FindObjectsOfType (typeof(AICharacterController));
			for (int i=0; i<ai.Length; i++) {
				
					
				ai [i].character.OnUpdate = false;
				for (int p=0; p<playersaround.Length; p++) {
					if (Vector3.Distance (ai [i].transform.position, playersaround [p].transform.position) < ai [i].DistanceMoveTo) {
						ai [i].character.OnUpdate = true;
					}
				}
					
				if (ai [i].character.OnUpdate) {
						
					ai [i].DistanceAttack = ai [i].character.PrimaryWeaponDistance;	
					float distance = Vector3.Distance (ai [i].PositionTarget, ai [i].gameObject.transform.position);
					Vector3 targetDirectiom = (ai [i].PositionTarget - ai [i].transform.position);
					Quaternion targetRotation = Quaternion.LookRotation (targetDirectiom);
					targetRotation.x = 0;
					targetRotation.z = 0;
					float str = ai [i].TurnSpeed * Time.time;
		
					ai [i].gameObject.transform.rotation = Quaternion.Lerp (ai [i].gameObject.transform.rotation, targetRotation, str);
		
					if (ai [i].ObjectTarget != null) {
						ai [i].PositionTarget = ai [i].ObjectTarget.transform.position;
			
						if (ai [i].aiTime <= 0) {
							ai [i].aiState = Random.Range (0, 4);
							ai [i].aiTime = Random.Range (10, 100);
						} else {
							ai [i].aiTime--;
						}
			
						if (distance <= ai [i].DistanceAttack) {
							if (ai [i].aiState == 0 || ai [i].BrutalMode) {
								ai [i].Attack (targetDirectiom);
							}
						} else {
							if (distance <= ai [i].DistanceMoveTo) {
								ai [i].gameObject.transform.rotation = Quaternion.Lerp (ai [i].gameObject.transform.rotation, targetRotation, str);
					
							} else {
								ai [i].ObjectTarget = null;
								if (ai [i].aiState == 0) {
									ai [i].aiState = 1;
									ai [i].aiTime = Random.Range (10, 500);
									ai [i].PositionTarget = ai [i].positionTemp + new Vector3 (Random.Range (-ai [i].PatrolRange, ai [i].PatrolRange), 0, Random.Range (-ai [i].PatrolRange, ai [i].PatrolRange));
								}
							}
						}
	
					} else {
	
						float length = float.MaxValue;
						for (int t=0; t< ai[i].TargetTag.Length; t++) {
							GameObject[] targets = (GameObject[])GameObject.FindGameObjectsWithTag (ai [i].TargetTag [t]);
				
							for (int v=0; v<targets.Length; v++) {
								float distancetargets = Vector3.Distance (targets [v].gameObject.transform.position, ai [i].gameObject.transform.position);
								if ((distancetargets <= length && (distancetargets <= ai [i].DistanceMoveTo || distancetargets <= ai [i].DistanceAttack || ai [i].RushMode)) && ai [i].ObjectTarget != targets [v].gameObject) {
									length = distancetargets;
									ai [i].ObjectTarget = targets [v].gameObject;
								}
							}
						}
						if (ai [i].aiState == 0) {
							ai [i].aiState = 1;
							ai [i].aiTime = Random.Range (10, 200);
							ai [i].PositionTarget = ai [i].positionTemp + new Vector3 (Random.Range (-ai [i].PatrolRange, ai [i].PatrolRange), 0, Random.Range (-ai [i].PatrolRange, ai [i].PatrolRange));
						}
						if (ai [i].aiTime <= 0) {
							ai [i].aiState = Random.Range (0, 4);
							ai [i].aiTime = Random.Range (10, 200);
						} else {
							ai [i].aiTime--;
						}
			
			
					}
					if (!offlineMode) {
						ai [i].character.networkViewer.RPC ("MoveToPosition", RPCMode.All, ai [i].PositionTarget);
					} else {
						ai [i].character.MoveToPosition (ai [i].PositionTarget);
					}
				}
				
			}
		}
	}
}
