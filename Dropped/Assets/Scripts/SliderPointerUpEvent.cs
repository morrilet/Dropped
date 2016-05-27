using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SliderPointerUpEvent : MonoBehaviour, IPointerUpHandler
{
	public bool pointerUp;

	void Start()
	{
		pointerUp = false;
	}

	void Update()
	{
		if (pointerUp)
			pointerUp = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pointerUp = true;
	}
}
