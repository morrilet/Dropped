using UnityEngine;
using System.Collections;

/// <summary>
/// This class will modify the tiling and offset of an object with a tileable material.
/// The result should be a seamless way to tile textures across multiple objects.
/// 
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// //////////////////////////////////////////////// Insanity at its finest. ///////////////////////////////////////////////
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// // So, we'll see how that goes. Good luck and fuck you, future Ethan.												  //
/// //																													  //																
/// // I did it you lonely bitch. As always, I take care of your shit. Fuck yourself and enjoy being trapped in the past. //
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>
public class TileMaterial : MonoBehaviour 
{
	Material mat;
	float tilingFactor = 6f; //I forgot why I use this but I did it in AnimatedMaterial.cs so I guess I'm stuck with it.
	Vector2 OFFSET_ANCHOR = Vector2.zero; //This is used to keep all the tiled objects in the scene using an offset based on a common point in world space.

	//Layer shit because with a meterial sprite sorting order stuff gets wonky. Manual control, bitches!
	public string sortingLayer;
	public int sortingOrder;

	//We can handle just about everything in awake because this is primarily meant for platforms. No update required.
	void Awake()
	{
		//Initializations.
		mat = GetComponent<Renderer> ().material;

		//Set layer shit.
		GetComponent<Renderer> ().sortingLayerName = sortingLayer;
		GetComponent<Renderer> ().sortingOrder = sortingOrder;

		//Set tiling.
		Vector2 texScale = new Vector2 (transform.localScale.x / tilingFactor, transform.localScale.y / tilingFactor);
		mat.SetTextureScale ("_MainTex", texScale);

		//Set offset.
		Vector2 offset = OFFSET_ANCHOR + (Vector2)transform.position;
		mat.SetTextureOffset("_MainTex", offset);
	}
}