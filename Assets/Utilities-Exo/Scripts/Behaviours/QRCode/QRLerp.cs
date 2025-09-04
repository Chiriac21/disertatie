
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.HandCoach;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogInit;

public class QRLerp : MonoBehaviour
{
    

    //Pozitii predefinite de deplasare a obiectelor cu care lucram
    [SerializeField]
    private Transform leftPartPosition, rightPartPosition, frontPartPosition,
        backPartPosition, conveyorPartPosition, conveyorDownScrewPosition, upPartsPosition;

    //Campuri in care vor fi introduse obiectele din scena cu care lucram
    [SerializeField] 
    private GameObject conveyorJos, conveyorLateral, conveyorSus, 
            surubOrizontalFata, surubOrizontalSpate, surubVerticalFata, surubVerticalSpate, 
            suruburiLateralDreaptaSus, suruburiLateralDreaptaJos, suruburiLateralStangaJos, 
            suruburiLateralStangaSus, tableAndKeys, initialPosition;

    //Campuri in care vor fi introduse Prefaburile obiectelor instantiate pe parcurs
    public GameObject lateralDreaptaPrefab, lateralStangaPrefab,
        fataPrefab, spatePrefab, conveyorPrefab;

    //Variabile folosite in aceasta clasa
    private GameObject instantiatedObj;
    private Stages currentStep;
    private bool isButtonPressed = false;
    private float desiredDuration = 3f;
    private float elapsedTime = 0;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 stopTablePos;

    public bool isSelected = false;
    public bool IsFirstTime = true;
    public void ButtonPressed()
    {
        isButtonPressed = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = initialPosition.transform.localPosition;
        endPosition = upPartsPosition.transform.localPosition;
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        CreateMovement();
    }

    private void CreateMovement()
    {
        currentStep = FindObjectOfType<DialogInit>().GetStage();

        elapsedTime += Time.deltaTime;
        var percentageComplete = elapsedTime / desiredDuration;

        if (currentStep == Stages.Start)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                percentageComplete = 0;
                IsFirstTime = false;
            }

