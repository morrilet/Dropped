using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
	
	Vector3 topPos;
	Vector3 bottomPos;
	float duration;

	void Start()
	{
		topPos = transform.position + new Vector3 (0, 0.15f);
		bottomPos = transform.position + new Vector3 (0, -.15f);
		duration = 2.5f;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			coll.GetComponent<Player> ().health = coll.GetComponent<Player> ().maxHealth;
			AkSoundEngine.PostEvent ("Health_Pick_Up", this.gameObject);
			Destroy(gameObject);
		}
	}
	void Update()
	{
		float lerpValue = Mathf.PingPong (Time.time, duration) / duration;
		transform.position = Vector3.Lerp (topPos, bottomPos, lerpValue);
	}
}
