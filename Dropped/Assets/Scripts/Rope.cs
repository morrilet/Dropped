using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rope : MonoBehaviour 
{
	[HideInInspector]
	public List<GameObject> ropeSegments;

	List<JointAngleLimits2D> storedSegmentJointData; //This is for pausing/unpausing the rope segments.

	void Start()
	{
		ropeSegments = new List<GameObject> ();
		//storedSegmentJointData = new List<JointAngleLimits2D> ();

		HingeJoint2D[] tempSegments = transform.GetComponentsInChildren<HingeJoint2D> ();
		for (int i = 0; i < tempSegments.Length; i++)
		{
			ropeSegments.Add (tempSegments [i].gameObject);
		}
	}

	public void Pause()
	{
		for (int i = 0; i < ropeSegments.Count; i++) 
		{
			//storedSegmentJointData.Add (ropeSegments [i].GetComponent<HingeJoint2D> ().limits); //Store the current joint limits.

			//Limit the joints movement to only its current angle.
			//JointAngleLimits2D tempLimit = new JointAngleLimits2D();
			//tempLimit.max = ropeSegments [i].GetComponent<HingeJoint2D> ().jointAngle;
			//tempLimit.min = ropeSegments [i].GetComponent<HingeJoint2D> ().jointAngle;
			//ropeSegments [i].GetComponent<HingeJoint2D> ().limits = tempLimit;

			ropeSegments [i].GetComponent<Rigidbody2D> ().isKinematic = true; //Stop the segment from recieving physics forces.
		}
			
		//Debug.Log ("PAUSE: Segments = " + ropeSegments.Count + ", StoredData = " + storedSegmentJointData.Count);
	}

	public void Resume()
	{
		//Debug.Log ("RESUME: Segments = " + ropeSegments.Count + ", StoredData = " + storedSegmentJointData.Count);

		for (int i = 0; i < ropeSegments.Count; i++)
		{
			//ropeSegments [i].GetComponent<HingeJoint2D> ().limits = storedSegmentJointData [i]; //Restore the joints limits.

			ropeSegments [i].GetComponent<Rigidbody2D> ().isKinematic = false; //Resume the segments physics forces.
		}
	}
}
