using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float bulletSpeed;

	void Update () 
	{
		transform.position += bulletSpeed * Vector3.right * Time.deltaTime;
	}
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Platforms")
			Destroy (gameObject);
	}
}
