using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class onMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject player;
	// Use this for initialization
	void Start () {
        //player = GetComponentInChildren<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("this is on i guess");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
