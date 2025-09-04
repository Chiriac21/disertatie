using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToRosy : MonoBehaviour
{
    [SerializeField]
    private string uriRosy1;

    [SerializeField]
    private string uriRosy2;


    public void OpenControlRosy1()
    {
        UnityEngine.WSA.Launcher.LaunchUri(uriRosy1, false);
    }


    public void OpenControlRosy2()
    {
        UnityEngine.WSA.Launcher.LaunchUri(uriRosy2, false);
    }
}
