using UnityEngine;
using System.Collections;

public class TrajectorySimulation : MonoBehaviour 
{
	public LineRenderer trajectoryLine;
	public Player player;

	public int segmentCount; //How many segmets to calculate. More = smoother.
	public float segmentScale; //Scale of the segments to use.

	private Collider2D hitObject;
	public Collider2D HitObject {get {return hitObject;}}

	void FixedUpdate()
	{
		if(player.throwingCorpse != false)
			SimulatePath ();
		//Debug.Log (player.corpseThrowDirection * player.corpseThrowForce);
	}

	void SimulatePath()
	{
		Vector3[] segments = new Vector3[segmentCount];

		segments [0] = player.corpseCarried.transform.position;

		Vector3 segVelocity = player.corpseThrowDirection * player.corpseThrowForce;

		hitObject = null;

		for (int i = 1; i < segmentCount; i++)
		{
			float segTime = (segVelocity.sqrMagnitude != 0) ? (segmentScale / player.corpseThrowForce) / segVelocity.magnitude : 0;

			segVelocity = segVelocity + (player.corpseThrowDirection * Mathf.Abs(1 - player.corpseThrowForce)) * segTime + (Vector3)Physics2D.gravity * segTime;

			RaycastHit2D hit = Physics2D.Raycast(segments[i - 1], (Vector2)segVelocity.normalized, segVelocity.magnitude, LayerMask.NameToLayer("Obstacle"));
			if (hit) 
			{
				if(!hit.collider.gameObject.Equals(player.corpseCarried))
				{
					hitObject = hit.collider;

					segments [i] = segments [i - 1] + segVelocity.normalized * hit.distance;
					segVelocity = segVelocity - (Vector3)Physics2D.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
				}
			} 
			else 
			{
				segments [i] = segments [i - 1] + segVelocity * segTime;
			}
		}

		Color startColor = Color.red;
		Color endColor = Color.blue;
		startColor.a = 1;
		endColor.a = 0;
		trajectoryLine.SetColors (startColor, endColor);

		trajectoryLine.SetVertexCount (segmentCount);
		for (int i = 0; i < segmentCount; i++) 
		{
			trajectoryLine.SetPosition (i, segments[i]);
		}
	}
}
