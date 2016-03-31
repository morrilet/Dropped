using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	//Gun data
	public bool isAuto; //True for full auto, false for semi auto or burst.
	public bool isFlamethrower; //If true, bullets transform relative to parent
	public float bulletsPerShot;//Amount of bullets in a shot (for shotguns mostly)
	public float shotsPerBurst; //Amount of times Shoot() is called per input (overrriden if isAuto = true)
	public float rotationDeviation;//Innacuracy of gun 
	public GameObject bulletPrefab; //Insert different bullet prefabs here for different guns
	public GameObject muzzleFlashPrefab;//Insert your preffered muzzle flash here
	public float fireRate; //Fire rate of this gun (lower = faster!)

	public float bulletOffSetX;//Controls origin point of bullet 
	public float bulletOffSetY;

	public float muzzleFlashOffSetX;//Controls origin point of muzzle flash
	public float muzzleFlashOffSetY;

	//Bullet Data
	public float bulletSpeed; //Speed of bullet
	public float bulletSpeedDeviation; //Random variance in projectile speed
	public float maxRange; //Weapon maximum range
	public float damage; //damage of bullet

	float fireRateCount;

	void Start()
	{
		fireRateCount = fireRate;
	}

	void Update ()
	{
		if (isAuto) { //Auto Fire
			if (Input.GetKey (KeyCode.X) && fireRateCount >= fireRate) {
				Shoot (bulletsPerShot);
				fireRateCount = 0;
			}
		}
		if (!isAuto) //Semi Auto Fire
		{
			if (Input.GetKeyDown (KeyCode.X) && fireRateCount >= fireRate)
			{
				Shoot (bulletsPerShot);
				fireRateCount = 0;
			}
		}
		fireRateCount += Time.deltaTime;
		
	}

	void Shoot(float bullets)
	{
		GameObject muzzleFlash = Instantiate (muzzleFlashPrefab, new Vector3(muzzleFlashOffSetX + transform.position.x, + muzzleFlashOffSetY + transform.position.y), transform.rotation) as GameObject;
		muzzleFlash.transform.SetParent (this.transform);

		for (float i = 0; i < bullets; i++)
		{
			Quaternion rotationDeviationBuffer = new Quaternion ();//Buffer to hold bullet rotation with deviation applied before bullet is instantiated
			rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-1 * rotationDeviation, rotationDeviation));
			GameObject bullet = Instantiate (bulletPrefab, new Vector3(bulletOffSetX + transform.position.x, bulletOffSetY + transform.position.y), transform.rotation * rotationDeviationBuffer) as GameObject;//Instantiate bullet
			bullet.GetComponent<Bullet> ().bulletSpeed = bulletSpeed;//Pass data to bullets
			bullet.GetComponent<Bullet> ().bulletSpeedDeviation = bulletSpeedDeviation;
			bullet.GetComponent<Bullet> ().maxRange = maxRange;
			bullet.GetComponent<Bullet> ().damage = damage;
			if (isFlamethrower)
				bullet.transform.SetParent (this.transform);
			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ()); //Bullet will ignore collisions with the gun that instantiated it

		}
	}
}
