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
		if (other.gameObject.tag == "Player" && player.corpseCarried == null) 
		{
			GUI.instance.grabAmmoText.enabled = true;
			if (Input.GetButtonDown("Action")) 
			{
				player.playerAmmo.RefillAll ();
				AudioManager.instance.PlaySoundEffect ("Ethan_AmmoBoxSound");
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" && player.corpseCarried == null) 
		{
			GUI.instance.grabAmmoText.enabled = true;
			if (Input.GetButtonDown("Action")) 
			{
				player.playerAmmo.RefillAll ();
				AudioManager.instance.PlaySoundEffect ("Ethan_AmmoBoxSound");
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GUI.instance.grabAmmoText.enabled = false;
		}
	}
}
