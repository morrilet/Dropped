using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Collider2D))]
public class BulletImpactSoundMaterial : MonoBehaviour 
{
	public string impactSoundEffect;
	//public LayerMask impactMask;

	public bool touchingBullet = false;

	void Update () 
	{
		foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) 
		{
			if (GetComponent<Collider2D> ().IsTouching(bullet.GetComponent<Collider2D>())) 
			{
				Debug.Log ("Here");
				AkSoundEngine.PostEvent (impactSoundEffect, this.gameObject);
			}
		}

		if (touchingBullet) 
		{
			Debug.Log ("Here2");
			AkSoundEngine.PostEvent (impactSoundEffect, this.gameObject);
		}
		touchingBullet = false;
	}
}
