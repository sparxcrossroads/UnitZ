//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterSystem))]

public class CharacterHUD : MonoBehaviour
{

	[HideInInspector]
	public CharacterSystem character;
	private CharacterLiving living;
	public Texture2D Icon_HP, Icon_Armor,Icon_Food,Icon_Water,Bg,Bar_Food,Bar_Water,Bar_Health;
	public GUISkin Skin;
	
	void Start ()
	{
		character = gameObject.GetComponent<CharacterSystem> ();
		living = gameObject.GetComponent<CharacterLiving> ();
		StyleManager style = (StyleManager)GameObject.FindObjectOfType(typeof(StyleManager));
		if(style && !Skin){
			Skin = style.GetSkin(0);	
		}
	}

	void DrwaIcon(int pos, Texture2D icon,string text,float percent,Texture2D bar){
		if (icon) {
			GUI.DrawTexture (new Rect (20 + (pos * 180)+20, Screen.height - 65, 50, 50), icon);	
		}
		if(Bg && bar){
			GUI.DrawTexture (new Rect (80 + (pos * 180)+20, Screen.height - 55, 100, 30), Bg);		
			GUI.DrawTexture (new Rect (80 + (pos * 180)+20, Screen.height - 55, 100 * percent, 30), bar);		
			
		}
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.skin.label.fontSize = 22;
		GUI.Label (new Rect (80 + (pos * 180)+30, Screen.height - 55, 200, 30), text);		
	}
	
	void OnGUI ()
	{
		if(Skin)
			GUI.skin = Skin;
		
		bool offlineMode = (!Network.isServer && !Network.isClient);
		if (offlineMode || (GetComponent<NetworkView>().isMine)) {
			
			DrwaIcon(0,Icon_HP,character.HP.ToString()+" %",((float)character.HP /(float)character.HPmax),Bar_Health);
			if(living){
				DrwaIcon(1,Icon_Food,living.Hungry.ToString()+" %",((float)living.Hungry /(float)living.HungryMax),Bar_Food);
				DrwaIcon(2,Icon_Water,living.Water.ToString()+" %",((float)living.Water /(float)living.WaterMax),Bar_Water);
			}
		}
	}
}
