using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
    public GM gm;

    //public buttons
    public Button reset;
    public Button menu;
    public GameObject targetPos;
    public Vector3 initPos;
    public Text modeText;

    // Use this for initialization
    void Start ()
    {
        gm = FindObjectOfType<GM>();
        reset.onClick.AddListener(buttonReset);
        menu.onClick.AddListener(buttonMenu);
        initPos = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            gameObject.transform.position = initPos;
        }

        if (gameObject.activeSelf)
        {
            gm.LerpUI(gameObject, targetPos.transform.position, 5f, true);
        }

        switch (gm.mode)
        {
            case GM.Modes.CLASSIC:
                modeText.text = "Classic Mode\nCollect and get to the end!";
                break;
            case GM.Modes.ENDLESS:
                modeText.text = "Casual Mode\nNo restrictions and have fun!";
                break;
            case GM.Modes.SOULLESS:
                modeText.text = "Soulless Mode\nAvoid collectibles and get lowest score you can!";
                break;
            case GM.Modes.LIMSWINGS:
                modeText.text = "Limited Mode\nComplete the level in a limited amount of swings!";
                break;
        }
    }

    public void buttonReset()
    {
        gm.ResetScene();
        gameObject.SetActive(false);
    }

    public void buttonMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
