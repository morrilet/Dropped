using UnityEngine;
using System.Collections;

public class WeakPoint : Entity 
{
	Animator weakpointAnimator; //The animator that this weakpoint will trigger.

	void Start()
	{
		base.Start ();

		weakpointAnimator = transform.parent.GetComponent<Animator> ();
	}

	void Update()
	{
		base.Update ();

		if (!isAlive) 
		{
			weakpointAnimator.SetBool ("Weakpoint_Destroyed", true);
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Bullet") 
		{
			health -= other.gameObject.GetComponent<Bullet> ().damage / 2f;
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.075f, .025f);
		}

		if (other.gameObject.tag == "Corpse") 
		{
			health -= Mathf.Abs (other.gameObject.GetComponent<Rigidbody2D> ().velocity.magnitude);
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
		}
	}
}
