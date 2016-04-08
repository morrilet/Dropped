using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	[HideInInspector]
	public float bulletSpeed; //Speed of bullet
	[HideInInspector]
	public float bulletSpeedDeviation; //Useful for shotguns
	[HideInInspector]
	public float maxRange;
	[HideInInspector]
	public float damage;
	[HideInInspector]
	public float maxDamage;
	[HideInInspector]
	public float damageFalloff;

	public Vector3 startPos;

	void Start()
	{
		startPos = transform.position;
		bulletSpeed += Random.Range (-1 * bulletSpeedDeviation, bulletSpeedDeviation);
		damage = maxDamage;
	}

	void Update () 
	{
		transform.position += bulletSpeed * transform.right * Time.deltaTime;

		if (maxRange <= Mathf.Abs(startPos.x - transform.position.x))
		{
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Platforms")
		{
			Destroy (gameObject);
		}

		if (coll.gameObject.tag == "Door" && !coll.gameObject.GetComponent<Door> ().isOpen) 
		{
			Destroy (gameObject);
		}

		if (coll.gameObject.tag == "Corpse") 
		{
			coll.gameObject.GetComponent<Rigidbody2D> ().AddForceAtPosition (new Vector2(bulletSpeed / 5f, 0f)
				* GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().direction, transform.position, ForceMode2D.Impulse);
			Physics2D.IgnoreCollision (GetComponent<Collider2D> (), coll.gameObject.GetComponent<Collider2D>());
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.075f, .025f);
		}

		if (coll.gameObject.tag == "Enemy")
		{
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
			ReduceDamage ();
		}
	}

	//Reduces the damage done by the bullet. Primarily used when the bullet passes through something.
	public void ReduceDamage()
	{
		damage -= damageFalloff * maxDamage; //-15%
		if (damage <= 0)
			Destroy (gameObject);
	}
}
