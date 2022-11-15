using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsDontDestroy : MonoBehaviour
{
    public static GameSettingsDontDestroy Instance;
    public GameSettings.PLAYER_STATUS PlayerChosenStatus;
    private void Awake()
    {
        if (Instance != null) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void HandlePlayerChooseStatus(int stats)
    {
        PlayerChosenStatus = (GameSettings.PLAYER_STATUS)stats;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
