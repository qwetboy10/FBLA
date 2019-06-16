using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  
/*
 * ButtonHover is a utility class that enables buttons to have custom
 * images when hovered over. This allows the buttons in game to have
 * our custom designs, rather than those provided by Unity.
 */
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite normal;
    public Sprite hover; 
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.image.sprite = normal;
    }
 	//called when the cursor enters
    public void OnPointerEnter(PointerEventData eventData)
    {
        button.image.sprite = hover; //Or however you do your color
    }
 	//called when the cursor exits
    public void OnPointerExit(PointerEventData eventData)
    {
        button.image.sprite = normal; //Or however you do your color
    }
}
