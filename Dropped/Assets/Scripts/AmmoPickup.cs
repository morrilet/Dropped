using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour 
{
	Player player;

	public enum AmmoTypes
	{
		MachineGun,
		ShotGun
	}
	public AmmoTypes ammoType; //The type of ammo to give to the player.
	public int ammoAmount; //The amount of ammo to give to the player.

	Vector3 topPos;
	Vector3 bottomPos;
	float duration;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		topPos = transform.position + new Vector3 (0, 0.15f);
		bottomPos = transform.position + new Vector3 (0, -.15f);
		duration = 2.5f;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			switch (ammoType) 
			{
			case AmmoTypes.MachineGun:
				player.playerAmmo.machineGunAmmo.ModifyAmmo (ammoAmount);
				break;
			case AmmoTypes.ShotGun:
				player.playerAmmo.shotgunAmmo.ModifyAmmo (ammoAmount);
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
}
