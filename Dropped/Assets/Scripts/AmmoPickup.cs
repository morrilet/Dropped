using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour 
{
	Player player;

	public enum AmmoTypes
	{
		MachineGun,
		ShotGun,
		Pistol
	}
	public AmmoTypes ammoType; //The type of ammo to give to the player.
	public int ammoAmount; //The amount of ammo to give to the player.

	public Sprite machineGunAmmo;
	public Sprite shotGunAmmo;
	public Sprite pistolAmmo;

	Vector3 topPos;
	Vector3 bottomPos;
	float duration;

	void Start()
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();

		topPos = transform.position + new Vector3 (0, 0.15f);
		bottomPos = transform.position + new Vector3 (0, -.15f);
		duration = 2.5f;

		SetImage ();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			AudioManager.instance.PlaySoundEffect ("Ethan_AmmoBoxSound");
			
			switch (ammoType) 
			{
			case AmmoTypes.MachineGun:
				player.playerAmmo.machineGunAmmo.ModifyAmmo (ammoAmount);
				break;
			case AmmoTypes.ShotGun:
				player.playerAmmo.shotgunAmmo.ModifyAmmo (ammoAmount);
				break;
			case AmmoTypes.Pistol:
				player.playerAmmo.pistolAmmo.ModifyAmmo (ammoAmount);
				break;
			}
			Destroy(gameObject);
		}
	}
	void Update()
	{
		float lerpValue = Mathf.PingPong (Time.time, duration) / duration;
		transform.position = Vector3.Lerp (topPos, bottomPos, lerpValue);
	}

	void SetImage()
	{
		switch (ammoType) 
		{
		case AmmoTypes.MachineGun:
			GetComponent<SpriteRenderer> ().sprite = machineGunAmmo;
			break;
		case AmmoTypes.ShotGun:
			GetComponent<SpriteRenderer> ().sprite = shotGunAmmo;
			break;
		case AmmoTypes.Pistol:
			GetComponent<SpriteRenderer> ().sprite = pistolAmmo;
			break;
		}
	}
}
