using UnityEngine;
using System.Collections;

public class CameraMoveTrigger : MonoBehaviour 
{
	public Vector3 cameraPositionToMoveTo;
	CameraFollowTrap cam;

	void Start()
	{
		cam = Camera.main.GetComponent<CameraFollowTrap> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
			cam.movementOverridden = true;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
			cam.Move (cameraPositionToMoveTo);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
			cam.movementOverridden = false;
	}
}
