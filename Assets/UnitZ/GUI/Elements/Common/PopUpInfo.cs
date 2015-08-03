using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpInfo : MonoBehaviour {

	public Text ContentText;
	void Start () {
	
	}
	
	public void Close(){
		gameObject.SetActive(false);	
	}

}
