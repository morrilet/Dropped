using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour 
{
	[HideInInspector]
	public int health;
	public int maxHealth;

	[HideInInspector]
	public bool isAlive;

	public virtual void Start()
	{
		health = maxHealth;
		isAlive = true;
	}

	public virtual void Update()
	{
		if (health <= 0)
			isAlive = false;
	}
}
