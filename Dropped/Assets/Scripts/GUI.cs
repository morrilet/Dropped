﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI : MonoBehaviour 
{
	Player player;

	public Text shotGunAmmoText;
	public Text machineGunAmmoText;

	public Text openDoorText;
	public Text grabAmmoText;

	public static GUI Instance { get; private set; }

	void Awake()
	{
		if (Instance != null && Instance != this) 
		{
			Destroy (gameObject);
		}
		Instance = this.GetComponent<GUI> ();
	}

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		openDoorText.enabled = false;
		grabAmmoText.enabled = false;
	}

	void Update()
	{
		shotGunAmmoText.text = "Shotgun Ammo: " + player.playerAmmo.shotgunAmmo.currentAmmo + " / " + player.playerAmmo.shotgunAmmo.maxAmmo;
		machineGunAmmoText.text = "MachineGun Ammo: " + player.playerAmmo.machineGunAmmo.currentAmmo + " / " + player.playerAmmo.machineGunAmmo.maxAmmo;
	}
}