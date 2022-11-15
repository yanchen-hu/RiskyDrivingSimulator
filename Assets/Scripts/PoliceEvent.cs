using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceEvent : BaseEvent
{
    public override void HandleEventStart()
    {
        base.HandleEventStart();
        MapManager.Instance.HandleEventSetup(EventManager.EVENTS.POLICE);
        GameSettings.Instance.HandlePoliceChasingAudio();
        StartCoroutine(waitAndPopUpOverlay());
    }
    IEnumerator waitAndPopUpOverlay()
    {
        yield return new WaitForSeconds(2);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandlePoliceCarFollow();
        yield return new WaitForSeconds(2);
        EventManager.Instance.HandleOpenChooseOverlay();
    }
    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }
    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(3);
        GameSettings.Instance.HandlePoliceAskingAudio();
        yield return new WaitForSeconds(7);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandlePoliceCarDeactive();
        yield return new WaitForSeconds(2);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StartMoving();
        HandleEventEnd();
    }
    public override void HandlePlayerChosen(int choice)
    {
        base.HandlePlayerChosen(choice);
        if (choice == 0 && GameSettings.Instance.PlayerStatus == GameSettings.PLAYER_STATUS.NORMAL)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopImmediately();
            MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandlePlayPoliceAudio();
            StartCoroutine(StartMoving());
        }
        else
        {
            GameSettings.Instance.HandleGameFailed();
        }

    }
}
