using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI : Singleton<GUI>
{
	Player player;

	[HideInInspector]
	public string weaponPickupYield;

	public Text pistolAmmoText;
	public Text shotGunAmmoText;
	public Text machineGunAmmoText;

	public Text openDoorText;
	public Text grabAmmoText;
	public Text grabGunText;

	public Text escapeGrabText;
	public GameObject escapeBar;
	public bool escapeObjectsEnabled;

	public override void Awake()
	{
		isPersistant = false;

		base.Awake ();
	}

	void Start()
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();

		openDoorText.enabled = false;
		grabAmmoText.enabled = false;
		grabGunText.enabled = false;
		escapeGrabText.enabled = false;

		escapeObjectsEnabled = false;

		weaponPickupYield = "null";
	}

	void Update()
	{
		if (escapeObjectsEnabled == true) 
		{
			escapeGrabText.enabled = true;
			escapeBar.SetActive (true);

			openDoorText.enabled = false;
			grabAmmoText.enabled = false;
			grabGunText.enabled = false;
		} 
		else 
		{
			escapeGrabText.enabled = false;
			escapeBar.SetActive (false);
		}

		pistolAmmoText.text = "Pistol Ammo: " + player.playerAmmo.pistolAmmo.ammountInClip + " / " + player.playerAmmo.pistolAmmo.currentAmmo;
		shotGunAmmoText.text = "Shotgun Ammo: " + player.playerAmmo.shotgunAmmo.ammountInClip + " / " + player.playerAmmo.shotgunAmmo.currentAmmo;
		machineGunAmmoText.text = "MachineGun Ammo: " + player.playerAmmo.machineGunAmmo.ammountInClip+ " / " + player.playerAmmo.machineGunAmmo.currentAmmo;
		grabGunText.text = "Press E to pick up " + weaponPickupYield + ".";

	}
		
}
