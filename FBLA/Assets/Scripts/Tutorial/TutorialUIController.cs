using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * TutorialUIController is a modified version of UIController.
 * The main differences come in the form of slide functions, which
 * appear throughout the code when various slides should occur.
 */
public class TutorialUIController : MonoBehaviour
{
	//singleton
	public static TutorialUIController instance;
	//heart images and objects
	public Sprite full;
	public Sprite half;
	private GameObject[] hearts;
	//room UI objects
	public Text questionText, scoreText, titleText;
	public Button a, b, c, d;
	public Button[] buttons;
	public Text category;
	//overlay variables
	public Color fadeColor;
	public Color damageColor;
	public Image overlayImage;
	//pause menu variables
	private bool paused;
	public GameObject pauseMenu;
	public GameObject escapeProg;
	//hitMarker variables for attacking
	public Image hitmarker;
	public Image hitmarkerBackground;
	private float hitmarkerVelocity;
	public int time;
	private bool pressedSpace;
	//damage variables
	public Text damageText;
	public Color damageTextColor;
	//in battle menu boolean
	public bool menu;
	//coin & potion text
	public Text coinText;
	public Text potionText;
	//back button on menu
	public Button back;
	//Tutorial variables, such as slide references
	public GameObject gameMenu, multi;
	public int slideCnt;
	private bool loaded;
	public GameObject[] slides;	
	void Start()
	{
		//singleton
		if (instance != null && instance != this)
			Destroy(this);
		instance = this;
		slideCnt = 0;
		hitmarkerVelocity = GameConstants.hitmarkerInitialVelocity * Screen.width / 1000f;
		hitmarker.transform.position = new Vector3(Screen.width / 2, hitmarker.transform.position.y, 0);
		//allow back button to return to battle menu
		back.onClick.AddListener(onBack);
	}

	void onBack()
	{
		//only allow battlemenu to return on slide 15, where you use the back button
		if (slideCnt == 14) {
			goToNextSlide ();
			menu = true;
			instance.updateUI ();
		}
	}
	//loads the attack bar
	public IEnumerator processBar()
	{
		hitmarker.gameObject.SetActive(true);
		hitmarkerBackground.gameObject.SetActive(true);
		while(!pressedSpace)
		{
			yield return null;
		}
		//loads the next slide if the current slide was the attack bar explanation
		if(slideCnt == 8) nextSlide ();
		pressedSpace=false;
		hitmarker.gameObject.SetActive(false);
		hitmarkerBackground.gameObject.SetActive(false);
	}
	//fades the overlay from a color to another at a given rate
	IEnumerator fadeFromTo(Color from, Color to, float end, float delta)
	{
		overlayImage.color = from;
		float cur = 0;
		while (cur < end)
		{
			//bring the overlayImage closer to the final value
			overlayImage.color = Color.Lerp(from, to, cur);
			cur = Mathf.Min(end, cur + delta * Time.deltaTime);
			yield return null;
		}

	}
	//initializes hearts based off current difficulty, as well as assigns click methods for buttons
	public void init(int diff)
	{
		hearts = new GameObject[5];
		for (int i = 0; i < 5; i++)
		{
			hearts[i] = GameObject.Find("Heart " + (i+1));
		}
		a.onClick.AddListener(A);
		b.onClick.AddListener(B);
		c.onClick.AddListener(C);
		d.onClick.AddListener(D);
		buttons = new Button[] { a, b, c, d };
		menu = true;
		updateUI ();
	}
	//returns to main menu
	public void exit() { StartCoroutine(backToMM()); }
	//utility methods that call pressButton with the button's index
	public void A() { pressButton(0); }
	public void B() { pressButton(1); }
	public void C() { pressButton(2); }
	public void D() { pressButton(3); }

