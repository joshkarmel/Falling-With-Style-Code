using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DescChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

	public Text title;
	public Text description;

	public string titleName;
	public string descName;
	// Use this for initialization
	void Start()
	{
		title = title.GetComponent<Text> ();
		description = description.GetComponent<Text> ();
	}
	public void OnPointerEnter(PointerEventData data)
	{
		title.text = titleName;
		description.text = descName;
		Debug.Log ("Moused Over!");
	}

	public void OnPointerExit(PointerEventData data)
	{
		title.text = "";
		description.alignment = TextAnchor.UpperCenter;
		description.text = "Mode Select";
		Debug.Log ("Moused Over!");
	}
		
}