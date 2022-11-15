using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool overrideMove;
    private float tarPositionZ;
    private float speedRatio;
    private bool stop;
    // Start is called before the first frame update
    void Start()
    {
        overrideMove = false;
        tarPositionZ = 0;
        stop = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (stop) return;
        if (overrideMove)
        {
            speedRatio -= (0.1f * Time.deltaTime);
            this.GetComponent<CharacterController>().Move(new Vector3(0, 0, -GameSettings.Instance.PlayerSpeed * Time.deltaTime * speedRatio));
            if (Mathf.Abs(this.transform.position.z - tarPositionZ) < 1f)
            {
                stop = true;
            }
        }
        else
        {
            float speed = GameSettings.Instance == null ? -50 : -GameSettings.Instance.PlayerSpeed;
            this.GetComponent<CharacterController>().Move(new Vector3(0, 0, speed * Time.deltaTime));
        }
    }

    public void MoveToTargetPositionAndStop(float z)
    {
        overrideMove = true;
        tarPositionZ = z;
        speedRatio = 1;
    }

    public void StartMoving()
    {
        overrideMove = false;
        stop = false;
    }
    public void StopImmediately()
    {
        overrideMove = false;
        stop = true;
    }
}
