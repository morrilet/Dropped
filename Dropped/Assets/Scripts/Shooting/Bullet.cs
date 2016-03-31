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
	public Vector3 startPos;

	void Start()
	{
		startPos = transform.position;
		bulletSpeed += Random.Range (-1 * bulletSpeedDeviation, bulletSpeedDeviation);
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
		if (coll.gameObject.tag == "Corpse") 
		{
			coll.gameObject.GetComponent<Rigidbody2D> ().AddForceAtPosition (new Vector2(bulletSpeed / 5f, 0f), transform.position, ForceMode2D.Impulse);
			Destroy (gameObject);
		}
	}
}
