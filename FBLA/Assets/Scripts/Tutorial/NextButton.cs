using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * NextButton is a class attached to each "Next ->" button of a slide
 * It links the click of a button to either removing the current slide or 
 * displaying the next one
 */
public class NextButton : MonoBehaviour
{
	public bool autoAdvance;
    void Start()
    {
		Button b = GetComponent<Button> ();
		if (autoAdvance)
			b.onClick.AddListener (TutorialUIController.instance.goToNextSlide);
		else
			b.onClick.AddListener (TutorialUIController.instance.nextSlide);
    }
}
