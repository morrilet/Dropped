using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	bool isOpen;

	GameObject player; //The player gameObject.
	Collider2D openCloseColliderLeft; //The left collider used for opening and closing the door.
	Collider2D openCloseColliderRight; //The right collider used for opening and closing the door.

	void Start()
	{
		isOpen = false;

		player = GameObject.FindGameObjectWithTag ("Player");
		//openCloseCollider = transform.FindChild ("OpenCloseCollider").GetComponent<Collider2D>();
		openCloseColliderLeft = transform.FindChild ("OpenCloseColliderLeft").GetComponent<Collider2D> ();
		openCloseColliderRight = transform.FindChild ("OpenCloseColliderRight").GetComponent<Collider2D> ();
	}

	void Update()
	{
		if (openCloseColliderLeft.IsTouching (player.GetComponent<Collider2D> ()))
		{
			if (Input.GetKeyDown (KeyCode.E) && player.GetComponent<Player>().direction == 1) 
			{
				if (isOpen)
					CloseDoor ();
				else
					OpenDoor ();

				isOpen = !isOpen;
			}
		}

		if (openCloseColliderRight.IsTouching (player.GetComponent<Collider2D> ()))
		{
			if (Input.GetKeyDown (KeyCode.E) && player.GetComponent<Player>().direction == -1)
			{
				if (isOpen)
					CloseDoor ();
				else
					OpenDoor ();

				isOpen = !isOpen;
			}
		}
	}

	void OpenDoor()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Default");
		GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, .25f);
	}

	void CloseDoor()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Obstacle");
		GetComponent<SpriteRenderer> ().color = new Color (150, 150, 150, 1);
	}
}
