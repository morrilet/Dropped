using UnityEngine;
using System.Collections;

public class GunPickup : MonoBehaviour
{
	public Player.CurrentGun pickUpGun; //Gun this pickup enables

	Vector3 topPos;
	Vector3 bottomPos;
	float duration;

	void Start()
	{
		topPos = transform.position + new Vector3 (0, 0.15f);
		bottomPos = transform.position + new Vector3 (0, -.15f);
		duration = 2.5f;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			switch (pickUpGun)
			{
			case Player.CurrentGun.MachineGun:
				coll.gameObject.GetComponent<Player> ().currentGun = Player.CurrentGun.MachineGun;
				break;
			case Player.CurrentGun.Shotgun:
				coll.gameObject.GetComponent<Player>().currentGun = Player.CurrentGun.Shotgun;
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
