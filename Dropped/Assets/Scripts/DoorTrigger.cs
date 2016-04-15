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
			if (door.GetPlayerFacingDoor () && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().corpseCarried == null)
			{
				GUI.instance.openDoorText.enabled = true;
				if (Input.GetButtonDown("Action") && !door.GetPlayerInsideDoor ()) 
				{
					if (!door.isOpen)
						door.OpenDoor ();
					else if (door.isOpen)
						door.CloseDoor ();
				}
			}
			else
			{
				GUI.instance.openDoorText.enabled = false;
			}
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (door.GetPlayerFacingDoor () && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().corpseCarried == null 
				&& GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GetTouchingCorpse() == null) 
			{
				GUI.instance.openDoorText.enabled = true;
				if (Input.GetButtonDown("Action") && !door.GetPlayerInsideDoor ()) 
				{
					if (!door.isOpen)
						door.OpenDoor ();
					else if (door.isOpen)
						door.CloseDoor ();
				}
			}
			else
			{
				GUI.instance.openDoorText.enabled = false;
			}
		}
	}

	void OnCollisionExit2D(Collision2D other)
	{
		if(other.gameObject.tag == "Player")
			GUI.instance.openDoorText.enabled = false;
	}
}
