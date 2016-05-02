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

	public LayerMask raycastLayerMask; //The mask to use for raycasting.

	void Start()
	{
		startPos = transform.position;
		bulletSpeed += Random.Range (-1 * bulletSpeedDeviation, bulletSpeedDeviation);
		damage = maxDamage;
		//Time.timeScale = .1f;
	}

	void Update () 
	{
		if (!GameManager.instance.isPaused)
			MoveBullet ();

		if (maxRange <= Mathf.Abs(startPos.x - transform.position.x))
		{
			Destroy (gameObject);
		}
		ReduceDamageWithRange ();
//		Debug.Log ("Bullet damage = " + damage);
	}

	//This method move the bullet while checking along the bullets path for obstacles to ensure that it doesn't pass them.
	void MoveBullet()
	{
		Vector3 velocity = bulletSpeed * transform.right * Time.deltaTime;

		Vector2 startPos = (Vector2)transform.position + (Vector2)(transform.right * GetComponent<Collider2D>().bounds.extents.x);
		Vector2 endPos = (Vector2)(startPos) + (Vector2)(velocity);

		RaycastHit2D hit = Physics2D.Raycast(startPos, (endPos - startPos).normalized, velocity.magnitude, raycastLayerMask);
		Debug.DrawLine (startPos, endPos, Color.red);
		if (hit && hit.transform.tag != "Corpse") 
		{
			velocity = hit.point - startPos;
			Debug.DrawLine (startPos, startPos + (Vector2)velocity, Color.blue);
			GameObject impact = Instantiate (impactEffect, hit.point, Quaternion.FromToRotation((velocity.x > 0)?-transform.right:transform.right, hit.normal)) as GameObject;
			Vector3 rot = new Vector3 (0f, 0f, impact.transform.rotation.eulerAngles.z);
			//impact.transform.rotation = Quaternion.Euler(rot);
			Debug.Log (hit.normal);
		}
			
		transform.position += velocity;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Platforms")
		{
//			Vector3 impactCollisionNormal = coll.contacts [0].normal;
			//GameObject impact = Instantiate (impactEffect, transform.position, transform.rotation) as GameObject;
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
