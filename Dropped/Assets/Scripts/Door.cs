using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	[HideInInspector]
	public bool isOpen;

	public Sprite closedDoorSprite;
	public Sprite openDoorSprite;
	Vector3 startingScale;

	GameObject player; //The player gameObject.
	Collider2D coll; //The door collider.

	Vector2 bottomLeftCorner, bottomRightCorner;
	Vector2 topLeftCorner, topRightCorner;

	Vector3[] verts;

	public LayerMask mask;

	void Start()
	{
		isOpen = false;

		player = GameObject.Find ("Player").gameObject;

		coll = GetComponent<Collider2D> ();

		verts = GetVertexPositions ();

		startingScale = transform.localScale;
	}

	//Returns whether the player is within the bounds of the door or not.
	public bool GetPlayerInsideDoor()
	{
		//Get the corners of the door.
		Vector2 bottomLeftCorner  = (Vector2)transform.position + new Vector2 (-coll.bounds.extents.x, -coll.bounds.extents.y);
		Vector2 bottomRightCorner = (Vector2)transform.position + new Vector2 (coll.bounds.extents.x, -coll.bounds.extents.y);
		Vector2 topLeftCorner     = (Vector2)transform.position + new Vector2 (-coll.bounds.extents.x, coll.bounds.extents.y);
		Vector2 topRightCorner    = (Vector2)transform.position + new Vector2 (coll.bounds.extents.x, coll.bounds.extents.y);

		//Rotate the corners of the door to match the doors rotation.
		bottomLeftCorner  = RotatePoint (bottomLeftCorner);
		bottomRightCorner = RotatePoint (bottomRightCorner);
		topLeftCorner     = RotatePoint (topLeftCorner);
		topRightCorner    = RotatePoint (topRightCorner);

		RaycastHit2D hitLeftSide = Physics2D.Linecast ((Vector2)verts[1], (Vector2)verts[2], mask);
		RaycastHit2D hitRightSide = Physics2D.Linecast ((Vector2)verts [3], (Vector2)verts [0], mask);

		if (!isOpen)
			return false;

		if(hitLeftSide && hitLeftSide.collider.tag == "Player")
		{
			return true;
		}
		else if (hitRightSide && hitRightSide.collider.tag == "Player")
		{
			return true;
		}
		else
		{
			return false;
		}

		/*
		//How far the player must be from the door to not get caught inside it.
		float requiredDistance = Mathf.Abs (player.GetComponent<Collider2D>().bounds.extents.x + coll.bounds.extents.x);
		if (Mathf.Abs (player.transform.position.x - transform.position.x) >= requiredDistance)
			return false;
		else
			return true;
		*/
	}

	private Vector3[] GetVertexPositions()
	{
		Vector3[] vertices = new Vector3[4];
		Matrix4x4 thisMatrix = transform.localToWorldMatrix;
		Quaternion storedRotation = transform.rotation;
		transform.rotation = Quaternion.identity;

		Vector3 extents = coll.bounds.extents;
		vertices [0] = thisMatrix.MultiplyPoint3x4 (new Vector3(extents.x, extents.y, extents.z)); //Top Right
		vertices [1] = thisMatrix.MultiplyPoint3x4 (new Vector3(-extents.x, extents.y, extents.z)); //Top Left
		vertices [2] = thisMatrix.MultiplyPoint3x4 (new Vector3(-extents.x, -extents.y, extents.z)); //Bottom Left
		vertices [3] = thisMatrix.MultiplyPoint3x4 (new Vector3(extents.x, -extents.y, extents.z)); //Bottom Right

		transform.rotation = storedRotation;
		return vertices;
	}

	//Applies the rotation of the gameObject to a point. Used for finding corners of a rotated GameObject.
	private Vector2 RotatePoint(Vector2 point)
	{
		Vector2 returnedPoint = point; //Point to return.

		//Make point relative to object center.
		float tempX = point.x - coll.bounds.center.x;
		float tempY = point.y - coll.bounds.center.y;

		//Apply the rotation to the point.
		float rotatedX = tempX * Mathf.Cos (transform.eulerAngles.z * Mathf.Deg2Rad) - tempY * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);// - Mathf.Sin (transform.eulerAngles.z * Mathf.Deg2Rad);
		float rotatedY = tempX * Mathf.Sin (transform.eulerAngles.z * Mathf.Deg2Rad) + tempY * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);// + Mathf.Cos (transform.eulerAngles.z * Mathf.Deg2Rad);

		//Set the points relative to world space.
		returnedPoint.x = rotatedX + coll.bounds.center.x;
		returnedPoint.y = rotatedY + coll.bounds.center.y;

		return returnedPoint;
	}

	//Returns whether or not the player is facing the door.
	public bool GetPlayerFacingDoor()
	{
		//Get the players position in our transform space.
		Vector3 relativePoint = transform.InverseTransformPoint (player.transform.position);
		if (relativePoint.x < 0f && player.GetComponent<Player> ().direction == 1 && transform.localScale.x > 0) //Player is on left facing right.
			return true;
		else if (relativePoint.x > 0f && player.GetComponent<Player> ().direction == -1 && transform.localScale.x > 0) //Player is on right facing left.
			return true;
		else if (relativePoint.x > 0f && player.GetComponent<Player> ().direction == 1 && transform.localScale.x < 0)
			return true;
		else if (relativePoint.x < 0f && player.GetComponent<Player> ().direction == -1 && transform.localScale.x < 0)
			return true;
		else
			return false;
	}

	public void OpenDoor()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Default_Hotspot");
		GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, .75f);
		GetComponent<SpriteRenderer> ().sprite = openDoorSprite;
		transform.localScale = new Vector3 (startingScale.x * Mathf.Sign (player.GetComponent<Player> ().direction), startingScale.y, startingScale.z);
		isOpen = true;
	}

	public void CloseDoor()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Obstacle");
		GetComponent<SpriteRenderer> ().color = new Color (150, 150, 150, 1);
		GetComponent<SpriteRenderer> ().sprite = closedDoorSprite;
		transform.localScale = new Vector3 (startingScale.x * Mathf.Sign (player.GetComponent<Player> ().direction), startingScale.y, startingScale.z);
		isOpen = false;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;

		if (coll != null) 
		{
			/*
			Vector2 bottomLeftCorner = (Vector2)coll.bounds.center + new Vector2 (-coll.bounds.extents.x, -coll.bounds.extents.y);
			Vector2 bottomRightCorner = (Vector2)coll.bounds.center + new Vector2 (coll.bounds.extents.x, -coll.bounds.extents.y);
			Vector2 topLeftCorner = (Vector2)coll.bounds.center + new Vector2 (-coll.bounds.extents.x, coll.bounds.extents.y);
			Vector2 topRightCorner = (Vector2)coll.bounds.center + new Vector2 (coll.bounds.extents.x, coll.bounds.extents.y);

			bottomLeftCorner = RotatePoint (bottomLeftCorner);
			bottomRightCorner = RotatePoint (bottomRightCorner);
			topLeftCorner = RotatePoint (topLeftCorner);
			topRightCorner = RotatePoint (topRightCorner);

			Gizmos.DrawLine ((Vector2)bottomLeftCorner, (Vector2)topLeftCorner);
			Gizmos.DrawLine ((Vector2)bottomRightCorner, (Vector2)topRightCorner);
			*/

			float top = coll.offset.y + (coll.bounds.size.y / 2);
			float btm = coll.offset.y - (coll.bounds.size.y / 2);
			float left = coll.offset.x - (coll.bounds.size.x / 2);
			float right = coll.offset.x + (coll.bounds.size.x / 2);

			Vector3 topLeft = transform.TransformPoint (new Vector3 (left, top, 0f));
			Vector3 topRight = transform.TransformPoint (new Vector3 (right, top, 0f));
			Vector3 btmLeft = transform.TransformPoint (new Vector3 (left, btm, 0f));
			Vector3 btmRight = transform.TransformPoint (new Vector3 (right, btm, 0f));

			//Debug.Log (verts [0]);

			Gizmos.DrawRay (verts [2], (verts [1] - verts [2]));
			Gizmos.DrawRay (verts [3], (verts [0] - verts [3]));

			//Gizmos.DrawLine (verts[0], verts[3]);
			//Gizmos.DrawLine (verts[1], verts[2]);
		}
	}
}