	//handles pressing a button
	public void pressButton(int b)
	{
		//check if the menu is currently open
		if(menu)
		{
			//attack button
			if(b==0 && (slideCnt == 6 || slideCnt == 13) && loaded)
			{
				goToNextSlide ();
				menu = false;
				updateUI();
			}
			//drink a potion button
			if(b==1 && slideCnt == 12 && loaded)
			{
				nextSlide ();
				//update dataModel
				if(TutorialRoomManager.instance.getPotions() >=1)
				{
					TutorialRoomManager.instance.changePotions(-1);
					TutorialRoomManager.instance.setHealth(GameConstants.maxHearts[Difficulty.medium]);
					updateUI();
					AudioManager.playPotion();
					StartCoroutine(TutorialRoomManager.instance.drinkPotion());
				}
				else
				{
					//play error if you have 0 potions
					AudioManager.playError();
				}
			}
			//use a hint
			if(b==2 && slideCnt == 15 && loaded)
			{
				nextSlide ();
				if(TutorialRoomManager.instance.getCoins()>=10)
				{
					TutorialRoomManager.instance.changeCoins(-10);
					TutorialRoomManager.instance.getQuestion().hint();
					menu = false;
					updateUI();
				}
				else
				{
					//error sound if not enough coins
					AudioManager.playError();
				}
			}
		}
		else
		{
			//Only allow A to be the correct answer for the first question
			if(b == 0 && slideCnt == 7 && loaded)
				goToNextSlide ();
			if (b >= TutorialRoomManager.instance.getQuestion().answers.Length)
				return;
			Boolean correct = TutorialRoomManager.instance.answerQuestion(b);
			Debug.Log("Answer is " + (correct ? "correct" : "wrong"));
			if (correct)
				StartCoroutine(TutorialRoomManager.instance.processRight());
			//no punishment for wrong answers, i.e. you have to get them right
		}

	}
	//displays damage dealt
	public IEnumerator showDamage(int i)
	{
		damageText.gameObject.SetActive(true);
		damageText.text = i.ToString();
		damageText.color = damageTextColor;
		Color final = damageTextColor;
		final.a = 0f;
		float cur = 0f;
		//fade out the text
		while (cur < GameConstants.damageNumberTime)
		{
			damageText.color = Color.Lerp(damageTextColor, final, cur);
			cur = Mathf.Min(GameConstants.damageNumberTime, cur + GameConstants.damageNumberFadeSpeed * Time.deltaTime);
			yield return null;
		}
		damageText.gameObject.SetActive(false);
	}
	//returns to main menu
	public IEnumerator backToMM()
	{
		yield return StartCoroutine("gameOverFade");
		TutorialRoomManager.instance.goToMM();
	}
	//handles fading for damage taken
	public Coroutine takeDamage()
	{
		return StartCoroutine(fadeFromTo(damageColor, new Color(0, 0, 0, 0), 1f, 2f));
	}
	//handles room fade out
	public Coroutine fadeOut()
	{
		return StartCoroutine(fadeFromTo(new Color(0, 0, 0, 0), fadeColor, 1f, .5f));
	}
	//handles room fade in
	public Coroutine fadeIn()
	{
		return StartCoroutine(fadeFromTo(fadeColor, new Color(0, 0, 0, 0), 1f, .5f));
	}
	//sets the room title and fades out
	public IEnumerator setTitle(String s, float wait)
	{
		titleText.text = s;
		float cur = 0, end = 1;
		Color transparent = new Color(255, 255, 255, 0), opaque = new Color(255, 255, 255, 1);
		titleText.color = opaque;
		yield return new WaitForSeconds(wait);
		while (cur < end)
		{
			titleText.color = Color.Lerp(opaque, transparent, cur);
			cur = Mathf.Min(end, cur + .7f * Time.deltaTime);
			yield return null;
		}
	}
	//updates all UI Components
	public void updateUI()
	{
		updateHearts();
		updateQuestion();
		updateAnswers();
		updateScore();
		updateCH();
	}
	//updates coins and potions
	public void updateCH()
	{
		coinText.text = TutorialRoomManager.instance.getCoins().ToString("00");
		potionText.text = TutorialRoomManager.instance.getPotions().ToString("00");
	}
	//updates current score
	public void updateScore()
	{
		scoreText.text = TutorialRoomManager.instance.getScore().ToString("D6");
	}
	//update current hearts
	public void updateHearts()
	{
		int halfHearts = TutorialRoomManager.instance.getHealth();
		for (int i = 0; i < hearts.Length * 2; i += 2)
		{
			hearts[i / 2].GetComponent<Image>().sprite = full;
			if (i < halfHearts)
			{
				Image im = hearts[i / 2].GetComponent<Image>();
				im.color = new Color(im.color.r, im.color.g, im.color.b, 1);
			}
			else
			{
				Image im = hearts[i / 2].GetComponent<Image>();
				im.color = new Color(im.color.r, im.color.g, im.color.b, 0);
			}
		}
		if (halfHearts % 2 == 1)
		{
			Image im = hearts[halfHearts / 2].GetComponent<Image>();
			im.sprite = half;
		}
	}
	//update current question
	public void updateQuestion()
	{
		Question q = TutorialRoomManager.instance.getQuestion();
		questionText.text = q == null ? "" : q.question;
		category.text = q == null ? "" : q.category;
	}
	//update answer choice buttons
	public void updateAnswers()
	{
		if(!menu)
		{
			int i;
			for (i = 0; TutorialRoomManager.instance.getQuestion() != null && i < TutorialRoomManager.instance.getQuestion().answers.Length; i++)
			{
				//only allow certain buttons to be interactable for certain slides
				buttons[i].interactable = slideCnt >= 7;
				buttons[i].gameObject.SetActive(true);
				buttons[i].GetComponentInChildren<Text>().text = TutorialRoomManager.instance.getQuestion().answers[i];
			}
			for (; i < 4; i++)
			{
				buttons[i].interactable = false;
				buttons[i].gameObject.SetActive(false);
			}
			if(TutorialRoomManager.instance.getQuestion() != null)
			{
				back.interactable = slideCnt >= 10;
				back.gameObject.SetActive(true);
			}
			else
			{
				back.interactable = false;
				back.gameObject.SetActive(false);
			}
		}
		else
		{
			for (int i = 0; i<3; i++)
			{
				buttons[i].interactable = slideCnt >= 10 || slideCnt == 6 && i == 0;
				buttons[i].gameObject.SetActive(true);

			}    
			buttons[3].interactable = slideCnt >= 10;
			buttons[3].gameObject.SetActive(false);
			back.interactable = false;
			back.gameObject.SetActive(false);
			buttons[0].GetComponentInChildren<Text>().text = "Attack";
			buttons[1].GetComponentInChildren<Text>().text = "Use Potion";
			buttons[2].GetComponentInChildren<Text>().text = "Hint\n(10 coins)";
		}

	}
	//handles pausing the game
	public void Pause()
	{
		pauseMenu.SetActive(paused = true);
		a.interactable = b.interactable = c.interactable = d.interactable = back.interactable = false;
		Time.timeScale = 0;

	}
	//handles unpausing the menu
	public void Unpause()
	{
		Time.timeScale = 1;
		pauseMenu.SetActive(paused = false);
		a.interactable = b.interactable = c.interactable = d.interactable = back.interactable = true;
	}
	public void loadSlide() {
		loaded = true;
		updateUI ();
		//handle slide specific requirements like pausing the game
		if (slideCnt == 9 || slideCnt == 10 || slideCnt == 16 || slideCnt == 17)
			Time.timeScale = 0;
		if (slideCnt == 12) {
			TutorialRoomManager.instance.setHealth (1);
			updateUI ();
		}
		if (slideCnt < slides.Length)
			slides [slideCnt].SetActive (true);
		else
			Debug.Log ("out of slides");
	}
	public void nextSlide() {
		loaded = false;
		//handles slide specific requirements like unpausing the game
		if (slideCnt == 9 || slideCnt == 10 || slideCnt == 16 || slideCnt == 17)
			Time.timeScale = 1;
		if (slideCnt == 10)
			StartCoroutine (TutorialRoomManager.instance.Defend ());
		if (slideCnt == 18)
			StartCoroutine(fadeOutInLoad(gameMenu, TutorialRoomManager.instance.gameOverScreen));
		if(slideCnt == 27)
			StartCoroutine(fadeOutInLoad(multi, gameMenu));
		slides [slideCnt].SetActive (false);
		slideCnt++;
	}
	//goes to and loads the next slide
	public void goToNextSlide() {
		nextSlide ();
		loadSlide ();
	}
	//handles a fade in, fade out, load slide sequence, with activation and deactivation of objects
	public IEnumerator fadeOutInLoad(GameObject enable, GameObject disable) {
		yield return fadeOut();
		if (enable != null)
			enable.SetActive (true);
		if (disable != null)
			disable.SetActive (false);
		yield return fadeIn();
		loadSlide ();
	}
	void FixedUpdate()
	{
		//updates hitmarker location
		float displacement = GameConstants.maxDisplacement * Screen.width / 1920f;
		hitmarker.transform.position = new Vector3(Mathf.Sin(((float)time / GameConstants.framesPerCycle) * 2 * Mathf.PI) * displacement + Screen.width / 2, hitmarker.transform.position.y, hitmarker.transform.position.z);
		time++;
	}
	//handles keyboard input for answer choices, blocking, and attacking
	void Update()
	{
		if (inputDelay > 0)
		{
			inputDelay--;
			return;
		}
		if (Input.GetAxis("Attack") > .5 && hitmarker.isActiveAndEnabled)
		{
			pressedSpace=true;
			inputDelay = 20;
		}
		if (TutorialRoomManager.instance.getQuestion() != null)
		{
			if (Input.GetAxis("A") > .5)
			{
				A();
				inputDelay = 20;
			}
			else if (Input.GetAxis("B") > .5)
			{
				B();
				inputDelay = 20;
			}
			else if (Input.GetAxis("C") > .5)
			{
				C();
				inputDelay = 20;
			}
			else if (Input.GetAxis("D") > .5)
			{
				D();
				inputDelay = 20;
			}
		}
		//handles pressing escape to pause/unpuase
		if (Input.GetKey(KeyCode.Escape))
		{
			if (paused)
				escapeCnt++;
			else
			{
				Pause();
				escapeCnt = -2147483648;
			}
		}
		else
		{
			//if duration is too short, that means user was trying to unpause
			if (escapeCnt < 10 && escapeCnt > 0)
				Unpause();
			escapeCnt = 0;
		}
		//start displaying progress bar if escape is held for a long time
		if (escapeCnt >= 25)
		{
			escapeProg.SetActive(true);
			escapeProg.GetComponent<Slider>().value = 2*(escapeCnt - 25);
			if (escapeCnt >= 75)
			{
				//go back to main menu if held for long enough
				Unpause();
				TutorialRoomManager.instance.goToMM();
			}
		}
		else
			escapeProg.SetActive(false); 

	}
	int escapeCnt;
	int inputDelay;
}
