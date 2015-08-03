//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIGameRoomLoader : MonoBehaviour
{
	private GameManager gameManage;
	public RectTransform GameRoomPrefab;
	public RectTransform Canvas;
	public float TimeOut = 10;
	
	private float timeTemp;
	
	void Start ()
	{
		gameManage = (GameManager)GameObject.FindObjectOfType (typeof(GameManager));
		Refresh ();
	}

	void ClearCanvas ()
	{
		if (Canvas == null) 
			return;
			
		foreach (Transform child in Canvas.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}
	
	void OnEnable ()
	{
		Refresh ();
	}
	
	public void Refresh ()
	{
		if (gameManage) {
			gameManage.Refresh ();	
		}
		StartCoroutine (LoadGameRoom ());
	}

	public void DrawGameLobby ()
	{
		if (gameManage == null || Canvas == null || GameRoomPrefab == null)
			return;
		
		if (gameManage.gameList != null) {	
			ClearCanvas ();
			for (int i=0; i<gameManage.gameList.Length; i++) {
						
				HostData hostgame = gameManage.gameList [i];
				string ips = "";
				foreach (string ip in hostgame.ip) {
					ips += ip;
				}	
				GameObject obj = (GameObject)GameObject.Instantiate (GameRoomPrefab.gameObject, Vector3.zero, Quaternion.identity);
				obj.transform.SetParent (Canvas.transform);
				GUIGameRoom room = obj.GetComponent<GUIGameRoom> ();
				RectTransform rect = obj.GetComponent<RectTransform> ();
				if (rect) {
					rect.anchoredPosition = new Vector2 (5, -((GameRoomPrefab.sizeDelta.y * i)));
					rect.localScale = GameRoomPrefab.gameObject.transform.localScale;
				}
						
				if (room.RoomName) {
					room.RoomName.text = hostgame.gameName + " " + ips;
					room.hostData = hostgame;
				}
						
			}
			RectTransform rootRect = Canvas.gameObject.GetComponent<RectTransform> ();
			rootRect.sizeDelta = new Vector2 (rootRect.sizeDelta.x, GameRoomPrefab.sizeDelta.y * gameManage.gameList.Length);
		}
	}
	
	IEnumerator LoadGameRoom ()
	{
		timeTemp = Time.time;
		bool timeOut = false;
		if (gameManage) {
			while (gameManage.IsRefreshing && !timeOut) {
				if (Time.time > timeTemp + TimeOut) {
					timeOut = true;
				}
				yield return new WaitForEndOfFrame();
			}
			timeTemp = Time.time;
			DrawGameLobby ();
		}
	}

}
