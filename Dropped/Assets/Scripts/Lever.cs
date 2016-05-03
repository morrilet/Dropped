using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
[RequireComponent (typeof (Rigidbody2D))]
public class Lever : MonoBehaviour 
{
	public LayerMask userLayer;

	public delegate void Del();
	public event Del LeverSwitched;

	Collider2D coll;
	Rigidbody2D rb2d;

	public void Start()
	{
		coll = GetComponent<Collider2D> ();
		rb2d = GetComponent<Rigidbody2D> ();

		coll.isTrigger = true;
		rb2d.isKinematic = true;
	}

	public void Update()
	{
		if(Input.GetButtonDown("Action"))
		{
			if(coll.IsTouchingLayers(userLayer))
			{
				Switch();
			}
		}
	}

	public void Switch()
	{
		if(LeverSwitched != null)
		{
			LeverSwitched();
		}
	}
}
