using UnityEngine;
using System.Collections;

public class DestroyTest : MonoBehaviour {

	public float duration = 5;
	float temp;
	void Start () {
		temp = Time.time;
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time>temp + duration){
			Remove();	
		}
	}
	
	void Remove(){
		Network.RemoveRPCs (GetComponent<NetworkView>().viewID);
		Network.Destroy (GetComponent<NetworkView>().viewID);	
	}
}
