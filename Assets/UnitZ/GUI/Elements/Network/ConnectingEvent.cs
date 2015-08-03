//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConnectingEvent : MonoBehaviour
{

	public Text TextInfo;
	public float Timeout = 10;
	public GameObject Preloading;
	private float timeTemp;
	private bool conneted;
	
	void OnEnable ()
	{
		if(Preloading)
			Preloading.SetActive(true);
		conneted = false;
		timeTemp = Time.time;	
		if (TextInfo)
			TextInfo.text = "Connecting to server";
	}
	
	void Start ()
	{
	}
	
	void Update ()
	{
		if(!conneted){
			if (Time.time >= Timeout + timeTemp) {
				if (TextInfo)
					TextInfo.text = "Connecting Time out";
				
				if(Preloading)
					Preloading.SetActive(false);
			}
		}
	}
	
	void OnFailedToConnect (NetworkConnectionError error)
	{
		if(Preloading)
			Preloading.SetActive(false);
		
		if (TextInfo)
			TextInfo.text = error.ToString ();
	}
	
	void OnFailedToConnectToMasterServer (NetworkConnectionError info)
	{
		if(Preloading)
			Preloading.SetActive(false);
		
		if (TextInfo)
			TextInfo.text = info.ToString ();
	}
	
	void OnConnectedToServer ()
	{
		conneted = true;
		if (TextInfo)
			TextInfo.text = "Connected! Loading GameOBject";
	}
	
	public void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		conneted = true;
		if(Preloading)
			Preloading.SetActive(false);
		
		if (TextInfo)
			TextInfo.text = "Disconnected!";
	}
}
