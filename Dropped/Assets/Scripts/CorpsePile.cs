using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CorpsePile : MonoBehaviour 
{
	List<Corpse> corpsesInScene;
	public List<Pile> pilesInScene;

	void Update()
	{
		GameObject[] tempCorpseObjectsInScene = GameObject.FindGameObjectsWithTag ("Ragdoll");
		corpsesInScene = new List<Corpse> ();
		for (int i = 0; i < tempCorpseObjectsInScene.Length; i++) 
		{
			corpsesInScene.Add (tempCorpseObjectsInScene[i].GetComponent<Corpse>());
		}
		pilesInScene = GetPiles ();
		pilesInScene = RefinePiles (pilesInScene, 0);

		Debug.Log (pilesInScene.Count);
	}

	List<Pile> GetPiles()
	{
		List<Pile> tempPiles = new List<Pile> ();
		List<Corpse> tempCorpses = new List<Corpse>();

		for (int i = 0; i < corpsesInScene.Count; i++) 
		{
			corpsesInScene[i].touchingCorpses = corpsesInScene [i].GetTouchingCorpses ();

			Pile pile = new Pile ();
			pile.SetCorpsesInPile(corpsesInScene [i].touchingCorpses);
			tempPiles.Add (pile);
		}

		return tempPiles;
		//return RefinePiles (tempPiles, 0);
	}

	List<Pile> RefinePiles(List<Pile> pilesToRefine, int iterationCount)
	{
		bool pilesRefined = true;
		iterationCount += 1;

		/*
		if(pilesToRefine.Count != 0)
		for (int i = pilesToRefine.Count - 1; i > 0; i--) 
		{
			for (int j = pilesToRefine.Count - 1; j > 0; j--) 
			{
				if (!pilesToRefine [i].Equals (pilesToRefine [j])) 
				{
					for (int x = pilesToRefine[i].corpsesInPile.Count - 1; x > 0; x--) 
					{
						for (int y = pilesToRefine[j].corpsesInPile.Count - 1; y > 0; y--) 
						{
							if (pilesToRefine [i].corpsesInPile [x].Equals (pilesToRefine [j].corpsesInPile [y])) 
							{
								pilesToRefine [i].corpsesInPile.AddRange (pilesToRefine [j].corpsesInPile);
								pilesToRefine.Remove (pilesToRefine [j]);
							} 
							else 
							{
								pilesRefined = false;
							}
						}
					}
				}
			}
		}
		*/

		foreach(Pile pile in pilesToRefine.ToArray()) 
		{
			if (pile.GetCorpsesInPile().Count < 2)
			{
				//Debug.Log (pile.corpsesInPile.Count);
				//pilesToRefine.Remove (pile);
			}
		}

		if (pilesToRefine.Count != 0) 
		{
			foreach (Pile pile1 in pilesToRefine.ToArray()) 
			{
				foreach (Pile pile2 in pilesToRefine.ToArray()) 
				{
					if (!pile1.Equals (pile2)) 
					{
						if (ComparePiles (pile1, pile2)) 
						{
							List<Corpse> newPileCorpses = new List<Corpse> ();
							newPileCorpses.AddRange (pile1.GetCorpsesInPile());
							newPileCorpses.AddRange (pile2.GetCorpsesInPile());
							newPileCorpses = newPileCorpses.Where (Corpse => Corpse != null).ToList ();
							newPileCorpses = newPileCorpses.Distinct ().ToList ();

							pile1.SetCorpsesInPile (newPileCorpses);

							//Debug.Log (pile1.GetCorpsesInPile().Count);
							//Debug.Log (pile2.corpsesInPile.Count);

							pilesToRefine.Remove (pile2);
							pilesRefined = false;
						}
					}
				}
			}
		}

		//if (!pilesRefined && iterationCount > 100)
			//return RefinePiles (pilesToRefine, iterationCount);
		//else
			return pilesToRefine;
	}

	bool ComparePiles(Pile pile1, Pile pile2) //Returns true if there are overlapping corpse elements in the piles.
	{
		bool pilesHaveSameCorpse = false;
		foreach (Corpse pile1Ragdoll in pile1.GetCorpsesInPile().ToArray()) 
		{
			foreach (Corpse pile2Ragdoll in pile2.GetCorpsesInPile().ToArray()) 
			{
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
		List<Corpse> corpsesInPile;
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
