using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class ScoreBoardController : MonoBehaviour
{
    public Text scoreBoardText;
    public Canvas canvas;
    public Button backButton;
    public Text nameColumn, scoreColumn,centerColumn;
    void Start()
    {
        backButton.onClick.AddListener(onBackButtonPressed);
        string[] names = new string[GameConstants.scoreBoardSize];
        string[] scores = new string[GameConstants.scoreBoardSize];
        for(int i=0;i<GameConstants.scoreBoardSize;i++)
        {
            string[] temp = DataModel.getHighScores()[i].toString().Split(new string[] { " --- " }, StringSplitOptions.None);
            names[i] = temp[0];
            scores[i] = temp[1];
        }
        nameColumn.text = names[0];
        scoreColumn.text = scores[0];
        for(int i=1;i<GameConstants.scoreBoardSize;i++)
        {
            Text t = Instantiate(nameColumn,GameConstants.scoreBoardStart,Quaternion.identity,transform);
            t.transform.SetParent(canvas.transform,true);
            t.transform.SetAsFirstSibling();
            t.transform.position = nameColumn.transform.position - new Vector3(0,i*GameConstants.scoreBoardDelta,0);
            t.text = names[i];
            t.transform.localScale = new Vector3(1,1,1);
        }   
        for(int i=1;i<GameConstants.scoreBoardSize;i++)
        {
            Text t = Instantiate(scoreColumn,GameConstants.scoreBoardStart,Quaternion.identity,transform);
            t.transform.SetParent(canvas.transform,true);
            t.transform.SetAsFirstSibling();
            t.transform.position = scoreColumn.transform.position - new Vector3(0,i*GameConstants.scoreBoardDelta,0);
            t.text = scores[i];
            t.transform.localScale = new Vector3(1,1,1);
        }   
        MusicController.playMusic();
    }
    void onBackButtonPressed()
	{
		SceneManager.LoadScene("MainMenu");
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
