using UnityEngine;
using System.Collections;

public class behaviour : MonoBehaviour {

	public string name;

	private Vector3 _TargetPosition;
	private string _TargetName;

	private int _Timer = 0;

	private const float _LimitDistance = 3f;
	private const float _FarDistance = 6f;
	private const float _MaxDistance = 100f;

	private const string _NPC1 = "it";
	private const string _NPC2 = "npc";
	private const string _NPC3 = "frozen";
	private const string _NPC4 = "none";

	private bool _Slow;
	private bool _Fast;

	private Vector3 _Forward = Vector3.zero;

	private const float _MaxVeloctiy = 2;

	private Vector3 _VelocityDirection;
	private Vector3 _Velocity;
	private bool _Rotattion = false;
	private float _CurrentVelocity;

	// Use this for initialization
	void Start () {
		_Forward.x = 0.02f;
		_CurrentVelocity = 1f;
		_Slow = true;
		_Fast = true;
	}
	
	// Update is called once per frame
	void Update () {
		Targeting ();
		if (this.tag != _NPC3)
			MoveKinematicSlow ();
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
		CheckingTargets (_NPC3);

		Vector3 itPosition = GameObject.FindGameObjectWithTag (_NPC1).transform.position;
		Vector3 myPosition = this.transform.position;

		float distance1 = Vector3.Distance (itPosition, myPosition);
		float distance2 = 0;
		if (_TargetPosition.x != 1000)
				distance2 = Vector3.Distance (_TargetPosition, myPosition);
		else {
			_TargetPosition = itPosition;
			_TargetName = _NPC1;
		}
		if (distance1 < distance2) {
			_TargetPosition = itPosition;
			_TargetName = _NPC1;
		}
	}

	void IT_Targets() {
		CheckingTargets (_NPC2);
	}

	void CheckingTargets(string target, float distance = _MaxDistance) {
		Vector3 myPosition = this.transform.position;
		_TargetName = _NPC4;
		_TargetPosition = Vector3.zero;
		_TargetPosition.x = 1000;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(target))
			if (distance > Vector3.Distance (myPosition, obj.transform.position)) {
				distance = Vector3.Distance (myPosition, obj.transform.position);
				_TargetPosition = obj.transform.position;
				_TargetName = _NPC3;
			}
	}

	#endregion

	#region Movements
	void KinematicAlgorithm() {
		if (_TargetName == _NPC2 || _TargetName == _NPC3)
			KinematicArrive ();
		else if (_TargetName == _NPC1)
			KinematicFlee ();
	}

	void MoveKinematicSlow() {
		KinematicAlgorithm ();
		float Magnitude = Vector3.Magnitude (_VelocityDirection);
		if (Magnitude > _FarDistance)
			Wander ();
		else {
			if (Magnitude > _LimitDistance) {
			}
			else {
			
			}
		
			this.transform.Translate(_Velocity * (_MaxVeloctiy ) * Time.deltaTime);
			IncreaseSpeed();
		}
	}

	void IncreaseSpeed() {
		if (_Timer == 250 && _CurrentVelocity < 2.0f) {
			_CurrentVelocity += 1f;
			_Timer = 0;
		}
		else
			_Timer++;
	}

	void CheckSpeed() {
		if (_CurrentVelocity <= 2) {

		} 
		else {
		}
	}

	void KinematicArrive() {
		_VelocityDirection = _TargetPosition - this.transform.position;
		_Velocity = Vector3.Normalize(_VelocityDirection);
	}

	void KinematicFlee() {
		_VelocityDirection = this.transform.position - _TargetPosition;
		_Velocity = Vector3.Normalize(_VelocityDirection);
	}

	void Wander() {
		this.transform.Translate(_Forward * (_CurrentVelocity / _MaxVeloctiy ));
		this.transform.Rotate (Vector3.up, Random.value * RandomNumber());
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
		if (c.gameObject.tag == _NPC1)
			this.tag = _NPC3;

	}

	void CheckLast() {

	}

	int RandomNumber() {
		float num = Random.value;
		if (num < 0.55)
			return 1;
		else
			return -1;
	}


}
