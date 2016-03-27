using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class JumpAbility : MonoBehaviour 
{
	public float jumpHeight = 4; //I say jump. This is how high.
	public float timeToJumpApex = .4f;  //How long until we reach the peak of our jump.

	[HideInInspector]
	public bool canJump;

	float jumpLeniencyTime = .1f; //When the player falls off an edge, they may still jump for this amount of time.
	float jumpLeniencyCountdownTimer; //Counts down from jumpLeniencyTime to 0.
	[HideInInspector]
	public bool countdownLeniency; //Whether or not the countdown should begin.

	bool isJumpingCurrent;
	bool isJumpingPrevious;

	float jumpVelocity;
	Player player;

	void Start()
	{
		player = GetComponent<Player> ();

		canJump = false;

		isJumpingCurrent  = false;
		isJumpingPrevious = false;

		jumpLeniencyCountdownTimer = jumpLeniencyTime;
		countdownLeniency = false;

		CalculateJumpPhysics ();
	}

	void Update()
	{
		if(player.controller.collisions.below)
			isJumpingCurrent = false;

		//Set PlayerInfo JustJumped state to true if we just jumped.
		if(isJumpingCurrent && !isJumpingPrevious)
			player.playerInfo.JustJumped = true;

		if (countdownLeniency)
			CountdownLeniency ();

		//We can jump if the countdown timer <= 0 because it's reset to jumpLeniencyTime if we're not falling.
		if (jumpLeniencyCountdownTimer <= 0)
			canJump = false;
		else
			canJump = true;

		//Debug.Log (jumpLeniencyCountdownTimer);
		isJumpingPrevious = isJumpingCurrent;
	}

	public void CountdownLeniency()
	{	
		jumpLeniencyCountdownTimer -= Time.deltaTime;
		jumpLeniencyCountdownTimer = Mathf.Clamp(jumpLeniencyCountdownTimer, 0, jumpLeniencyTime);
	}

	public void ResetLeniency()
	{
		jumpLeniencyCountdownTimer = jumpLeniencyTime;
	}

	public void Jump(ref Vector3 velocity)
	{
		countdownLeniency = false;
		jumpLeniencyCountdownTimer = 0;

		isJumpingCurrent = true;

		velocity.y += jumpVelocity;
	}

	//Calculates gravity and jump velocity for a desired jumpHeight and timeToJumpApex.
	public void CalculateJumpPhysics()
	{
		//Based on REAL physics!! #FeaturePorn
		player.gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (player.gravity) * timeToJumpApex;

		print ("Gravity: " + player.gravity + " Jump Velocity: " + jumpVelocity);
	}
}