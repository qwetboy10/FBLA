using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CreditsUIController : MonoBehaviour {

	public Button backButton;
	// Use this for initialization
	void Start () {
		backButton.onClick.AddListener(onBackButtonPressed);
		MusicController.playMusic();
	}
	void onBackButtonPressed()
	{
		SceneManager.LoadScene("MainMenu");
	}
	// Update is called once per frame
	void Update () {
		
	}
}
