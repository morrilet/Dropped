using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public bool isAuto; //True for full auto, false for semi auto.

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
				Shoot ();
				fireRateCount = 0;
			}
		}
		if (!isAuto) //Semi Auto Fire
		{
			if (Input.GetKeyDown (KeyCode.X) && fireRateCount >= fireRate)
			{
				Shoot ();
				fireRateCount = 0;
			}
		}
		fireRateCount += Time.deltaTime;
		
	}

	void Shoot()
	{
		GameObject bullet = Instantiate (bulletPrefab, transform.position, transform.rotation) as GameObject;
		Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ());
	}
}
