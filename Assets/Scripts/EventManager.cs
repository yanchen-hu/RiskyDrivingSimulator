using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public enum EVENTS
    {
        RED_LIGHT,
        PEDESTRIAN,
        BRAKE,
        POLICE,
    }
    public int EventGenerateInterval;
    public int EventNumsInThisGame;
    public GameObject ChooseOverlay;
    public GameObject[] Choices;
    public GameObject Slider;
    public string[] ChoicesTextForRedLight;
    public string[] ChoicesTextForPedestrian;
    public string[] ChoicesTextForBrake;
    public string[] ChoicesTextForPolice;

    private BaseEvent[] eventInstances;
    private int roadTraveledSinceLastEvent;
    private int currentEvent;
    private ArrayList eventPool;
    private int eventDoneNum;
    private Coroutine currentChooseCoroutine;
    private bool shouldUpdateSlider;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        shouldUpdateSlider = false;
        roadTraveledSinceLastEvent = 0;
        currentEvent = -1;
        eventDoneNum = 0;
        eventPool = new ArrayList();
        //create a event pool to do the luck draw
        eventPool.Add(0);
        eventPool.Add(1);
        eventPool.Add(2);
        eventPool.Add(3);

        eventInstances = new BaseEvent[4];
        eventInstances[0] = this.GetComponent<RedLightEvent>();
        eventInstances[1] = this.GetComponent<PedestrianEvent>();
        eventInstances[2] = this.GetComponent<BrakeEvent>();
        eventInstances[3] = this.GetComponent<PoliceEvent>();
    }

    public void HandleCurrentEventEnd()
    {
        roadTraveledSinceLastEvent = MapManager.Instance.GetRoadNumGenerated();
        eventDoneNum++;
        if(eventDoneNum == EventNumsInThisGame)
        {
            GameSettings.Instance.HandleGameSuccess();
        }
    }

    IEnumerator HighLightAndCloseTheOverlay(int choice)
    {
        Choices[choice].GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(2f);
        Choices[choice].GetComponent<Image>().color = Color.white;
        shouldUpdateSlider = false;
        ChooseOverlay.SetActive(false);
    }

    IEnumerator TimeCountForPlayerChoose(float sec)
    {
        yield return new WaitForSeconds(sec);
        HandlePlayerChooseDefaultValue();
    }

    public void HandlePlayerChooseDefaultValue()
    {
        HandlePlayerChoice(eventInstances[currentEvent].GetDefaultChoice());
    }

    public void HandlePlayerChoice(int choice)
    {
        GameSettings.Instance.ResetFadeIn();
        if (currentChooseCoroutine != null)
        {
            StopCoroutine(currentChooseCoroutine);
        }
        if (eventInstances[currentEvent].IsRandomChoiceEnabled())
        {
            choice = Random.Range(0, 3);
        }
        StartCoroutine(HighLightAndCloseTheOverlay(choice));
        
        eventInstances[currentEvent].HandlePlayerChosen(choice);
        Time.timeScale = 1;
    }


    public void HandleOpenChooseOverlay()
    {
        SetupText();
        ChooseOverlay.SetActive(true);
        Time.timeScale = 0.1f;
        GameSettings.Instance.SetFadeInToUserChoice();
        shouldUpdateSlider = true;
        Slider.GetComponent<Slider>().value = 0;
        //here the reaction time should /10 because the time scale is 0.1
        currentChooseCoroutine = StartCoroutine(TimeCountForPlayerChoose(GameSettings.Instance.GetReactionTimeBaseOnCurrentStatus((EventManager.EVENTS)currentEvent)/10));
    }

    private void SetupText()
    {
        switch (currentEvent)
        {
            case 0:
                {
                    for(int i = 0;i<3;i++)
                    {
                        Choices[i].GetComponentInChildren<Text>().text = ChoicesTextForRedLight[i];
                    }
                    break;
                }
            case 1:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Choices[i].GetComponentInChildren<Text>().text = ChoicesTextForPedestrian[i];
                    }
                    break;
                }
            case 2:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Choices[i].GetComponentInChildren<Text>().text = ChoicesTextForBrake[i];
                    }
                    break;
                }
            case 3:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Choices[i].GetComponentInChildren<Text>().text = ChoicesTextForPolice[i];
                    }
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (eventDoneNum >= EventNumsInThisGame) return;

        //update slider
        if(shouldUpdateSlider)
        {
            Slider.GetComponent<Slider>().value += (1 / (GameSettings.Instance.GetReactionTimeBaseOnCurrentStatus((EventManager.EVENTS)currentEvent) / 10)) * Time.deltaTime;
        }

        //update event
        if (MapManager.Instance.GetRoadNumGenerated() - roadTraveledSinceLastEvent >= EventGenerateInterval 
            && (currentEvent==-1 || eventInstances[currentEvent].IsEventEnd))
        {
            int randomPick = Random.Range(0, eventPool.Count);
            currentEvent = (int)eventPool[randomPick];
            eventPool.RemoveAt(randomPick);
            eventInstances[currentEvent].HandleEventStart();
            
        }
        if(currentEvent!=-1 && !eventInstances[currentEvent].IsEventEnd)
        {
            eventInstances[currentEvent].HandleUpdate();
        }
    }
}
