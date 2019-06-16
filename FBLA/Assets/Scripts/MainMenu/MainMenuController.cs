using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
	Main menu controller handles the UI on the main page while also setting up data structures to be used in future scenes
 */
public class MainMenuController : MonoBehaviour {

	public Button loadNewGameButton, credits, options, exit,scoreboard,instructions,newgame,loadgame,timer,easy,medium,hard,multi,back;
	public GameObject mm,sm;
	public Color untoggled;
	public Color toggled;
	public int difficulty;
	public AudioSource errorSound;
	// Use this for initialization
	void Start () {
		//initialize data structures
		QuestionManager.init ();
		DataModel.init();
		DataModel.setDifficulty(difficulty);
		//sets up button listeners
		newgame.onClick.AddListener(startNewGame);
		exit.onClick.AddListener (Application.Quit);
		credits.onClick.AddListener(loadCredits);
		options.onClick.AddListener(loadOptions);
		instructions.onClick.AddListener(loadTutorial);
		scoreboard.onClick.AddListener(loadScores);
		MusicController.updateVolumes();
		//fills scoreboard with empty values
		initializeScores();
		easy.onClick.AddListener(easyB);
		medium.onClick.AddListener(mediumB);
		hard.onClick.AddListener(hardB);
		loadgame.onClick.AddListener(load);
		timer.onClick.AddListener(toggleTimer);
		loadNewGameButton.onClick.AddListener(toggleMenu);
		multi.onClick.AddListener(toggleMulti);
		MusicController.playMusic();
		back.onClick.AddListener(toggleBack);
	}
	void toggleMulti()
	{
		//inverts multiplayer option
		DataModel.setMultiplayer(!DataModel.getMultiplayer());
		//switches color
		if(DataModel.getMultiplayer())
		{
			multi.GetComponentInChildren<HoverHandler>().toggle = true;
			multi.GetComponentInChildren<Text>().color = toggled;
		}
		else
		{
			multi.GetComponentInChildren<HoverHandler>().toggle = false;
			multi.GetComponentInChildren<Text>().color = untoggled;
		}
	}
	void toggleMenu()
	{
		//sets main menu inactive and loads submenu
		mm.SetActive(false);
		sm.SetActive(true);
		updateDifficulty(0);
		timer.GetComponentInChildren<Text>().color = untoggled;
		multi.GetComponentInChildren<Text>().color = untoggled;
	}
	void toggleBack()
	{
		//reverts to main menu
		mm.SetActive(true);
		sm.SetActive(false);
	}
	void toggleTimer()
	{
		//switches timer on and off
		DataModel.setTimer(!DataModel.getTimer());
		if(DataModel.getTimer())
		{
			timer.GetComponentInChildren<HoverHandler>().toggle = true;
			timer.GetComponentInChildren<Text>().color = toggled;
		}
		else
		{
			timer.GetComponentInChildren<HoverHandler>().toggle = false;
			timer.GetComponentInChildren<Text>().color = untoggled;
		}
	}
	void initializeScores()
	{
		//fills scoreboard with empty scores
		for(int i=0;i<GameConstants.scoreBoardSize;i++) if(!PlayerPrefs.HasKey(i+"")) PlayerPrefs.SetString(i+"",new Score("---",0).toString());
		PlayerPrefs.Save();
	}
	void easyB() {updateDifficulty(0);}
	void mediumB() {updateDifficulty(1);}
	void hardB() {updateDifficulty(2);}
	void updateDifficulty(int dif)
	{
		//untoggles all options
		easy.GetComponentInChildren<Text>().color = untoggled;
		medium.GetComponentInChildren<Text>().color = untoggled;
		hard.GetComponentInChildren<Text>().color = untoggled;
		easy.GetComponentInChildren<HoverHandler>().toggle = false;
		medium.GetComponentInChildren<HoverHandler>().toggle = false;
		hard.GetComponentInChildren<HoverHandler>().toggle = false;
		//toggle correct option
		if(dif==0)
		{
			
			easy.GetComponentInChildren<HoverHandler>().toggle = true;
			easy.GetComponentInChildren<Text>().color = toggled;
			
		}
		if(dif==1)
		{
			medium.GetComponentInChildren<Text>().color = toggled;
			medium.GetComponentInChildren<HoverHandler>().toggle = true;			
		}
		if(dif==2)
		{
			hard.GetComponentInChildren<Text>().color = toggled;
			hard.GetComponentInChildren<HoverHandler>().toggle = true;
		}
		difficulty = dif;
		DataModel.setDifficulty(difficulty);
	}
	void load()
	{
		//load data
		bool b = DataModel.loadGame();
		//data present
		if(b)
		{
			if(DataModel.getMultiplayer()) SceneManager.LoadScene("Multiplayer Room");
			else SceneManager.LoadScene("Room");
			Debug.Log("Loading Game");
		}
		//no data
		else
		{
			StartCoroutine(flashLoadRed());
			errorSound.Play();
		}
	}
	IEnumerator flashLoadRed()
	{
		//turns load text red briefly
		Text temp = loadgame.gameObject.GetComponentInChildren<Text>();
		Color original = temp.color;
		temp.color = Color.red;
		yield return new WaitForSeconds(1f);
		temp.color = original;
	}
	void startNewGame() {
		//loads new game
		if(DataModel.getMultiplayer()) SceneManager.LoadScene("Multiplayer Room");
		else SceneManager.LoadScene("Room");
		Debug.Log("Starting New Game");
	}
	void loadCredits() {
		//loads credit scene
		SceneManager.LoadScene("Credits");
	}
	void loadOptions() {
		//loads options scene
		SceneManager.LoadScene("Options");
	}
	void loadTutorial() {
		//loads tutorial scene
		SceneManager.LoadScene("Tutorial");
	}
	void loadScores() {
		//loads scoreboard
		SceneManager.LoadScene("Scoreboard");
	}
}
