using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
/*
 * MusicController controls volumes for music, as well as
 * when to play various music clips for rooms.
 */
public class MusicController : MonoBehaviour
{
	//audio references for rooms
	public AudioMixer mixer;
	public AudioSource menu;
	public AudioSource boss;
	public AudioSource room;
	//accessor method for mixer
	public AudioMixer GetMixer() {return mixer;}
    private static MusicController instance = null;
    public static MusicController Instance
    {
        get { return instance; }
    }
	//scale a float up to a max value
	public static float scale(float f,float max)
	{
		return (f-.5f) * 2 * max;
	}
	//scale music volume
	public static void setMusicVolume(float f)
	{
		instance.mixer.SetFloat("musicVolume",scale(f,8));
		if(f == 0) instance.mixer.SetFloat("musicVolume",-80f);
	}
	//scale effects volume
	public static void setEffectsVolume(float f)
	{
		instance.mixer.SetFloat("effectsVolume",scale(f,5));
		if(f == 0) instance.mixer.SetFloat("effectsVolume",-80f);
	}
	//update volumes based off option changes
	public static void updateVolumes()
	{
		setEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));
		setMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
	}
	//initialize music controller singleton
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
		//make sure music controller exists between scenes
        DontDestroyOnLoad(this.gameObject);
    }
	//start playing music based off current scene and room
	public static void playMusic()
	{
		Debug.Log("Play");
		string scene = SceneManager.GetActiveScene().name;
		if (scene == "Credits" || scene == "EnterScore" || scene == "Instructions" || scene == "MainMenu" || scene == "Options" || scene == "Scoreboard") {
			//play menu if the scene is non room related
			if (!instance.menu.isPlaying)
				instance.menu.Play ();
			instance.room.Stop ();
			instance.boss.Stop ();
		} else if (scene == "Tutorial") {
			//play basic room music in the tutorial
			instance.boss.Stop ();
			instance.menu.Stop ();
			instance.room.Play ();
		} else if (scene == "Room") {
			//play boss or regular room music, depending on whether you are at the final boss or not
			if (DataModel.getRoomCnt () == GameConstants.enemyCount [DataModel.getDifficulty ()] - 1) {
				instance.boss.Play ();
			
				instance.menu.Stop ();
				instance.room.Stop ();
			} else {
				
				instance.room.Play ();

				instance.boss.Stop ();
				instance.menu.Stop ();
			}
		} else if (scene == "Multiplayer Room") {	
			//play basic room music in multiplayer
			instance.room.Play();

				instance.boss.Stop();
				instance.menu.Stop();
		}
	}
}