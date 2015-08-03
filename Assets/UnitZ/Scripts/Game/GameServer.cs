//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class GameServer : MonoBehaviour
{
	
	public string ServerName = "UZ_coop";
	public int Port = 25000;
	public int MaxPlayer = 32;
	public string IPServer = "127.0.0.1";
	public bool LanOnly;
	public bool UseNat = false;
	[HideInInspector]
	public bool isActive = false;
	private NetworkView networkViewer;
	
	void Start ()
	{
		networkViewer = this.GetComponent<NetworkView> ();
	}

	void Update ()
	{
	
	}
	
	public void StartServer ()
	{
		isActive = false;
		
		if (LanOnly) {
			UseNat = false;
		} else {
			UseNat = !Network.HavePublicAddress ();
		}
		
		Network.InitializeServer (MaxPlayer, Port, UseNat);

		if (!LanOnly) {
			MasterServer.RegisterHost (ServerName, "World " + SystemInfo.deviceName + "  " + SystemInfo.deviceType);
		}
	}
	
	public void KillServer ()
	{
		isActive = false;
		Network.Disconnect ();
		if (Network.isServer) {
			MasterServer.UnregisterHost ();
		}
		UnitZ.playersManager.PlayerList.Clear ();
		Debug.Log ("Kill Server");
	}
	
	void OnPlayerConnected (NetworkPlayer player)
	{
		if (Network.isServer) {
			Debug.Log ("Player " + player.ipAddress + ":" + player.port + " is connected");
			
			if (UnitZ.chatLog != null)
				UnitZ.chatLog.AddLog ("<color=gray>" + player.ipAddress + " is joined!</color>");
			
			if (networkViewer)
				networkViewer.RPC ("PlayerConnectedCallback", player, player.ToString (), UnitZ.gameManager.CurrentLevel, UnitZ.gameManager.lastLevelPrefix, UnitZ.GameKeyVersion, isActive);
			
		}
	}
	
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		if (Network.isServer) {
			if (UnitZ.chatLog != null)
				UnitZ.chatLog.AddLog ("<color=gray>" + player.ipAddress + " is disconnected!</color>");
			
			Network.RemoveRPCs (player);
			Network.DestroyPlayerObjects (player);
			
			UnitZ.playerManager.RemovePlayerCharacter(player.ToString());
		}
	}
	
	void OnMasterServerEvent (MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.RegistrationSucceeded)
			Debug.Log ("Server registered"); 
	}
	
	[RPC]
	void PlayerRegisteration (NetworkPlayer player, string name)
	{
		if (UnitZ.playersManager)
			UnitZ.playersManager.AddPlayer (player.ToString ());
	}
	
	void OnServerInitialized ()
	{
		if(networkViewer)
			UnitZ.gameManager.PlayerID = networkViewer.owner.ToString ();
		Debug.Log ("Server initialized!");
	}
	
	public string GetLocalIPAddress ()
	{
		IPHostEntry host;
		string localIP = "";
		host = Dns.GetHostEntry (Dns.GetHostName ());
		foreach (IPAddress ip in host.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				localIP = ip.ToString ();
			}
		}
		return localIP;
	}
	
	[RPC]
	void PingTest (float time,NetworkMessageInfo info)
	{
		if (networkViewer)
			networkViewer.RPC ("PingReceived", info.sender,time);
	}
}

