using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;
    public GameObject Effect;
    public AudioSource AudioSor;
    public AudioClip[] Clips;
    public enum PLAYER_STATUS
    {
        NORMAL,
        DRUNK,
        TIRED,
        RACING,
    }
    public PLAYER_STATUS PlayerStatus;
    public float PlayerSpeed;
    public float[] ReactionTimeNormal;
    public float[] ReactionTimeDrunk;
    public float[] ReactionTimeTired;
    public float[] ReactionTimeRacing;
    public Image FadeIn;
    public GameObject SuccessText;
    public PostProcessProfile[] Profiles;
    public PostProcessVolume PP;
    public AudioSource BGMAudio;
    public bool GameFailed;

    private bool isDistortionAddingUp;
    private LensDistortion lensDistor;
    private float closeEyeTimer;
    private Coroutine openEye;
    private float gameEndTimer;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public float GetReactionTime(PLAYER_STATUS status, EventManager.EVENTS evt)
    {
        switch(status)
        {
            case PLAYER_STATUS.NORMAL:
                return ReactionTimeNormal[(int)evt];
            case PLAYER_STATUS.DRUNK:
                return ReactionTimeDrunk[(int)evt];
            case PLAYER_STATUS.TIRED:
                return ReactionTimeTired[(int)evt];
            case PLAYER_STATUS.RACING:
                return ReactionTimeRacing[(int)evt];
        }
        return 0;
    }

    public float GetReactionTimeBaseOnCurrentStatus(EventManager.EVENTS evt)
    {
        return GetReactionTime(PlayerStatus, evt);
    }

    IEnumerator ShowGameEndTips(PLAYER_STATUS stats)
    {
        yield return new WaitForSeconds(2);
        GameEndResearchTipManager.Instance.HandleShowEndGameTips(stats);
    }

    public void HandleGameFailed()
    {
        GameFailed = true;
        StartCoroutine(ShowGameEndTips(PlayerStatus));
    }

    public void HandleLoadTitleScene()
    {
        GameObject settings = GameObject.FindGameObjectWithTag("CARRIEDOVERSETTINGS");
        Destroy(settings);
        SceneManager.LoadScene(0);
    }

    IEnumerator WaitAndLoadTitle()
    {
        yield return new WaitForSeconds(3);
        HandleLoadTitleScene();
    }

    public void HandleGameSuccess()
    {
        SuccessText.SetActive(true);
        StartCoroutine(WaitAndLoadTitle());
    }
    // Start is called before the first frame update
    void Start()
    {
        SuccessText.SetActive(false);
        GameFailed = false;
        gameEndTimer = 0;
        GameObject settings = GameObject.FindGameObjectWithTag("CARRIEDOVERSETTINGS");
        if(settings!=null)
        {
            PlayerStatus = settings.GetComponent<GameSettingsDontDestroy>().PlayerChosenStatus;
            HandleSetupPlayerStatus(PlayerStatus);
        }
    }
    
    public void HandleSetupPlayerStatus(GameSettings.PLAYER_STATUS stats)
    {
        PP.profile = Profiles[(int)stats];
        switch (stats)
        {
            //we do nothing for the normal stats
            case PLAYER_STATUS.NORMAL:
                break;
            case PLAYER_STATUS.RACING:
                PlayerSpeed *= 1.25f;
                BGMAudio.Play();
                break;
            case PLAYER_STATUS.DRUNK:
                isDistortionAddingUp = true;
                PP.profile.TryGetSettings<LensDistortion>(out lensDistor);
                break;
            case PLAYER_STATUS.TIRED:
                closeEyeTimer = Random.Range(2,4);
                break;
        }
    }

    public void SetFadeInToUserChoice()
    {
        Color color = FadeIn.color;
        color.a = 0.5f;
        FadeIn.color = color;
    }

    public void ResetFadeIn()
    {
        Color color = FadeIn.color;
        color.a = 0;
        FadeIn.color = color;
    }

    IEnumerator OpenEye()
    {
        yield return new WaitForSeconds(0.6f);
        ResetFadeIn();
    }


    public void HandlePlayCarCrashAudio()
    {
        AudioSor.clip = Clips[0];
        AudioSor.Play();
    }

    public void HandlePlayCarCrashHumanAudio()
    {
        AudioSor.clip = Clips[1];
        AudioSor.Play();
    }

    public void HandlePoliceChasingAudio()
    {
        AudioSor.clip = Clips[2];
        AudioSor.Play();
    }

    public void HandlePoliceAskingAudio()
    {
        AudioSor.clip = Clips[3];
        AudioSor.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
        GameObject settings = GameObject.FindGameObjectWithTag("CARRIEDOVERSETTINGS");
        if (settings != null)
        {
            if (PlayerStatus != settings.GetComponent<GameSettingsDontDestroy>().PlayerChosenStatus)
            {
                PlayerStatus = settings.GetComponent<GameSettingsDontDestroy>().PlayerChosenStatus;
                HandleSetupPlayerStatus(PlayerStatus);
            }
        }


        if (PlayerStatus == PLAYER_STATUS.DRUNK)
        {
            if(lensDistor!=null)
            {
                FloatParameter fp = lensDistor.intensity;
                if(isDistortionAddingUp)
                {
                    fp.value += Random.Range(1, 5) * Time.deltaTime * 10;
                }
                else
                {
                    fp.value -= Random.Range(1, 5) * Time.deltaTime * 10;
                }
                if(fp.value>=90f)
                {
                    isDistortionAddingUp = false;
                }
                else if(fp.value<=-90f)
                {
                    isDistortionAddingUp = true;
                }
                lensDistor.intensity = fp;
            }
        }
        if(PlayerStatus == PLAYER_STATUS.TIRED && gameEndTimer== 0)
        {
            closeEyeTimer -= Time.deltaTime;
            if(closeEyeTimer<=0)
            {
                //handle close eyes
                Color color = FadeIn.color;
                color.a = 1;
                FadeIn.color = color;
                closeEyeTimer = Random.Range(2, 4);
                openEye = StartCoroutine(OpenEye());
            }
        }
        if(GameFailed)
        {
            Effect.SetActive(false);
            if (openEye != null) StopCoroutine(openEye);
            gameEndTimer += Time.deltaTime;
            Color color = FadeIn.color;
            color.a += 0.5f * Time.deltaTime;
            FadeIn.color = color;
            if(gameEndTimer>=10f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
