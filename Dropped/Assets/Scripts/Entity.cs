using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour 
{
	[HideInInspector]
	public float health;
	public float maxHealth;

	[HideInInspector]
	public bool isAlive;

	public virtual void Awake()
	{
		health = maxHealth;
		isAlive = true;
	}

	public virtual void Update()
	{
		if (health <= 0) 
		{
			isAlive = false;
			health = 0;
		}
		if (health >= maxHealth) 
		{
			health = maxHealth;
		}
	}
}
