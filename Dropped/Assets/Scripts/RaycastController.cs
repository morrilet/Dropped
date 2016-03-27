using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour 
{
	public LayerMask collisionMask;

	public const float skinWidth = .015f; //Inset; distance inside the obj that the rays are cast from.
	public int horizontalRayCount = 4; //# of rays to be cast horizontally
	public int verticalRayCount = 4;   //# of rays to be cast vertically

	[HideInInspector]
	public float horizontalRaySpacing; //The (vertical) distance between each ray cast horizontally.
	[HideInInspector]
	public float verticalRaySpacing; //The (horizontal) distance between each ray cast vertically.

	[HideInInspector]
	public BoxCollider2D coll;

	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	public RaycastOrigins raycastOrigins;

	public virtual void Start()
	{
		coll = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}

	//set the raycast origins to their proper corners.
	public void UpdateRaycastOrigins()
	{
		Bounds bounds = coll.bounds;
		bounds.Expand(skinWidth * -2); //-2 because -1 would shrink it inwards by 1/2 skinWidth on all sides.

		raycastOrigins.bottomLeft  = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft     = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight    = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing()
	{
		Bounds bounds = coll.bounds;
		bounds.Expand(skinWidth * -2); //-2 because -1 would shrink it inwards by 1/2 skinWidth on all sides.

		//Ensures that we will always have at least 2 rays (one at each corner) horizontally or vertically.
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
}
