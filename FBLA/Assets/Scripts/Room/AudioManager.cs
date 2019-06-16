using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Audio Manager handles the playing of various
 * sounds based off ingame occurences. RoomManager interacts
 * with this to ensure audio effects are timed precisely.
 */
public class AudioManager : MonoBehaviour
{
    //AudioSources for the various sounds
	public AudioSource attack;
    public AudioSource potion;
    public AudioSource error;
    public AudioSource block;
    public AudioSource coin;

	//singleton
    public static AudioManager instance;
    void Start()
    {
        //singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
    }
    
	//play sound effect methods
    public static void playAttack()
    {
    
        instance.attack.Play();
    }
    public static void playPotion()
    {
        instance.potion.Play();
    }
    public static void playError()
    {
        instance.error.Play();
    }
    public static void playBlock()
    {
        instance.block.Play();
    }
    public static void playCoin()
    {
        instance.coin.Play();
    }
   
}
