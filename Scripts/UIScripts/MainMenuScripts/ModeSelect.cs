using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour {
	//PUBLIC SCRIPT REFERENCES
	public GM gm;
    public SoundManager sm;

    //PUBLIC ATTRIBUTES
    public Button classic;
	public Button soulless;
	public Button casual;
	public Button limSwings;
	public Button back;

	public GameObject menu;
	public GameObject levelSel;
	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GM>();
		Button btn = classic.GetComponent<Button> ();
		btn.onClick.AddListener(Classic);

		btn = soulless.GetComponent<Button> ();
		btn.onClick.AddListener (Soulless);

		btn = casual.GetComponent<Button>();
		btn.onClick.AddListener(Casual);

		btn = limSwings.GetComponent<Button>();
		btn.onClick.AddListener(LimSwings);

		btn = back.GetComponent<Button> ();
		btn.onClick.AddListener (GoBack);
	}
	void Classic()
	{
        //gm.resetTimer();
        //gm.mode = GM.Modes.CLASSIC;
        SoundManager.mode = GM.Modes.CLASSIC;
        LoadMuertoLevel();
        levelSel.SetActive (true);
		gameObject.SetActive (false);
	}

	// loads test scene
	void Soulless()
	{
        //gm.resetTimer();
        //gm.mode = GM.Modes.SOULLESS;
        SoundManager.mode = GM.Modes.SOULLESS;
        LoadMuertoLevel();
        levelSel.SetActive (true);
		gameObject.SetActive (false);
	}

	void Casual()
	{
        //gm.resetTimer();
        //SceneManager.LoadScene("Level_3");
        //gm.mode = GM.Modes.ENDLESS;
        SoundManager.mode = GM.Modes.ENDLESS;
        LoadMuertoLevel();
        levelSel.SetActive (true);
		gameObject.SetActive (false);
	}

	void LimSwings()
	{
        //gm.mode = GM.Modes.LIMSWINGS;
        SoundManager.mode = GM.Modes.LIMSWINGS;
        LoadMuertoLevel();
        levelSel.SetActive (true);
		gameObject.SetActive (false);
	}

	//go back to main menu screen
	void GoBack()
	{
		menu.SetActive(true);
		gameObject.SetActive(false);
	}

    void LoadMuertoLevel()
    {
        SceneManager.LoadScene("Muerto_Level 1");
    }
}
