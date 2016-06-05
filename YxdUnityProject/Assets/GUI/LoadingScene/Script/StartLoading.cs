using UnityEngine;
using System.Collections;

public class StartLoading : MonoBehaviour {

	public UISlider Slider;

	void Awake()
	{
		Slider.value = 0;
	}

	public void UpdateSliderProcess(float value)
	{
		Slider.value = value;
	}
}