            if (GameObject.Find("TableAndKeys(Clone)").transform.localPosition != startPosition)
                GameObject.Find("TableAndKeys(Clone)").transform.localPosition = Vector3.Lerp(new Vector3(0, 0, 0.00888f), startPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            else
                elapsedTime = 0;

            suruburiLateralDreaptaSus.SetActive(false);
        }
        else if (currentStep == Stages.Stage1UpArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                suruburiLateralDreaptaJos.SetActive(false);
                suruburiLateralDreaptaSus.SetActive(true);
                IsFirstTime = false;
            }
            suruburiLateralDreaptaSus.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire obiect
            Transform[] children = suruburiLateralDreaptaSus.GetComponentsInChildren<Transform>();
            children[4].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[8].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 90, 0), Quaternion.Euler(270, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(suruburiLateralDreaptaSus.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }

            if (Vector3.Distance(GameObject.Find("TableAndKeys(Clone)").transform.localPosition, new Vector3(0, 0, 0.00888f)) != 0f)
            {
                GameObject.Find("TableAndKeys(Clone)").transform.localPosition = Vector3.Lerp(startPosition, new Vector3(0, 0, 0.00888f), Mathf.SmoothStep(0, 1, percentageComplete));
            }
        }
        else if (currentStep == Stages.Stage1DownArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                suruburiLateralDreaptaJos.SetActive(true);
                suruburiLateralDreaptaSus.SetActive(false);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                IsFirstTime = false;
            }

            suruburiLateralDreaptaJos.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            //Rotire
            Transform[] children = suruburiLateralDreaptaJos.GetComponentsInChildren<Transform>();
            children[4].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[8].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 90, 0), Quaternion.Euler(270, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(suruburiLateralDreaptaJos.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage1Finish)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                suruburiLateralDreaptaJos.SetActive(false);
                suruburiLateralStangaSus.SetActive(false);
                startPosition = initialPosition.transform.localPosition;
                endPosition = rightPartPosition.localPosition;
                GameObject lateralDreapta = gameObject.transform.Find("Fete Laterale").gameObject;
                instantiatedObj = Instantiate(lateralDreaptaPrefab, lateralDreapta.transform);
                IsFirstTime = false;
            }

            instantiatedObj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(instantiatedObj.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = rightPartPosition.localPosition;
            }
        }
        else if (currentStep == Stages.Stage2UpArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                suruburiLateralStangaJos.SetActive(false);
                suruburiLateralStangaSus.SetActive(true);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                IsFirstTime = false;
            }

            suruburiLateralStangaSus.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            //Rotire
            Transform[] children = suruburiLateralStangaSus.GetComponentsInChildren<Transform>();
            children[4].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[8].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(180, 90, 0), Quaternion.Euler(90, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));


            if (Vector3.Distance(suruburiLateralStangaSus.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage2DownArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                suruburiLateralStangaSus.SetActive(false);
                suruburiLateralStangaJos.SetActive(true);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                IsFirstTime = false;
            }

            suruburiLateralStangaJos.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = suruburiLateralStangaJos.GetComponentsInChildren<Transform>();
            children[4].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[8].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(180, 90, 0), Quaternion.Euler(90, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(suruburiLateralStangaJos.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage2Finish)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                suruburiLateralStangaJos.SetActive(false);
                surubOrizontalFata.SetActive(false);
                startPosition = initialPosition.transform.localPosition;
                endPosition = leftPartPosition.localPosition;
                GameObject lateralStanga = gameObject.transform.Find("Fete Laterale").gameObject;
                instantiatedObj = Instantiate(lateralStangaPrefab, lateralStanga.transform);
                IsFirstTime = false;
            }

            instantiatedObj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(instantiatedObj.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = leftPartPosition.localPosition;
            }
        }
        else if (currentStep == Stages.Stage3UpArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                if (instantiatedObj != null) Destroy(instantiatedObj);
                surubVerticalFata.SetActive(false);
                surubOrizontalFata.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = frontPartPosition.transform.localPosition;
                IsFirstTime = false;
            }

            surubOrizontalFata.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = surubOrizontalFata.GetComponentsInChildren<Transform>();
            children[2].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[10].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(180, 0, 0), Quaternion.Euler(90, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(surubOrizontalFata.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = frontPartPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage3DownArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                surubOrizontalFata.SetActive(false);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                surubVerticalFata.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                IsFirstTime = false;
            }

            surubVerticalFata.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = surubVerticalFata.GetComponentsInChildren<Transform>();
            children[4].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[6].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(180, 90, 0), Quaternion.Euler(90, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(surubVerticalFata.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage3Finish)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                surubVerticalFata.SetActive(false);
                surubOrizontalSpate.SetActive(false);
                startPosition = initialPosition.transform.localPosition;
                endPosition = frontPartPosition.localPosition;
                GameObject fata = gameObject.transform.Find("FataSpate").gameObject;
                instantiatedObj = Instantiate(fataPrefab, fata.transform);
                IsFirstTime = false;
            }

            instantiatedObj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(instantiatedObj.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = frontPartPosition.localPosition;
            }
        }
        else if (currentStep == Stages.Stage4UpArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                if (instantiatedObj != null) Destroy(instantiatedObj);
                surubVerticalSpate.SetActive(false);
                surubOrizontalSpate.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = backPartPosition.transform.localPosition;
                IsFirstTime = false;
            }

            surubOrizontalSpate.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = surubOrizontalSpate.GetComponentsInChildren<Transform>();
            children[2].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[10].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(90, 180, 0), Quaternion.Euler(0, 180, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(surubOrizontalSpate.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = backPartPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage4DownArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                surubOrizontalSpate.SetActive(false);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                surubVerticalSpate.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                IsFirstTime = false;
            }

            surubVerticalSpate.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = surubVerticalSpate.GetComponentsInChildren<Transform>();
            children[4].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[6].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(90, 90, 0), Quaternion.Euler(0, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(surubVerticalSpate.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage4Finish)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                surubVerticalSpate.SetActive(false);
                conveyorSus.SetActive(false);
                startPosition = initialPosition.transform.localPosition;
                endPosition = backPartPosition.localPosition;
                GameObject spate = gameObject.transform.Find("FataSpate").gameObject;
                instantiatedObj = Instantiate(spatePrefab, spate.transform);
                IsFirstTime = false;
            }

            instantiatedObj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(instantiatedObj.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = backPartPosition.localPosition;
            }
        }
        else if (currentStep == Stages.Stage5UpArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                conveyorJos.SetActive(false);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                conveyorSus.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
                IsFirstTime = false;
            }

            conveyorSus.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire obiect
            Transform[] children = conveyorSus.GetComponentsInChildren<Transform>();
            children[6].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[10].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 90, 0), Quaternion.Euler(270, 90, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(conveyorSus.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = upPartsPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage5DownArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                conveyorSus.SetActive(false);
                conveyorLateral.SetActive(false);
                conveyorJos.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = conveyorDownScrewPosition.transform.localPosition;
                IsFirstTime = false;
            }

            conveyorJos.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = conveyorJos.GetComponentsInChildren<Transform>();
            children[6].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[18].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(180, 270, 0), Quaternion.Euler(270, 270, 0), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(conveyorJos.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = conveyorDownScrewPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage5SideArrows)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                conveyorJos.SetActive(false);
                if (instantiatedObj != null) Destroy(instantiatedObj);
                conveyorLateral.SetActive(true);
                startPosition = initialPosition.transform.localPosition;
                endPosition = conveyorDownScrewPosition.transform.localPosition;
                IsFirstTime = false;
            }

            conveyorLateral.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
            //Rotire
            Transform[] children = conveyorLateral.GetComponentsInChildren<Transform>();
            children[12].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(270, 0, 0), Mathf.SmoothStep(0, 1, percentageComplete));
            children[20].transform.localRotation = Quaternion.Lerp(Quaternion.Euler(180, 90, 270), Quaternion.Euler(180, 0, 270), Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(conveyorLateral.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = conveyorDownScrewPosition.transform.localPosition;
            }
        }
        else if (currentStep == Stages.Stage5Finish)
        {
            if (IsFirstTime == true)
            {
                elapsedTime = 0;
                conveyorLateral.SetActive(false);
                startPosition = initialPosition.transform.localPosition;
                endPosition = conveyorPartPosition.localPosition;
                GameObject conv = gameObject.transform.Find("Conveyor").gameObject;
                instantiatedObj = Instantiate(conveyorPrefab, conv.transform);
                IsFirstTime = false;
            }

            instantiatedObj.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));

            if (Vector3.Distance(instantiatedObj.transform.localPosition, endPosition) == 0f)
            {
                elapsedTime = 0;
                startPosition = initialPosition.transform.localPosition;
                endPosition = conveyorPartPosition.localPosition;
            }
        }
        else if (currentStep == Stages.Finish)
        {
            if (IsFirstTime == true)
            {
                if (instantiatedObj != null) Destroy(instantiatedObj);
                IsFirstTime = false;
            }
        }
    }
}



