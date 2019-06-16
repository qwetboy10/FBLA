using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MUIController : MonoBehaviour
{
    public static MUIController instance;
    public Sprite full;
    public Sprite half;
    public GameObject[] ahearts;
    public GameObject[] bhearts;

    public Text questionText, titleText;
    public Button[] buttons;

    public Color fadeColor;
    public Color damageColor;
    public Image overlayImage;

    private bool paused;
    public GameObject pauseMenu;
    public GameObject escapeProg;
    public Image hitmarker;
    public Image hitmarkerBackground;
    public Text aDamageText;
    public Text bDamageText;
    public Color damageTextColor;
    public int time;
    private bool pressedSpace;
    public Text category;
    public Text aTurn,bTurn,wrong;

    //instantiates required variables
    void Start()
    {
        //singleton
        if (instance != null && instance != this)
            Destroy(this);
        instance = this;

        //resets health if not loading from save game
        if(DataModel.aGetHearts()==0) DataModel.aSetHearts(GameConstants.maxHearts[DataModel.getDifficulty()]);
        if(DataModel.bGetHearts()==0) DataModel.bSetHearts(GameConstants.maxHearts[DataModel.getDifficulty()]);

        //displays room title
        StartCoroutine(setTitle("ROOM " + (DataModel.getRoomCnt() + 1), 1));

        //resets hitmarker position
        hitmarker.transform.position = new Vector3(Screen.width / 2, hitmarker.transform.position.y, 0);
        //sets whos turn it is
        if(DataModel.getTurn()==0)
        {
            aTurn.gameObject.SetActive(true);
            bTurn.gameObject.SetActive(false);
        }
        else
        {
            bTurn.gameObject.SetActive(true);
            aTurn.gameObject.SetActive(false);
        }
    }
    public IEnumerator processBar()
    {
        //waits untill player presses space then returns
        hitmarker.gameObject.SetActive(true);
        hitmarkerBackground.gameObject.SetActive(true);
        while(!pressedSpace)
        {
            yield return null;
        }
        pressedSpace=false;
        hitmarker.gameObject.SetActive(false);
        hitmarkerBackground.gameObject.SetActive(false);
    }
    //fades from color "from" to color "to" with speed delta and duration end
    IEnumerator fadeFromTo(Color from, Color to, float end, float delta)
    {
        overlayImage.color = from;
        float cur = 0;
        while (cur < end)
        {
            overlayImage.color = Color.Lerp(from, to, cur);
            cur = Mathf.Min(end, cur + delta * Time.deltaTime);
            yield return null;
        }

    }
    //initializes buttons
    public void init(int diff)
    {
        buttons[0].onClick.AddListener(A);
        buttons[1].onClick.AddListener(B);
        buttons[2].onClick.AddListener(C);
        buttons[3].onClick.AddListener(D);
        updateUI();
    }
    public void A() { pressButton(0); }
    public void B() { pressButton(1); }
    public void C() { pressButton(2); }
    public void D() { pressButton(3); }
    //handles button presses
    public void pressButton(int b)
    {
            //return if b is out of range
            if (b >= DataModel.getQuestion().answers.Length)
                return;
            Boolean correct = DataModel.answerQuestion(b);

            //unhighlights button
            buttons[b].GetComponent<ButtonHover>().OnPointerExit(null);
            Debug.Log("Answer is " + (correct ? "correct" : "wrong"));
            //handle answer
            if (!correct)
                StartCoroutine(MRoomManagerScript.instance.processWrong(false));
            else
                StartCoroutine(MRoomManagerScript.instance.processRight());
        
    }
    public IEnumerator die()
    {
        //fade out then go to main menu
        yield return StartCoroutine(fadeFromTo(new Color(0, 0, 0, 0), fadeColor, 1f, .3f));
		SceneManager.LoadScene ("MainMenu");
    }
    public IEnumerator showDamage(int i)
    {
        //displays damage number above head
        if(DataModel.getTurn()==0)
        {
            aDamageText.gameObject.SetActive(true);
            aDamageText.text = i.ToString();
            aDamageText.color = damageTextColor;
            Color final = damageTextColor;
            final.a = 0f;
            float cur = 0f;
            //fade out damage text
            while (cur < GameConstants.damageNumberTime)
            {
                aDamageText.color = Color.Lerp(damageTextColor, final, cur / (GameConstants.damageNumberTime));
                cur = Mathf.Min(GameConstants.damageNumberTime, cur + GameConstants.damageNumberFadeSpeed * Time.deltaTime);
                yield return null;
            }
            aDamageText.gameObject.SetActive(false);
        }
        else
        {
            bDamageText.gameObject.SetActive(true);
            bDamageText.text = i.ToString();
            bDamageText.color = damageTextColor;
            Color final = damageTextColor;
            final.a = 0f;
            float cur = 0f;
             //fade out damage text
            while (cur < GameConstants.damageNumberTime)
            {
                bDamageText.color = Color.Lerp(damageTextColor, final, cur / (GameConstants.damageNumberTime));
                cur = Mathf.Min(GameConstants.damageNumberTime, cur + GameConstants.damageNumberFadeSpeed * Time.deltaTime);
                yield return null;
            }
            bDamageText.gameObject.SetActive(false);
        }
       
    }
    public Coroutine takeDamage()
    {
        //flashes screen on damage
        return StartCoroutine(fadeFromTo(damageColor, new Color(0, 0, 0, 0), 1f, 2f));
    }
    public Coroutine fadeOut()
    {
        //fades to black
        return StartCoroutine(fadeFromTo(new Color(0, 0, 0, 0), fadeColor, 1f, .5f));
    }
    public Coroutine fadeIn()
    {
        //fades from black
        return StartCoroutine(fadeFromTo(fadeColor, new Color(0, 0, 0, 0), 1f, .5f));
    }
    public IEnumerator setTitle(String s, float wait)
    {
        //displays title then fades it out
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
        updateHearts();
        updateQuestion();
        updateAnswers();
    }
    public void updateHearts()
    {
        //updates heart sprites
        int aHalfHearts = DataModel.aGetHearts();
        for (int i = 0; i < ahearts.Length * 2; i += 2)
        {
            //sets heart to full
            ahearts[i / 2].GetComponent<Image>().sprite = full;
            if (i < aHalfHearts)
            {
                //sets as opaque
                Image im = ahearts[i / 2].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 1);
            }
            else
            {
                //sets as transparent
                Image im = ahearts[i / 2].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 0);
            }
        }
        //handles lagging half heart if number of half hearts is odd
        if (aHalfHearts % 2 == 1)
        {
            Image im = ahearts[aHalfHearts / 2].GetComponent<Image>();
            im.sprite = half;
        }
        //same but for second set of hearts
        int bHalfHearts = DataModel.bGetHearts();
        for (int i = 0; i < bhearts.Length * 2; i += 2)
        {
            //sets heart to full
            bhearts[i / 2].GetComponent<Image>().sprite = full;
            if (i < bHalfHearts)
            {
                //sets as opaque
                Image im = bhearts[i / 2].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 1);
                //flips heart sprite
                im.gameObject.transform.localScale = new Vector3(-1,1,1);
            }
            else
            {
                //sets as transparent
                Image im = bhearts[i / 2].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 0);
            }
        }
        //handles lagging half heart if number of half hearts is odd
        if (bHalfHearts % 2 == 1)
        {
            Image im = bhearts[bHalfHearts / 2].GetComponent<Image>();
            im.sprite = half;
            //flips heart sprite
            im.gameObject.transform.localScale = new Vector3(-1,1,1);
        }
    }
    
    public void updateQuestion()
    {
        //sets new question
        Question q = DataModel.getQuestion();
        questionText.text = q == null ? "" : q.question;
        category.text = q == null ? "" : q.category;
    }
    public void updateAnswers()
    {
        //puts answer text on buttons
            int i;
            for (i = 0; DataModel.getQuestion() != null && i < DataModel.getQuestion().answers.Length; i++)
            {
                buttons[i].interactable = true;
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = DataModel.getQuestion().answers[i];
            }
            for (; i < 4; i++)
            {
                buttons[i].interactable = false;
                buttons[i].gameObject.SetActive(false);
            }
    }
    public void Pause()
    {
        //dim everything
        pauseMenu.SetActive(paused = true);
        for(int i=0;i<4;i++) buttons[i].interactable = false;
        //stops time
        Time.timeScale = 0;

    }
    public void Unpause()
    {
        //resume time
        Time.timeScale = 1;
        //get rid of overlay
        pauseMenu.SetActive(paused = false);
        for(int i=0;i<4;i++) buttons[i].interactable = true;
    }
    void FixedUpdate()
    {
        //moves timer bar
        float displacement = GameConstants.maxDisplacement * Screen.width / 1920f;
        if(hitmarker.isActiveAndEnabled) hitmarker.transform.position = new Vector3(Mathf.Sin(((float)time / GameConstants.framesPerCycle) * 2 * Mathf.PI) * displacement + Screen.width / 2, hitmarker.transform.position.y, hitmarker.transform.position.z);
        time++;
    }
    void Update()
    {
        //stops buttons from being pressed multiple times
        if (inputDelay > 0)
        {
            inputDelay--;
            return;
        }
        //hitmarker
        if (Input.GetKeyDown(KeyCode.Space) && instance.hitmarker.isActiveAndEnabled)
            {
                pressedSpace=true;
                inputDelay = 20;
            }
        //handles pressing buttons to answer questions
        if (DataModel.getQuestion() != null)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Alpha1))
            {
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
        //pause
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
        //unpause
        else
        {
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
                //go back to main menu and save
                Unpause();
                DataModel.saveGame();
                SceneManager.LoadScene ("MainMenu");
            }
        }
        else
            escapeProg.SetActive(false);
    }
    int escapeCnt;
    int inputDelay;
    
}
