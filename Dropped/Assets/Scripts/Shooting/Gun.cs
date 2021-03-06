﻿using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	Player player;

	//Gun data
	public bool isAuto; //True for full auto, false for semi auto or burst.
	[Range(1, int.MaxValue)]
	public int bulletsPerShot;//Amount of bullets in a shot (for shotguns mostly)
	public float rotationDeviation;//Innacuracy of gun
	public GameObject bulletPrefab; //Insert different bullet prefabs here for different guns
	public GameObject muzzleFlashPrefab;//Insert your preffered muzzle flash here
	public float fireRate; //Fire rate of this gun (lower = faster!)
	public float playerKnockBack; //Pushback on player when fired
	public float corpseKnockBack; //Amount corpse go flying on enemy death
	public float sleepFramesOnHit; //Amount of sleep frames when the bullet hits an enemy

	public int clipSize; //Shots per clip
	[HideInInspector]
	public int ammoInClip;
	[HideInInspector]
	public int bulletsSpentFromCurrentClip;
	public float reloadTime; //Time it takes to reload
	float reloadCount;
	[HideInInspector]
	public bool isReloading;
	int bulletsToLoad;

	//Offsets
	public Vector2 bulletOffset;//Controls origin point of bullet
	public Vector2 muzzleFlashOffset;//Controls origin point of muzzle flash

	//Bullet Data
	public float bulletSpeed; //Speed of bullet
	public float bulletSpeedDeviation; //Random variance in projectile speed
	public float maxRange; //Weapon maximum range
	public float maxDamage; //damage of bullet
	public float damageFalloff; //Damage reduction after passing through a corpse.
	public float rangeDamageFallOff; //Ratio of damage lost as bullet travels

	[HideInInspector]
	public float fireRateCount;

	Vector3 knockBackVelocity; //Velocity to use for knockback.
	float velocityXSmoothing; //Smoothing to apply to knockback movement. 0.
	Vector3 defaultKickBackOffset; //Raw offset before direction applied
	Vector3 kickBackOffset; //Amount the gun "kicks" per shot
	Vector3 defaultPos;
	Vector3 defaultPositionOffset;
	bool isKnockingBack;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		fireRateCount = fireRate;
		defaultKickBackOffset = new Vector3 (-.10f, 0, 0);
		defaultPositionOffset = (transform.position - transform.parent.position);
		defaultPositionOffset.x *= transform.parent.GetComponent<Player> ().direction;
		reloadCount = reloadTime;
		//ammoInClip = clipSize;
		isReloading = false;
	}

	void Update()
	{
		fireRateCount += Time.deltaTime;
		if (isReloading)
			reloadCount += Time.deltaTime;
		defaultPos = transform.parent.position + new Vector3(defaultPositionOffset.x * transform.parent.GetComponent<Player>().direction, defaultPositionOffset.y, defaultPositionOffset.z);
		if (!isKnockingBack)
			transform.position = defaultPos;
		if (isReloading && reloadCount >= reloadTime)
		{
			isReloading = false;
			ammoInClip += bulletsToLoad;
		}

		bulletsSpentFromCurrentClip = clipSize - ammoInClip;

		//Debug.Log (ammoInClip);

		kickBackOffset = defaultKickBackOffset * transform.parent.GetComponent<Player> ().direction;
	}

	//Shoots the gun. Returns true if a shot was fired.
	public bool Shoot()
	{
		if (fireRateCount >= fireRate && ammoInClip > 0 && !isReloading) 
		{
			//AudioManager.instance.PlaySoundEffectVariation ("Ethan_Gunshot", .9f, 1.1f);
			AkSoundEngine.PostEvent("Gunshot", Camera.main.gameObject);

			if(isKnockingBack)
				StopCoroutine (KickBack());
			StartCoroutine (KickBack ());
			InstantiateShot (bulletsPerShot);
			fireRateCount = 0;
			ammoInClip -= 1;
			return true;
		}
		return false;
	}

	void InstantiateShot(float bullets)
	{
		GameObject muzzleFlash = Instantiate (muzzleFlashPrefab, new Vector3(muzzleFlashOffset.x * transform.parent.GetComponent<Player>().direction, muzzleFlashOffset.y) + transform.position, transform.rotation) as GameObject;
		muzzleFlash.transform.SetParent (this.transform);
		muzzleFlash.GetComponent<SpriteRenderer>().sortingLayerName = "Gun";

		for (float i = 0; i < bullets; i++)
		{
			Quaternion rotationDeviationBuffer = new Quaternion ();//Buffer to hold bullet rotation with deviation applied before bullet is instantiated

			if(transform.parent.GetComponent<Player>().direction == 1)
				rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-rotationDeviation, rotationDeviation));
			else if(transform.parent.GetComponent<Player>().direction == -1)
				rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-rotationDeviation, rotationDeviation) + 180);

			//Check if there's something between the player and the bullets offset before firing.
			LayerMask hitMask = new LayerMask();
			hitMask = hitMask + (1 << LayerMask.NameToLayer ("Obstacle"));
			hitMask = hitMask + (1 << LayerMask.NameToLayer ("Semi_Obstacle"));
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x, bulletOffset.y), bulletOffset, Mathf.Abs(player.transform.position.x - bulletOffset.x), hitMask);

			GameObject bullet;
			if(!hit)
				bullet = Instantiate (bulletPrefab, new Vector3(bulletOffset.x * transform.parent.GetComponent<Player>().direction, bulletOffset.y) 
					+ transform.position, transform.rotation * rotationDeviationBuffer) as GameObject;
			else
				bullet = Instantiate (bulletPrefab, new Vector3(0, bulletOffset.y) 
					+ transform.position, transform.rotation * rotationDeviationBuffer) as GameObject;

			//All needed bullet data is passed here
			bullet.GetComponent<Bullet> ().bulletSpeed = bulletSpeed;
			bullet.GetComponent<Bullet> ().bulletSpeedDeviation = bulletSpeedDeviation;
			bullet.GetComponent<Bullet> ().maxRange = maxRange;
			bullet.GetComponent<Bullet> ().maxDamage = maxDamage;
			bullet.GetComponent<Bullet> ().damageFalloff = damageFalloff;
			bullet.GetComponent<Bullet> ().corpseKnockback = corpseKnockBack;
			bullet.GetComponent<Bullet> ().sleepFramesOnHit = sleepFramesOnHit;
			bullet.GetComponent<Bullet> ().rangeDamageFallOff = rangeDamageFallOff;

			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ()); //Bullet will ignore collisions with the gun that instantiated it

			knockBackVelocity = Vector3.zero;
			knockBackVelocity.y = -.0000001f;

			float targetVelocityX = playerKnockBack * -transform.parent.GetComponent<Player> ().direction;
			knockBackVelocity.x = Mathf.Lerp(0f, targetVelocityX, 
				(transform.parent.GetComponent<Player>().controller.collisions.below)?transform.GetComponentInParent<Player>().accelerationTimeGrounded:transform.GetComponentInParent<Player>().accelerationTimeAirborne * 2f);

			transform.parent.GetComponent<Player>().controller.Move(knockBackVelocity * Time.deltaTime);
