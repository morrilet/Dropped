using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float bulletSpeed;
	public float maxRange;
	Vector3 startPos;

	void Start()
	{
		startPos = transform.position;
	}

	void Update () 
	{
		transform.position += bulletSpeed * Vector3.right * Time.deltaTime;

		if (maxRange <= startPos.x + transform.position.x)
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
