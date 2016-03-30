using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public bool isAuto; //True for full auto, false for semi auto.
	public float bulletsPerShot;//Amount of bullets in a shot (for shotguns mostly)
	public float rotationDeviation;//Innacuracy of gun 
	public GameObject bulletPrefab; //Insert different bullet prefabs here for different guns
	public float fireRate; //Fire rate of this gun (lower = faster!)
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
		for (float i = 0; i < bullets; i++)
		{
			Quaternion rotationDeviationBuffer = new Quaternion ();
			rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-1 * rotationDeviation, rotationDeviation));
			GameObject bullet = Instantiate (bulletPrefab, transform.position, transform.rotation * rotationDeviationBuffer) as GameObject;
			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ());
		}
	}
}
