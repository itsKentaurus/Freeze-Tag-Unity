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
		_SeekVelocity = Vector3.right;
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
		float rndRadius = 10.0f;
		
		Vector3 rndPosition = Vector3.zero;
		destPos = pointList[rndIndex].transform.position + rndPosition;
		destPos = pointList[rndIndex].transform.position;
	}
	
	protected override void FSMUpdate() {
		if (Input.GetButton ("Switch") && _ButtonTimer >= 100) {
			_Kinematic = !_Kinematic;
			_ButtonTimer = 0;
			print ("SWITCH");
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

		if (_Kinematic)
			KinematicArrive (Direction, CurrentRotation);
		else
			SteeringArrive (Direction, CurrentRotation);

	}
	protected void SteeringArrive(Vector3 Direction, float CurrentRotation) {
		Vector3 Acceleration = _MaxSpeed * Vector3.Normalize (Direction);

		_CurrenSpeed = Vector3.Magnitude (_SeekVelocity);
			
		if (_CurrenSpeed < _MaxSpeed) {
			if (distPos <= _LowerLimit) {
				this.transform.Rotate(Vector3.up, CurrentRotation);
				_SeekVelocity = _SeekVelocity + Acceleration * (1 / 4);
			}
			else if (distPos < _UpperLimit){
				_CurrenSpeed = 0;
				if (CurrentRotation > 1)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < -1)
					this.transform.Rotate(Vector3.up, -1);
				else 
					_SeekVelocity = _SeekVelocity + Acceleration * (1 / 4);
			}
		} else {
			if (Vector3.Angle(Direction, transform.forward) <= _FoV) {
				if (CurrentRotation > 0)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < 0)
					this.transform.Rotate(Vector3.up, -1);
			} else {
				_CurrenSpeed = 0f;
				_SeekVelocity = _SeekVelocity + Acceleration * (1 / 4);
			}
		}

		this.transform.Translate (Time.deltaTime * 1.50f * (_SeekVelocity + Vector3.forward));
	}
	protected void KinematicArrive(Vector3 Direction, float CurrentRotation) {
		if (_CurrenSpeed < 1.5f) {
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
		_CurrenSpeed = Mathf.Clamp (_CurrenSpeed, 0, _MaxSpeed);
		this.transform.Translate (Vector3.forward * Time.deltaTime * _MaxVelocity * 1.75f * _CurrenSpeed);
	}

	protected void UpdateFlee() {
		Vector3 Direction = this.transform.position - destPos;
		Vector3 perp = Vector3.Cross (transform.forward, Direction);
		float CurrentRotation = Vector3.Dot(perp, Vector3.up);

		if (_Kinematic)
			KinematicFlee (Direction, CurrentRotation);
		else
			SteeringFlee (Direction, CurrentRotation);

	}
	protected void SteeringFlee (Vector3 Direction, float CurrentRotation) {
		Vector3 Acceleration = _MaxSpeed * Vector3.Normalize (Direction);
		
		_CurrenSpeed = Vector3.Magnitude (_SeekVelocity);
		
		if (_CurrenSpeed < _MaxSpeed) {
			if (distPos <= _LowerLimit) {
				this.transform.Rotate(Vector3.up, CurrentRotation);
				_SeekVelocity = _SeekVelocity + Acceleration * (1 / 4);
			}
			else if (distPos < _UpperLimit){
				_CurrenSpeed = 0;
				if (CurrentRotation > 1)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < -1)
					this.transform.Rotate(Vector3.up, -1);
				else 
					_SeekVelocity = _SeekVelocity + Acceleration * (1 / 4);
			}
		} else {
			if (Vector3.Angle(Direction, transform.forward) <= _FoV) {
				if (CurrentRotation > 0)
					this.transform.Rotate(Vector3.up, 1);
				else if (CurrentRotation < 0)
					this.transform.Rotate(Vector3.up, -1);
			} else {
				_CurrenSpeed = 0f;
				_SeekVelocity = _SeekVelocity + Acceleration * (1 / 4);
			}
		}
		
		this.transform.Translate (Time.deltaTime * (_SeekVelocity + Vector3.forward));
	}
	protected void KinematicFlee(Vector3 Direction, float CurrentRotation) {
		Vector3 yo = -destPos;
		yo.y = 0.5f;
		Direction = -Direction;
		Direction.y = 0.5f;
//		if (distPos <= _LowerLimit) {
//			_CurrenSpeed = 3f;
//		} else if (distPos <= _UpperLimit) {
//			_CurrenSpeed = 0;
//			if (CurrentRotation > -1)
//				this.transform.Rotate(Vector3.up, 1);
//			else if (CurrentRotation < 1)
//				this.transform.Rotate(Vector3.up, -1);
//			else 
//				_CurrenSpeed = 3f;
//		}
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
		_CurrenSpeed = Mathf.Clamp (_CurrenSpeed, 0, _MaxSpeed);
		//		this.transform.Rotate (Vector3.up, 180);
		this.transform.Translate (Vector3.forward * Time.deltaTime * _MaxVelocity * 1.35f * _CurrenSpeed);
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
