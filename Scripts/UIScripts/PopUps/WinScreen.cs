using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * USED
 * JOSH KARMEL
 * 
 * ATTACHED TO THE WinScreen
*/

public class WinScreen : MonoBehaviour
{

    //PUBLIC SCRIPT REFERENCES
    public GM gm;
    public GrappleController gCtrl;

    //public buttons
    public Button resetButton;
    public Button menuButton;
	public Button nextButton;
    public Text scoreText;
    public GameObject targetPos;
    private Vector3 initPos;

    // Use this for initialization
    void Start()
    {
        gCtrl = FindObjectOfType<GrappleController>(); 
        gm = FindObjectOfType<GM>();
        resetButton.onClick.AddListener(buttonReset);
        menuButton.onClick.AddListener(buttonMenu);
		nextButton.onClick.AddListener (buttonNext);
        scoreText.text = "Score: ";
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            gm.LerpUI(gameObject, targetPos.transform.position, 5f, true);
        }

        if (gm.timer <= 0)
        {
            scoreText.text = "Ran out of time!\nLost Souls: " + gm.colCount.ToString() + "/20   Golden Souls: " + gm.goldColCount.ToString() + "/3\nTime: " + gm.roundedTimer.ToString() + "   Shots Taken: " + gCtrl.shots + "\nTotal Score: " + gm.calculateScore();
        }
        else if (gm.mode == GM.Modes.LIMSWINGS)
        {
            scoreText.text = "Ran out of shots!\nLost Souls: " + gm.colCount.ToString() + "/20   Golden Souls: " + gm.goldColCount.ToString() + "/3\nTime: " + gm.roundedTimer.ToString() + "   Shots Left: " + gCtrl.shots + "\nTotal Score: " + gm.calculateScore();
        }
        else
        {
            scoreText.text = "Lost Souls: " + gm.colCount.ToString() + "/20   Golden Souls: " + gm.goldColCount.ToString() + "/3\nTime: " + gm.roundedTimer.ToString() + "   Shots Taken: " + gCtrl.shots + "\nTotal Score: " + gm.calculateScore();
        }
    }

    public void buttonReset()
    {
        gm.ResetScene();
        gameObject.transform.position = initPos;
    }

    public void buttonMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

	public void buttonNext()
	{
		if (gm.level == GM.Levels.TUTORIAL) 
		{
			SceneManager.LoadScene ("Level_1-pass3", LoadSceneMode.Single);
		} 
		else if (gm.level == GM.Levels.LEVEL1) 
		{
			SceneManager.LoadScene("Level_2-new", LoadSceneMode.Single);
		} 
		else if (gm.level == GM.Levels.LEVEL2) 
		{
			SceneManager.LoadScene("Level_3_pass2", LoadSceneMode.Single);
		} 
		else if (gm.level == GM.Levels.LEVEL3) 
		{
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		}
	}
}
