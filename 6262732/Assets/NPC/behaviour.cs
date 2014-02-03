using UnityEngine;
using System.Collections;

public class behaviour : MonoBehaviour {

	private Vector3 _TargetPosition;
	private string _TargetName;

	private const float _LimitDistance = 4f;
	private const float _FarDistance = 10f;
	private const float _UpperDistance = 100f;

	private const string _NPC1 = "it";
	private const string _NPC2 = "npc";
	private const string _NPC3 = "frozen";
	private const string _NPC4 = "none";

	private Vector3 _Forward = Vector3.zero;
	// Use this for initialization
	void Start () {
		_Forward.x = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
		Targeting ();
		Movement ();
		Wander ();
		CheckBorder ();
	}

	void Targeting() {
		if (this.tag == _NPC1)
			IT_Targets ();
		else if (this.tag == _NPC2)
			NPC_Targets ();
		else {
			_TargetPosition = Vector3.zero;
			_TargetName = _NPC4;
		}
	}

	#region Targeting
	void NPC_Targets() {

		Vector3 itPosition = _TargetPosition = GameObject.FindGameObjectWithTag (_NPC1).transform.position;
		_TargetName = _NPC1;
		Vector3 myPosition = this.transform.position;

		if (CheckDistanceFar (GameObject.FindGameObjectWithTag (_NPC1))) {
			float distance = Vector3.Distance (itPosition, myPosition);
			CheckingTargets (_NPC3, distance);
		}
	}

	void IT_Targets() {
		CheckingTargets (_NPC2);
	}

	void CheckingTargets(string target, float distance = _UpperDistance) {
		Vector3 myPosition = this.transform.position;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(target))
			if (distance > Vector3.Distance (myPosition, obj.transform.position)) {
				distance = Vector3.Distance (myPosition, obj.transform.position);
				_TargetPosition = obj.transform.position;
				_TargetName = _NPC3;
			}
	}

	#endregion

	#region Movements
	void Movement() {
		
	}
	void Wander() {
		this.transform.Translate (_Forward);
		this.transform.Rotate (Vector3.up, Random.value * (-1f * RandomNumber()));
	}

	#endregion

	bool CheckDistanceFar(GameObject o) {
		return (Vector3.Distance (o.transform.position, this.transform.position) > _LimitDistance);
	}

	void CheckBorder() {
		Vector3 position = this.transform.position;
		Vector3 movement = Vector3.zero;
		movement.z = 20;
		if (position.z >= 10)
			this.transform.position = position - movement;
		else if (position.z <= -10)
			this.transform.position = position + movement;

		movement.z = 0;
		movement.x = 20;
		if (position.x >= 10)
			this.transform.position = position - movement;
		else if (position.x <= -10)
			this.transform.position = position + movement;
	}

	void OnCollisionEnter(Collision c){

		if (this.tag == _NPC1) {
			if (c.gameObject.tag == _NPC2)
				c.gameObject.tag = _NPC3;
		}
		if (this.tag == _NPC2) {
			if (c.gameObject.tag == _NPC3)
				c.gameObject.tag = _NPC2;
		}
	}

	int RandomNumber() {
		float num = Random.value;
		if (num < 0.5)
			return 0;
		else
			return 1;
	}


}
