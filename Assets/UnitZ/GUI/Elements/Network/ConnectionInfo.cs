//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionInfo : MonoBehaviour
{

	public InputField PortText;
	public InputField ServerIPText;

	void Start ()
	{
		if (UnitZ.gameServer) {
			if (PortText)
				PortText.text = UnitZ.gameServer.Port.ToString ();
			if (ServerIPText)
				ServerIPText.text = UnitZ.gameServer.IPServer;	
		}
	}
	
	public void SetServerIP (InputField num)
	{
		if (UnitZ.gameServer) {
			UnitZ.gameServer.IPServer = num.text;
		}
	}

	public void SetPort (InputField num)
	{
		if (UnitZ.gameServer) {
			int val = UnitZ.gameServer.Port;
			if (int.TryParse (num.text, out val)) {
				UnitZ.gameServer.Port = val;
			}
		}
	}
	
	
		

}
