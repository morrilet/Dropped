using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_Script : Singleton<GUI_Script>
{
	Player player;

	[HideInInspector]
	public string weaponPickupYield;

	public Text pistolAmmoText;
	public Text shotGunAmmoText;
	public Text machineGunAmmoText;

	public GameObject openDoorText;
	public GameObject grabAmmoText;
	public GameObject grabGunText;

	public GameObject escapeGrabText;
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

		openDoorText.SetActive (false);
		grabAmmoText.SetActive (false);
		grabGunText.SetActive (false);
		escapeGrabText.SetActive (false);

		escapeObjectsEnabled = false;

		weaponPickupYield = "null";
	}

	void Update()
	{
		if (escapeObjectsEnabled == true) 
		{
			escapeGrabText.SetActive (true);
			escapeBar.GetComponent<EscapeBar>().SetBarActive(true);

			openDoorText.SetActive (false);
			grabAmmoText.SetActive (false);
			grabGunText.SetActive (false);
		} 
		else 
		{
			escapeGrabText.SetActive (false);
			escapeBar.GetComponent<EscapeBar>().SetBarActive(false);
		}

		pistolAmmoText.text = player.playerAmmo.pistolAmmo.ammountInClip + "\n\n\n" + player.playerAmmo.pistolAmmo.currentAmmo;
		shotGunAmmoText.text = player.playerAmmo.shotgunAmmo.ammountInClip + "\n\n\n" + player.playerAmmo.shotgunAmmo.currentAmmo;
		machineGunAmmoText.text = player.playerAmmo.machineGunAmmo.ammountInClip+ "\n\n\n" + player.playerAmmo.machineGunAmmo.currentAmmo;

		//This changes the text shown when the player tries to grab different kinds of guns. Doesn't work well with the
		//background of the text... Need to make different background sizes for each gun or do it by code.
		//grabGunText.transform.GetComponentInChildren<Text> ().text = "Press E to pick up " + weaponPickupYield + ".";
	}
		
}
