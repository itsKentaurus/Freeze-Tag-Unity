using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour {

	private GameObject LASTONE;

	// Use this for initialization
	void Start () {
		int num = (int) (Random.Range(0,3));
		GameObject.FindGameObjectsWithTag ("NPC") [num].tag = "IT";
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectsWithTag ("NPC").Length == 1)
			LASTONE = GameObject.FindGameObjectWithTag ("NPC");

		if (GameObject.FindGameObjectsWithTag ("FROZEN").Length == 4) {
			GameObject.FindGameObjectWithTag ("IT").tag = "NPC";
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("FROZEN"))
				if (obj.transform.position.x <= 20 
				    && obj.transform.position.x >= -20 
				    && obj.transform.position.z <= 20 
				    && obj.transform.position.z >= -20)
					obj.tag = "NPC";
			LASTONE.tag = "IT";
		}
	}
}
