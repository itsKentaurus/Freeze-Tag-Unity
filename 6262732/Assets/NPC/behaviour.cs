using UnityEngine;
using System.Collections;

public class behaviour : MonoBehaviour {

	private Vector3 _TargetPosition;
	private string _TargetName;

	private float _SmallDistance = 4;
	private float _BigDistance = 7;

	private bool _Slow;

	private const string _NPC1 = "it";
	private const string _NPC2 = "npc";
	private const string _NPC3 = "frozen";
	private const string _NPC4 = "none";
	
	private Vector3 _Forward = Vector3.zero;

	private Vector3 _VelocityDirection;
	private Vector3 _Velocity;

	private float _CurrentRotation;

	private float NUMBER = 0;
	private float Timer = 0;
	private float _Orientation;
	void Start() {
		_Orientation = 0;
		_Slow = true;
		_Forward.z = Mathf.Sin(_Orientation);
		_Forward.x = Mathf.Cos(_Orientation);
	}

	void Update() {
		Targeting ();
		KinamaticAlgorithm ();
		if (Timer == 90) {
				print (90);
			Timer = 0;
		}
//		Timer++;
		transform.Rotate (Vector3.up, 1);
		CheckBorder ();
	}

	void KinamaticAlgorithm() {
		float Distance = Vector3.Distance (_TargetPosition, this.transform.position);
		Vector3 Hipothenuse = _TargetPosition - this.transform.position;

		float RotationNeeded = Mathf.Acos(Vector3.Dot(Hipothenuse, _Forward));

		if ( _CurrentRotation == 0)
			_CurrentRotation = RotationNeeded / 100f;

		if (_TargetName == _NPC2 || _TargetName == _NPC3)
			KinematicArrive (_CurrentRotation, RotationNeeded);
		else if (_TargetName == _NPC1)
			KinematicFlee (_CurrentRotation, RotationNeeded);
		else {
			_TargetPosition = Vector3.zero;
			_TargetName = _NPC4;
		}
		if (_TargetName != _NPC4) {
			UpdateForward ();
			this.transform.Translate (_Forward);
		}
	}
	void KinematicArrive(float Distance, float RotationNeeded) {

		_VelocityDirection = _TargetPosition - this.transform.position;
		_Velocity = Vector3.Normalize(_VelocityDirection);

		float Rotate = Mathf.Acos(Vector3.Dot (_Velocity, _Forward) / Vector3.Magnitude(_Forward));

		if (Distance < _SmallDistance) {
			_Orientation += Rotate ;
		} else if (Distance < _BigDistance) {
		}
		else Wander ();
	}
	void KinematicFlee(float Distance, float Number) {
		_VelocityDirection = this.transform.position - _TargetPosition;
		_Velocity = Vector3.Normalize(_VelocityDirection);

		if (_Slow) {
			if (Distance < _SmallDistance) {
			} else if (Distance < _BigDistance) {
			}
			else Wander ();
		} else {
			if (Distance < _SmallDistance) {

			} else if (Distance < _BigDistance) {
			}
			else Wander ();
		}
	}

	void Wander() {
		float Rotate = Random.value * 2 * RandomNumber ();
		_Orientation += Rotate;
		UpdateForward ();
		this.transform.Rotate (Vector3.up, _Orientation);

	}

	void UpdateForward() {
		if (_Orientation > 360)
			_Orientation -= 360;
		if (_Orientation < 0)
			_Orientation += 360;
		float Temp = _Orientation % (Mathf.PI * 2);
		_Forward.z = Mathf.Sin(Temp );
		_Forward.x = Mathf.Cos(Temp );
		_Forward /= 8f;
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
	
	void CheckingTargets(string target) {
		float distance = 100f;
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

	int RandomNumber() {
		float num = Random.value;
		if (num < 0.55)
			return 1;
		else
			return -1;
	}

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == _NPC1)
			this.tag = _NPC3;
	}
}
