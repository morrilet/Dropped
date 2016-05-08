using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorpseRagdoll : MonoBehaviour 
{
	Player player;

	Collider2D[] childColliders;
	[HideInInspector]
	public GameObject upperTorso;
	GameObject lowerTorso;
	List<GameObject> limbs;
	Vector3[] limbPositions;

	JointAngleLimits2D lowerTorsoStartingLimits;

	[HideInInspector]
	public bool isCarried;
	[HideInInspector]
	public bool isCarriedPrev;

	float ignorePlayerTime; //Time to ignore collision with the player right after the corpse has been thrown.

	void Start () 
	{
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();

		isCarried = false;

		upperTorso = transform.FindChild ("UpperTorso").gameObject;
		lowerTorso = upperTorso.transform.FindChild ("LowerTorso").gameObject;
		Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), lowerTorso.GetComponent<Collider2D> ());

		limbs = new List<GameObject> ();
		for (int i = 0; i < upperTorso.GetComponentsInChildren<HingeJoint2D> ().Length; i++) 
		{
			if (upperTorso.GetComponentsInChildren<HingeJoint2D> () [i] != lowerTorso)
				limbs.Add (upperTorso.GetComponentsInChildren<HingeJoint2D> () [i].gameObject);
			//if (limbs.Contains (lowerTorso))
			//limbs.Remove (lowerTorso);
		}

		limbPositions = new Vector3[limbs.Count];
		for (int i = 0; i < limbPositions.Length; i++) 
		{
			limbPositions [i] = limbs [i].transform.localPosition;
		}

		lowerTorsoStartingLimits = lowerTorso.GetComponent<HingeJoint2D> ().limits;

		childColliders = transform.GetComponentsInChildren<Collider2D> ();
		Debug.Log (childColliders.Length);
		for (int i = 0; i < childColliders.Length; i++) 
		{
			for (int j = 0; j < childColliders.Length; j++) 
			{
				if(i != j)
					Physics2D.IgnoreCollision (childColliders[i], childColliders[j]);
			}
		}

		ignorePlayerTime = .1f;
	}

	void Update()
	{
		//Time.timeScale = .5f;
		for (int i = 0; i < limbPositions.Length; i++) 
		{
			//limbs [i].transform.localPosition = limbPositions [i];
		}

		JointAngleLimits2D lowerTorsoLimits = new JointAngleLimits2D ();
		if (isCarried && !isCarriedPrev) 
		{
			for (int i = 0; i < limbs.Count; i++) 
			{
				limbs [i].GetComponent<Rigidbody2D> ().isKinematic = true;
			}
		}
		else if (isCarried) 
		{
			lowerTorsoLimits.min = 0;
			lowerTorsoLimits.max = 0;
			lowerTorso.GetComponent<HingeJoint2D> ().limits = lowerTorsoLimits;

			//upperTorso.transform.position = transform.position + new Vector3(-.25f, .1f, 0);
			upperTorso.GetComponent<Rigidbody2D>().MovePosition(new Vector2(-.25f, .1f));
			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = true;
			upperTorso.GetComponent<Rigidbody2D> ().MoveRotation (180f);
			upperTorso.layer = LayerMask.NameToLayer("Default");

			//lowerTorso.transform.position = transform.position + new Vector3(-.25f, 0, 0);
			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = true;
			//lowerTorso.transform.rotation = Quaternion.identity;
			//lowerTorso.transform.rotation = Quaternion.Euler(lowerTorso.transform.rotation.eulerAngles.x, lowerTorso.transform.rotation.eulerAngles.y, lowerTorso.transform.rotation.eulerAngles.z - 180);
			lowerTorso.layer = LayerMask.NameToLayer("Default");

			for (int i = 0; i < limbs.Count; i++) 
			{
				limbs [i].GetComponent<Collider2D> ().enabled = false;
				limbs [i].GetComponent<Rigidbody2D> ().isKinematic = false;
				//limbs [i].layer = LayerMask.NameToLayer ("Default");
			}
		} 
		else if (!isCarried && isCarriedPrev)
		{
			StartCoroutine ("IgnorePlayerCollision");

			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			//lowerTorso.layer = LayerMask.NameToLayer ("Obstacle");
			lowerTorso.GetComponent<HingeJoint2D> ().limits = lowerTorsoStartingLimits;

			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			//upperTorso.layer = LayerMask.NameToLayer ("Obstacle");

			for (int i = 0; i < limbs.Count; i++) 
			{
				limbs [i].GetComponent<Collider2D> ().enabled = true;
				//limbs [i].GetComponent<Rigidbody2D> ().isKinematic = false;
				//limbs [i].layer = LayerMask.NameToLayer ("Ragdoll_Limb");
			}
		}

		isCarriedPrev = isCarried;
	}

	IEnumerator IgnorePlayerCollision()
	{
		Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), player.GetComponent<Collider2D> ());
		Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), player.GetComponent<Collider2D> ());
		for (int i = 0; i < limbs.Count; i++) 
		{
			Physics2D.IgnoreCollision (limbs [i].GetComponent<Collider2D> (), player.GetComponent<Collider2D> ());
		}
		upperTorso.layer = LayerMask.NameToLayer ("Default");
		lowerTorso.layer = LayerMask.NameToLayer ("Default");
		yield return new WaitForSeconds (ignorePlayerTime);
		upperTorso.layer = LayerMask.NameToLayer ("Obstacle");
		lowerTorso.layer = LayerMask.NameToLayer ("Obstacle");
		Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), player.GetComponent<Collider2D> (), false);
		Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), player.GetComponent<Collider2D> (), false);
		for (int i = 0; i < limbs.Count; i++) 
		{
			Physics2D.IgnoreCollision (limbs [i].GetComponent<Collider2D> (), player.GetComponent<Collider2D> (), false);
		}
	}

	public void SetOutline(bool outlineEnabled)
	{
	}

	public void PauseCorpsePhysics()
	{
	}

	public void ResumeCorpsePhysics()
	{
	}

	public void AddForce(Vector3 force, ForceMode2D forceMode)
	{
		upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		upperTorso.GetComponent<Rigidbody2D> ().AddForce (force / 1.5f, forceMode);

		lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		lowerTorso.GetComponent<Rigidbody2D> ().AddForce (force / 1.5f, forceMode);
	}
}
