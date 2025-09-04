using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform leftPartPosition;
    [SerializeField] private Transform rightPartPosition;
    [SerializeField] private Transform frontPartPosition;
    [SerializeField] private Transform backPartPosition;
    [SerializeField] private Transform conveyorPartPosition;
    [SerializeField] private Transform conveyorDownScrewPosition;
    [SerializeField] private Transform upPartsPosition;

    [SerializeField] private GameObject conveyorJos;
    [SerializeField] private GameObject conveyorLateralStanga;
    [SerializeField] private GameObject conveyorLateralDreapta;
    [SerializeField] private GameObject conveyorSus;
    [SerializeField] private GameObject conveyorPrincipal;
    [SerializeField] private GameObject parteaFata;
    [SerializeField] private GameObject parteaSpate;
    [SerializeField] private GameObject surubOrizontalFata;
    [SerializeField] private GameObject surubOrizontalSpate;
    [SerializeField] private GameObject surubVertical;
    [SerializeField] private GameObject parteaLateralaStanga;
    [SerializeField] private GameObject parteaLateralaDreapta;
    [SerializeField] private GameObject suruburiPartiLaterale;
    [SerializeField] private GameObject dezasamblare;


    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance!=null && Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public Transform GetLeftPartPosition()
    {
        return leftPartPosition;
    }

    public Transform GetRightPartPosition()
    {
        return rightPartPosition;
    }

    public Transform GetFrontPartPosition()
    {
        return frontPartPosition;
    }

    public Transform GetBackPartPosition()
    {
        return backPartPosition;
    }

    public Transform GetConveyorPartPosition()
    {
        return conveyorPartPosition;
    }

    public Transform GetConveyorDownScrewPosition()
    {
        return conveyorDownScrewPosition;
    }

    public Transform GetUpPartsPosition()
    {
        return upPartsPosition;
    }

    public GameObject GetConveyorJos()
    {
        return conveyorJos;
    }

    public GameObject GetConveyorLateralStanga()
    {
        return conveyorLateralStanga;
    }

    public GameObject GetConveyorLateralDreapta()
    {
        return conveyorLateralDreapta;
    }

    public GameObject GetConveyorSus()
    {
        return conveyorSus;
    }

    public GameObject GetConveyorPrincipal()
    {
        return conveyorPrincipal;
    }

    public GameObject GetParteaFata()
    {
        return parteaFata;
    }

    public GameObject GetParteaSpate()
    {
        return parteaSpate;
    }

    public GameObject GetSurubOrizontalFata()
    {
        return surubOrizontalFata;
    }

    public GameObject GetSurubOrizontalSpate()
    {
        return surubOrizontalSpate;
    }

    public GameObject GetSurubVertical()
    {
        return surubVertical;
    }

    public GameObject GetParteaLateralaStanga()
    {
        return parteaLateralaStanga;
    }

    public GameObject GetParteaLateralaDreapta()
    {
        return parteaLateralaDreapta;
    }

    public GameObject GetSuruburiPartiLaterale()
    {
        return suruburiPartiLaterale;
    }

    public GameObject GetDezasamblare()
    {
        return dezasamblare;
    }



}
