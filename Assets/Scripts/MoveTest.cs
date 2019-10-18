using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour 
{
	public GameObject target;

	// Use this for initialization
	void Start () 
	{

		
	}
	
	// Update is called once per frame
	void Update () 
	{
		MoveForwardToTarget( transform.right, Time.deltaTime, target.transform );
	}

	void MoveForwardToTarget(Vector3 forwardVelocity, float deltaTime, Transform target)
	{
		float distanceLeft = (target.position - transform.position).magnitude;
		float deltaDist = forwardVelocity.magnitude * deltaTime;

		if (distanceLeft < deltaDist)
		{
			//forwardVelocity = forwardVelocity * distanceLeft / deltaDist;
			//forwardVelocity = forwardVelocity * distanceLeft;
			transform.position = target.position;

			Debug.DrawRay( transform.position, forwardVelocity, Color.red, deltaTime );
			//Debug.Break();
		}
		else
		{
			Debug.DrawRay( transform.position, forwardVelocity, Color.green, deltaTime );
			transform.position += forwardVelocity * deltaTime;
		}

		//transform.position += forwardVelocity * deltaTime;
	}
}
