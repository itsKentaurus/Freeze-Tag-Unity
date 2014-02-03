using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour {


	// Use this for initialization
	void Start () {
		int num = (int) (Random.value * 12);
		num %= 4;
		GameObject.FindGameObjectsWithTag ("npc") [num].tag = "it";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
