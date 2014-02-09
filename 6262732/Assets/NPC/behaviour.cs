using UnityEngine;
using System.Collections;

public class behaviour : FSM {

	public enum FSMState {
		Wander, 
		Arrive,
		Flee,
		Frozen
	}

	public FSMState _CurrentState;

	private float _CurrenSpeed;

	private float _CurrentRotSpeed;
	private float _CompleteRot;
	private float _MaxSpeed = 0.5f;

	protected override void Initialize() {
		_CurrentState = FSMState.Arrive;
		_CurrenSpeed = 3.0f;
		_CurrentRotSpeed = 1.5f;

		pointList = GameObject.FindGameObjectsWithTag("WANDER");
		FindNextPoint();
	}

	protected void FindNextPoint()
	{
		int rndIndex = Random.Range(0, pointList.Length);
		float rndRadius = 12.0f;
		
		Vector3 rndPosition = Vector3.zero;
		destPos = pointList[rndIndex].transform.position + rndPosition;
		
//		rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0.0f, Random.Range(-rndRadius, rndRadius));
		destPos = pointList[rndIndex].transform.position;
	}
	
	protected override void FSMUpdate() {
		if (this.tag == "IT")		FindTarget ("NPC");
		if (this.tag == "NPC")		FindTarget ("FROZEN");

		switch (_CurrentState) {
		case FSMState.Wander: 	UpdateWander (); break;
		case FSMState.Arrive: 	UpdateArrive (); break;
		case FSMState.Flee:		UpdateFlee();	 break;
		case FSMState.Frozen: 					 break;
		}

		UpdateBounds ();
	}

	protected void UpdateBounds() {
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
	protected void UpdateArrive() {
		Vector3 Direction = destPos - this.transform.position;
		Direction.y = 0;
		Vector3 Normal = Vector3.Normalize (Direction);

		this.transform.LookAt (destPos);

		this.transform.Translate (Vector3.forward * Time.deltaTime * _MaxSpeed * _CurrenSpeed);

	}
	protected void UpdateFlee() {
		Vector3 Direction = this.transform.position - destPos;
		Direction.y = 0;
		Vector3 Normal = Vector3.Normalize (Direction);
		this.transform.LookAt (-destPos);
		this.transform.Translate (Normal * Time.deltaTime * _MaxSpeed * _CurrenSpeed);
	}

	protected void FindTarget(string tag) {
		// Determine targets for IT
		// Loop through each NPC
		distPos = 12.0f;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag)) {
			if (Vector3.Distance (this.transform.position, obj.transform.position) <= distPos) {
				distPos = Vector3.Distance (this.transform.position, obj.transform.position);
				destPos = obj.transform.position;
			}
		}
		if (tag == "Frozen") {
			GameObject obj = GameObject.FindGameObjectWithTag("IT");
			if (Vector3.Distance (this.transform.position, obj.transform.position) <= distPos) {
				distPos = Vector3.Distance (this.transform.position, obj.transform.position);
				destPos = obj.transform.position;
				_CurrentState = FSMState.Flee;
			}
		}
		if (distPos == 12.0f)
			_CurrentState = FSMState.Wander;
	}

	protected void UpdateWander() {
		if (Vector3.Distance(transform.position, destPos) <= 2.0f)
			FindNextPoint();

		transform.LookAt (destPos);
		transform.Translate(Vector3.forward * Time.deltaTime * _MaxSpeed * _CurrenSpeed);
	}
	
	void OnCollisionEnter(Collision c){
		behaviour a =(behaviour) c.gameObject.GetComponent(typeof(behaviour));
		if (c.gameObject.tag == "NPC" && this.tag == "IT") {
			a.Encounter();
		}
		if (c.gameObject.tag == "NPC" && this.tag == "NPC") {
			a.Save();
		}
	}

	public void Encounter() {
		this.tag = "FROZEN";
		_CurrentState = FSMState.Frozen;
	}
	public void Save() {
		this.tag = "NPC";
		_CurrentState = FSMState.Wander;
	}
}
