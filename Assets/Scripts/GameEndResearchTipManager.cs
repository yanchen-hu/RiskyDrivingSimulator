using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndResearchTipManager : MonoBehaviour
{
    public static GameEndResearchTipManager Instance;

    public string[] TipsForNormalDrive;
    public string[] TipsForDrunkDrive;
    public string[] TipsForTiredDrive;
    public string[] TipsForRacing;

    public GameObject Overlay;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void HandleShowEndGameTips(GameSettings.PLAYER_STATUS status)
    {
        Overlay.SetActive(true);
        string[] tipsToShow = TipsForNormalDrive;
        switch (status)
        {
            case GameSettings.PLAYER_STATUS.DRUNK:
                tipsToShow = TipsForDrunkDrive;
                break;
            case GameSettings.PLAYER_STATUS.TIRED:
                tipsToShow = TipsForTiredDrive;
                break;
            case GameSettings.PLAYER_STATUS.RACING:
                tipsToShow = TipsForRacing;
                break;
        }
        Overlay.GetComponent<Text>().text = tipsToShow[Random.Range(0,tipsToShow.Length)];

    }
    // Start is called before the first frame update
    void Start()
    {
        Overlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
