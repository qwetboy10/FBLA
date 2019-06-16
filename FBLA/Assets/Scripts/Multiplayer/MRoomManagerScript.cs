using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MRoomManagerScript : MonoBehaviour
{
   	public GameObject playerPrefab,smoke;
	public GameObject redPlayerPrefab;
	public Camera maincam;
    public static MRoomManagerScript instance;
    public Vector3 aSpawnLoc,bSpawnLoc;
    private GameObject a, b;
    public Player ascript;
    public Player bscript;
	public Text ablock;
	public Text bblock;
	public Image aTimerImage;
	public Image aTimerProgress;
	public Image bTimerImage;
	public Image bTimerProgress;
	private float time;
	
    // Use this for initialization
    void Start()
    {
        //singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        MUIController.instance.init(DataModel.getDifficulty());
		//make enemy/player
		a = Instantiate(playerPrefab, aSpawnLoc, Quaternion.identity, transform);
        b = Instantiate(redPlayerPrefab, bSpawnLoc, Quaternion.identity, transform);
        ascript = a.GetComponent<Player>();
        bscript = b.GetComponent<Player>();
		//sets direction
		ascript.faceRight();
		bscript.faceLeft();
		//plays music
		MusicController.playMusic();
		//starts new room
		StartCoroutine(startRoom());
	}
	public IEnumerator processWrong(bool timeout)
	{
		//gets rid of ui and display wrong text
		aTimerImage.gameObject.SetActive(false);
		bTimerImage.gameObject.SetActive(false);
		StartCoroutine(MUIController.instance.setTitle (timeout ? "OUT OF TIME":"INCORRECT", .5f));
		DataModel.setQuestion (null);
		changeTurn();
		//get new question
		DataModel.setQuestion(QuestionManager.getRandomQuestion (1+DataModel.getDifficulty ()));
		MUIController.instance.updateUI();
		yield return null;
	}
	public IEnumerator processRight()
	{
		//get rid of ui and display bar
		aTimerImage.gameObject.SetActive(false);
		bTimerImage.gameObject.SetActive(false);
		DataModel.setQuestion(null);
		MUIController.instance.updateUI();
		StartCoroutine(MUIController.instance.setTitle ("CORRECT", .5f));
		yield return StartCoroutine (MUIController.instance.processBar ());

		//calculates damage
		float loc = Mathf.Sin (((float)MUIController.instance.time / GameConstants.framesPerCycle) * 2 * Mathf.PI);
		int dmg = (Mathf.RoundToInt (Mathf.Abs (1 - Mathf.Abs (loc)) * GameConstants.baseDamage)) / (GameConstants.baseDamage / 4);
		Debug.Log(dmg);

		if(DataModel.getTurn()==0)
		{
			//damage enemy
			yield return ascript.attackPlayer(aSpawnLoc,bSpawnLoc,1.5f,bscript,dmg,false);
			ascript.faceRight();
			//if hearts is zero enemy is dead
			if(DataModel.bGetHearts()==0)
			{
				yield return endRoom(1);
			}
		}
		else
		{
			//damage enemy
			yield return bscript.attackPlayer(bSpawnLoc,aSpawnLoc,-1.5f,ascript,dmg,true);
			bscript.faceLeft();
			//if hearts is zero enemy is dead
			if(DataModel.aGetHearts()==0)
			{
				yield return endRoom(0);
			}
		}
		//change turn and set new question
		changeTurn();
		DataModel.setQuestion(QuestionManager.getRandomQuestion (1+DataModel.getDifficulty ()));
		MUIController.instance.updateUI();
		
	}
	void changeTurn()
	{
		//change turn/ yourTurn text/ timer
		if(DataModel.getTurn() == 0) DataModel.setTurn(1);
		else DataModel.setTurn(0);
		if(DataModel.getTurn() == 0)
		{
			MUIController.instance.aTurn.gameObject.SetActive(true);
			MUIController.instance.bTurn.gameObject.SetActive(false);
		}
		if(DataModel.getTurn() == 1)
		{
			MUIController.instance.aTurn.gameObject.SetActive(false);

			MUIController.instance.bTurn.gameObject.SetActive(true);
		}
		resetTime();
	}
	public IEnumerator startRoom()
	{
		//both players enter
		MUIController.instance.fadeIn();
		ascript.MoveFromTo(ascript.leftOffScreen,aSpawnLoc);
		bscript.MoveFromTo(bscript.rightOffScreen,bSpawnLoc);
		//sets up timer
		resetTime();
		if(DataModel.getTimer())
		{
			if(DataModel.getTurn()==0)
        	{
            	aTimerImage.gameObject.SetActive(true);
           		bTimerImage.gameObject.SetActive(false);
	        }
    	    else
        	{
            	bTimerImage.gameObject.SetActive(true);
            	aTimerImage.gameObject.SetActive(false);
        	}
		}
		else
		{
			aTimerImage.gameObject.SetActive(false);
			bTimerImage.gameObject.SetActive(false);
		}
		yield return null;
		if(DataModel.getQuestion()==null) DataModel.setQuestion(QuestionManager.getRandomQuestion (1+DataModel.getDifficulty ()));
		MUIController.instance.updateUI();
	}
	public IEnumerator endRoom(int dead)
	{
		//dead is which player is the dead one
		if(dead==0)
		{
			//a is dead
			a.SetActive(false);
			//instantaite smoke
			GameObject sm = Instantiate(smoke,aSpawnLoc - new Vector3(0,1,0),Quaternion.identity,transform);
			yield return new WaitForSeconds(.45f);
			sm.SetActive(false);
			//player leaves
			yield return bscript.MoveFromTo(bSpawnLoc,bscript.rightOffScreen);
			StartCoroutine(MUIController.instance.setTitle ("PLAYER TWO WINS",.5f));
			yield return MUIController.instance.die();
		}
		else
		{
			//b is dead
			b.SetActive(false);
			//instantates smoke
			GameObject sm = Instantiate(smoke,bSpawnLoc - new Vector3(0,1,0),Quaternion.identity,transform);
			yield return new WaitForSeconds(.45f);
			sm.SetActive(false);
			//player leaves
			StartCoroutine(MUIController.instance.setTitle ("PLAYER ONE WINS", .5f));
			yield return ascript.MoveFromTo(aSpawnLoc,ascript.leftOffScreen);
			yield return MUIController.instance.die();
		}
	}
	public void resetTime()
	{
		//sets timer back to max and switches which is active
		if(DataModel.getTimer())
		{
			if(DataModel.getTurn() == 0)
			{
				bTimerImage.gameObject.SetActive(false);
				aTimerImage.gameObject.SetActive(true);
			}
			else
			{
				aTimerImage.gameObject.SetActive(false);
				bTimerImage.gameObject.SetActive(true);
			}
		}
		
		time = GameConstants.timerLength[DataModel.getDifficulty()];
	}
	void FixedUpdate()
	{
		//decreases timer
		if(aTimerImage.isActiveAndEnabled)
		{
			//FixedUpdate is called 60 times per second so we subtract 1/60 seconds
			time -= (1/60f);
			//sets fill amount of circle
			aTimerProgress.fillAmount = time / GameConstants.timerLength[DataModel.getDifficulty()];
			aTimerImage.GetComponentInChildren<Text>().text = Mathf.RoundToInt(time).ToString();
			if(time<=0)
			{
				//ran out of time
				StartCoroutine(processWrong(true));
				resetTime();
			}
		}
		else if(bTimerImage.isActiveAndEnabled)
		{
			//FixedUpdate is called 60 times per second so we subtract 1/60 seconds
			time -= (1/60f);
			//sets fill amount of circle
			bTimerProgress.fillAmount = time / GameConstants.timerLength[DataModel.getDifficulty()];
			bTimerImage.GetComponentInChildren<Text>().text = Mathf.RoundToInt(time).ToString();
			if(time<=0)
			{
				//ran out of time
				StartCoroutine(processWrong(true));
				resetTime();
			}
		}
	}
}