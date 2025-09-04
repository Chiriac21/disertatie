using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTouch : MonoBehaviour
{
    private int counter = 0;

    private void Awake()
    {
           counter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(counter);
    }

    public void CallIfTouchedStarted()
    {

        if (FindObjectOfType<DialogInit>().currentStage == DialogInit.Stages.Start && counter == 0)
        {
            counter++;
            FindObjectOfType<DialogInit>().NextPressed();
        }
        else
        { 
            counter = 0;
        }
    }
}
