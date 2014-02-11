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

	private float _CurrentRotSpeed;

	private const float _MaxSpeed = 3.0f;
	private float _CurrenSpeed;
	private float _MaxVelocity = 0.5f;

	private const float  _UpperLimit = 8f;
	private const float  _LowerLimit = 4f;
	private const float _FoV = 30f;
	private float _LastRotation = _FoV;
	private Vector3 _SeekVelocity;
	private bool _Kinematic;
	private int _ButtonTimer;

	protected override void Initialize() {
		_Kinematic = true;
		_SeekVelocity = Vector3.right * 1.5f;
		_CurrentState = FSMState.Arrive;
		_CurrenSpeed = _MaxSpeed;
		_CurrentRotSpeed = 1.5f;
		pointList = GameObject.FindGameObjectsWithTag("WANDER");
		FindNextPoint();
		_ButtonTimer = 500;
	}

	protected void FindNextPoint()
	{
		int rndIndex = Random.Range(0, pointList.Length);
		destPos = pointList[rndIndex].transform.position;
	}
	
	protected override void FSMUpdate() {
		if (Input.GetButton ("Switch") && _ButtonTimer >= 100) {
			_Kinematic = !_Kinematic;
			_ButtonTimer = 0;
		} else
			_ButtonTimer++;

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
		movement.x = position.x;
		movement.z = 10;
		if (position.z >= 10)
			this.transform.position = -movement;
		else if (position.z <= -10)
			this.transform.position = movement;
		
		movement.z = position.z;
		movement.x = 10;
		if (position.x >= 10)
			this.transform.position = -movement;
		else if (position.x <= -10)
			this.transform.position = movement;

		movement = Vector3.zero;

		if (transform.position.y != 0.5f) {
			movement.y = 0.5f - transform.position.y;
			this.transform.Translate (movement);
		}

	}
	protected void UpdateArrive() {
		Vector3 Direction = destPos - this.transform.position;
		Vector3 perp = Vector3.Cross (transform.forward, Direction);
		float CurrentRotation = Vector3.Dot(perp, Vector3.up);

		if (_Kinematic) {
			Kinematic (Direction, CurrentRotation);
			_CurrenSpeed = Mathf.Clamp (_CurrenSpeed, 0, _MaxSpeed);
			this.transform.Translate (Vector3.forward * Time.deltaTime * _MaxVelocity * 1.75f * _CurrenSpeed);
		} else {
			Steering (Direction, CurrentRotation);
			this.transform.Translate (Time.deltaTime * 1.50f * (_SeekVelocity + Vector3.forward));
		}

	}
	protected void Steering(Vector3 Direction, float CurrentRotation) {
		Vector3 Acceleration = _MaxSpeed * Vector3.Normalize (Direction);
		_CurrenSpeed = Vector3.Magnitude (_SeekVelocity);
		float TimeToTarget = 2.5f;

		if (_CurrenSpeed > _MaxSpeed) {
			Vector3.Normalize(_SeekVelocity);
			_SeekVelocity = _SeekVelocity * _MaxSpeed;
		}
		else if (_CurrenSpeed < _MaxSpeed) {
			if (distPos <= _LowerLimit) {
				this.transform.Rotate(Vector3.up, CurrentRotation);
				_SeekVelocity = Acceleration - Vector3.forward;
				_SeekVelocity /= TimeToTarget;
			}
			else if (distPos < _UpperLimit){
				if (CurrentRotation > 1)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < -1)
					this.transform.Rotate(Vector3.up, -1);
				else {
					_SeekVelocity = Acceleration - Vector3.forward;
					_SeekVelocity /= TimeToTarget;
				}
			}
		} else {
			if (Vector3.Angle(Direction, transform.forward) <= _FoV) {
				if (CurrentRotation > 0)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < 0)
					this.transform.Rotate(Vector3.up, -1);
				_SeekVelocity = Acceleration - Vector3.forward;
				_SeekVelocity /= TimeToTarget;
			}
			else {
				_CurrenSpeed = 0;
				_SeekVelocity = Vector3.right * 1.5f;
			}
		}
	}
	protected void Kinematic(Vector3 Direction, float CurrentRotation) {
		if (_CurrenSpeed < 3f) {
			if (distPos <= _LowerLimit) {
				this.transform.Rotate(Vector3.up, CurrentRotation);
				_CurrenSpeed = 3f;
			}
			else if (distPos < _UpperLimit){
				_CurrenSpeed = 0;
				if (CurrentRotation > 1)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < -1)
					this.transform.Rotate(Vector3.up, -1);
				else 
					_CurrenSpeed = 3f;
			}
		} else {
			if (Vector3.Angle(Direction, transform.forward) <= _FoV) {
				if (CurrentRotation > 0)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < 0)
					this.transform.Rotate(Vector3.up, -1);
			} else {
				_CurrenSpeed = 0f;
			}
		}
	}

	protected void UpdateFlee() {
		Vector3 Direction = this.transform.position - destPos;
		Vector3 perp = Vector3.Cross (transform.forward, Direction);
		float CurrentRotation = Vector3.Dot(perp, Vector3.up);

		if (_Kinematic) {
			Kinematic (Direction, CurrentRotation);
			this.transform.Translate (Vector3.forward * Time.deltaTime * _MaxVelocity * 1.105f * _CurrenSpeed);
		} else {
			Steering (Direction, CurrentRotation);
			this.transform.Translate (Time.deltaTime * (_SeekVelocity + Vector3.forward));
		}

	}

	protected void FindTarget(string target) {
		distPos = _UpperLimit;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag(target)) {
			if (Vector3.Distance (this.transform.position, obj.transform.position) < distPos) {
				distPos = Vector3.Distance (this.transform.position, obj.transform.position);
				destPos = obj.transform.position;
				_CurrentState = FSMState.Arrive;
			}
		}
		if (target == "FROZEN" && this.tag != "IT") {
			GameObject obj = GameObject.FindGameObjectWithTag("IT");
			if (Vector3.Distance (this.transform.position, obj.transform.position) < distPos) {
				distPos = Vector3.Distance (this.transform.position, obj.transform.position);
				destPos = obj.transform.position;
				_CurrentState = FSMState.Flee;
			}
		}
		if (distPos >= _UpperLimit)
			_CurrentState = FSMState.Wander;
		destPos.y = 0.5f;
	}

	protected void UpdateWander() {
		_CurrenSpeed = 3f;
		if (Vector3.Distance(transform.position, destPos) <= 2.0f)
			FindNextPoint();

		transform.LookAt (destPos);
		transform.Translate(Vector3.forward * Time.deltaTime * _MaxVelocity * _CurrenSpeed);
	}
	
	void OnCollisionEnter(Collision c){
		behaviour a =(behaviour) c.gameObject.GetComponent(typeof(behaviour));
		if (c.gameObject.tag == "NPC" && this.tag == "IT") {
			a.Encounter();
			this._CurrentState = FSMState.Wander;
		}
		if (c.gameObject.tag == "FROZEN" && this.tag == "NPC") {
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
