using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSetup : MonoBehaviour
{
    public GameObject Light;
    public GameObject[] Cars;
    public Transform CarTargetPos;
    public Transform HumanTargetPos;
    public Transform[] BrakeTargetPos;
    public GameObject Human;
    public GameObject BrakeCar;
    private bool Move;
    private bool isSlowMove;
    private int brakeCarIndex;
    public bool isPoliceCar;
    private bool hasPlayedCrashAudio;
    // Start is called before the first frame update
    void Start()
    {
        Move = false;
        isSlowMove = false;
        brakeCarIndex = 0;
        hasPlayedCrashAudio = false;
        if (Human!=null)
        {
            HandlePedestrianMove();
        }
    }
    public void StartChangingLight()
    {
        Tiled_Texture_Animation[] scripts = Light.GetComponentsInChildren<Tiled_Texture_Animation>();
        for(int i = 0;i<scripts.Length;i++)
        {
            scripts[i]._fps = 2;
        }
    }

    public void HandleCarsDriveAway()
    {
        Move = true;
    }

    public void HandlePedestrianMove()
    {
        Move = true;
        Human.GetComponentInChildren<Animator>().SetBool("StartWalk", true);
    }

    public void HandlePedestrianStopMove()
    {
        Human.GetComponentInChildren<Animator>().SetBool("StartWalk", false);
    }

    public void HandlePedestrianRun()
    {
        Human.GetComponentInChildren<Animator>().SetBool("Run", true);
    }

    public void HandleObjSlowlyMoveAway()
    {
        Move = true;
        isSlowMove = true;
    }
    public void HandleObjMoveAway()
    {
        Move = true;
        isSlowMove = false;
    }

    public void HandlePoliceCarFollow()
    {
        Move = true;
        isSlowMove = false;
        isPoliceCar = true;
    }
    public void HandlePoliceCarDeactive()
    {
        isPoliceCar = true;
        BrakeCar.SetActive(false);
    }
    public void HandlePlayPoliceAudio()
    {
        //play police audio
    }

    public float GetLightPos()
    {
        return Light.transform.position.z;
    }


    // Update is called once per frame
    void Update()
    {
        if(!hasPlayedCrashAudio)
        {
            if (Human != null)
            {
                if (GameSettings.Instance.GameFailed && Vector3.Distance(Human.transform.position,GameObject.FindGameObjectWithTag("Player").transform.position)<=8)
                {
                    hasPlayedCrashAudio = true;
                    GameSettings.Instance.HandlePlayCarCrashHumanAudio();
                }
            }
            else if(Cars.Length!=0)
            {
                if(GameSettings.Instance.GameFailed)
                {
                    for (int i = 0; i < Cars.Length; i++)
                    {
                        if (Vector3.Distance(Cars[i].transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= 15)
                        {
                            hasPlayedCrashAudio = true;
                            GameSettings.Instance.HandlePlayCarCrashAudio();
                            break;
                        }
                    }
                }
                
            }
            else if(BrakeCar && !isPoliceCar)
            {
                if (GameSettings.Instance.GameFailed && Vector3.Distance(BrakeCar.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= 10)
                {
                    hasPlayedCrashAudio = true;
                    GameSettings.Instance.HandlePlayCarCrashAudio();
                }
            }

        }

        if(Move)
        {
            if(Cars.Length!=0)
            {
                for (int i = 0; i < Cars.Length; i++)
                {
                    Vector3 targetP = Cars[i].transform.position;
                    targetP.x = CarTargetPos.position.x;
                    Cars[i].transform.position = Vector3.MoveTowards(Cars[i].transform.position, targetP, 3 * Time.deltaTime);
                    if (Vector3.Distance(Cars[i].transform.position, targetP) <= 1)
                    {
                        Cars[i].SetActive(false);
                    }
                }
            }
            if(Human!=null)
            {
                if (Vector3.Distance(Human.transform.position, HumanTargetPos.position) <= 1)
                {
                    Human.SetActive(false);
                }
            }
            if (BrakeCar != null)
            {
                if (brakeCarIndex >= BrakeTargetPos.Length)
                {
                    BrakeCar.SetActive(false);
                }
                else
                {
                    if(isPoliceCar)
                    {
                        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
                        playerPos.z -= 3;
                        BrakeCar.transform.position = playerPos;
                    }
                    else
                    {
                        float speed = isSlowMove ? 0.75f : 15;
                        BrakeCar.transform.position = Vector3.MoveTowards(BrakeCar.transform.position, BrakeTargetPos[brakeCarIndex].position, speed * Time.deltaTime);
                        Vector3 lookAtPos = BrakeTargetPos[brakeCarIndex].position;
                        lookAtPos.y = BrakeCar.transform.position.y;
                        BrakeCar.transform.LookAt(lookAtPos);
                        if (Vector3.Distance(BrakeCar.transform.position, BrakeTargetPos[brakeCarIndex].position) <= 1)
                        {
                            brakeCarIndex++;
                        }
                    }
                   
                }
               
            }
        }
        
    }
}