//			Debug.Log (knockBackVelocity.x);
		}
	}

	public void Reload(int bulletsAvailableToLoad)
	{
		bulletsToLoad = bulletsAvailableToLoad;
		isReloading = true;
		//AudioManager.instance.PlaySoundEffect ("Ethan_AmmoBoxSound");
		AkSoundEngine.PostEvent("Ammo_Pickup", Camera.main.gameObject);
		reloadCount = 0;
	}

	IEnumerator KickBack()
	{
		float duration = 0;

		if(isAuto)
			duration = fireRate;
		else if(!isAuto)
			duration = .15f;

		isKnockingBack = true;

		for (float t = 0; t <= duration; t += Time.deltaTime)
		{
			Vector3 pos = transform.position;
			pos.x = Mathf.Lerp (defaultPos.x, kickBackOffset.x + defaultPos.x, Mathf.PingPong(t, duration / 2) / (duration / 2));
			transform.position = pos;

			yield return null;
		}
		isKnockingBack = false;
	}



	void OnDrawGizmos()
	{
		//Draw the muzzleFlashOffset display.
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine (new Vector3(muzzleFlashOffset.x - .2f, muzzleFlashOffset.y, 0) + transform.position, 
			new Vector3(muzzleFlashOffset.x + .2f, muzzleFlashOffset.y, 0) + transform.position); //Horizontal line
		Gizmos.DrawLine (new Vector3(muzzleFlashOffset.x, muzzleFlashOffset.y - .2f, 0) + transform.position, 
			new Vector3(muzzleFlashOffset.x, muzzleFlashOffset.y + .2f, 0) + transform.position); //Vertical line

		//Draw the bulletOffset display.
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (new Vector3(bulletOffset.x - .15f, bulletOffset.y, 0) + transform.position, 
			new Vector3(bulletOffset.x + .15f, bulletOffset.y, 0) + transform.position); //Horizontal line
		Gizmos.DrawLine (new Vector3(bulletOffset.x, bulletOffset.y - .15f, 0) + transform.position, 
			new Vector3(bulletOffset.x, bulletOffset.y + .15f, 0) + transform.position); //Vertical line
	}
}
