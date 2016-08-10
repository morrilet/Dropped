using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GunPickup : MonoBehaviour
{
	public Player.CurrentGun pickUpGun; //Gun this pickup enables

	Vector3 topPos;
	Vector3 bottomPos;
	float duration;

	public Sprite pistolImage;
	public Sprite shotgunImage;
	public Sprite machineGunImage;

	void Start()
	{
		topPos = transform.position + new Vector3 (0, 0.15f);
		bottomPos = transform.position + new Vector3 (0, -.15f);
		duration = 2.5f;

		SetImage ();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player") {
			SetGuiText (coll);
//			GUI.instance.grabGunText.enabled = true;
		}
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player" && Input.GetButtonDown("Action") && GameObject.Find ("Player").GetComponent<Player>().corpseCarried == null &&
			!GameObject.Find ("Player").GetComponent<Player>().GetTouchingCorpse() && GameObject.Find ("Player").GetComponent<Player>().corpseCarried == null
			&& GameObject.Find("Player").GetComponent<Player>().grapplingEnemies.Count == 0) 
		{
			AudioManager.instance.PlaySoundEffect ("Ethan_AmmoBoxSound");
			switch (pickUpGun) 
			{
			case Player.CurrentGun.MachineGun:
				pickUpGun = coll.gameObject.GetComponent<Player> ().currentGun;
				coll.gameObject.GetComponent<Player> ().currentGun = Player.CurrentGun.MachineGun;
				SetGuiText (coll);
				SetImage ();
				break;
			case Player.CurrentGun.Shotgun:
				pickUpGun = coll.gameObject.GetComponent<Player> ().currentGun;
				coll.gameObject.GetComponent<Player> ().currentGun = Player.CurrentGun.Shotgun;
				SetGuiText (coll);
				SetImage ();
				break;
			case Player.CurrentGun.Pistol:
				pickUpGun = coll.gameObject.GetComponent<Player> ().currentGun;
				coll.gameObject.GetComponent<Player> ().currentGun = Player.CurrentGun.Pistol;
				SetGuiText (coll);
				SetImage ();
				break;
			}
		}
		if (coll.gameObject.tag == "Player" && coll.GetComponent<Player> ().GetTouchingCorpse () == null)
			GUI_Script.instance.grabGunText.SetActive (true);

		if (coll.gameObject.tag == "Player" && coll.GetComponent<Player> ().corpseCarried == null)
			GUI_Script.instance.grabGunText.SetActive (true);

		if (coll.gameObject.tag == "Player" && coll.GetComponent<Player> ().GetTouchingCorpse ())
			GUI_Script.instance.grabGunText.SetActive (false);
		
		if (coll.gameObject.tag == "Player" && coll.GetComponent<Player> ().corpseCarried != null)
			GUI_Script.instance.grabGunText.SetActive (false);
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
			GUI_Script.instance.grabGunText.SetActive (false);
	}

	void Update()
	{
		float lerpValue = Mathf.PingPong (Time.time, duration) / duration;
		transform.position = Vector3.Lerp (topPos, bottomPos, lerpValue);
		if (pickUpGun == Player.CurrentGun.None) {
			GUI_Script.instance.grabGunText.SetActive (false);
			Destroy (gameObject);
		}
	}

	void SetGuiText(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			switch (pickUpGun) {
			case Player.CurrentGun.MachineGun:
				GUI_Script.instance.weaponPickupYield = "Machine Gun";
				break;
			case Player.CurrentGun.Shotgun:
				GUI_Script.instance.weaponPickupYield = "Shotgun";
				break;
			case Player.CurrentGun.Pistol:
				GUI_Script.instance.weaponPickupYield = "Pistol";
				break;
			}
		}
	}

	void SetImage()
	{
		switch (pickUpGun)
		{
		case Player.CurrentGun.MachineGun:
			transform.GetComponent<SpriteRenderer> ().sprite = machineGunImage as Sprite;
			break;
		case Player.CurrentGun.Shotgun:
			transform.GetComponent<SpriteRenderer> ().sprite = shotgunImage as Sprite;
			break;
		case Player.CurrentGun.Pistol:
			transform.GetComponent<SpriteRenderer> ().sprite = pistolImage as Sprite;
			break;
		}
	}
}
