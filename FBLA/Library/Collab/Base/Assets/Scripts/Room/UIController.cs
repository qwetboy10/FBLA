using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Sprite full;
    public Sprite half;
    public GameObject[] hearts;

    public Text questionText, scoreText, titleText, winText;
    public Button[] buttons;

    public Color fadeColor;
    public Color damageColor;
    public Image overlayImage;

    private bool paused;
    public GameObject pauseMenu;
    public GameObject escapeProg;

    public Text gameOverText, continueText, exitText;
    public Button continueBut, exitBut;
    public Image hitmarker;
    public Image hitmarkerBackground;
    public Text damageText;
    public Color damageTextColor;
    public int time;
    private bool pressedSpace;
    public Text category;
    public bool menu;
    public Text coinText;
    public Text potionText;
    public GameObject dungeon;
    public GameObject forest;
    public Button back;
    public Text plusText;

    void Start()
    {
        //singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;
        //displays room number
        StartCoroutine(setTitle("ROOM " + (DataModel.getRoomCnt() + 1), 1));
        //resets hitmarker postition
        hitmarker.transform.position = new Vector3(Screen.width / 2, hitmarker.transform.position.y, 0);
        back.onClick.AddListener(onBack);
    }
    void onBack()
    {
        //go back to options
        menu = true;
        instance.updateUI();
    }
    public IEnumerator processBar()
    {
        
        hitmarker.gameObject.SetActive(true);
        hitmarkerBackground.gameObject.SetActive(true);
        //wait for the player to press space
        while(!pressedSpace)
        {
            yield return null;
        }
        pressedSpace=false;
        hitmarker.gameObject.SetActive(false);
        hitmarkerBackground.gameObject.SetActive(false);
    }
    IEnumerator fadeFromTo(Color from, Color to, float end, float delta)
    {
        //fades from color "from" to color "to"
        //end is duration, delta is speed
        overlayImage.color = from;
        float cur = 0;
        while (cur < end)
        {
            overlayImage.color = Color.Lerp(from, to, cur);
            cur = Mathf.Min(end, cur + delta * Time.deltaTime);
            yield return null;
        }

    }
    public void init(int diff)
    {
        //sets up handlers for ui buttons
        buttons[0].onClick.AddListener(A);
        buttons[1].onClick.AddListener(B);
        buttons[2].onClick.AddListener(C);
        buttons[3].onClick.AddListener(D);
        exitBut.onClick.AddListener(exit);
        continueBut.onClick.AddListener(cont);
        menu = true;
        updateUI();
    }
    public void exit() { StartCoroutine(backToMM()); }
    public void cont() { StartCoroutine(continueGame()); }
    public void A() { Debug.Log("BBB"); pressButton(0); }
    public void B() { pressButton(1); }
    public void C() { pressButton(2); }
    public void D() { pressButton(3); }
    public void pressButton(int b)
    {
        Debug.Log("CCC");
        Debug.Log(b);
        if(menu)
        {
            
            //Attack
            if(b==0)
            {
                menu = false;
                updateUI();
            }
            //Potion
            if(b==1)
            {
                if(DataModel.getPotions()>=1)
                {
                    //remove 1 potion
                    DataModel.changePotions(-1);
                    DataModel.setHearts(GameConstants.maxHearts[DataModel.getDifficulty()]);
                    updateUI();
                    //play animation and sound
                   
                    StartCoroutine(RoomManagerScript.instance.pscript.drinkPotion());
                }
                else
                {
                    //if you dont have a potion
                    AudioManager.playError();
                }
            }
            //Hint
            if(b==2)
            {
                if(DataModel.getCoins()>=10 && !DataModel.getQuestion().hinted)
                {
                    //remove coins, remove two answer choices form hint
                    DataModel.changeCoins(-10);
                    DataModel.getQuestion().hint();
                    menu = false;
                    updateUI();
                    
                }
                else
                {
                    //if you dont have enough coins
                    AudioManager.playError();
                }
            }
        }
        else
        {
            //clicking on button that doesnt exist
            if (b >= DataModel.getQuestion().answers.Length)
                return;
            Boolean correct = DataModel.answerQuestion(b);
            Debug.Log("Answer is " + (correct ? "correct" : "wrong"));
            
            //branches based on if you get the question right
            if (!correct)
            {
                AudioManager.playError();
                StartCoroutine(RoomManagerScript.instance.processWrong(false));
            }
            else
            {
                StartCoroutine(RoomManagerScript.instance.processRight());
            }
                
        }
        
    }
    public IEnumerator getThing(String s)
    {
        plusText.text = s;
        yield return new WaitForSeconds(1);
        plusText.text = "";
    }
    public IEnumerator die()
    {
        //fade to black
        yield return StartCoroutine(fadeFromTo(new Color(0, 0, 0, 0), fadeColor, 1f, .3f));
        float cur = 0, end = 1;
        Color transparent = new Color(255, 255, 255, 0), opaque = new Color(255, 255, 255, 1);
        while (cur < end)
        {
            //makes gameover text more opaque over time
            gameOverText.color = Color.Lerp(transparent, opaque, cur);
            cur = Mathf.Min(end, cur + .5f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(.1f);
        cur = 0; end = 1;
        while (cur < end)
        {
            //makes continue and exit buttons appear
            continueText.color = exitText.color = Color.Lerp(transparent, opaque, cur);
            cur = Mathf.Min(end, cur + .5f * Time.deltaTime);
            yield return null;
        }
        //make them interactable
        continueBut.interactable = exitBut.interactable = true;
    }
    public IEnumerator showDamage(int i)
    {
        //damage text appears
        damageText.gameObject.SetActive(true);
        damageText.text = i.ToString();
        damageText.color = damageTextColor;
        Color final = damageTextColor;
        final.a = 0f;
        float cur = 0f;
        while (cur < GameConstants.damageNumberTime)
        {
            //damage text fades over time
            damageText.color = Color.Lerp(damageTextColor, final, cur);
            cur = Mathf.Min(GameConstants.damageNumberTime, cur + GameConstants.damageNumberFadeSpeed * Time.deltaTime);
            yield return null;
        }
        damageText.gameObject.SetActive(false);
    }
    public IEnumerator gameOverFade()
    {
        //continue and exit fade out
        float cur = 0, end = 1;
        Color transparent = new Color(255, 255, 255, 0), opaque = new Color(255, 255, 255, 1);
        while (cur < end)
        {
            continueText.color = exitText.color = gameOverText.color = Color.Lerp(opaque, transparent, cur);
            cur = Mathf.Min(end, cur + .5f * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    public IEnumerator winScreen()
    {
        //fade in to win text
        yield return StartCoroutine(fadeFromTo(new Color(0, 0, 0, 0), fadeColor, 1f, .3f));
        float cur = 0, end = 1;
        Color transparent = new Color(255, 255, 255, 0), opaque = new Color(255, 255, 255, 1);
        while (cur < end)
        {
            winText.color = Color.Lerp(transparent, opaque, cur);
            cur = Mathf.Min(end, cur + .5f * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    public IEnumerator continueGame()
    {
        continueBut.interactable = exitBut.interactable = false;
        //lost 1000 score for restarting
        DataModel.changeScore(-1000);
        //fade
        yield return StartCoroutine("gameOverFade");
        DataModel.setHearts(GameConstants.maxHearts[DataModel.getDifficulty()]);
        RoomManagerScript.instance.goToNextRoom();
    }
    public IEnumerator backToMM()
    {
        //buttons become uninteractable
        continueBut.interactable = exitBut.interactable = false;
        //fade then go back to main menu
        yield return StartCoroutine("gameOverFade");
        RoomManagerScript.instance.goToMM();
    }
    public Coroutine takeDamage()
    {
        //red flash when you take damage
        return StartCoroutine(fadeFromTo(damageColor, new Color(0, 0, 0, 0), 1f, 2f));
    }
    public Coroutine fadeOut()
    {
        return StartCoroutine(fadeFromTo(new Color(0, 0, 0, 0), fadeColor, 1f, .5f));
    }
    public Coroutine fadeIn()
    {
        return StartCoroutine(fadeFromTo(fadeColor, new Color(0, 0, 0, 0), 1f, .5f));
    }
    public IEnumerator setTitle(String s, float wait)
    {
        //changes title
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
    public void updateUI()
    {
        //updates all of the UI
        updateHearts();
        updateQuestion();
        updateAnswers();
        updateScore();
        updateCH();
        updateBackground();
    }
    public void updateBackground()
    {
        //sets the background to the correct one based of the current enemy
        forest.SetActive(false);
        dungeon.SetActive(false);
        if(DataModel.getBackground()==0) forest.gameObject.SetActive(true);
        if(DataModel.getBackground()==1) dungeon.gameObject.SetActive(true);
    }
    public void updateCH()
    {
        //updates coins and potions
        coinText.text = DataModel.getCoins().ToString("00");
        potionText.text = DataModel.getPotions().ToString("00");
    }
    public void updateScore()
    {
        //updates score
        scoreText.text = DataModel.getScore().ToString("D6");
    }
    public void updateHearts()
    {
        //get number of half hearts
        int halfHearts = DataModel.getHearts();
        for (int i = 0; i < hearts.Length * 2; i += 2)
        {
            //sets heart to full
            hearts[i / 2].GetComponent<Image>().sprite = full;
            if (i < halfHearts)
            {
                //sets as opaque
                Image im = hearts[i / 2].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 1);
            }
            else
            {
                //sets as transparent
                Image im = hearts[i / 2].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 0);
            }
        }
        //handles lagging half heart if number of half hearts is odd
        if (halfHearts % 2 == 1)
        {
            Image im = hearts[halfHearts / 2].GetComponent<Image>();
            im.sprite = half;
        }
    }

    public void updateQuestion()
    {
        //gets new question
        Question q = DataModel.getQuestion();
        questionText.text = q == null ? "" : q.question;
        category.text = q == null ? "" : q.category;
    }
    public void updateAnswers()
    {
        if(!menu)
        {
            //sets buttons as interactable and visible if they have an associated answer choice
            int i;
            for (i = 0; DataModel.getQuestion() != null && i < DataModel.getQuestion().answers.Length; i++)
            {
                buttons[i].interactable = true;
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = DataModel.getQuestion().answers[i];
            }
            //gets rid of extra buttons
            for (; i < 4; i++)
            {
                buttons[i].interactable = false;
                buttons[i].gameObject.SetActive(false);
            }
            if(DataModel.getQuestion() != null)
            {
                back.interactable = true;
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
                buttons[i].interactable = true;
                buttons[i].gameObject.SetActive(true);
               
            }    
            buttons[3].interactable = false;
            buttons[3].gameObject.SetActive(false);
            back.interactable = false;
            back.gameObject.SetActive(false);
            //sets three menu buttons
            buttons[0].GetComponentInChildren<Text>().text = "Attack";
            buttons[1].GetComponentInChildren<Text>().text = "Use Potion";
            buttons[2].GetComponentInChildren<Text>().text = "Hint\n(10 coins)";
        }
        
    }
    public void Pause()
    {
        //stops time and shows pause menu
        pauseMenu.SetActive(paused = true);
        for(int i=0;i<4;i++) buttons[i].interactable = false;
        back.interactable = false;
        Time.timeScale = 0;

    }
    public void Unpause()
    {
        //restarts time and hides pause menu
        Time.timeScale = 1;
        pauseMenu.SetActive(paused = false);
        for(int i=0;i<4;i++) buttons[i].interactable = false;
        back.interactable = true;
    }
    void FixedUpdate()
    {
        //moves hitmarkedr
        float displacement = GameConstants.maxDisplacement * Screen.width / 1920f;
        if(hitmarker.isActiveAndEnabled) hitmarker.transform.position = new Vector3(Mathf.Sin(((float)time / GameConstants.framesPerCycle) * 2 * Mathf.PI) * displacement + Screen.width / 2, hitmarker.transform.position.y, hitmarker.transform.position.z);
		time++;
    }
    void Update()
    {
        //prevents keys from being pressed multiple times
        if (inputDelay > 0)
        {
            inputDelay--;
            return;
        }
        //handles space press for attack
        if (Input.GetKeyDown(KeyCode.Space) && UIController.instance.hitmarker.isActiveAndEnabled)
        {
                pressedSpace=true;
                inputDelay = 20;
        }
        if (DataModel.getQuestion() != null)
        {
            //handles four answer choices
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("AAA");
                A();
                inputDelay = 20;
            }
            else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                B();
                inputDelay = 20;
            }
            else if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                C();
                inputDelay = 20;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                D();
                inputDelay = 20;
            }
        }
        //handles pause menu
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
            //unpauses
            if (escapeCnt < 10 && escapeCnt > 0)
                Unpause();
            escapeCnt = 0;
        }
        if (escapeCnt >= 25)
        {
            escapeProg.SetActive(true);
            escapeProg.GetComponent<Slider>().value = escapeCnt - 25;
            if (escapeCnt >= 125)
            {
                Unpause();
                DataModel.saveGame();
                RoomManagerScript.instance.goToMM();
            }
        }
        else
            escapeProg.SetActive(false);
    }
    int escapeCnt;
    int inputDelay;
}
