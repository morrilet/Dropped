using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour 
{
	Door door;

	bool playerIntersectsDoor;
	BoxCollider2D triggerCollider;

	Vector2 triggerStartScale;
	Vector2 triggerStartOffset;

	void Start()
	{
		door = transform.parent.GetComponent<Door>();
		playerIntersectsDoor = false;
		triggerCollider = GetComponent<BoxCollider2D> ();

		triggerStartScale = triggerCollider.size;
		triggerStartOffset = triggerCollider.offset;
	}

	void Update()
	{
		if (door.isOpen) 
		{
			if (door.transform.localScale.x < 0) 
			{
				triggerCollider.size = new Vector2 (1.5f, 3f);
				triggerCollider.offset = new Vector2 (.25f, 0f);
			}
			if (door.transform.localScale.x > 0) 
			{
				triggerCollider.size = new Vector2 (1.5f, 3f);
				triggerCollider.offset = new Vector2 (.25f, 0f);
			}
		} 
		else
		{
			triggerCollider.size = triggerStartScale;
			triggerCollider.offset = triggerStartOffset;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (door.GetPlayerFacingDoor () && GameObject.Find ("Player").GetComponent<Player>().corpseCarried == null)
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
			if (door.GetPlayerFacingDoor () && GameObject.Find ("Player").GetComponent<Player>().corpseCarried == null 
				&& GameObject.Find ("Player").GetComponent<Player>().GetTouchingCorpse() == null) 
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
