//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {
	
	private FPSController fpsController;
	public Texture2D CrosshairImg;
	public Texture2D CrosshairZoomImg;
	void Start () {
		if (this.transform.root) {
			fpsController = this.transform.root.GetComponent<FPSController> ();
		} else {
			fpsController = this.transform.GetComponent<FPSController> ();
		}
	}
	
	void Update () {
	
	}
	
	void OnGUI(){
		if(fpsController){
			if(fpsController.zooming){
				if(CrosshairZoomImg){
					GUI.DrawTexture(new Rect(Screen.width/2 - CrosshairZoomImg.width/2,Screen.height/2 - CrosshairZoomImg.height/2,CrosshairZoomImg.width,CrosshairZoomImg.height),CrosshairZoomImg);	
				}
			}else{
				if(CrosshairImg){
					GUI.DrawTexture(new Rect(Screen.width/2 - CrosshairImg.width/2,Screen.height/2 - CrosshairImg.height/2,CrosshairImg.width,CrosshairImg.height),CrosshairImg);	
				}
			}
		}else{
			if(CrosshairImg){
				GUI.DrawTexture(new Rect(Screen.width/2 - CrosshairImg.width/2,Screen.height/2 - CrosshairImg.height/2,CrosshairImg.width,CrosshairImg.height),CrosshairImg);	
			}
		}
	}
}
