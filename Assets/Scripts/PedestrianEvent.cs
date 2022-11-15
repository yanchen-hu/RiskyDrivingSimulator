using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianEvent : BaseEvent
{
    public override void HandleEventStart()
    {
        base.HandleEventStart();
        MapManager.Instance.HandleEventSetup(EventManager.EVENTS.PEDESTRIAN);
        StartCoroutine(waitAndPopUpOverlay());
    }
    IEnumerator waitAndPopUpOverlay()
    {
        yield return new WaitForSeconds(3);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandlePedestrianStopMove();
        yield return new WaitForSeconds(1);
        EventManager.Instance.HandleOpenChooseOverlay();
    }
    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }
    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(3);
        MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().HandlePedestrianRun();
        yield return new WaitForSeconds(4);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().StartMoving();
        HandleEventEnd();
    }
    public override void HandlePlayerChosen(int choice)
    {
        base.HandlePlayerChosen(choice);
        if (choice == 0)
        {
            float targetZ = MapManager.Instance.GetCurrentIntersectionRoad().GetComponent<RoadSetup>().GetLightPos();
            targetZ += 15;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MoveToTargetPositionAndStop(targetZ);
            StartCoroutine(StartMoving());
        }
        else
        {
            GameSettings.Instance.HandleGameFailed();
        }

    }
}
