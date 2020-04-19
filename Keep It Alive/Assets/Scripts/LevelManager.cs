using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    GameMaster GM;
    AudioSource Bgm;
    AudioSource monsterBgm;
    AudioSource Sfx;
    public AudioClip[] bgmClip;
    public AudioClip sfxClip;

    public float fadingTime = 1.5f;

    public Image heartBar;
    public GameObject GameOverUI;
    bool runOnce;

    // fading screen variables :
    public Image fadingScreen = null;
    bool fadeIn = false, fadeOut = false;
    Color fadingCol = Color.black;
    public float fadeStep = 0.04f;

    void Awake()
    {
        fadingScreen.gameObject.SetActive(true);
        fadingCol = fadingScreen.color;
        GameOverUI.SetActive(false);
    }

    void Start()
    {
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        Bgm = GameObject.Find("Bgm").GetComponent<AudioSource>();
        Sfx = GameObject.Find("Sfx").GetComponent<AudioSource>();
        monsterBgm = GetComponent<AudioSource>();

        monsterBgm.ignoreListenerVolume = true;
        monsterBgm.volume = GM.BgmVol;

        Bgm.clip = bgmClip[0];
        monsterBgm.clip = bgmClip[2];

        if (!Bgm.isPlaying) StartCoroutine(GM.AudioFadeIn(Bgm, fadingTime));
        StartCoroutine("ScreenFadeIn");
    }

    private void FixedUpdate()
    {
        FadeScreen();

        if (GM.HeartPoint > 0)
        {
            GM.HeartPoint -= GM.HeartDropRate * Time.fixedDeltaTime;
            heartBar.fillAmount = GM.HeartPoint;
        }
        else
        {
            if(!runOnce)
            {
                GameOverUI.SetActive(true);
                runOnce = true;
                StartCoroutine("ChangeToLevelBgm");
            }
        }
    }

    public IEnumerator ChangeToMonsterBgm()
    {
        StartCoroutine(GM.AudioFadeOut(Bgm, fadingTime));
        StartCoroutine(GM.AudioFadeIn(monsterBgm, fadingTime));
        yield return new WaitForSeconds(fadingTime);
    }

    public IEnumerator ChangeToLevelBgm()
    {
        StartCoroutine(GM.AudioFadeOut(monsterBgm, fadingTime));
        StartCoroutine(GM.AudioFadeIn(Bgm, fadingTime));
        yield return new WaitForSeconds(fadingTime);
    }

    public void GameOverButton (int button)
    {
        Sfx.PlayOneShot(sfxClip);
        if (button == 0)
        {
            StartCoroutine(GoToScene(2));
        }
        else if(button == 1)
        {
            StartCoroutine(GoToScene(1));
        }
    }

    IEnumerator GoToScene(int scene)
    {
        GM.HeartPoint = 1;
        GM.HeartDropRate = GM.InitialHeartRate;
        fadingScreen.gameObject.SetActive(true);
        fadeOut = true;
        StartCoroutine(GM.AudioFadeOut(Bgm, fadingTime));
        yield return new WaitForSeconds(fadingTime);
        fadeOut = false;
        SceneManager.LoadScene(scene);
    }

    IEnumerator ScreenFadeIn()
    {
        fadeIn = true;
        yield return new WaitForSeconds(fadingTime);
        fadeIn = false;
        fadingScreen.gameObject.SetActive(false);
    }

    void FadeScreen()
    {
        if (fadeIn && fadingCol.a > 0)
        {
            fadingCol.a -= fadeStep;
        }
        if (fadeOut && fadingCol.a < 1)
        {
            fadingCol.a += fadeStep;
        }
        fadingScreen.color = fadingCol;
    }
}
