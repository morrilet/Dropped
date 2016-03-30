using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public bool isAuto; //True for full auto, false for semi auto.
	public float bulletsPerShot;//Amount of bullets in a shot (for shotguns mostly)
	public float rotationDeviation;//Innacuracy of gun 
	public GameObject bulletPrefab; //Insert different bullet prefabs here for different guns
	public GameObject muzzleFlashPrefab;//Insert your preffered muzzle flash here
	public float fireRate; //Fire rate of this gun (lower = faster!)

	public float bulletOffSetX;//Controls origin point of bullet 
	public float bulletOffSetY;

	public float muzzleFlashOffSetX;//Controls origin point of muzzle flash
	public float muzzleFlashOffSetY;

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
			Quaternion rotationDeviationBuffer = new Quaternion ();
			rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-1 * rotationDeviation, rotationDeviation));
			GameObject bullet = Instantiate (bulletPrefab, new Vector3(bulletOffSetX + transform.position.x, bulletOffSetY + transform.position.y), transform.rotation * rotationDeviationBuffer) as GameObject;
			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ());
		}
	}
}
