using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour 
{
	Door door;

	bool playerIntersectsDoor;

	void Start()
	{
		door = transform.parent.GetComponent<Door>();
		playerIntersectsDoor = false;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (door.GetPlayerFacingDoor ())
			{
				GUI.Instance.openDoorText.enabled = true;
				if (Input.GetKeyDown (KeyCode.E) && !door.GetPlayerInsideDoor ()) 
				{
					if (!door.isOpen)
						door.OpenDoor ();
					else if (door.isOpen)
						door.CloseDoor ();
				}
			}
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (door.GetPlayerFacingDoor ())
			{
				GUI.Instance.openDoorText.enabled = true;
				if (Input.GetKeyDown (KeyCode.E) && !door.GetPlayerInsideDoor ()) 
				{
					if (!door.isOpen)
						door.OpenDoor ();
					else if (door.isOpen)
						door.CloseDoor ();
				}
			}
		}
	}

	void OnCollisionExit2D(Collision2D other)
	{
		if(other.gameObject.tag == "Player")
			GUI.Instance.openDoorText.enabled = false;
	}
}
