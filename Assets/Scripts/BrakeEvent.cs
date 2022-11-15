using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeEvent : BaseEvent
{
    public override void HandleEventStart()
    {
        base.HandleEventStart();
        MapManager.Instance.HandleEventSetup(EventManager.EVENTS.BRAKE);
        StartCoroutine(waitAndPopUpOverlay());
    }
    IEnumerator waitAndPopUpOverlay()
    {
        yield return new WaitForSeconds(1.5f);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandleObjMoveAway();
        yield return new WaitForSeconds(3);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandleObjSlowlyMoveAway();
        if (GameSettings.Instance.PlayerStatus != GameSettings.PLAYER_STATUS.RACING) yield return new WaitForSeconds(2f);
        EventManager.Instance.HandleOpenChooseOverlay();
    }
    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }
    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(1);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandleObjMoveAway();
        yield return new WaitForSeconds(4);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StartMoving();
        HandleEventEnd();
    }
    public override void HandlePlayerChosen(int choice)
    {
        base.HandlePlayerChosen(choice);
        if (choice == 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StopImmediately();
            StartCoroutine(StartMoving());
        }
        else
        {
            GameSettings.Instance.HandleGameFailed();
        }

    }
}
