using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/*
 * Console is a development tool that allows forced commands,
 * such as getting items or instantly dying. It is used to test
 * whether many ingame actions occur properly without going through
 * entire runs of the game.
 * 
 * Console is not available in the actual final version of the game
 * as it is a debug tool.
 */

public class Console : MonoBehaviour
{
	//Input text for console commands
    public InputField field;
	//boolean of whether the field is currently focused
    private bool wasFocused;

	//called once a frame
    void Update()
    {
		//check if user hit enter on the field
        if (wasFocused && Input.GetKeyDown(KeyCode.Return))
        {
			//attempt to use the current command
            try {
                Parse(field.text);
            } catch (ArgumentException) {
				//in case of an invalid argument, log it
                Debug.Log("Bad Arguments");
            }
			//reset the field
            field.text = "";
        }
		//update wasFocused
        wasFocused = field.isFocused;
    }

	//handles parsing of commands
    void Parse(string text)
    {
		//lowercase the command in case of capital letters
        text = text.ToLower();
        if (text.Equals("attack")) {
			//force an attack sequence
            RoomManagerScript.instance.Attack();
        } else if (text.Equals("defend")) {
			//force a defend sequence
            RoomManagerScript.instance.Defend();
        } else if (text.Equals("enter")) {
			//force an enter sequence - no longer exists
            //RoomManagerScript.instance.Enter();
        } else if (text.Equals("exit")) {
			//force an exit sequence
            StartCoroutine(RoomManagerScript.instance.nextRoom());
        } else if (text.StartsWith("question")) {
			//get random question if text is "question"
            if (text.Equals("question"))
                DataModel.setQuestion(QuestionManager.getRandomQuestion(DataModel.getDifficulty()));
			//log all question categories if text is "question list"
            else if (text.Equals("question list"))
                Debug.Log(String.Join(",", QuestionManager.getCategories().ToArray()));
			//log a question of a certain category if text is "question _____", where ____ is the category
            else
                Debug.Log(QuestionManager.getQuestionByCategory(text.Substring(9), 0));
        } else if (text.StartsWith("hearts")) {
			//increase health by 1
            if (text.EndsWith("+"))
                DataModel.changeHearts(1);
			//decrease health by 1
            else if (text.EndsWith("-"))
                DataModel.changeHearts(-1);
			//set health to given value
            else if (text.Contains("="))
                DataModel.setHearts(Int32.Parse(text.Substring(text.IndexOf("=") + 1)));
			//invalid command, throw ArgumentException
            else
                throw new ArgumentException();
        } else if (text.Equals("save")) {
			//force the game to save
            DataModel.saveGame();
        } else if (text.Equals("load")) {
			//force the game to load
            DataModel.loadGame();
        } else if (text.Equals("difficulty")) {
			//log current difficulty
            Debug.Log(DataModel.getDifficulty());
        } else if (text.Equals("damage")) {
			//play damage animations
            UIController.instance.takeDamage();
        }
        else if (text.StartsWith("play")) {
			if (text.EndsWith ("effects")) {
				//play sound effects
				(GameObject.Find ("RoomEffectsSource").GetComponent<AudioSource> ()).Play ();
			} else if (text.EndsWith ("music")) {
				//play music
				(GameObject.Find ("RoomMusicSource").GetComponent<AudioSource> ()).Play ();
			} else {
				//invalid command, throw ArgumentException
				throw new ArgumentException ();
			}
        }
        else if(text.StartsWith("score")) {
            if(text.Equals("score list")) {
				//print all high scores
                foreach(Score s in DataModel.getHighScores()) {
                    Debug.Log(s.toString());
                }

            }
            if(text.StartsWith("score add"))
            {
				//add a new high score
                string[] k = text.Split(' ');
                DataModel.addHighScore(k[2],int.Parse(k[3]));
            }
            if(text.Equals("score fill"))
            {
				//fill scoreboard with random scores
                for(int i=0;i<15;i++) DataModel.addHighScore(Char.ToString((char)(i+65))+Char.ToString((char)(i+65))+Char.ToString((char)(i+65))+Char.ToString((char)(i+65))+Char.ToString((char)(i+65))+Char.ToString((char)(i+65)),UnityEngine.Random.Range(0,1000));
            }
        }
        else if(text.Equals("kill")) {
			//simulate a "correct answer"
           StartCoroutine(RoomManagerScript.instance.processRight());
        }
        else if(text.Equals("stuff"))
        {
			//gives user potions and coins
            DataModel.changeCoins(30);
            DataModel.changePotions(3);
        }
        else if(text.Equals("forest"))
		{
			//sets background to forest
            DataModel.setBackground(0);
        }
        else if(text.Equals("dungeon"))
		{
			//sets background to dungeon
            DataModel.setBackground(1);
        }
        else if (text.Equals("test"))
        {
			//tests if parse is being called properly; only function is to not throw an ArgumentException
        }
		//invalid command, throw ArgumentException
        else
            throw new ArgumentException();
    }
}
