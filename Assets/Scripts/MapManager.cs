using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public GameObject[] StraightRoadPrefabs;
    //0 - redlight 1- pedestrain
    public GameObject[] IntersectionPrefabs;
    public GameObject ConstructionParent;
    public int InitRoadSize;

    public bool ShouldNextRoadIntersection;
    private float currentRoadMidPointZ;
    private const float RoadSize = 50f;
    private GameObject[] RoadList;
    private GameObject player;
    private float posLastGeneratingRoad;
    private GameObject currentIntersectionRoad;
    private int roadGenerated;
    private EventManager.EVENTS nextEvent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        posLastGeneratingRoad = player.transform.position.z;
        RoadList = new GameObject[InitRoadSize];
        currentRoadMidPointZ = RoadSize;
        for(int i = 0;i<InitRoadSize;i++)
        {
            RoadList[i] = GenerateNewRoad();
        }
        ShouldNextRoadIntersection = false;
        roadGenerated = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceTraveled = posLastGeneratingRoad - player.transform.position.z;
        if (distanceTraveled > RoadSize)
        {
            posLastGeneratingRoad -= RoadSize;
            Destroy(RoadList[0]);
            for(int i = 0;i< InitRoadSize-1;i++)
            {
                RoadList[i] = RoadList[i + 1];
            }
            RoadList[InitRoadSize - 1] = GenerateNewRoad();
            roadGenerated++;
        }
    }

    public int GetRoadNumGenerated()
    {
        return roadGenerated;
    }

    public GameObject GenerateNewRoad()
    {
        int index = ShouldNextRoadIntersection ? (int)nextEvent : Random.Range(0, StraightRoadPrefabs.Length);
        GameObject obj = ShouldNextRoadIntersection? Instantiate(IntersectionPrefabs[index]):Instantiate(StraightRoadPrefabs[index]);
        obj.transform.position = new Vector3(0, 0, currentRoadMidPointZ - RoadSize);
        obj.transform.parent = ConstructionParent.transform;
        currentRoadMidPointZ -= RoadSize;
        if(ShouldNextRoadIntersection)
        {
            currentIntersectionRoad = obj;
        }
        ShouldNextRoadIntersection = false;
        return obj;
    }


    public void HandleEventSetup(EventManager.EVENTS evt)
    {
        ShouldNextRoadIntersection = true;
        nextEvent = evt;
    }

    public GameObject GetCurrentIntersectionRoad()
    {
        return currentIntersectionRoad;
    }
}
