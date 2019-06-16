using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class OptionsUIController : MonoBehaviour{

	public Slider musicSlider;
	public Slider effectsSlider;
	public Slider clearSlider;
	public Button clearButton;
	public Button backButton;
	private bool pressed;
	
	// Use this for initialization
	void Start () {
		pressed = false;
		musicSlider.onValueChanged.AddListener(onMusicVolumeUpdate);
		effectsSlider.onValueChanged.AddListener(onEffectsVolumeUpdate);
		backButton.onClick.AddListener(onBackButtonPressed);
		MusicController.updateVolumes();
		musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
		effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume");
		EventTrigger trigger = clearButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry2);
		MusicController.playMusic();
	}
	public void OnPointerDownDelegate(PointerEventData dat) {pressed = true;}
	public void OnPointerUpDelegate(PointerEventData dat) {pressed = false;}
	void onMusicVolumeUpdate(float f)
	{
		MusicController.setMusicVolume(f);
		PlayerPrefs.SetFloat("musicVolume",f);
	}
		
	void onEffectsVolumeUpdate(float f)
	{
		MusicController.setEffectsVolume(f);
		PlayerPrefs.SetFloat("effectsVolume",f);
	}
	
	void onBackButtonPressed()
	{
		PlayerPrefs.Save();
		SceneManager.LoadScene("MainMenu");
	}
	// Update is called once per frame
	void FixedUpdate () {
		if(pressed)
		{
			clearSlider.gameObject.SetActive(true);
			if(clearSlider.value > .98)
			{
				for(int i=0;i<GameConstants.scoreBoardSize;i++) PlayerPrefs.SetString(i+"",new Score("---",0).toString());
				PlayerPrefs.Save();
				clearSlider.value=0;
				pressed=false;
			}
			else clearSlider.value += .01f;
		}
		else
		{
			clearSlider.gameObject.SetActive(false);
		}
	}
}
