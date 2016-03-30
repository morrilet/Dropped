using UnityEngine;
using System.Collections;


public class Controller2D : RaycastController 
{
	float maxClimbAngle = 80;
	float maxDescendAngle = 75;

	public CollisionInfo collisions;

	public override void Start()
	{
		base.Start ();
	}

	public void Move(Vector3 velocity, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins ();
		collisions.Reset (standingOnPlatform);
		collisions.velocityOld = velocity;

		if (velocity.y < 0)
			DescendSlope (ref velocity);

		//Before we translate, we will check for collisions.
		if(velocity.x != 0)
			HorizontalCollisions (ref velocity); //If horizColl doesn't happen before vertColl, the char will climb the side of a wall it's pushing on... For SOME reason...
		if(velocity.y != 0)
			VerticalCollisions   (ref velocity); //ref passes a reference to an existing variable, instead of creating a copy of an existing variable.

		transform.Translate (velocity);

		if (standingOnPlatform)
			collisions.below = true;
	}

	//This will check for (and handle) any vertical collisions.
	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign (velocity.y); //1=Up, -1=Down.
		float rayLength  = Mathf.Abs (velocity.y) + skinWidth;

		for(int i = 0; i < verticalRayCount; i++)
		{
			//Set rayOrigin to bottomLeft or topLeft based on directionY, A.K.A. direction of our velocity.
			//Shorthand 'if' statement: statement => '?' (means "if true") => action1 => ':' (means "if false") => action2.
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x); //Add velocity.x because we want to raycast from where we WILL be.

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //The actual raycast.

			//For testing.
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			//If the ray hit something...
			if(hit)
			{
				if(hit.transform.tag == "MovingPlatform")
					collisions.movingPlatform = hit.transform.GetComponent<PlatformController>();

				//Set velocity.y to a value that will bring us to what we've hit, while maintaining the same direction.
				velocity.y = (hit.distance - skinWidth) * directionY; //We subtract skinWidth because we added it to rayLength earlier.
				rayLength = hit.distance; //Change rayLength to distance so that the ray doesn't continue on and hit something farther than the first object.

				if(collisions.climbingSlope)
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				//Set the apropriate collision value to true based on which way we were going.
				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);

				//If this is the leftmost raycastOrigin then our bottom left corner is in a collision.
				if (i == 0)
					collisions.belowLeft = true;
				//If this is the rightmost raycastOrigin then our bottom right corner is in a collision.
				if (i == verticalRayCount)
					collisions.belowRight = true;
			}
		}

		if(collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(velocity.x);
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if(hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(slopeAngle != collisions.slopeAngle) //We've hit a new slope.
				{
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	//This will check for (and handle) any horizontal collisions.
	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x); //1=Up, -1=Down.
		float rayLength  = Mathf.Abs (velocity.x) + skinWidth;

		for(int i = 0; i < horizontalRayCount; i++)
		{
			//Set rayOrigin to bottomLeft or bottomRight based on directionX, A.K.A. direction of our velocity.
			//Shorthand 'if' statement: statement => '?' (means "if true") => action1 => ':' (means "if false") => action2.
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //The actual raycast.

			//For testing.
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			//If the ray hit something...
			if(hit)
			{
				if(hit.distance == 0)
				{
					continue;
				}

				if(hit.transform.tag == "MovingPlatform")
					collisions.movingPlatform = hit.transform.GetComponent<PlatformController>();

				//Firstly, we'll check the angle of the thing we're colliding with.
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if(i == 0 && slopeAngle <= maxClimbAngle)
				{
					if(collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}

					float distanceToSlopeStart = 0;
					//If we've just started climbing a slope.
					if(slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
				{
					//Set velocity.y to a value that will bring us to what we've hit, while maintaining the same direction.
					velocity.x = (hit.distance - skinWidth) * directionX; //We subtract skinWidth because we added it to rayLength earlier.
					rayLength = hit.distance; //Change rayLength to distance so that the ray doesn't continue on and hit something farther than the first object.

					if(collisions.climbingSlope)
					{
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					//Set the apropriate collision value to true based on which way we were going.
					collisions.left  = (directionX == -1);
					collisions.right = (directionX == 1);
				}
			}
		}
	}

	//Uses trig to figure out how far in the x and y direction we need to go
	//based on the slope we're climbing in order to maintain our current velocity.
	void ClimbSlope(ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if(velocity.y <= climbVelocityY) //If we're not jumping...
		{
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);

			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if(hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if(Mathf.Sign(hit.normal.x) == directionX)
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;
		public bool belowLeft, belowRight;

		public bool abovePrev, belowPrev;
		public bool leftPrev, rightPrev;
		public bool belowLeftPrev, belowRightPrev;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public PlatformController movingPlatform;

		public void Reset(bool standingOnMovingPlatform)
		{
			abovePrev = above;
			belowPrev = below;
			leftPrev  = left;
			rightPrev = right;
			belowLeftPrev = belowLeft;
			belowRightPrev = belowRight;

			above = below   = false;
			left  = right   = false;
			belowLeft = belowRight = false;

			climbingSlope   = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;

			if(!standingOnMovingPlatform)
				movingPlatform = null;
		}
	}
}