using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;  
using UnityEngine.UI;


public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     public Color normal;
     public Color hover;
     private Text text;
     public bool toggle;

     void Start()
     {
        text = GetComponent<Text>();
        if(!toggle) text.color = normal;
     }
 
     public void OnPointerEnter(PointerEventData eventData)
     {
        text.color = hover;
     }
 
     public void OnPointerExit(PointerEventData eventData)
     {
        if(!toggle) text.color = normal;
     }
}
