using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CameraTrap))]
public class CameraFollowTrap : MonoBehaviour 
{
	//////////PLANS//////////
	//For now, the camera only follows the trap. Once there's more to the game,
	//I'll add in level boundaries (bounds that the camera will never pass) and
	//different camera modes, such as bottom/top lock and free (currently follow trap).

	public Level level;
	public float dampTime;

	CameraTrap trap; //The trap to follow.
	ExtendedTrap extendedTrap;

	Vector2 cameraExtents;
	CameraSides cameraSides;

	public CameraLockMode cameraLockMode; //Cameras various locking modes.
	public CameraFollowMode cameraFollowMode; //How the camera follows the trap.

	public bool movementOverridden;

	void Start()
	{
		trap = this.GetComponent<CameraTrap> ();
		extendedTrap.Offset = new Vector2 (2.6f, 2.6f);

		cameraExtents.y = this.GetComponent<Camera> ().orthographicSize;
		cameraExtents.x = (cameraExtents.y * Screen.width) / Screen.height;

		movementOverridden = false;
	}

	void Update()
	{
		switch(cameraLockMode)
		{
		case CameraLockMode.Free:
			trap.trapMode = CameraTrap.TrapMode.ContainPlayer;
			break;
		case CameraLockMode.PlatformLock:
			trap.trapMode = CameraTrap.TrapMode.LockToPlatform;
			break;
		}

		CalculateExtendedTrapBounds ();
		if(!movementOverridden && !GameManager.instance.isPaused)
			FollowTrap ();
		CalculateCameraSides ();
		BindCameraToLevel ();
	}

	#region CameraFollowing
	private Vector3 FollowTrap()
	{
		Vector2 targetPositionXY = Vector2.zero;

		switch(cameraFollowMode)
		{
		case CameraFollowMode.Center:
			targetPositionXY = trap.Trap.Center;
			break;
		case CameraFollowMode.Top:
			targetPositionXY = new Vector2 (trap.Trap.Center.x, trap.Trap.Top);
			break;
		case CameraFollowMode.Bottom:
			targetPositionXY = new Vector2 (trap.Trap.Center.x, trap.Trap.Bottom);
			break;
		case CameraFollowMode.Left:
			targetPositionXY = new Vector2 (trap.Trap.Left, trap.Trap.Center.y);
			break;
		case CameraFollowMode.Right:
			targetPositionXY = new Vector2 (trap.Trap.Right, trap.Trap.Center.y);
			break;
		case CameraFollowMode.Top_Extended:
			targetPositionXY = new Vector2 (trap.Trap.Center.x, extendedTrap.Top);
			break;
		case CameraFollowMode.Bottom_Extended:
			targetPositionXY = new Vector2 (trap.Trap.Center.x, extendedTrap.Bottom);
			break;
		case CameraFollowMode.Left_Extended:
			targetPositionXY = new Vector2 (extendedTrap.Left, trap.Trap.Center.y);
			break;
		case CameraFollowMode.Right_Extended:
			targetPositionXY = new Vector2 (extendedTrap.Right, trap.Trap.Center.y);
			break;
		}

		return Move((Vector3)targetPositionXY);
	}

	public Vector3 Move(Vector3 targetPos)
	{
		Vector3 newPos = transform.position; //The new position of the camera after this frame.
		Vector2 velocity = Vector2.zero; //Here for use in smooth damp.

		//Used Vector2 for these because I didn't want this affecting the cameras z position.
		Vector2 cameraPositionXY = new Vector2 (transform.position.x, transform.position.y);
		Vector2 targetPositionXY = (Vector2)targetPos;

		Vector2 newPosXY = Vector2.SmoothDamp (cameraPositionXY, targetPositionXY, ref velocity, dampTime);

		newPos.x = newPosXY.x;
		newPos.y = newPosXY.y;
		transform.position = newPos;

		return newPos;
	}
	#endregion

	private void BindCameraToLevel()
	{
		Vector3 newPos = transform.position;

		if(cameraSides.Left < level.xMin)
			newPos.x = level.xMin + cameraExtents.x;
		if (cameraSides.Right > level.xMax)
			newPos.x = level.xMax - cameraExtents.x;
		if (cameraSides.Top > level.yMax)
			newPos.y = level.yMax - cameraExtents.y;
		if (cameraSides.Bottom < level.yMin)
			newPos.y = level.yMin + cameraExtents.y;

		transform.position = newPos;
	}

	private void CalculateCameraSides()
	{
		cameraSides.Left   = transform.position.x - cameraExtents.x;
		cameraSides.Right  = transform.position.x + cameraExtents.x;
		cameraSides.Top    = transform.position.y + cameraExtents.y;
		cameraSides.Bottom = transform.position.y - cameraExtents.y;
	}

	private void CalculateExtendedTrapBounds()
	{
		extendedTrap.Top = trap.Trap.Top + extendedTrap.Offset.y;
		extendedTrap.Bottom = trap.Trap.Bottom - extendedTrap.Offset.y;
		extendedTrap.Left = trap.Trap.Left - extendedTrap.Offset.x;
		extendedTrap.Right = trap.Trap.Right + extendedTrap.Offset.x;
	}

	public void ScreenShake(float duration, float intensity)
	{
		StartCoroutine(CameraShake(duration, intensity));
	}

	private IEnumerator CameraShake(float duration, float intensity)
	{
		Vector3 startPos = transform.position;
		for (float t = 0; t < duration; t += Time.deltaTime) 
		{
			if (t % .05 <= .15f && !movementOverridden)
				transform.position = FollowTrap ();
			else
				transform.position = startPos;
			transform.position += new Vector3 (Random.Range (-intensity, intensity), Random.Range (-intensity, intensity), 0f);
			yield return null;
		}
	}

	#region Custom Data
	public enum CameraLockMode
	{
		Free, 
		PlatformLock
	}

	public enum CameraFollowMode
	{
		Top,
		Center,
		Bottom,
		Left,
		Right,
		Top_Extended,
		Bottom_Extended,
		Left_Extended,
		Right_Extended
	}

	struct CameraSides
	{
		public float Left, Right;
		public float Top, Bottom;
	}

	struct ExtendedTrap
	{
		public Vector2 Offset; //Amount this trap extends the default trap
		public float Top, Bottom;
		public float Left, Right;
	}
	#endregion

	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;

		//Horiz lines.
		Gizmos.DrawLine (new Vector3 (extendedTrap.Left, extendedTrap.Top), new Vector3 (extendedTrap.Right, extendedTrap.Top));
		Gizmos.DrawLine (new Vector3 (extendedTrap.Left, extendedTrap.Bottom), new Vector3 (extendedTrap.Right, extendedTrap.Bottom));

		//Vert lines.
		Gizmos.DrawLine (new Vector3 (extendedTrap.Left, extendedTrap.Top), new Vector3 (extendedTrap.Left, extendedTrap.Bottom));
		Gizmos.DrawLine (new Vector3 (extendedTrap.Right, extendedTrap.Top), new Vector3 (extendedTrap.Right, extendedTrap.Bottom));
	}
}