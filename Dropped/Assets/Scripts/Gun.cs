using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	
	public GameObject bulletPrefab;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.X))
			Shoot ();
	}

	void Shoot()
	{
		GameObject bullet = Instantiate (bulletPrefab, transform.position, transform.rotation) as GameObject;
		Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ());
	}
}
