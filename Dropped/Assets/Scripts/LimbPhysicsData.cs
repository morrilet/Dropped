using UnityEngine;
using System.Collections;

public class LimbPhysicsData : MonoBehaviour
{
	Rigidbody2D rigidBody;
	HingeJoint2D hinge;
	JointAngleLimits2D hingeLimits;
	Vector2 velocity;
	float angularVelocity;
	float gravityScale;
	float inertia;

	public void StoreData()
	{
		rigidBody = GetComponent<Rigidbody2D> ();

		if (GetComponent<HingeJoint2D> () != null) 
		{
			hinge = GetComponent<HingeJoint2D> ();
			hingeLimits = hinge.limits;
		}

		velocity = rigidBody.velocity;
		angularVelocity = rigidBody.angularVelocity;
		gravityScale = rigidBody.gravityScale;
		inertia = rigidBody.inertia;
	}

	public void SetToStoredData()
	{
		rigidBody = GetComponent<Rigidbody2D> ();

		if (GetComponent<HingeJoint2D> () != null) 
		{
			hinge = GetComponent<HingeJoint2D> ();
			hinge.limits = hingeLimits;
		}

		rigidBody.velocity = velocity;
		//rigidBody.angularVelocity = angularVelocity;
		//rigidBody.gravityScale = gravityScale;
		//rigidBody.inertia = inertia;
	}

	public void Freeze()
	{
		rigidBody = GetComponent<Rigidbody2D> ();

		if (hinge != null) 
		{
			JointAngleLimits2D tempLimits = new JointAngleLimits2D ();
			tempLimits.min = 0;
			tempLimits.max = 0;
			hinge.limits = tempLimits;
		}

		rigidBody.isKinematic = true;
		rigidBody.velocity = Vector2.zero;
		//rigidBody.angularVelocity = 0f;
		//rigidBody.gravityScale = 0f;
		//rigidBody.inertia = 0f;
		//rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	public void UnFreeze()
	{
		rigidBody = GetComponent<Rigidbody2D> ();

		if (hinge != null) 
		{
			hinge.limits = hingeLimits;
		}

		rigidBody.isKinematic = false;
		rigidBody.velocity = velocity;
		//rigidBody.angularVelocity = angularVelocity;
		//rigidBody.gravityScale = gravityScale;
		//rigidBody.inertia = inertia;
		//rigidBody.constraints = RigidbodyConstraints2D.None;
	}
}
