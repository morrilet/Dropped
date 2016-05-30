using UnityEngine;
using System.Collections;

public class Fog : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			StartCoroutine (WaitToKillEntity (other.GetComponent<Entity> (), .75f));
		} 
		else if (other.tag == "Enemy") 
		{
			other.GetComponent<Entity> ().health = 0;
		}
		else if (other.tag == "Corpse") 
		{
			Destroy (other.transform.parent.gameObject, 2.5f);
		}
	}

	IEnumerator WaitToKillEntity(Entity entity, float time)
	{
		yield return new WaitForSeconds (time);
		entity.health = 0f;
	}
}