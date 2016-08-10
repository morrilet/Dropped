using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorTrigger : MonoBehaviour 
{
	Door door;

	bool playerIntersectsDoor;
	BoxCollider2D triggerCollider;

	Vector2 triggerStartScale;
	Vector2 triggerStartOffset;

	bool knockedBackEnemies;

	void Start()
	{
		door = transform.parent.GetComponent<Door>();
		playerIntersectsDoor = false;
		triggerCollider = GetComponent<BoxCollider2D> ();

		triggerStartScale = triggerCollider.size;
		triggerStartOffset = triggerCollider.offset;

		knockedBackEnemies = false;
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

			if (!knockedBackEnemies) 
			{
				List<GameObject> enemies = GameManager.instance.level.GetComponent<Level>().enemies;
				for (int i = 0; i < enemies.Count; i++) 
				{
					if (IsTouchingEnemy(enemies[i].gameObject)) 
					{
						if(Mathf.Sign(door.transform.localScale.x) == Mathf.Sign(enemies[i].transform.position.x - door.transform.position.x))
						Debug.Log ("Here");
						enemies [i].GetComponent<EnemyAI> ().KnockBack (new Vector3(1f * Mathf.Sign (door.transform.localScale.x), 0f, 0f), .25f);
					}
				}

				knockedBackEnemies = true;
			}
		} 
		else
		{
			triggerCollider.size = triggerStartScale;
			triggerCollider.offset = triggerStartOffset;
			knockedBackEnemies = false;
		}
	}

	//We use this to check if we're touching an enemy because we don't detect collisions on these layers.
	bool IsTouchingEnemy(GameObject enemy)
	{
		bool touching = false;
		Collider2D enemyColl = enemy.GetComponent<Collider2D> ();

		if (Mathf.Abs ((transform.position.x + triggerCollider.offset.x) - enemy.transform.position.x) <= 
			enemyColl.bounds.extents.x + triggerCollider.bounds.extents.x) 
		{
			touching = true;
		}

		return touching;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (door.GetPlayerFacingDoor () && GameObject.Find ("Player").GetComponent<Player>().corpseCarried == null 
				&& GameObject.Find("Player").GetComponent<Player>().grapplingEnemies.Count == 0)
			{
				GUI_Script.instance.openDoorText.SetActive (true);
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
				GUI_Script.instance.openDoorText.SetActive (false);
			}
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			if (door.GetPlayerFacingDoor () && GameObject.Find ("Player").GetComponent<Player>().corpseCarried == null 
				&& GameObject.Find ("Player").GetComponent<Player>().GetTouchingCorpse() == null 
				&& GameObject.Find("Player").GetComponent<Player>().grapplingEnemies.Count == 0) 
			{
				GUI_Script.instance.openDoorText.SetActive (true);
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
				GUI_Script.instance.openDoorText.SetActive (false);
			}
		}
	}

	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player")
			GUI_Script.instance.openDoorText.SetActive (false);
	}
}
