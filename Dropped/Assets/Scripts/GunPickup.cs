using UnityEngine;
using System.Collections;

public class GunPickup : MonoBehaviour
{
	public Player.CurrentGun pickUpGun; //Gun this pickup enables

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
}
