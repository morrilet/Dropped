using UnityEngine;
using System.Collections;

public class Corpse : MonoBehaviour 
{
	Player player;

	GameObject upperTorso;
	GameObject lowerTorso;
	HingeJoint2D hingeJoint;
	Vector2 initialAngleLimits;

	[HideInInspector]
	public bool isCarried;
	public bool isCarriedPrev;

	//Maybe put this in a struct in the future.
	public GameObject[] limbs;
	Vector2[] limbAnchors;
	Vector2[] limbConnectedAnchors;

	GameObject[] outlines;

	//Stored physics values for the upper torso to be used during pausing.
	Vector2 upperTorsoStoredVelocity;
	float upperTorsoStroredAngularVelocity;
	float upperTorsoStoredGravityScale;
	float upperTorsoStoredInertia;

	//Stored physics values for the lower torso to be used during pausing.
	Vector2 lowerTorsoStoredVelocity;
	float lowerTorsoStoredAngularVelocity;
	float lowerTorsoStoredGravityScale;
	float lowerTorsoStoredInertia;

	void Start()
	{
		isCarried = false;

		upperTorso = transform.FindChild ("UpperTorso").gameObject;
		lowerTorso = transform.FindChild ("LowerTorso").gameObject;
		hingeJoint = lowerTorso.GetComponent<HingeJoint2D> ();
		initialAngleLimits = new Vector2 (hingeJoint.limits.min, hingeJoint.limits.max);

		outlines = new GameObject[2];
		outlines [0] = upperTorso.transform.FindChild ("Outline").gameObject;
		outlines [1] = lowerTorso.transform.FindChild ("Outline").gameObject;
		SetOutline (false);

		//limbAnchors = new Vector2[limbs.Length];
		//limbConnectedAnchors = new Vector2[limbs.Length];
		for (int i = 0; i < limbs.Length; i++) 
		{
			Physics2D.IgnoreCollision (upperTorso.GetComponent<Collider2D> (), limbs [i].GetComponent<Collider2D> ());
			Physics2D.IgnoreCollision (lowerTorso.GetComponent<Collider2D> (), limbs [i].GetComponent<Collider2D> ());
			for(int j = 0; j < limbs.Length; j++)
			{
				Physics2D.IgnoreCollision (limbs [i].GetComponent<Collider2D> (), limbs [j].GetComponent<Collider2D> ());
			}

			//limbAnchors [i] = limbs [i].GetComponent<HingeJoint2D> ().anchor;
			//limbConnectedAnchors [i] = limbs [i].GetComponent<HingeJoint2D> ().connectedAnchor;

			//limbs [i].GetComponent<HingeJoint2D> ().anchor *= -1f;
			//limbs [i].GetComponent<HingeJoint2D> ().connectedAnchor *= -1f;
		}
	}

	void Update()
	{
		JointAngleLimits2D limits = new JointAngleLimits2D();

		if (isCarried) //Behaviour for if the corpse is carried or not.
		{
			limits.max = 0;
			limits.min = 0;
			hingeJoint.limits = limits;

			upperTorso.transform.position = transform.position + new Vector3(.25f, 0, 0);
			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = true;
			upperTorso.transform.rotation = Quaternion.identity;
			upperTorso.layer = LayerMask.NameToLayer("Default");

			lowerTorso.transform.position = transform.position + new Vector3(-.25f, 0, 0);
			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = true;
			lowerTorso.transform.rotation = Quaternion.identity;
			lowerTorso.transform.rotation = Quaternion.Euler(lowerTorso.transform.rotation.eulerAngles.x, lowerTorso.transform.rotation.eulerAngles.y, lowerTorso.transform.rotation.eulerAngles.z - 180);
			lowerTorso.layer = LayerMask.NameToLayer("Default");

			for (int i = 0; i < limbs.Length; i++) 
			{
				//limbs [i].GetComponent<HingeJoint2D> ().anchor = limbAnchors [i];
				//limbs [i].GetComponent<HingeJoint2D> ().connectedAnchor = limbConnectedAnchors [i];
			}
		}
		else if(!isCarried && isCarriedPrev)
		{
			/*
			limits.min = initialAngleLimits.x;
			limits.max = initialAngleLimits.y;
			hingeJoint.limits = limits;

			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			upperTorso.layer = LayerMask.NameToLayer ("Obstacle");

			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			lowerTorso.layer = LayerMask.NameToLayer ("Obstacle");
			*/

			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			//lowerTorso.transform.rotation = Quaternion.Euler(0f, 0f, upperTorso.transform.rotation.eulerAngles.z -);
			lowerTorso.layer = LayerMask.NameToLayer ("Obstacle");

			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			//upperTorso.transform.rotation = Quaternion.identity;
			upperTorso.layer = LayerMask.NameToLayer ("Obstacle");

			//Change the angle limits to the other side because we flipped lowerTorso.
			limits.min = initialAngleLimits.x - 180;
			limits.max = initialAngleLimits.y - 180;
			hingeJoint.limits = limits;
		}

		isCarriedPrev = isCarried;
	}

