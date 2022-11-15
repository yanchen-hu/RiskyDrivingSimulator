using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEvent : MonoBehaviour
{
    public int DefaultChoice;
    private bool isEventEnd;
    public int playerChoice;
    public bool playerMadeChoice;
    private Coroutine currentChooseCoroutine;
    public bool IsEventEnd
    {
        get { return isEventEnd; }
        set { isEventEnd = value; }
    }

    public virtual bool IsRandomChoiceEnabled()
    {
        if (GameSettings.Instance.PlayerStatus == GameSettings.PLAYER_STATUS.DRUNK) return true;

        return false;
    }

    public virtual int GetDefaultChoice()
    {
        if (GameSettings.Instance.PlayerStatus != GameSettings.PLAYER_STATUS.NORMAL)
        {
            return 1;
        }
        return 0;
    }
    public virtual void HandleEventStart()
    {
        IsEventEnd = false;
        playerMadeChoice = false;
    }
    public virtual void HandleEventEnd()
    {
        EventManager.Instance.HandleCurrentEventEnd();
        IsEventEnd = true;
    }
    public virtual void HandleUpdate()
    {

    }

    public virtual void HandlePlayerChosen(int choice)
    {
        playerChoice = choice;
        playerMadeChoice = true;
    }
}
