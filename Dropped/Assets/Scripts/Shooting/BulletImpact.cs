using UnityEngine;
using System.Collections;

public class BulletImpact : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (KillOnAnimationEnd());
	}
		
	private IEnumerator KillOnAnimationEnd()
	{
		yield return new WaitForSeconds (.1f);
		Destroy (gameObject);
	}
}
