using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * DataModel handles all data interactions in the game.
 * It uses the Data class to store current game state information,
 * such as current health and room.
 */

//static class allows DataModel methods to be easily accessed
public static class DataModel
{
	//data for the current game
    static Data current;
    public static void init()
    {
        //starts at room 0
        current = new Data();
    }
	//returns current difficulty
    public static int getDifficulty()
    {
        return current.difficulty;
    }
	//sets current difficulty
    public static void setDifficulty(int d)
    {
        current.difficulty = d;
		//change hearts based on difficulty
        if(d == Difficulty.easy) setHearts(10);
		else if(d == Difficulty.medium) setHearts(8);
		else if(d == Difficulty.hard) setHearts(6); 
    }
	//returns current room
    public static int getRoomNumber()
    {
        return current.room;
    }
	//sets current room
    public static void setRoomNumber(int rn)
    {
        current.room = rn;
    }
	//get current health
    public static int getHearts()
    {
        return current.halfHearts;
    }
	//set current health
    public static int setHearts(int newHearts)
    {
        int temp = current.halfHearts;
        current.halfHearts = newHearts;
		//make sure we dont 
        if (current.halfHearts < 0)
            current.halfHearts = 0;
        if (current.halfHearts > 10)
            current.halfHearts = 10;
		//update UI if you are in a game
        if (SceneManager.GetActiveScene().name == "Room")
            UIController.instance.updateUI();
        return temp;
    }
	//changes hearts by delta
    public static void changeHearts(int delta)
    {
        setHearts(getHearts() + delta);
    }
	//get the current question
    public static Question getQuestion()
    {
        //Debug.Log(current);
        //Debug.Log(current.q);
        return current.q;
    }
	//set the current qustion
    public static void setQuestion(Question q)
    {
        if(current.q != null & q == null) current.q.reset();
		current.q = q;
		//update Room to display the new question
		if (SceneManager.GetActiveScene().name == "Room")
			UIController.instance.updateUI();
    }
	//returns if the answer choice is the correct one
    public static Boolean answerQuestion(int a)
    {
        return DataModel.getQuestion().correctIndex == a;
    }
	//accessor/mutator methods for enemy health, score, and room count
    public static int getEnemyHealth() { return current.enemyHealth; }
	public static void setEnemyHealth(int i) { current.enemyHealth = i; }
	public static void changeEnemyHealth(int delta) { setEnemyHealth(getEnemyHealth() + delta); }
	public static int getScore() { return current.score; }
	public static void setScore(int i) {
		current.score = i; 
		if (SceneManager.GetActiveScene().name == "Room")
			UIController.instance.updateUI();
	}
	public static void changeScore(int delta) { setScore(getScore() + delta); }
	public static int getRoomCnt() { return current.roomCnt; }
	public static void setRoomCnt(int i) { current.roomCnt = i; }
	public static void changeRoomCnt(int delta) { setRoomCnt(getRoomCnt() + delta); }
	//save and load games by writing to files using the serializability of Data
    public static void saveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, current);
        file.Close();
        Debug.Log("Game saved to" +
        Application.persistentDataPath + "/gamesave.save");
    }
    public static bool loadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Data data = (Data)bf.Deserialize(file);
            file.Close();
            current = data;
            if (SceneManager.GetActiveScene().name == "Room")
                UIController.instance.updateUI();
            return true;

        }
        else
        {	
			//log that loading failed
            Debug.Log("No Save File Found");
            return false;
        }
    }
	//returns the high scores
    public static Score[] getHighScores() {
        Score[] ret = new Score[GameConstants.scoreBoardSize];
        for(int i=0;i<GameConstants.scoreBoardSize;i++)
        {
            ret[i] = new Score(PlayerPrefs.GetString(i+""));
        }
        return ret;
    }
	//add a high score to current scores
    public static void addHighScore(Score s)
    {
        Debug.Log("High score added " + s.toString());
        Score[] k = new Score[GameConstants.scoreBoardSize+1];
        for(int i=0;i<k.Length-1;i++) k[i] = new Score(PlayerPrefs.GetString(i+""));
        k[k.Length-1] = s;
        Array.Sort(k);
        for(int i=0;i<k.Length-1;i++) PlayerPrefs.SetString(""+i,k[i].toString());
    }
	//utility method that turns a name and a score into a Score class
    public static void addHighScore(String name,int score)
    {
        addHighScore(new Score(name,score));
    }
	//methods for getting currently blocking status
    public static bool isBlocking()
    {
        return current.block;
    }
    public static void setBlocking(bool b)
    {
        current.block = b;
    }
	//accessor/mutator methods for coins, potions, backgrounds, timer status, and enemy info
    public static int getCoins()
    {
        return current.coins;
    }
    public static void setCoins(int i)
    {
        current.coins = i;
		//update UI
         UIController.instance.updateUI();
    }
    public static void changeCoins(int d)
    {
        DataModel.setCoins(DataModel.getCoins()+d);
		//play sound effects
        AudioManager.playCoin();
        
    }
    public static int getPotions()
    {
        return current.potions;
    }
    public static void setPotions(int i)
    {
		current.potions = i;
		//update UI
        UIController.instance.updateUI();
    }
    public static void changePotions(int d)
    {
        DataModel.setPotions(DataModel.getPotions()+d);
    }
    public static void setBackground(int d)
    {
		current.background = d;
		//update UI
        UIController.instance.updateUI();
    }
    public static int getBackground()
    {
        return current.background;
    }
    public static bool getTimer()
    {
        return current.timer;
    }
    public static void setTimer(bool b)
    {
        current.timer = b;
    }
    public static bool getMultiplayer()
    {
        return current.multiplayer;
    }
    public static void setMultiplayer(bool b)
    {
        current.multiplayer = b;
    }
    public static int getEnemyType()
    {
        return current.enemyType;
    }
    public static void setEnemyType(int i)
    {
        current.enemyType = i;
    }
    public static void changeEnemyType(int d)
    {
        setEnemyType(getEnemyType()+d);
    }
     public static int aGetHearts()
    {
        return current.aHalfHearts;
    }

	//accessor/mutator methods for multiplayer to account for 2 players
    public static int aSetHearts(int newHearts)
    {
        int temp = current.aHalfHearts;
        current.aHalfHearts = newHearts;
        if (current.aHalfHearts < 0)
            current.aHalfHearts = 0;
        if (current.aHalfHearts > 10)
            current.aHalfHearts = 10;
        if (SceneManager.GetActiveScene().name == "Multiplayer Room")
            MUIController.instance.updateUI();
        return temp;
    }
    public static void aChangeHearts(int delta)
    {
        aSetHearts(aGetHearts() + delta);
    }
     public static int bGetHearts()
    {
        return current.bHalfHearts;
    }

    public static int bSetHearts(int newHearts)
    {
        int temp = current.bHalfHearts;
        current.bHalfHearts = newHearts;
        if (current.bHalfHearts < 0)
            current.bHalfHearts = 0;
        if (current.bHalfHearts > 10)
            current.bHalfHearts = 10;
        if (SceneManager.GetActiveScene().name == "Multiplayer Room")
            MUIController.instance.updateUI();
        return temp;
    }
    public static void bChangeHearts(int delta)
    {
        bSetHearts(bGetHearts() + delta);
    }
    public static int getTurn()
    {
        return current.turn;
    }
    public static void setTurn(int t)
    {
        current.turn = t;
    }
}
