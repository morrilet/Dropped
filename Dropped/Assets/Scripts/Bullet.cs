using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float bulletSpeed;
	public float maxRange;
	public float damage;
	[HideInInspector]
	public Vector3 startPos;

	void Start()
	{
		startPos = transform.position;
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
			Destroy (gameObject);
	}
}
