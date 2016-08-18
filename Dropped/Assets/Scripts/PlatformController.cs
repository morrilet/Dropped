using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController 
{
	public LayerMask passengerMask;

	public Lever[] levers; //Null if LeverControlType = None.
	public LeverControlType leverControlType = LeverControlType.None;

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;
	public float easeAmount;

	int fromWaypointIndex; //The index of the global waypoint we're moving away from.
	int toWaypointIndex; //The index of the global waypoint we're moving towards.
	float percentBetweenWaypoints; //Percentage between 0 and 1.
	float nextMoveTime;

	bool shouldMove; //Whether or not the platform should be moving.
	bool shouldGetNextWaypoint; //Whether or not the platform should find the next waypoint and proceed.

	List<PassengerMovement> passengerMovement; //Stores all of the movement to be applied to each passenger.
	Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

	public override void Start()
	{
		base.Start ();

		if(leverControlType == LeverControlType.None)
			shouldMove = true;
		else 
			shouldMove = false;

		if (leverControlType == LeverControlType.Waypoint)
			shouldGetNextWaypoint = false;
		else
			shouldGetNextWaypoint = true;

		foreach(Lever lever in levers)
			if(lever != null)
				lever.LeverSwitched += LeverAction;

		globalWaypoints = new Vector3[localWaypoints.Length];
		for(int i = 0; i < localWaypoints.Length; i++)
		{
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	public void OnDisable()
	{
		foreach(Lever lever in levers)
			if(lever != null)
				lever.LeverSwitched -= LeverAction;
	}

	void Update()
	{
		UpdateRaycastOrigins ();

		Vector3 velocity = Vector3.zero; 
		if(shouldMove)
		{
			velocity = CalculatePlatformMovement();
		}

		CalculatePassengerMovement (velocity);

		MovePassengers (true);
		transform.Translate (velocity);
		MovePassengers (false);
	}

	float Ease(float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow (x, a) / (Mathf.Pow (x, a) + Mathf.Pow (1 - x, a));
	}

	//Calculates the movement vector of the platform based on its waypoints.
	Vector3 CalculatePlatformMovement()
	{
		if(Time.time < nextMoveTime)
		{
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);

		percentBetweenWaypoints += Time.deltaTime * (speed / distanceBetweenWaypoints);
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);

		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		//If we need to start moving to the next waypoint...
		if(percentBetweenWaypoints >= 1 && shouldGetNextWaypoint)
		{
			if(leverControlType == LeverControlType.Waypoint)
				shouldGetNextWaypoint = false;
			GetNextWaypoint();
		}

		//Return the amount we want to move in this frame.
		return newPos - transform.position;
	}

	void GetNextWaypoint()
	{
		percentBetweenWaypoints = 0;
		fromWaypointIndex ++;

		if(!cyclic) //If the platform isn't cyclic...
		{
			if(fromWaypointIndex >= globalWaypoints.Length - 1) //If reached the end of the array...
			{
				//Reverse the array and start going through it again.
				fromWaypointIndex = 0;
				System.Array.Reverse(globalWaypoints);
			}
		}
		nextMoveTime = Time.time + waitTime;
	}

	//This will move passengers either before we move the 
	//platform or after we move the platform.
	void MovePassengers(bool beforeMovePlatform)
	{
		foreach(PassengerMovement passenger in passengerMovement)
		{
			//We store a passenger dictionary so that we don't have to use GetComponent every 
			//frame for every passenger. Instead, we will only use it for new passengers.
			if (passenger.transform.GetComponent<Controller2D> () != null) 
			{
				if (!passengerDictionary.ContainsKey (passenger.transform)) 
				{
					passengerDictionary.Add (passenger.transform, passenger.transform.GetComponent<Controller2D> ());
				}

				if (passenger.moveBeforePlatform == beforeMovePlatform) 
				{ //Notice the way we use beforeMovePlatform here and in Update.
					passengerDictionary [passenger.transform].Move (passenger.velocity, passenger.standingOnPlatform);
				}
			}
		}
	}

	//Passenger = Any Controller2D moved by the platform in ANY way.
	void CalculatePassengerMovement(Vector3 velocity)
	{
		//This is used so that we don't apply platform velocity 
		//multiple times per frame on the same passenger.
		HashSet<Transform> movedPassengers = new HashSet<Transform>(); 
		passengerMovement = new List<PassengerMovement> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		//Vertically moving platform
		if (velocity.y != 0) 
		{
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for(int i = 0; i < verticalRayCount; i++)
			{
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);

				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
				RaycastHit2D enemyhit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength); //This one checks if we hit any gameobjs tagged enemy.

				if(hit)
				{
					if(!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add (hit.transform);

						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
					}
				}

				if (enemyhit && enemyhit.transform.gameObject.tag == "Enemy") 
				{
					if(!movedPassengers.Contains(enemyhit.transform))
					{
						movedPassengers.Add (enemyhit.transform);

						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (enemyhit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(enemyhit.transform, new Vector3(pushX, pushY), directionY == 1, true));
					}
				}
			}
		}

		//Horizontally moving platform
		if(velocity.x != 0)
		{
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for(int i = 0; i < verticalRayCount; i++)
			{
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
				RaycastHit2D enemyhit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength); //This one checks if we hit any gameobjs tagged enemy.

				if(hit)
				{
					if(!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add (hit.transform);

						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
					}
				}

				if (enemyhit && enemyhit.transform.gameObject.tag == "Enemy") 
				{
					if(!movedPassengers.Contains(enemyhit.transform))
					{
						movedPassengers.Add (enemyhit.transform);

						float pushX = velocity.x - (enemyhit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovement.Add(new PassengerMovement(enemyhit.transform, new Vector3(pushX, pushY), false, true));
					}
				}
			}
		}

		//Passenger on top of a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) 
		{
			float rayLength = skinWidth * 2;

			for(int i = 0; i < verticalRayCount; i++)
			{
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
				RaycastHit2D enemyhit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength); //This one checks if we hit any gameobjs tagged enemy.

				if(hit)
				{
					if(!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add (hit.transform);

						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
					}
				}

				if (enemyhit && enemyhit.transform.gameObject.tag == "Enemy") 
				{
					if(!movedPassengers.Contains(enemyhit.transform))
					{
						movedPassengers.Add (enemyhit.transform);

						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add(new PassengerMovement(enemyhit.transform, new Vector3(pushX, pushY), true, false));
					}
				}
			}
		}
	}

	struct PassengerMovement
	{
		public Transform transform;     //Transform of the passenger.
		public Vector3 velocity;        //Desired velocity of the passenger.
		public bool standingOnPlatform; //Is the passenger standing on top of the platform?
		public bool moveBeforePlatform; //Should we move the passenger before moving the platform?

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
		{
			transform = _transform;
			velocity  = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}

	#region LeverControls
	//Determines how the platform behaves when a corresponding lever is switched.
	public enum LeverControlType
	{
		None, //Lever may be null, no change in behaviour.
		Free, //Platform stops/starts in place when the lever is switched.
		Waypoint //Platform moves to next waypoint and stops there when lever is switched.
	}

	public void LeverAction()
	{
		switch (leverControlType)
		{
		case LeverControlType.None:
			break;
		case LeverControlType.Free:
			TogglePlatformMovement();
			break;
		case LeverControlType.Waypoint:
			StopAtNextWaypoint();
			break;
		}
	}

	void TogglePlatformMovement()
	{
		shouldMove = !shouldMove;
	}

	void StopAtNextWaypoint()
	{
		shouldMove = true;
		if (percentBetweenWaypoints >= 1)
		{
			shouldGetNextWaypoint = true;
		}
	}
	#endregion

	void OnDrawGizmos()
	{
		if(localWaypoints != null)
		{
			Gizmos.color = Color.red;
			float size = .3f;

			for(int i = 0; i < localWaypoints.Length; i++)
			{
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i]:(localWaypoints[i] + transform.position);
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}