//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class SceneSave : MonoBehaviour
{
	public bool AutoSave = true;
	public float SaveInterval = 3;
	public bool ClearEveryplay = false;
	private ItemManager itemManager;
	private float timeTemp;
	
	void Start ()
	{
		itemManager = (ItemManager)GameObject.FindObjectOfType (typeof(ItemManager));
		//LoadObjectPlacing ();
		timeTemp = Time.time;
		
	}
	
	void Update ()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer)) {
			if (AutoSave) {
				if (Time.time > timeTemp + SaveInterval) {
					SaveObjectPlacing ();
					timeTemp = Time.time;
				}
			}
		}
	}
	public void LevelLoaded(){
		if(ClearEveryplay){
			//ClearObjectPlacing();
		}else{
			LoadObjectPlacing();
		}
	}
	
	public void SaveObjectPlacing ()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer)) {
			ObjectPlacing[] objectPlacings = (ObjectPlacing[])GameObject.FindObjectsOfType (typeof(ObjectPlacing));
			string Key = Application.loadedLevelName;
			string objID = "";
			string objPosition = "";
			string objRotation = "";
		
			for (int i=0; i<objectPlacings.Length; i++) {
				objID += objectPlacings [i].ItemID + ",";
				objPosition += objectPlacings [i].transform.position.x + "," + objectPlacings [i].transform.position.y + "," + objectPlacings [i].transform.position.z + "|";
				objRotation += objectPlacings [i].transform.rotation.x + "," + objectPlacings [i].transform.rotation.y + "," + objectPlacings [i].transform.rotation.z + "," + objectPlacings [i].transform.rotation.w + "|";
			}
		
			PlayerPrefs.SetString (Key + "OBJID", objID);
			PlayerPrefs.SetString (Key + "OBJPOS", objPosition);
			PlayerPrefs.SetString (Key + "OBJROT", objRotation);
		}
		
	}
	
	public void LoadObjectPlacing ()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer)) {
			string Key = Application.loadedLevelName;
			if (itemManager) {
				string objID = PlayerPrefs.GetString (Key + "OBJID");
				string objPosition = PlayerPrefs.GetString (Key + "OBJPOS");
				string objRotation = PlayerPrefs.GetString (Key + "OBJROT");
			
				string[] ObjectsID = objID.Split ("," [0]);
				string[] ObjectsPositionRaw = objPosition.Split ("|" [0]);
				string[] ObjectsRotationRaw = objRotation.Split ("|" [0]);
				Vector3[] ObjectsPosition = new Vector3[ObjectsID.Length];
				Quaternion[] ObjectsRotation = new Quaternion[ObjectsID.Length];
			
				for (int i=0; i<ObjectsID.Length; i++) {
					if (ObjectsID [i] != "") {
						string[] positionraw = ObjectsPositionRaw [i].Split ("," [0]);
						if (positionraw.Length > 2) {
							Vector3 position = Vector3.zero;
							float.TryParse (positionraw [0], out position.x);
							float.TryParse (positionraw [1], out position.y);
							float.TryParse (positionraw [2], out position.z);
							ObjectsPosition [i] = position;
						}
					
						string[] rotationraw = ObjectsRotationRaw [i].Split ("," [0]);
						if (rotationraw.Length > 3) {
							Quaternion rotation = Quaternion.identity;
							float.TryParse (rotationraw [0], out rotation.x);
							float.TryParse (rotationraw [1], out rotation.y);
							float.TryParse (rotationraw [2], out rotation.z);
							float.TryParse (rotationraw [3], out rotation.w);
							ObjectsRotation [i] = rotation;
						}
					
						itemManager.DirectPlacingObject (ObjectsID [i], ObjectsPosition [i], ObjectsRotation [i]);
					}
				}
			}
		}
	}
	
	public void ClearObjectPlacing ()
	{
		string Key = Application.loadedLevelName;
		PlayerPrefs.SetString (Key + "OBJID", "");
		PlayerPrefs.SetString (Key + "OBJPOS", "");
		PlayerPrefs.SetString (Key + "OBJROT", "");
	}
	
	/*void OnGUI ()
	{
		if (GUI.Button (new Rect (0, 0, 100, 30), "Save")) {
			SaveObjectPlacing ();	
		}
		
		if (GUI.Button (new Rect (0, 40, 100, 30), "Load")) {
			LoadObjectPlacing ();	
		}
	}*/
}
