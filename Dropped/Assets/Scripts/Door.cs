using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	bool isOpen;
	bool playerFacingCorrectDirection;

	GameObject player; //The player gameObject.
	Collider2D openCloseColliderLeft; //The left collider used for opening and closing the door.
	Collider2D openCloseColliderRight; //The right collider used for opening and closing the door.

	void Start()
	{
		isOpen = false;
		playerFacingCorrectDirection = false;

		player = GameObject.FindGameObjectWithTag ("Player");
		//openCloseCollider = transform.FindChild ("OpenCloseCollider").GetComponent<Collider2D>();
		openCloseColliderLeft = transform.FindChild ("OpenCloseColliderLeft").GetComponent<Collider2D> ();
		openCloseColliderRight = transform.FindChild ("OpenCloseColliderRight").GetComponent<Collider2D> ();

		openCloseColliderLeft.gameObject.layer = LayerMask.NameToLayer ("Default_Hotspot");
		openCloseColliderRight.gameObject.layer = LayerMask.NameToLayer ("Default_Hotspot");
	}

	void Update()
	{
		if (openCloseColliderLeft.IsTouching (player.GetComponent<Collider2D> ())) 
		{
			if (player.GetComponent<Player> ().direction == 1)
				playerFacingCorrectDirection = true;
			else
				playerFacingCorrectDirection = false;
			
			if (Input.GetKeyDown (KeyCode.E) && playerFacingCorrectDirection) 
			{
				if (isOpen)
					CloseDoor ();
				else
					OpenDoor ();

				isOpen = !isOpen;
			}
		} 
		else if (openCloseColliderRight.IsTouching (player.GetComponent<Collider2D> ())) 
		{
			if (player.GetComponent<Player> ().direction == -1)
				playerFacingCorrectDirection = true;
			else
				playerFacingCorrectDirection = false;
			
			if (Input.GetKeyDown (KeyCode.E) && playerFacingCorrectDirection) 
			{
				if (isOpen)
					CloseDoor ();
				else
					OpenDoor ();

				isOpen = !isOpen;
			}
		}

		if (Vector3.Distance (transform.position, player.transform.position) <= 1.5f && playerFacingCorrectDirection)
			GUI.Instance.openDoorText.enabled = true;
		else
			GUI.Instance.openDoorText.enabled = false;
	}

	void OpenDoor()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Default_Hotspot");
		GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, .25f);
	}

	void CloseDoor()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Obstacle");
		GetComponent<SpriteRenderer> ().color = new Color (150, 150, 150, 1);
	}
}
