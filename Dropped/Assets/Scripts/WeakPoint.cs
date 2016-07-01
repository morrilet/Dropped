﻿using UnityEngine;
using System.Collections;

public class WeakPoint : Entity 
{
	public Animator weakpointAnimator; //The animator that this weakpoint will trigger.
	public string parameterName; //The parameter to set to true.

	void Awake()
	{
		base.Awake ();

		//weakpointAnimator = GameObject.FindGameObjectWithTag("Level").GetComponent<Animator> ();
		//Debug.Log (weakpointAnimator.gameObject);
	}

	void Update()
	{
		base.Update ();

		if (!isAlive) 
		{
			weakpointAnimator.SetTrigger (parameterName);
			Destroy (GetComponent<WeakPoint> ());
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Bullet")
		{
			health -= other.gameObject.GetComponent<Bullet> ().damage / 2f;
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.075f, .025f);
			GameManager.instance.Sleep (other.gameObject.GetComponent<Bullet>().sleepFramesOnHit);
		}

		if (other.gameObject.tag == "Corpse") 
		{
			health -= Mathf.Abs (other.gameObject.GetComponent<Rigidbody2D> ().velocity.magnitude);
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
			GameManager.instance.Sleep (3);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			health = 0;
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .1f);
		}
	}
}
