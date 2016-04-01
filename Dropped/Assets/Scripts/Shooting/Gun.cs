using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{

	//Gun data
	public bool isAuto; //True for full auto, false for semi auto or burst.
	public bool isFlamethrower; //If true, bullets transform relative to parent
	[Range(1, int.MaxValue)]
	public float bulletsPerShot;//Amount of bullets in a shot (for shotguns mostly)
	[Range(1, int.MaxValue)]
	public float shotsPerBurst; //Amount of times Shoot() is called per input (overrriden if isAuto = true)
	public float rotationDeviation;//Innacuracy of gun 
	public GameObject bulletPrefab; //Insert different bullet prefabs here for different guns
	public GameObject muzzleFlashPrefab;//Insert your preffered muzzle flash here
	public float fireRate; //Fire rate of this gun (lower = faster!)
	public float playerKnockBack; //Pushback on player when fired

	public Vector2 bulletOffset;//Controls origin point of bullet

	public Vector2 muzzleFlashOffset;//Controls origin point of muzzle flash

	//Bullet Data
	public float bulletSpeed; //Speed of bullet
	public float bulletSpeedDeviation; //Random variance in projectile speed
	public float maxRange; //Weapon maximum range
	public float damage; //damage of bullet

	float fireRateCount;

	void Start()
	{
		fireRateCount = fireRate;
	}

	void Update ()
	{
		if (isAuto) 
		{ 	//Auto Fire
			if (Input.GetKey (KeyCode.X) && fireRateCount >= fireRate) {
				Shoot (bulletsPerShot);
				fireRateCount = 0;
			}
		}
		if (!isAuto) //Semi Auto Fire
		{
			if (Input.GetKeyDown (KeyCode.X) && fireRateCount >= fireRate)
			{
				Shoot (bulletsPerShot);
				fireRateCount = 0;
			}
		}
		fireRateCount += Time.deltaTime;
	}

	void Shoot(float bullets)
	{
		GameObject muzzleFlash = Instantiate (muzzleFlashPrefab, (Vector3)muzzleFlashOffset + transform.position, transform.rotation) as GameObject;
		muzzleFlash.transform.SetParent (this.transform);

		for (float i = 0; i < bullets; i++)
		{
			Quaternion rotationDeviationBuffer = new Quaternion ();//Buffer to hold bullet rotation with deviation applied before bullet is instantiated

			if(transform.parent.GetComponent<Player>().direction == 1)
				rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-rotationDeviation, rotationDeviation));
			else if(transform.parent.GetComponent<Player>().direction == -1)
				rotationDeviationBuffer.eulerAngles = new Vector3 (0, 0, Random.Range (-rotationDeviation, rotationDeviation) + 180);
			
			GameObject bullet = Instantiate (bulletPrefab, (Vector3)bulletOffset + transform.position, transform.rotation * rotationDeviationBuffer) as GameObject;
			bullet.GetComponent<Bullet> ().bulletSpeed = bulletSpeed;
			bullet.GetComponent<Bullet> ().bulletSpeedDeviation = bulletSpeedDeviation;
			bullet.GetComponent<Bullet> ().maxRange = maxRange;
			bullet.GetComponent<Bullet> ().damage = damage;

			if (isFlamethrower)
				bullet.transform.SetParent (this.transform);

			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), transform.parent.GetComponent<Collider2D> ()); //Bullet will ignore collisions with the gun that instantiated it

			//transform.parent.GetComponent<Player>().velocity += new Vector3(playerKnockBack * direction, 0, 0);
			transform.parent.GetComponent<Player>().controller.Move(new Vector3(playerKnockBack * -transform.parent.GetComponent<Player>().direction, 0, 0));
		}
	}

	void OnDrawGizmos()
	{
		//Draw the muzzleFlashOffset display.
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine (new Vector3(muzzleFlashOffset.x - .2f, muzzleFlashOffset.y, 0) + transform.position, 
			new Vector3(muzzleFlashOffset.x + .2f, muzzleFlashOffset.y, 0) + transform.position); //Horizontal line
		Gizmos.DrawLine (new Vector3(muzzleFlashOffset.x, muzzleFlashOffset.y - .2f, 0) + transform.position, 
			new Vector3(muzzleFlashOffset.x, muzzleFlashOffset.y + .2f, 0) + transform.position); //Vertical line

		//Draw the bulletOffset display.
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (new Vector3(bulletOffset.x - .15f, bulletOffset.y, 0) + transform.position, 
			new Vector3(bulletOffset.x + .15f, bulletOffset.y, 0) + transform.position); //Horizontal line
		Gizmos.DrawLine (new Vector3(bulletOffset.x, bulletOffset.y - .15f, 0) + transform.position, 
			new Vector3(bulletOffset.x, bulletOffset.y + .15f, 0) + transform.position); //Vertical line
	}
}
