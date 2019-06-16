using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	public Button loadNewGameButton, credits, options, exit,scoreboard,quit;
	public Dropdown difficultySelect;
	public Button instructions;
	public int difficulty = Difficulty.easy;
	// Use this for initialization
	void Start () {
		QuestionManager.init ();
		DataModel.init();
		DataModel.setDifficulty(difficulty);
		loadNewGameButton.onClick.AddListener(startNewGame);
		difficultySelect.onValueChanged.AddListener(updateDifficulty);
		exit.onClick.AddListener (Application.Quit);
		credits.onClick.AddListener(loadCredits);
		options.onClick.AddListener(loadOptions);
		instructions.onClick.AddListener(loadTutorial);
		scoreboard.onClick.AddListener(loadScores);
		MusicController.updateVolumes();
		quit.onClick.AddListener(quitGame);
		initializeScores();
	}
	void quitGame()
	{
		Application.Quit();
	}
	void initializeScores()
	{
		for(int i=0;i<GameConstants.scoreBoardSize;i++) if(!PlayerPrefs.HasKey(i+"")) PlayerPrefs.SetString(i+"",new Score("---",0).toString());
		PlayerPrefs.Save();
	}
	void updateDifficulty(int dif)
	{
		difficulty = dif;
		DataModel.setDifficulty(difficulty);
		Debug.Log(DataModel.getDifficulty());
	}
	void startNewGame() {
		SceneManager.LoadScene("Room");
		Debug.Log("Starting New Game");
	}
	void loadCredits() {
		SceneManager.LoadScene("Credits");
	}
	void loadOptions() {
		SceneManager.LoadScene("Options");
	}
	void loadTutorial() {
		SceneManager.LoadScene("Tutorial");
	}
	void loadScores() {
		SceneManager.LoadScene("Scoreboard");
	}
}
