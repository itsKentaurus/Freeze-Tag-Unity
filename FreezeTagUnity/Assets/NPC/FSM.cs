using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour {
	
	//Player Transform
	protected Transform playerTransform;
	
	//Next destination position of NPC 
	public Vector3 destPos;
	protected float distPos;
	public behaviour targetNPC;

	//List of points for patrolling
	protected GameObject[] pointList;
	
	protected virtual void Initialize() { }
	protected virtual void FSMUpdate() { }
	
	// Use this for initialization
	void Start () 
	{
		Initialize();
	}
	
	// Update is called once per frame
	void Update () 
	{
		FSMUpdate();
	}    
}
