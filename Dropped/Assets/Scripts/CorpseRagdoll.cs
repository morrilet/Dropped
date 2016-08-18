using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorpseRagdoll : MonoBehaviour 
{
	Player player;

	Collider2D[] childColliders;
	[HideInInspector]
	public GameObject upperTorso;
	[HideInInspector]
	public GameObject lowerTorso;
	[HideInInspector]
	public List<GameObject> limbs;
	Vector3[] limbPositions;

	JointAngleLimits2D lowerTorsoStartingLimits;

	List<GameObject> outlines;

	[HideInInspector]
	public bool isCarried;
	[HideInInspector]
	public bool isCarriedPrev;

	float ignorePlayerTime; //Time to ignore collision with the player right after the corpse has been thrown.
	float ignoreCorpseTime; //Time to ignore collision with other corpses right after the corpse has been picked up.

	public int direction;

	List<LimbPhysicsData> limbsPhysicsData;

	Rope attachedRope; //The rope this ragdoll is attached to.
	public GameObject attachedRopeSegment; //The rope segment this ragdoll is attached to.

	void Awake()
	{
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();

		SpriteRenderer[] sprites = transform.GetComponentsInChildren<SpriteRenderer> ();
		outlines = new List<GameObject> ();
		for (int i = 0; i < sprites.Length; i++) 
		{
			if (sprites [i].gameObject.name == "Outline")
				outlines.Add (sprites [i].gameObject);
		}
		//SetOutline (false);

		isCarried = false;

		upperTorso = transform.FindChild ("UpperTorso").gameObject;
		lowerTorso = transform.FindChild ("LowerTorso").gameObject;
		Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), lowerTorso.GetComponent<Collider2D> ());

		limbs = new List<GameObject> ();
		for (int i = 0; i < transform.GetComponentsInChildren<HingeJoint2D> ().Length; i++) 
		{
			if (transform.GetComponentsInChildren<HingeJoint2D> () [i] != lowerTorso)
				limbs.Add (transform.GetComponentsInChildren<HingeJoint2D> () [i].gameObject);
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
		//Debug.Log (childColliders.Length);
		for (int i = 0; i < childColliders.Length; i++) 
		{
			for (int j = 0; j < childColliders.Length; j++) 
			{
				if(i != j)
					Physics2D.IgnoreCollision (childColliders[i], childColliders[j]);
			}
		}

		//Flip (direction);
		//SetupLimbsPhysicsDataList();

		ignorePlayerTime = .2f;
		ignoreCorpseTime = .15f;
	}

	void Start () 
	{
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();

		SpriteRenderer[] sprites = transform.GetComponentsInChildren<SpriteRenderer> ();
		outlines = new List<GameObject> ();
		for (int i = 0; i < sprites.Length; i++) 
		{
			if (sprites [i].gameObject.name == "Outline")
				outlines.Add (sprites [i].gameObject);
		}
		//SetOutline (false);

		isCarried = false;
		isCarriedPrev = false;

		upperTorso = transform.FindChild ("UpperTorso").gameObject;
		lowerTorso = transform.FindChild ("LowerTorso").gameObject;
		Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), lowerTorso.GetComponent<Collider2D> ());

		limbs = new List<GameObject> ();
		for (int i = 0; i < transform.GetComponentsInChildren<HingeJoint2D> ().Length; i++) 
		{
			if (transform.GetComponentsInChildren<HingeJoint2D> () [i] != lowerTorso)
				limbs.Add (transform.GetComponentsInChildren<HingeJoint2D> () [i].gameObject);
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
		//Debug.Log (childColliders.Length);
		for (int i = 0; i < childColliders.Length; i++) 
		{
			for (int j = 0; j < childColliders.Length; j++) 
			{
				if(i != j)
					Physics2D.IgnoreCollision (childColliders[i], childColliders[j]);
			}
		}

		Flip (direction);
		//SetupLimbsPhysicsDataList ();

		if(attachedRopeSegment != null)
			attachedRope = attachedRopeSegment.transform.parent.GetComponent<Rope> ();

		ignorePlayerTime = .1f;
		ignoreCorpseTime = .1f;
	}

	public void Update()
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

			if (attachedRopeSegment != null) 
			{
				attachedRopeSegment = null;
			}

			StartCoroutine ("IgnoreCorpseCollision");
		}
		else if (isCarried) 
		{
			lowerTorsoLimits.min = 0;
			lowerTorsoLimits.max = 0;
			lowerTorso.GetComponent<HingeJoint2D> ().limits = lowerTorsoLimits;

			//upperTorso.transform.position = transform.position + new Vector3(-.25f, .1f, 0);
			upperTorso.GetComponent<Rigidbody2D> ().MovePosition(Vector2.Lerp((Vector2)upperTorso.transform.position, (Vector2)player.transform.position + new Vector2(.25f * player.direction, 1f), Time.deltaTime * 30f));
			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = true;
			upperTorso.GetComponent<Rigidbody2D> ().MoveRotation (180f);// * direction);
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
				limbs [i].GetComponent<Rigidbody2D> ().isKinematic = false;
				//limbs [i].layer = LayerMask.NameToLayer ("Ragdoll_Limb");
			}
		}

		if (attachedRopeSegment != null)
		{
			for (int i = 0; i < attachedRope.ropeSegments.Count; i++) 
			{
				Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), attachedRope.ropeSegments [i].GetComponent<Collider2D> ());
				Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), attachedRope.ropeSegments [i].GetComponent<Collider2D> ());
				for (int j = 0; j < limbs.Count; j++) 
				{
					Physics2D.IgnoreCollision (limbs [j].GetComponent<Collider2D> (), attachedRope.ropeSegments [i].GetComponent<Collider2D> ());
				}
			}

			upperTorso.layer = LayerMask.NameToLayer ("Ragdoll_Limb");

			GameObject head = transform.FindChild ("Head").gameObject;
			head.GetComponent<Rigidbody2D> ().MovePosition ((Vector2)attachedRopeSegment.transform.position 
				+ new Vector2 ((head.GetComponent<Collider2D> ().bounds.extents.x / 3f) * direction, head.GetComponent<Collider2D> ().bounds.extents.y));
		} 
		else if (attachedRope != null)
		{
			for (int i = 0; i < attachedRope.ropeSegments.Count; i++) 
			{
				Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), attachedRope.ropeSegments [i].GetComponent<Collider2D> (), false);
				Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), attachedRope.ropeSegments [i].GetComponent<Collider2D> (), false);
				for (int j = 0; j < limbs.Count; j++) 
				{
					Physics2D.IgnoreCollision (limbs [j].GetComponent<Collider2D> (), attachedRope.ropeSegments [i].GetComponent<Collider2D> (), false);
				}
			}
			attachedRope = null;
		}

		#region SquishingEffect
		bool squished = false;
		for (int i = 0; i < limbs.Count; i++) 
		{
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Corpse")) 
			{
				if (!limbs.Contains (obj) && obj != upperTorso && obj != lowerTorso) 
				{
					if (limbs [i].GetComponent<Collider2D> ().IsTouching (obj.GetComponent<Collider2D> ())) 
					{
						if (limbs [i].GetComponent<Rigidbody2D> ().velocity.magnitude >= 3.5f)
						{
							AkSoundEngine.PostEvent ("Squishing", upperTorso.gameObject);
							squished = true;
							break;
						}
					}
				}
				if(squished)
					break;
			}
		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Corpse"))
		{
			if (!limbs.Contains(obj) && obj != upperTorso && obj != lowerTorso && !squished)
			{
				if(upperTorso.GetComponent<Collider2D> ().IsTouching (obj.GetComponent<Collider2D> ()))
					if (upperTorso.GetComponent<Rigidbody2D>().velocity.magnitude >= 2.5f) 
					{
						AkSoundEngine.PostEvent ("Squishing", upperTorso.gameObject);
						squished = true;
						break;
					}
				if (lowerTorso.GetComponent<Collider2D> ().IsTouching (obj.GetComponent<Collider2D> ()))
					if (lowerTorso.GetComponent<Rigidbody2D> ().velocity.magnitude >= 2.5f) 
					{
						AkSoundEngine.PostEvent ("Squishing", upperTorso.gameObject);
						squished = true;
						break;
					}
				if(squished)
					break;
				//Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), obj.GetComponent<Collider2D> (), false);
				//Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), obj.GetComponent<Collider2D> (), false);
			}
		}
		#endregion

		isCarriedPrev = isCarried;
	}

	//Flips the ragdoll to the right (1) or left (-1)
	public void Flip(int direction)
	{
		for (int i = 0; i < limbs.Count; i++) 
		{
			HingeJoint2D limbJoint = limbs [i].GetComponent<HingeJoint2D> ();
			limbJoint.enabled = false;

			Vector3 newScale = limbs [i].transform.localScale;
			newScale.y *= direction;
			limbs [i].transform.localScale = newScale;

			JointAngleLimits2D newLimbLimits = limbJoint.limits;
			newLimbLimits.max *= direction;
			newLimbLimits.min *= direction;
			if (newLimbLimits.max < newLimbLimits.min) 
			{
				float tempMax = newLimbLimits.max;
				newLimbLimits.max = newLimbLimits.min;
				newLimbLimits.min = tempMax;
			}
			limbJoint.limits = newLimbLimits;

			limbJoint.enabled = true;

			JointAngleLimits2D newLowerTorsoLimits = lowerTorsoStartingLimits;
			newLowerTorsoLimits.max *= direction;
			newLowerTorsoLimits.min *= direction;
		}
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

		Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), player.GetComponent<Collider2D> (), false);
		Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), player.GetComponent<Collider2D> (), false);
		for (int i = 0; i < limbs.Count; i++) 
		{
			Physics2D.IgnoreCollision (limbs [i].GetComponent<Collider2D> (), player.GetComponent<Collider2D> (), false);
		}
		upperTorso.layer = LayerMask.NameToLayer ("Obstacle");
		lowerTorso.layer = LayerMask.NameToLayer ("Obstacle");
	}

	IEnumerator IgnoreCorpseCollision()
	{
		for (int i = 0; i < limbs.Count; i++) 
		{
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Corpse")) 
			{
				Physics2D.IgnoreCollision (limbs [i].GetComponent<Collider2D> (), obj.GetComponent<Collider2D> ());
			}
		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Corpse")) 
		{
			Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), obj.GetComponent<Collider2D> ());
			Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), obj.GetComponent<Collider2D> ());
		}

		yield return new WaitForSeconds (ignoreCorpseTime);

		for (int i = 0; i < limbs.Count; i++) 
		{
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Corpse")) 
			{
				if(!limbs.Contains(obj) && obj != upperTorso && obj != lowerTorso)
					Physics2D.IgnoreCollision (limbs [i].GetComponent<Collider2D> (), obj.GetComponent<Collider2D> (), false);
			}
		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Corpse"))
		{
			if (obj != upperTorso && obj != lowerTorso) 
			{
				Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), obj.GetComponent<Collider2D> (), false);
				Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), obj.GetComponent<Collider2D> (), false);
			}
		}
	}

	public void SetOutline(bool outlineEnabled)
	{
		for (int i = 0; i < outlines.Count; i++)
		{
			outlines [i].SetActive (outlineEnabled);
		}
	}

	public void PauseCorpsePhysics()
	{
		upperTorso.GetComponent<LimbPhysicsData> ().StoreData ();
		upperTorso.GetComponent<LimbPhysicsData> ().Freeze ();

		lowerTorso.GetComponent<LimbPhysicsData> ().StoreData ();
		lowerTorso.GetComponent<LimbPhysicsData> ().Freeze ();

		for (int i = 0; i < limbs.Count; i++)
		{
			limbs [i].GetComponent<LimbPhysicsData> ().StoreData ();
			limbs [i].GetComponent<LimbPhysicsData> ().Freeze ();
		}
	}

	public void ResumeCorpsePhysics()
	{
		upperTorso.GetComponent<LimbPhysicsData> ().SetToStoredData ();
		upperTorso.GetComponent<LimbPhysicsData> ().UnFreeze ();

		lowerTorso.GetComponent<LimbPhysicsData> ().SetToStoredData ();
		lowerTorso.GetComponent<LimbPhysicsData> ().UnFreeze ();

		for (int i = 0; i < limbs.Count; i++)
		{
			limbs [i].GetComponent<LimbPhysicsData> ().SetToStoredData ();
			limbs [i].GetComponent<LimbPhysicsData> ().UnFreeze ();
		}
	}

	public void AddForce(Vector3 force, ForceMode2D forceMode)
	{
		//Time.timeScale = .25f;
		//Debug.Log (force);

		//Don't know why these two chunks work, but they do. It's not so bad I guess, aside from the wild spinning.
		upperTorso.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		lowerTorso.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		//lowerTorso.GetComponent<Rigidbody2D> ().AddForce (force * .5f, forceMode); //Removed to remove rotation speed. Was .75f
		//upperTorso.GetComponent<Rigidbody2D> ().AddForce (force * .5f, forceMode); // Used to be .75f then was 1.025f
		lowerTorso.GetComponent<Rigidbody2D>().velocity = new Vector2(player.corpseThrowDirection.x * player.corpseThrowForce * 1.45f, player.corpseThrowDirection.y * player.corpseThrowForce * 1.45f);
		upperTorso.GetComponent<Rigidbody2D>().velocity = new Vector2(player.corpseThrowDirection.x * player.corpseThrowForce * 1.45f, player.corpseThrowDirection.y * player.corpseThrowForce * 1.45f);
		//upperTorso.GetComponent<Rigidbody2D> ().velocity = Vector2.zero; //Removed to remove rotation speed.

		if (attachedRopeSegment != null)
		{
			attachedRopeSegment.GetComponent<Rigidbody2D> ().AddForce (force, forceMode);
		}

		//for (int i = 0; i < limbs.Count; i++) 
		//{
			//limbs [i].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		//}
		
		//Debug.Log ("UT-V: " + upperTorso.GetComponent<Rigidbody2D> ().velocity);
		//Debug.Log ("LT-V: " + lowerTorso.GetComponent<Rigidbody2D> ().velocity);
	}

	public void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode2D forceMode)
	{
		if (attachedRopeSegment == null) 
		{
			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			upperTorso.GetComponent<Rigidbody2D> ().AddForceAtPosition (force / 1.5f, position, forceMode);

			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			lowerTorso.GetComponent<Rigidbody2D> ().AddForceAtPosition (force / 1.5f, position, forceMode);
		}

		if (attachedRopeSegment != null) 
		{
			attachedRopeSegment.GetComponent<Rigidbody2D> ().AddForce (force / 200f, forceMode);
		}
	}

	/*
	//Creates a list of physics data for use in pausing and resuming corpse physics.
	void SetupLimbsPhysicsDataList()
	{
		limbsPhysicsData = new List<LimbPhysicsData> ();
		for (int i = 0; i < limbs.Count; i++) 
		{
			LimbPhysicsData limbData = ScriptableObject.CreateInstance<LimbPhysicsData> ();
			//limbData.limb = limbs [i].gameObject;
			limbsPhysicsData.Add (limbData);
		}

		LimbPhysicsData upperTorsoData = new LimbPhysicsData ();
		//upperTorsoData.limb = upperTorso;
		limbsPhysicsData.Add (upperTorsoData);

		LimbPhysicsData lowerTorsoData = new LimbPhysicsData ();
		//lowerTorsoData.limb = lowerTorso;
		limbsPhysicsData.Add (lowerTorsoData);
	}
	*/

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if(upperTorso != null)
			Gizmos.DrawWireSphere (upperTorso.transform.position + (upperTorso.transform.up), .15f);
	}
}
