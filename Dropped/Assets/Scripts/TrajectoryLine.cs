using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrajectoryLine : MonoBehaviour 
{
	Player player;
	LineRenderer lineRenderer;

	public LayerMask mask;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		lineRenderer = GetComponent<LineRenderer> ();
	}

	void FixedUpdate()
	{
		if (player.throwingCorpse)
			UpdateTrajectory (player.corpseCarried.transform.position, player.corpseThrowDirection * player.corpseThrowForce, Physics2D.gravity);
		else
			lineRenderer.SetVertexCount(0);
	}

	void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
	{
		int numSteps = 100;
		float timeDelta = 1.0f / initialVelocity.magnitude;

		lineRenderer.SetVertexCount (numSteps);

		Color startColor = Color.red;
		Color endColor = Color.blue;
		startColor.a = 1;
		endColor.a = .25f;
		lineRenderer.SetColors (startColor, endColor);

		Vector3 position = initialPosition;
		Vector3 velocity = initialVelocity;
		for (int i = 0; i < numSteps; i++) 
		{
			lineRenderer.SetPosition (i, position);

			float dragForceMagnitude = velocity.magnitude * velocity.magnitude * .0075f;
			Vector3 dragForceVector = dragForceMagnitude * -velocity.normalized;

			velocity += dragForceVector;

			RaycastHit2D hit = Physics2D.Linecast ((Vector2)position, (Vector2)position + (Vector2)velocity * timeDelta + 0.5f * (Vector2)gravity * timeDelta * timeDelta, mask);

			if (hit)
			{
				lineRenderer.SetVertexCount (i + 1);
				//lineRenderer.SetPosition (i, velocity.normalized * hit.distance);
				position += (velocity.normalized * hit.distance) * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
				break;
			}
			else
			{
				position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
				velocity += gravity * timeDelta;
			}
		}
	}
}
