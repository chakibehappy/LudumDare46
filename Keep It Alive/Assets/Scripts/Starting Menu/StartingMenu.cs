using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/* This code support both selection from joystick, keyboard or mouse
 * Joystick : Analog / Directional button
 * Keyboard : WASD / Arrow key
 */

public class StartingMenu : MonoBehaviour
{
    GameMaster GM;
    public int startingSceneLevel = 2;
    int selection = 0, subSelection = 0;
    public Text[] txtMainMenu;
    public Color selectedButtonColor = Color.green;
    public Color activeButtonColor = Color.white;
    public GameObject SettingUI;
    public Text[] txtSettingMenu;
    bool onSetting;
    public Slider bgmSlider;
    public Slider sfxSlider;

    // joy stick and axis input related :
    bool mouseOnButton;
    bool isPressingVertical, isPressingHorizontal;

    // sound related :
    AudioSource Bgm;
    AudioSource Sfx;
    public AudioClip bgmClip = null;
    public AudioClip[] sfxClip = null;

    // fading screen variables :
    public Image fadingScreen = null;
    bool fadeIn = false, fadeOut = false;
    Color fadingCol = Color.black;
    public float fadingTime = 1f;
    public float fadeStep = 0.04f;

    Gamepad gamepad;

    void Awake()
    {
        fadingScreen.gameObject.SetActive(true);
        fadingCol = fadingScreen.color;
        SettingUI.SetActive(false);
    }

    void Start()
    {
        gamepad = Gamepad.current;
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        Bgm = GameObject.Find("Bgm").GetComponent<AudioSource>();
        Sfx = GameObject.Find("Sfx").GetComponent<AudioSource>();
        bgmSlider.value = GM.BgmVol;
        sfxSlider.value = GM.SfxVol;
        Bgm.ignoreListenerVolume = true;

        Bgm.clip = bgmClip;
        if (!Bgm.isPlaying) StartCoroutine(GM.AudioFadeIn(Bgm, fadingTime));
        StartCoroutine("ScreenFadeIn");
    }

    void Update()
    {
        InputHandler();
        MenuManager();
    }

    void FixedUpdate()
    { 
        FadeScreen();
    }

    void InputHandler()
    {
        // for up-down joystick or keyboard axis input (WS/arrow key)
        float y = Input.GetAxisRaw("Vertical");
        if (y != 0)
        {
            if (isPressingVertical == false)
            {
                isPressingVertical = true;
                Sfx.PlayOneShot(sfxClip[0]);
                if (!onSetting)
                {
                    if (y > 0)
                    {
                        selection--;
                        if (selection < 0) selection = txtMainMenu.Length - 1;
                    }
                    else if (y < 0)
                    {
                        selection++;
                        if (selection > txtMainMenu.Length - 1) selection = 0;
                    }
                }
                else
                {
                    // on setting menu :
                    if (y > 0)
                    {
                        subSelection--;
                        if (subSelection < 0) subSelection = txtSettingMenu.Length - 1;
                    }
                    else if (y < 0)
                    {
                        subSelection++;
                        if (subSelection > txtSettingMenu.Length - 1) subSelection = 0;
                    }
                }
                
            }
        }
        if (y == 0)
        {
            isPressingVertical = false;
        }

        // for left-right joystick or keyboard axis input (AD/arrow key)
        float x = Input.GetAxisRaw("Horizontal");
        if (onSetting)
        {
            ApplySetting();
            if (x != 0)
            {
                if (isPressingHorizontal == false)
                {
                    isPressingHorizontal = true;
                    if (x > 0)
                    {
                        if (subSelection == 0 && GM.BgmVol < 1) GM.BgmVol += 0.1f;
                        else if (subSelection == 1 && GM.SfxVol < 1) GM.SfxVol += 0.1f;
                    }
                    else if (x < 0)
                    {
                        if (subSelection == 0 && GM.BgmVol > 0) GM.BgmVol -= 0.1f;
                        else if (subSelection == 1 && GM.SfxVol > 0) GM.SfxVol -= 0.1f;
                    }
                    bgmSlider.value = GM.BgmVol;
                    sfxSlider.value = GM.SfxVol;
                }
            }
            if (x == 0)
            {
                isPressingHorizontal = false;
            }
        }
        
        if (Input.GetButtonDown("Fire2")) SelectingMenu();
        if (Input.GetKeyDown(KeyCode.Return)) SelectingMenu();
        if (Input.GetMouseButtonDown(0) && mouseOnButton) SelectingMenu();

    }

    void MenuManager()
    {
        for (int i = 0; i < txtMainMenu.Length; i++)
        {
            if (i == selection) txtMainMenu[i].color = selectedButtonColor;
            else txtMainMenu[i].color = activeButtonColor;
        }
        // setting menu :
        for (int i = 0; i < txtSettingMenu.Length; i++)
        {
            if (i == subSelection) txtSettingMenu[i].color = selectedButtonColor;
            else txtSettingMenu[i].color = activeButtonColor;
        }
    }

    void ApplySetting()
    {
        GM.BgmVol = bgmSlider.value;
        GM.SfxVol = sfxSlider.value;
        Bgm.volume = bgmSlider.value;
        AudioListener.volume = sfxSlider.value;
    }

    void SelectingMenu()
    {
        if(!onSetting)
        {
            Sfx.PlayOneShot(sfxClip[1]);
            if (selection == 0)
            {
                // Handheld.Vibrate(); // vibrate for phone
                if(gamepad != null) gamepad.SetMotorSpeeds(0.123f, 0.234f);
                StartCoroutine("GoToStartingScene");
            }
            else if (selection == 1)
            {
                subSelection = 0;
                onSetting = true;
                SettingUI.SetActive(true);
            }
            else if (selection == 2)
            {
                StartCoroutine("QuitGame");
            }
        }
        else
        {
            if(subSelection == 2)
            {
                selection = 0;
                Sfx.PlayOneShot(sfxClip[2]);
                onSetting = false;
                SettingUI.SetActive(false);
            }
        }
    }

    public void MouseEnterButton(int button)
    {
        Sfx.PlayOneShot(sfxClip[0]);
        mouseOnButton = true;
        selection = button;
        subSelection = button;
    }

    public void MouseExitButton()
    {
        mouseOnButton = false;
    }

    IEnumerator QuitGame()
    {
        fadingScreen.gameObject.SetActive(true);
        fadeOut = true;
        StartCoroutine(GM.AudioFadeOut(Bgm, fadingTime));
        yield return new WaitForSeconds(fadingTime);
        fadeOut = false;
        Application.Quit();
    }

    IEnumerator GoToStartingScene()
    {
        fadingScreen.gameObject.SetActive(true);
        fadeOut = true;
        StartCoroutine(GM.AudioFadeOut(Bgm, fadingTime));
        yield return new WaitForSeconds(fadingTime);
        fadeOut = false;
        SceneManager.LoadScene(startingSceneLevel);
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
