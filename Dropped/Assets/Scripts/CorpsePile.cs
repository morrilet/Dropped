using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CorpsePile : MonoBehaviour 
{
	List<Corpse> corpsesInScene;
	public List<Pile> pilesInScene;

	void FixedUpdate()
	{
		GameObject[] tempCorpseObjectsInScene = GameObject.FindGameObjectsWithTag ("Ragdoll");
		corpsesInScene = new List<Corpse> ();
		for (int i = 0; i < tempCorpseObjectsInScene.Length; i++) 
		{
			corpsesInScene.Add (tempCorpseObjectsInScene[i].GetComponent<Corpse>());
		}

		pilesInScene = GetPiles ();
		//pilesInScene = RefinePiles (pilesInScene, 0);

		Debug.Log (pilesInScene.Count);
	}

	List<Pile> GetPiles()
	{
		List<Pile> tempPiles = new List<Pile> ();

		for (int i = 0; i < corpsesInScene.Count; i++) 
		{
			corpsesInScene[i].touchingCorpses = corpsesInScene [i].GetTouchingCorpses ();

			Pile pile = new Pile ();
			pile.SetCorpsesInPile(corpsesInScene [i].touchingCorpses);
			tempPiles.Add (pile);
		}

		tempPiles = RefinePiles (tempPiles, 0);
		return tempPiles;
		//return RefinePiles (tempPiles, 0);
	}

	List<Pile> RefinePiles(List<Pile> pilesToRefine, int iterationCount)
	{
		bool pilesRefined = true;
		iterationCount += 1;
			
		if (pilesToRefine.Count != 0)
		{
			Pile[] tempPiles = pilesToRefine.ToArray ();
			
			for (int pile1 = 0; pile1 < tempPiles.Length; pile1++) 
			{
				for (int pile2 = 0; pile2 < tempPiles.Length; pile2++) 
				{
					if (!tempPiles[pile1].Equals (tempPiles[pile2])) 
					{
						if (ComparePiles (tempPiles[pile1], tempPiles[pile2])) 
						{
							List<Corpse> newPile1Corpses = new List<Corpse> ();
							newPile1Corpses.AddRange (tempPiles[pile1].GetCorpsesInPile());
							newPile1Corpses = newPile1Corpses.Where (Corpse => Corpse != null).ToList ();
							newPile1Corpses = newPile1Corpses.Distinct ().ToList ();

							List<Corpse> newPile2Corpses = new List<Corpse> ();
							newPile2Corpses.AddRange (tempPiles[pile2].GetCorpsesInPile ());
							newPile2Corpses = newPile2Corpses.Where (Corpse => Corpse != null).ToList ();
							newPile2Corpses = newPile2Corpses.Distinct ().ToList ();

							newPile1Corpses.AddRange (tempPiles[pile2].GetCorpsesInPile());
							//newPile1Corpses = newPile1Corpses.Where (Corpse => Corpse != null).ToList ();
							newPile1Corpses = newPile1Corpses.Distinct ().ToList ();

							tempPiles[pile1].SetCorpsesInPile (newPile1Corpses);

							//Debug.Log (tempPiles[pile1].GetCorpsesInPile().Count + " in pile " + tempPiles[pile1].ToString());
							//Debug.Log (pile2.GetCorpsesInPile().Count);

							pilesToRefine.Remove(tempPiles[pile2]); //remove pile2
							pilesRefined = false;
						}
					}
				}
			}
		}

		if (!pilesRefined)// && iterationCount > 100)
			return RefinePiles (pilesToRefine, iterationCount);
		else
			return pilesToRefine;
	}

	bool ComparePiles(Pile pile1, Pile pile2) //Returns true if there are overlapping corpse elements in the piles.
	{
		bool pilesHaveSameCorpse = false;
		foreach (Corpse pile1Ragdoll in pile1.GetCorpsesInPile()) 
		{
			//pile1Ragdoll.touchingCorpses = pile1Ragdoll.GetTouchingCorpses ();
			foreach (Corpse pile2Ragdoll in pile2.GetCorpsesInPile()) 
			{
				//pile2Ragdoll.touchingCorpses = pile2Ragdoll.GetTouchingCorpses ();
				if (pile1Ragdoll.Equals (pile2Ragdoll))
					pilesHaveSameCorpse = true;
			}
		}
		return pilesHaveSameCorpse;
	}

	void SetPileCollider()
	{
	}

	#region CustomData
	[System.Serializable]
	public struct Pile
	{
		public List<Corpse> corpsesInPile;
		PolygonCollider2D collider;

		public List<Corpse> GetCorpsesInPile()
		{
			return corpsesInPile;
		}

		public void SetCorpsesInPile(List<Corpse> corpses)
		{
			corpsesInPile = corpses;
		}
	}
	#endregion
}
