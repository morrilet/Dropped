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

	Vector2 cameraExtents;
	CameraSides cameraSides;

	public CameraLockMode cameraLockMode; //Cameras various locking modes.
	public CameraFollowMode cameraFollowMode; //How the camera follows the trap.

	void Start()
	{
		trap = this.GetComponent<CameraTrap> ();

		cameraExtents.y = this.GetComponent<Camera> ().orthographicSize;
		cameraExtents.x = (cameraExtents.y * Screen.width) / Screen.height;
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

		FollowTrap ();
		CalculateCameraSides ();
		BindCameraToLevel ();
	}

	#region CameraFollowing
	private void FollowTrap()
	{
		Vector3 newPos = transform.position; //The new position of the camera after this frame.
		Vector2 velocity = Vector2.zero; //Here for use in smooth damp.

		//Used Vector2 for these because I didn't want this affecting the cameras z position.
		Vector2 cameraPositionXY = new Vector2 (transform.position.x, transform.position.y);
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
		}

		Vector2 newPosXY = Vector2.SmoothDamp (cameraPositionXY, targetPositionXY, ref velocity, dampTime);

		newPos.x = newPosXY.x;
		newPos.y = newPosXY.y;
		transform.position = newPos;
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

	public void ScreenShake(float duration, float intensity)
	{
		StartCoroutine(CameraShake(duration, intensity));
	}

	private IEnumerator CameraShake(float duration, float intensity)
	{
		Vector3 startPos = transform.position;
		for (float t = 0; t < duration; t += Time.deltaTime) 
		{
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
		Right
	}

	struct CameraSides
	{
		public float Left, Right;
		public float Top, Bottom;
	}
	#endregion
}