	public void SetOutline(bool outlineEnabled) //true if outline on, false if outline off.
	{
		for (int i = 0; i < outlines.Length; i++) 
		{
			outlines [i].SetActive (outlineEnabled);
		}
	}

	public void PauseCorpsePhysics()
	{
		Rigidbody2D upperTorsoRB = upperTorso.GetComponent<Rigidbody2D> ();
		Rigidbody2D lowerTorsoRB = lowerTorso.GetComponent<Rigidbody2D> ();

		//Store upper torso data.
		upperTorsoStoredVelocity = upperTorsoRB.velocity;
		upperTorsoStroredAngularVelocity = upperTorsoRB.angularVelocity;
		upperTorsoStoredGravityScale = upperTorsoRB.gravityScale;
		upperTorsoStoredInertia = upperTorsoRB.inertia;

		//Freeze upper torso;
		upperTorsoRB.velocity = Vector2.zero;
		upperTorsoRB.angularVelocity = 0f;
		upperTorsoRB.gravityScale = 0f;
		upperTorsoRB.inertia = 0f;
		upperTorsoRB.constraints = RigidbodyConstraints2D.FreezeAll;

		//Store lower torso data.
		lowerTorsoStoredVelocity = lowerTorsoRB.velocity;
		lowerTorsoStoredAngularVelocity = lowerTorsoRB.angularVelocity;
		lowerTorsoStoredGravityScale = lowerTorsoRB.gravityScale;
		lowerTorsoStoredInertia = lowerTorsoRB.inertia;

		//freeze lowerTorso;
		lowerTorsoRB.velocity = Vector2.zero;
		lowerTorsoRB.angularVelocity = 0f;
		lowerTorsoRB.gravityScale = 0f;
		lowerTorsoRB.inertia = 0f;
		lowerTorsoRB.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	public void ResumeCorpsePhysics()
	{
		Rigidbody2D upperTorsoRB = upperTorso.GetComponent<Rigidbody2D> ();
		Rigidbody2D lowerTorsoRB = lowerTorso.GetComponent<Rigidbody2D> ();

		upperTorsoRB.velocity = upperTorsoStoredVelocity;
		upperTorsoRB.angularVelocity = upperTorsoStroredAngularVelocity;
		upperTorsoRB.gravityScale = upperTorsoStoredGravityScale;
		upperTorsoRB.inertia = upperTorsoStoredInertia;
		upperTorsoRB.constraints = RigidbodyConstraints2D.None;

		lowerTorsoRB.velocity = lowerTorsoStoredVelocity;
		lowerTorsoRB.angularVelocity = lowerTorsoStoredAngularVelocity;
		lowerTorsoRB.gravityScale = lowerTorsoStoredGravityScale;
		lowerTorsoRB.inertia = lowerTorsoStoredInertia;
		lowerTorsoRB.constraints = RigidbodyConstraints2D.None;
	}

	public void AddForce(Vector3 force, ForceMode2D forceMode)
	{
		upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		upperTorso.GetComponent<Rigidbody2D> ().AddForce (force, forceMode);

		lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		lowerTorso.GetComponent<Rigidbody2D> ().AddForce (transform.TransformVector(force), forceMode);
	}
}
