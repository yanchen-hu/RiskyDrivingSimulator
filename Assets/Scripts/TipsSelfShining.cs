using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsSelfShining : MonoBehaviour
{
    private Text text;
    private bool isGoingUp;
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<Text>();
        isGoingUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        Color co = text.color;
        if (isGoingUp)
        {
            co.a += Time.deltaTime;
            text.color = co;
        }
        else
        {
            co.a -= Time.deltaTime;
            text.color = co;
        }
        if (co.a >= 1) isGoingUp = false;
        else if (co.a <= 0) isGoingUp = true;
    }
}
