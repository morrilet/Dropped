using UnityEngine;
using System.Collections;

public class AmmoBox : MonoBehaviour 
{
	Player player;
	Collider2D coll;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		coll = GetComponent<Collider2D> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GUI.Instance.grabAmmoText.enabled = true;
			if (Input.GetKey (KeyCode.E)) 
			{
				player.playerAmmo.RefillAll ();
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GUI.Instance.grabAmmoText.enabled = true;
			if (Input.GetKey (KeyCode.E)) 
			{
				player.playerAmmo.RefillAll ();
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GUI.Instance.grabAmmoText.enabled = false;
		}
	}
}
