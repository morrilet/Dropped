using UnityEngine;
using System.Collections;

public class CameraMoveTrigger : MonoBehaviour 
{
	//Move camera if the player is facing the correct direction AND in the trigger.
	public int direction; //-1 = left, 0 = don't use direction, 1 = right.

	public Vector3 cameraPositionToMoveTo;
	CameraFollowTrap cam;

	void Start()
	{
		cam = Camera.main.GetComponent<CameraFollowTrap> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (direction != 0 && other.GetComponent<Player> ().direction == direction)
				cam.movementOverridden = true;
			else if (direction == 0)
				cam.movementOverridden = true;
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (direction != 0 && other.GetComponent<Player> ().direction == direction)
				cam.movementOverridden = true;
			else if (direction == 0)
				cam.movementOverridden = true;
			else
				cam.movementOverridden = false;
		}

		if (other.gameObject.tag == "Player") 
		{
			if (direction != 0 && other.GetComponent<Player> ().direction == direction)
				cam.Move (cameraPositionToMoveTo);
			else if (direction == 0)
				cam.Move (cameraPositionToMoveTo);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
			cam.movementOverridden = false;
	}
}
