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

	void Awake()
	{
		isPersistant = true;

		base.Awake ();
	}

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		openDoorText.enabled = false;
		grabAmmoText.enabled = false;
		grabGunText.enabled = false;
		escapeGrabText.enabled = false;

		weaponPickupYield = "null";
	}

	void Update()
	{

		if (escapeGrabText.enabled == true) 
		{
			openDoorText.enabled = false;
			grabAmmoText.enabled = false;
			grabGunText.enabled = false;
		}

		pistolAmmoText.text = "Pistol Ammo: " + player.playerAmmo.pistolAmmo.currentAmmo + " / " + player.playerAmmo.pistolAmmo.maxAmmo;
		shotGunAmmoText.text = "Shotgun Ammo: " + player.playerAmmo.shotgunAmmo.currentAmmo + " / " + player.playerAmmo.shotgunAmmo.maxAmmo;
		machineGunAmmoText.text = "MachineGun Ammo: " + player.playerAmmo.machineGunAmmo.currentAmmo + " / " + player.playerAmmo.machineGunAmmo.maxAmmo;
		grabGunText.text = "Press E to pick up " + weaponPickupYield + ".";

	}
		
}
