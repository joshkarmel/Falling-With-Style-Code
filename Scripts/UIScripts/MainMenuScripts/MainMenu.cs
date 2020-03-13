using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/* USED
 * =================
 * Zachary Lopez
 * 
 * Script for controlling Main Menu Screen including
 * Starting from Level 1
 * Selecting a level by switching scenes
 * Quit and exit application
 */
public class MainMenu : MonoBehaviour {

	public Button start; 
	public Button options;
	public Button quit;
	public Canvas menu;
	public GameObject mode;
	public GameObject optionsScreen;
	// Use this for initialization
	// adding listeners to buttons
	void Start () {
		Button btn = start.GetComponent<Button> ();
		btn.onClick.AddListener (ModeSelect);

		btn = quit.GetComponent<Button> ();
		btn.onClick.AddListener (Quit);

		btn = options.GetComponent<Button> ();
		btn.onClick.AddListener (Options);
	}
	// Start the game, proto scene
	void Options()
	{
		gameObject.SetActive (false);
		optionsScreen.SetActive (true);
	}

	//Quits out application
	void Quit()
	{
		Application.Quit ();
	}

	//seperact scene?
	void ModeSelect()
	{
		gameObject.SetActive (false);
		mode.SetActive(true);

	}
		
}
