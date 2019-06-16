using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class RoomManagerScript : MonoBehaviour
{
	public GameObject[] enemies;
    public GameObject enemyPrefab, playerPrefab, healthbar,smoke;
	public Camera maincam;
    public static RoomManagerScript instance;
    public Vector3 playerSpawnLoc,enemySpawnLoc;
    private GameObject player, enemy;
    public Player pscript;
    public Enemy escript;
	public GameObject chest;
	public Text block;
	public Image timerImage;
	public Image timerProgress;
	private float time;
	
    // Use this for initialization
    void Start()
    {
        //singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;

		//Initialize UIController
        UIController.instance.init(DataModel.getDifficulty());

		try
		{
			//Move to next enemy variety
			if(DataModel.getRoomCnt() >= GameConstants.enemyChange[DataModel.getDifficulty()][DataModel.getEnemyType()])
			{
				DataModel.changeEnemyType(+1);
			}
		}
		catch {}	

		enemyPrefab = enemies[DataModel.getEnemyType()];

		//setup variables
		enemy = Instantiate(enemyPrefab, enemySpawnLoc, Quaternion.identity, transform);
        player = Instantiate(playerPrefab, playerSpawnLoc, Quaternion.identity, transform);
        pscript = player.GetComponent<Player>();
        escript = enemy.GetComponent<Enemy>();
		DataModel.setEnemyHealth (escript.getMaxHealth ());
		DataModel.setBackground(escript.background);

		//play music
		MusicController.playMusic();
		
		//Start
		StartCoroutine ("StartRoom");
	}
	public IEnumerator StartRoom() {
		
		UIController.instance.fadeIn ();

		//Player Enters Room
		yield return pscript.Enter ();

		//Starts the question timer
		resetTime();
		if(DataModel.getTimer()) timerImage.gameObject.SetActive(true);
		else timerImage.gameObject.SetActive(false);

		//Sets question
		DataModel.setQuestion(QuestionManager.getRandomQuestion (DataModel.getEnemyType() + DataModel.getDifficulty ()));
	}
	public Coroutine Exit()
    {
		//Player leaves room
        return pscript.Exit();
    }
	public IEnumerator Attack()
	{
		//Shows attack bar
		yield return StartCoroutine (UIController.instance.processBar ());

		//calculates damage
		float loc = Mathf.Sin (((float)UIController.instance.time / GameConstants.framesPerCycle) * 2 * Mathf.PI);
		int dmg = Mathf.RoundToInt (Mathf.Abs (1 - Mathf.Abs (loc)) * GameConstants.baseDamage);

		//plays attack animation
		yield return StartCoroutine (pscript.Attack (escript.getPlayerAttackLoc (), dmg, damageEnemy, DataModel.getEnemyHealth () > dmg));
	}
	public IEnumerator Defend()
    {
		//Handles blocking
		StartCoroutine("Block");

		//Plays enemy attack animation
        yield return StartCoroutine(escript.Attack());
    }
	public IEnumerator Block()
	{
		//only get one attempt to block
		bool blockAttempt = false;

		//current time
		float cur = 0;
		while(cur <= escript.maxDefendTime)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				
				//right time window and first attempt
				if(cur >= escript.minDefendTime && !blockAttempt)
				{
					//blocks hit
					DataModel.setBlocking(true);
					AudioManager.playBlock();
					yield return pscript.block();
				}
				else blockAttempt = true;
			}
			yield return null;
			cur += Time.deltaTime;
		}
	}
	public IEnumerator Phase()
	{
		//Causes player to turn transparent and back
		//Used for testing
		SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
		Color init = sr.color;
		Color trans = sr.color;
		trans.a = .5f;
		sr.color = trans;
		yield return new WaitForSeconds(1);
		sr.color = init;
	}
	public void damageEnemy(int h) {
		//Hurts enemy / Plays enemy damage taking animation
		DataModel.changeEnemyHealth(-h);
		DataModel.changeScore (h*50*(1+DataModel.getDifficulty()));
		StartCoroutine(UIController.instance.showDamage(h));
		StartCoroutine (escript.Defend ());
		healthbar.GetComponent<Slider>().value = (((float) DataModel.getEnemyHealth()) / escript.getMaxHealth()) * 100;
	}
	public void damagePlayer(int h) {
		//No damage if blocking
		if(DataModel.isBlocking())
		{
			StartCoroutine(blockText());
			DataModel.setBlocking(false);
			return;
		}
		
		//plays getting hurt animation
		DataModel.changeHearts (-h);
		StartCoroutine (pscript.Defend ());
	}
	public IEnumerator blockText()
	{
		//Pops up text for 1 second
		block.gameObject.SetActive(true);
		yield return new WaitForSeconds(1);
		block.gameObject.SetActive(false);
	}
	public IEnumerator processRight() {
		//gets rid of old question
		DataModel.setQuestion (null);
		//stops timer
		timerImage.gameObject.SetActive(false);
		StartCoroutine(UIController.instance.setTitle ("CORRECT", .5f));
		//player attacks
		yield return StartCoroutine(Attack ());
		yield return new WaitForSeconds (.3f);
		if (DataModel.getEnemyHealth () <= 0) {
			//if enemy is dead
			healthbar.SetActive (false);
			yield return StartCoroutine (escript.Die ());
			//gets rid of enemy
			enemy.SetActive (false);
			//creates smoke
			smoke = Instantiate(smoke, GameConstants.smokeLocation, Quaternion.identity, transform);
			//move to win screen
			StartCoroutine("win");
			yield return new WaitForSeconds(.45f);
			smoke.SetActive(false);
			
		} else {
			//restart timer
			resetTime();
			UIController.instance.menu = true;
			//gets new question
			DataModel.setQuestion (QuestionManager.getRandomQuestion (DataModel.getDifficulty ()));
		}
	}
	public void resetTime()
	{
		time = GameConstants.timerLength[DataModel.getDifficulty()];
	}
	public IEnumerator processWrong(bool timeout) {
		Question q = DataModel.getQuestion ();
		//popup text
		StartCoroutine(UIController.instance.setTitle (timeout ? "OUT OF TIME":"INCORRECT", .5f));
		//gets rid of question
		DataModel.setQuestion (null);
		timerImage.gameObject.SetActive(false);
		//if its a timeout then dont give the opportunity to block
		if(!timeout) yield return Defend ();
		else yield return escript.Attack();
		//if player is dead
		if (DataModel.getHearts() <= 0) {
			yield return StartCoroutine (UIController.instance.die ());
		} else {
			//try the question again
			UIController.instance.menu = true;
			DataModel.setQuestion (q);
		}
	}
	public IEnumerator openChest()
	{
		//make chest appear
		
		chest.SetActive(true);
		//player moves to chest
		yield return pscript.MoveTo(new Vector3(chest.transform.position.x - GameConstants.chestDelta,player.transform.position.y,player.transform.position.z));
		//open chest
		chest.GetComponent<Animator>().SetTrigger("Open");
		//add chest rewards
		yield return new WaitForSeconds(.75f);
		if(UnityEngine.Random.Range(0f,1f) < .5)
		{
			DataModel.changeCoins(10);
			StartCoroutine(UIController.instance.getThing("+10 Coins"));
		}
		else
		{
			DataModel.changePotions(1);
			StartCoroutine(UIController.instance.getThing("+1 Potion"));
		}
		yield return new WaitForSeconds(3f);
		//get rid of chest
		chest.SetActive(false);
	}
	public IEnumerator win() {
		//50% chance of getting a chest
		if(Random.Range(0f,1f) < .5) yield return StartCoroutine("openChest");
		//go to next room
		DataModel.changeRoomCnt (1);
		DataModel.changeScore ((1+DataModel.getDifficulty()) * escript.getPointsPerKill()*50);
		DataModel.changeScore(DataModel.getCoins() * 100);
		DataModel.changeScore(DataModel.getPotions()*1000);
		if(DataModel.getRoomCnt() < GameConstants.enemyCount[DataModel.getDifficulty()])
		{
			//move to next room
			yield return StartCoroutine (nextRoom ());
		}
		else { 
			//done with game
			yield return StartCoroutine (UIController.instance.winScreen ());
			yield return new WaitForSeconds (2f);
			gotToHS ();
		}
	}
	public IEnumerator nextRoom() {
		//move to next room
		Coroutine c = pscript.Exit ();
		yield return new WaitForSeconds (3);
		UIController.instance.fadeOut ();
		yield return c;
		goToNextRoom ();
	}	
	public void goToNextRoom() {
		SceneManager.LoadScene("Room");
	}
	public void goToMM() {
		SceneManager.LoadScene ("MainMenu");
	}
	public void gotToHS() {
		SceneManager.LoadScene ("EnterScore");
	}
	void FixedUpdate()
	{
		if(timerImage.isActiveAndEnabled)
		{
			//FixedUpdate is called 60 times per second so we subtract 1/60 seconds
			time -= (1/60f);
			//sets fill amount of circle
			timerProgress.fillAmount = time / GameConstants.timerLength[DataModel.getDifficulty()];
			timerImage.GetComponentInChildren<Text>().text = Mathf.RoundToInt(time).ToString();
			if(time<=0)
			{
				//ran out of time
				StartCoroutine(processWrong(true));
				resetTime();
			}
		}
	}
}