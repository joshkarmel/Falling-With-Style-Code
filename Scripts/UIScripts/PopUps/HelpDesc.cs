using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpDesc : MonoBehaviour {
	public bool active;
	public GameObject helpWindow;
    public GameObject showHelp;

	// Use this for initialization
	void Start () {
		active = false;
		helpWindow.SetActive (active);
        showHelp.SetActive(!active);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			active = !active;
			helpWindow.SetActive (active);
            showHelp.SetActive(!active);
		}
	}
}
