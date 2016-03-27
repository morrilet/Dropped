using UnityEngine;
using System.Collections;

public class CameraTrap : MonoBehaviour 
{
	public Player player; //The player to follow.
	Bounds playerBounds;

	//Trap dimensions.
	public Vector2 trapCenter;
	public float trapHeight;
	public float trapWidth;

	TrapBounds trap;
	[HideInInspector]
	public TrapMode trapMode;

	public TrapBounds Trap
	{
		get {return trap;}
	}

	void Start()
	{
		trap.Center = trapCenter;
		trapMode = TrapMode.ContainPlayer;

		playerBounds = player.GetComponent<SpriteRenderer> ().bounds;
	}

	void Update()
	{
		CalculateTrapBounds ();

		if (trapMode == TrapMode.ContainPlayer)
			ContainPlayer();

		if (trapMode == TrapMode.LockToPlatform)
			LockToPlatform ();
	}

	#region TrapModes
	private void ContainPlayer()
	{
		//ContainPlayer is split into X and Y so that other trap 
		//modes may make use of containment code on a specific axis.
		ContainPlayerX ();
		ContainPlayerY ();
	}

	private void ContainPlayerX()
	{
		Vector2 playerPosition = new Vector2 (player.transform.position.x, player.transform.position.y);
		Vector3 playerExtents = playerBounds.extents;

		if(!PlayerInTrapX())
		{
			if(playerPosition.x > trap.Center.x)
			{
				trap.Center.x += Mathf.Abs((playerPosition.x + playerExtents.x) - trap.Right);
			}
			if(playerPosition.x < trap.Center.x)
			{
				trap.Center.x -= Mathf.Abs((playerPosition.x - playerExtents.x) - trap.Left);
			}
		}
	}

	private void ContainPlayerY()
	{
		Vector2 playerPosition = new Vector2 (player.transform.position.x, player.transform.position.y);
		Vector3 playerExtents = playerBounds.extents;

		if(!PlayerInTrapY())
		{
			if(playerPosition.y > trap.Center.y)
			{
				trap.Center.y += Mathf.Abs((playerPosition.y + playerExtents.y) - trap.Top);
			}
			if(playerPosition.y < trap.Center.y)
			{
				trap.Center.y -= Mathf.Abs((playerPosition.y - playerExtents.y) - trap.Bottom);
			}
		}
	}

	private void LockToPlatform()
	{
		Vector2 playerPosition = new Vector2 (player.transform.position.x, player.transform.position.y);
		Vector3 playerExtents = playerBounds.extents;

		ContainPlayerX ();

		if(player.controller.collisions.below)
			trap.Center.y = (playerPosition.y - playerExtents.y) + trapHeight;
		else
			ContainPlayerY();
	}
	#endregion

	#region Bounds
	private bool PlayerInTrapX()
	{
		bool inTrapX = true;
		Vector3 playerExtents = playerBounds.extents;

		if (player.transform.position.x + playerExtents.x > trap.Right || 
			player.transform.position.x - playerExtents.x < trap.Left)
			inTrapX = false;

		return inTrapX;
	}

	private bool PlayerInTrapY()
	{
		bool inTrapY = true;
		Vector3 playerExtents = playerBounds.extents;

		if (player.transform.position.y + playerExtents.y > trap.Top || 
			player.transform.position.y - playerExtents.y < trap.Bottom)
			inTrapY = false;

		return inTrapY;
	}

	//Sets the trap bounds each frame.
	private void CalculateTrapBounds()
	{
		trap.Left = trap.Center.x - trapWidth;
		trap.Right = trap.Center.x + trapWidth;
		trap.Bottom = trap.Center.y - trapHeight;
		trap.Top = trap.Center.y + trapHeight;
	}
	#endregion

	#region Custom Data
	public struct TrapBounds
	{
		public Vector2 Center;
		public float Top, Bottom;
		public float Right, Left;
	}

	public enum TrapMode
	{
		ContainPlayer,
		LockToPlatform
	}
	#endregion

	//Draws the trap to the editor.
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		//Center
		Gizmos.DrawLine (new Vector3 (trap.Center.x, trap.Center.y - .25f), new Vector3 (trap.Center.x, trap.Center.y + .25f));
		Gizmos.DrawLine (new Vector3 (trap.Center.x - .25f, trap.Center.y), new Vector3 (trap.Center.x + .25f, trap.Center.y));

		//Horiz lines.
		Gizmos.DrawLine (new Vector3 (trap.Left, trap.Top), new Vector3 (trap.Right, trap.Top));
		Gizmos.DrawLine (new Vector3 (trap.Left, trap.Bottom), new Vector3 (trap.Right, trap.Bottom));

		//Vert lines.
		Gizmos.DrawLine (new Vector3 (trap.Left, trap.Top), new Vector3 (trap.Left, trap.Bottom));
		Gizmos.DrawLine (new Vector3 (trap.Right, trap.Top), new Vector3 (trap.Right, trap.Bottom));
	}
}