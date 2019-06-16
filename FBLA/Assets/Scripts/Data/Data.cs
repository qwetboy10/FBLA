using UnityEngine;
[System.Serializable]
/*
 * Data is a serializable data container.
 * It stores all game related information, including current health and room.
 * DataModel is used to change these values, as well as save and load them.
 * DO NOT CHANGE THESE VALUES UNLESS THEY ARE CHANGED THROUGH DATA MODEL.
 */
public class Data
{
    public int halfHearts;
    public int room;
    public int difficulty;
	public Question q;
    public int enemyHealth;
    public float musicVolume;
    public float effectsVolume;
	public int score;
	public int roomCnt;
    public bool block;
    public int coins;
    public int potions;
    public int background;
    public bool timer;
    public bool multiplayer;
    public int enemyType;
    public int aHalfHearts;
    public int bHalfHearts;
    public int turn;
}