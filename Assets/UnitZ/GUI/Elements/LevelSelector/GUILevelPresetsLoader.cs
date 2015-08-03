//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUILevelPresetsLoader : MonoBehaviour
{

	private GameManager gameManage;
	public RectTransform LevelPrefab;
	public Transform Canvas;

	void Start ()
	{
		gameManage = (GameManager)GameObject.FindObjectOfType (typeof(GameManager));
		DrawGameLobby ();
	}

	void Update ()
	{
	
	}
	
	void ClearCanvas ()
	{
		if (Canvas == null)
			return;
		
		foreach (Transform child in Canvas.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}

	public void DrawGameLobby ()
	{
		if (gameManage == null || Canvas == null || LevelPrefab == null)
			return;
		
		if (UnitZ.sceneManager != null) {	
			ClearCanvas ();
			for (int i=0; i<UnitZ.sceneManager.LevelPresets.Length; i++) {

				GameObject obj = (GameObject)GameObject.Instantiate (LevelPrefab.gameObject, Vector3.zero, Quaternion.identity);
				obj.transform.SetParent (Canvas.transform);
				
				GUILevelPresetSelector level = obj.GetComponent<GUILevelPresetSelector> ();
				if (level) {
					level.Level = UnitZ.sceneManager.LevelPresets [i];	
				}
				
				RectTransform rect = obj.GetComponent<RectTransform> ();
				if (rect) {
					rect.anchoredPosition = new Vector2 (5, -(((LevelPrefab.sizeDelta.y + 5) * i)));
					rect.localScale = LevelPrefab.gameObject.transform.localScale;
				}	
			}
			RectTransform rootRect = Canvas.gameObject.GetComponent<RectTransform> ();
			rootRect.sizeDelta = new Vector2 (rootRect.sizeDelta.x, (LevelPrefab.sizeDelta.y + 5) * UnitZ.sceneManager.LevelPresets.Length);
		}
	}

}
