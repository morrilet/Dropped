using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Corpse : MonoBehaviour 
{
	Player player;

	[HideInInspector]
	public GameObject upperTorso;
	[HideInInspector]
	public GameObject lowerTorso;
	HingeJoint2D hingeJoint;
	Vector2 initialAngleLimits;

	[HideInInspector]
	public bool isCarried;

	public List<Corpse> touchingCorpses; //A list of corpses that this corpse is currently touching.

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

		touchingCorpses = new List<Corpse> ();
	}

	void Update()
	{
		touchingCorpses = new List<Corpse> ();
		touchingCorpses = GetTouchingCorpses ();

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
			lowerTorso.layer = LayerMask.NameToLayer("Default");
		}
		else 
		{
			limits.min = initialAngleLimits.x;
			limits.max = initialAngleLimits.y;
			hingeJoint.limits = limits;

			upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			upperTorso.layer = LayerMask.NameToLayer ("Obstacle");

			lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
			lowerTorso.layer = LayerMask.NameToLayer ("Obstacle");
		}
	}

	public List<Corpse> GetTouchingCorpses()
	{
		//touchingCorpses = new List<Corpse> ();
		List<Corpse> tempCorpsesTouching = new List<Corpse> ();
		List<Corpse> corpsesInScene = new List<Corpse> ();

		GameObject[] tempCorpseObjectsInScene = GameObject.FindGameObjectsWithTag ("Ragdoll");
		for (int i = 0; i < tempCorpseObjectsInScene.Length; i++) 
		{
			corpsesInScene.Add (tempCorpseObjectsInScene [i].GetComponent<Corpse> ());
		}

		for(int i = 0; i < corpsesInScene.Count; i++)
		{
			if (!corpsesInScene [i].Equals (this.GetComponent<Corpse> ())) 
			{
				if (upperTorso.GetComponent<Collider2D> ().IsTouching (corpsesInScene [i].upperTorso.GetComponent<Collider2D> ())
				    || upperTorso.GetComponent<Collider2D> ().IsTouching (corpsesInScene [i].lowerTorso.GetComponent<Collider2D> ())) 
				{
					//tempCorpsesTouching.Add (corpsesInScene [i]);
					tempCorpsesTouching.AddRange (corpsesInScene [i].touchingCorpses);
					tempCorpsesTouching = tempCorpsesTouching.Distinct ().ToList ();
				} 
				else 
				{
					for (int j = 0; j < corpsesInScene [i].touchingCorpses.Count; j++) 
					{
						tempCorpsesTouching.Remove (corpsesInScene [i]);
					}
				}
				
				if (lowerTorso.GetComponent<Collider2D> ().IsTouching (corpsesInScene [i].upperTorso.GetComponent<Collider2D> ())
				   || lowerTorso.GetComponent<Collider2D> ().IsTouching (corpsesInScene [i].lowerTorso.GetComponent<Collider2D> ()))
				{
						//tempCorpsesTouching.Add (corpsesInScene [i]);
						tempCorpsesTouching.AddRange(corpsesInScene[i].touchingCorpses);
						tempCorpsesTouching = tempCorpsesTouching.Distinct ().ToList ();
				}
				else 
				{
					for (int j = 0; j < corpsesInScene [i].touchingCorpses.Count; j++) 
					{
						tempCorpsesTouching.Remove (corpsesInScene [i]);
					}
				}
			}
		}

		tempCorpsesTouching.Add (this.GetComponent<Corpse> ());
		return tempCorpsesTouching.Distinct().ToList();
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
