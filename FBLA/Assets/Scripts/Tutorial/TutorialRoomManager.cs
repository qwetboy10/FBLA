using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
/*
 * TutorialRoomManager is a modification of the regular RoomManager.
 * To prevent saving of a tutorial game as well as allow for the
 * constant pausing for info slides, TutorialRoomManager contains
 * a fake DataModel that TutorialUIController also interacts with.
 */
public class TutorialRoomManager : MonoBehaviour
{
	//references to other entities in the game
	public GameObject enemyPrefab, playerPrefab, healthbar,smoke;
	public Camera maincam;
	public static TutorialRoomManager instance;
	public Vector3 playerSpawnLoc,enemySpawnLoc;
	private GameObject player, enemy;
	public Player pscript;
	public Enemy escript;
	public GameObject chest;
	public Text block;

	//fake dataModel variables
	private int enemyHealth, playerHealth, curScore, curPotions, curCoins;
	private bool blocking;
	private Question q;

	//tutorial related objects
	public Button[] exitButtons;
	public GameObject gameOverScreen;
	// Use this for initialization
	IEnumerator Start()
	{
		//singleton
		if (instance != null && instance != this)
			Destroy(this);
		instance = this;
		//wait for UIController to be created
		while (TutorialUIController.instance == null)
			yield return null;

		//make enemy/player
		enemy = Instantiate(enemyPrefab, enemySpawnLoc, Quaternion.identity, transform);
		player = Instantiate(playerPrefab, playerSpawnLoc, Quaternion.identity, transform);
		pscript = player.GetComponent<Player>();
		escript = enemy.GetComponent<Enemy>();
		enemyHealth = escript.getMaxHealth ();
		playerHealth = GameConstants.maxHearts [Difficulty.medium];
		q = new Question ("What is the first letter of FBLA?", new string[] {"F","B","L","A"}, 0, "Tutorial Questions");
		curPotions = 1;
		curCoins = 10;
		TutorialUIController.instance.init(Difficulty.medium);
		MusicController.playMusic();
		StartCoroutine ("StartRoom");
		foreach(Button exitButton in exitButtons) exitButton.onClick.AddListener (goToMM);
		yield return null;
	}
	//starts the room
	public IEnumerator StartRoom() {
		TutorialUIController.instance.fadeIn ();
		yield return pscript.Enter ();
		TutorialUIController.instance.updateUI ();
		yield return new WaitForSeconds (.15f);
		TutorialUIController.instance.loadSlide ();
	}
	//makes the player leave
	public Coroutine Exit()
	{
		return pscript.Exit();
	}
	//handles damage calculations and player attacking sequences
	public IEnumerator Attack() {
		yield return StartCoroutine(TutorialUIController.instance.processBar());
		int dmg = 50;
		yield return StartCoroutine(pscript.Attack(escript.getPlayerAttackLoc(),dmg, damageEnemy, enemyHealth > dmg));
		if(TutorialUIController.instance.slideCnt == 10)
			TutorialUIController.instance.loadSlide ();
	}
	//handles defense sequences and blocking processes
	public IEnumerator Defend()
	{
		StartCoroutine("Block");
		yield return StartCoroutine(escript.Attack());
		//guaranteed easy question for Tutorial, rather than pulled from the question bank
		q = new Question ("How many letters are in the acronym FBLA?", new string[] {"1","2","3","4"}, 3, "Tutorial Questions");
		TutorialUIController.instance.menu = true;
		TutorialUIController.instance.updateUI ();
		//load a slide, as this is after the how to defend portion of the tutorial
		TutorialUIController.instance.loadSlide ();
	}
	//determines whether the user's first space press is within a given time window, and prevents damage if it is
	public IEnumerator Block()
	{
		bool displayMsg = false, blocked = false;
		float cur = 0;
		while(cur <= escript.maxDefendTime)
		{
			//pause the game to let the user see how blocking works
			if (cur >= escript.minDefendTime && !displayMsg) {
				Time.timeScale = 0;
				displayMsg = true;
				TutorialUIController.instance.loadSlide ();
			}
			//process when the user hits space
			if (displayMsg && Input.GetKey (KeyCode.Space) && !blocked) {
				blocked = true;
				Time.timeScale = 1;
				//advance the tutorial
				TutorialUIController.instance.nextSlide ();
				AudioManager.playBlock();
				yield return pscript.block();
			}
			yield return null;
			cur += Time.deltaTime;
		}
	}
	//starts the phasing sequence - this was used before the defend animation existed, and should not be used except for testing.
	public IEnumerator Phase()
	{
		SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
		Color init = sr.color;
		Color trans = sr.color;
		trans.a = .5f;
		sr.color = trans;
		yield return new WaitForSeconds(1);
		sr.color = init;
	}
	//handles the damage enemy process, from updating UI to the dataModel
	public void damageEnemy(int h) {
		enemyHealth -= h;
		curScore += h;
		//start damage showing process
		StartCoroutine(TutorialUIController.instance.showDamage(h));
		//start defend animation
		StartCoroutine (escript.Defend ());
		//update health bar
		healthbar.GetComponent<Slider>().value = (((float) enemyHealth) / escript.getMaxHealth()) * 100;
		if (TutorialUIController.instance.slideCnt == 9)
			TutorialUIController.instance.loadSlide ();
	}
	//handles the damage player process
	public void damagePlayer(int h) {
		//the player can not be hit in tutorial, therefore always block instead of getting damaged
		StartCoroutine(blockText());
		blocking = false;
		return;
	}
	//displays the blocked text
	public IEnumerator blockText()
	{
		block.gameObject.SetActive(true);
		yield return new WaitForSeconds(1);
		block.gameObject.SetActive(false);
	}
	//processes a correct answer, including bringing up the attack bar and determining win status
	public IEnumerator processRight() {
		//remove the question
		q = null;
		TutorialUIController.instance.updateUI ();
		//start attack animation
		yield return StartCoroutine (Attack ());
		yield return new WaitForSeconds (.3f);
		//check if enemy died; if so, then start win process
		if(enemyHealth <= 0) {
			//remove enemy related objects
			healthbar.SetActive (false);
			yield return StartCoroutine (escript.Die ());
			enemy.SetActive (false);
			//spawn smoke for chest
			smoke = Instantiate(smoke, GameConstants.smokeLocation, Quaternion.identity, transform);
			//start win process
			StartCoroutine("win");
			yield return new WaitForSeconds(.45f);
			smoke.SetActive(false);
		}
		yield return null;
	}
	//makes player drink a potion
	public IEnumerator drinkPotion() {
		yield return pscript.drinkPotion ();
		//loads the next slide in the tutorial after the potion explanation
		TutorialUIController.instance.loadSlide ();
	}
	//open chest process
	public IEnumerator openChest()
	{
		chest.SetActive(true);
		//moves player to the chest
		yield return pscript.MoveTo(new Vector3(chest.transform.position.x - GameConstants.chestDelta,player.transform.position.y,player.transform.position.z));
		//plays chest opening after the player reaches the chest
		chest.GetComponent<Animator>().SetTrigger("Open");
		yield return new WaitForSeconds (3f);
	}
	//win process
	public IEnumerator win() {
		//open the chest
		yield return StartCoroutine("openChest");
		//load the slide, but don't display yet
		TutorialUIController.instance.loadSlide ();
		//player leaves
		pscript.Exit ();
		yield return new WaitForSeconds (1f);
		//show the game over screen
		StartCoroutine (TutorialUIController.instance.fadeOutInLoad (gameOverScreen, null));
	}
	//method that, when called, redirects the user to the main menu
	public void goToMM() {
		Time.timeScale = 1;
		SceneManager.LoadScene ("MainMenu");
	}

	//fake dataModel accessor/mutator methods
	public void setHealth(int h) {
		playerHealth = h;
	}
	public void changePotions(int p) {
		curPotions += p;
	}
	public void changeCoins(int c) {
		curCoins += c;
	}
	public int getHealth() {
		return playerHealth;
	}
	public int getPotions() {
		return curPotions;
	}
	public int getCoins() {
		return curCoins;
	}
	public Question getQuestion() {
		return q;
	}
	public int getScore() {
		return curScore;
	}
	public bool answerQuestion(int i) {
		return q.correctIndex == i;
	}
}