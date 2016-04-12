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

	void Start()
	{
		isCarried = false;

		upperTorso = transform.FindChild ("UpperTorso").gameObject;
		lowerTorso = transform.FindChild ("LowerTorso").gameObject;
		hingeJoint = lowerTorso.GetComponent<HingeJoint2D> ();
		initialAngleLimits = new Vector2 (hingeJoint.limits.min, hingeJoint.limits.max);
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

	public void AddForce(Vector3 force, ForceMode2D forceMode)
	{
		upperTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		upperTorso.GetComponent<Rigidbody2D> ().AddForce (force, forceMode);

		lowerTorso.GetComponent<Rigidbody2D> ().isKinematic = false;
		lowerTorso.GetComponent<Rigidbody2D> ().AddForce (transform.TransformVector(force), forceMode);
	}
}
