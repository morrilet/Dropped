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
	[HideInInspector]
	public float corpseKnockback;
	[HideInInspector]
	public float sleepFramesOnHit;
	[HideInInspector]
	public float rangeDamageFallOff;

	public GameObject impactEffect;

	public Vector3 startPos;

	void Start()
	{
		startPos = transform.position;
		bulletSpeed += Random.Range (-1 * bulletSpeedDeviation, bulletSpeedDeviation);
		damage = maxDamage;
	}

	void Update () 
	{
		if(!GameManager.instance.isPaused)
			transform.position += bulletSpeed * transform.right * Time.deltaTime;

		if (maxRange <= Mathf.Abs(startPos.x - transform.position.x))
		{
			Destroy (gameObject);
		}
		ReduceDamageWithRange ();
//		Debug.Log ("Bullet damage = " + damage);
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Platforms")
		{
//			Vector3 impactCollisionNormal = coll.contacts [0].normal;
			GameObject impact = Instantiate (impactEffect, transform.position, transform.rotation) as GameObject;
//			Vector3 impactCollisionNormalNormalized = impactCollisionNormal.normalized;
//			impact.transform.rotation.eulerAngles = impactCollisionNormalNormalized;
			Destroy (gameObject);
		}

		if (coll.gameObject.tag == "Door" && !coll.gameObject.GetComponent<Door> ().isOpen) 
		{
			Instantiate (impactEffect, transform.position, transform.rotation);
			Destroy (gameObject);
		}

		if (coll.gameObject.tag == "Corpse") 
		{
			coll.gameObject.GetComponent<Rigidbody2D> ().AddForceAtPosition (new Vector2(bulletSpeed / 5f, 0f)
				* GameObject.Find ("Player").GetComponent<Player>().direction, transform.position, ForceMode2D.Impulse);
			Physics2D.IgnoreCollision (GetComponent<Collider2D> (), coll.gameObject.GetComponent<Collider2D>());
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.075f, .025f);
		}

		if (coll.gameObject.tag == "Enemy")
		{
//			GameObject impact = Instantiate (impactEffect, transform.position, transform.rotation) as GameObject;
//			impact.transform.SetParent (coll.transform);
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
		}
	}

	//Reduces the damage done by the bullet. Primarily used when the bullet passes through something.
	public void ReduceDamage()
	{
		damage -= damageFalloff * maxDamage; //-15%
		if (damage <= 0)
			Destroy (gameObject);
	}

	//Reduces damage using a value multiplied by delta time.
	public void ReduceDamageWithRange()
	{
		damage -= rangeDamageFallOff * maxDamage * Time.deltaTime;
		if (damage <= 0)
			Destroy (gameObject);
	}
}
