using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rope : MonoBehaviour 
{
	[HideInInspector]
	public List<GameObject> ropeSegments;

	void Start()
	{
		ropeSegments = new List<GameObject> ();

		HingeJoint2D[] tempSegments = transform.GetComponentsInChildren<HingeJoint2D> ();
		for (int i = 0; i < tempSegments.Length; i++) 
		{
			ropeSegments.Add (tempSegments [i].gameObject);
		}
	}
}
