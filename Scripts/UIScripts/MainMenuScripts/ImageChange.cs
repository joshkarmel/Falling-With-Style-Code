using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  {

	public Image pic;
	public Sprite level;
	public Text desc;

	private Sprite start;
	// Use this for initialization
	void Start () {
		pic = pic.GetComponent<Image> ();
		start = pic.sprite;
		desc = desc.GetComponent<Text> ();
	}
	
	public void OnPointerEnter(PointerEventData data)
	{
		pic.sprite = level;	
		desc.text = "";
	}

	public void OnPointerExit(PointerEventData data)
	{
		pic.sprite = start;
		desc.text = "Level Select";
	}
}
