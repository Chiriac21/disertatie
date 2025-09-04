using Microsoft.MixedReality.Toolkit.UI.HandCoach;
using QRTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QRLerp;



public class DialogInit : MonoBehaviour
{
    public enum Stages
    {
        Start = 0, Stage1UpArrows, Stage1DownArrows, Stage1Finish,
        Stage2UpArrows, Stage2DownArrows, Stage2Finish,
        Stage3UpArrows, Stage3DownArrows, Stage3Finish,
        Stage4UpArrows, Stage4DownArrows, Stage4Finish,
        Stage5UpArrows, Stage5DownArrows, Stage5SideArrows, Stage5Finish, Finish
    }

    public bool IsDetected = false;
    public bool IsInstantiated = false;

    private int stageCount;

    [SerializeField]
    private QRCodesManager QRManager;
    private GameObject instantiatedArrow, instantiatedPanel, instantiatedPDF,
            instantiatedTableAndKeys;
    public Stages currentStage;


    //prefabs to instantiate
    public GameObject panelPrefab, pdfDocument, tableAndKeys;
    public GameObject Stage1UpArrowsPrefab, Stage1DownArrowsPrefab,
        Stage2UpArrowsPrefab, Stage2DownArrowsPrefab,
        Stage3UpArrowsPrefab, Stage3DownArrowsPrefab,
        Stage4UpArrowsPrefab, Stage4DownArrowsPrefab,
        Stage5UpArrowsPrefab, Stage5DownArrowsPrefab, Stage5SideArrowsPrefab;

    // Start is called before the first frame update
    void Start()
    {
        stageCount = 0;
        currentStage = Stages.Start;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDetected)
        {
            InitStage(StageSelector(stageCount));
        }

    }


    public void QRDetected()
    {
        IsDetected = true;
        stageCount = 0;
        transform.parent.gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("MainMenu").SetActive(false);
        FindObjectOfType<InstantiateRosy>().welcomePanel.SetActive(false);
        QRManager.StopQRTracking();
    }

    private void InitStage(Stages stage)
    {
        if (IsInstantiated == false)
        {
            
            if (stage == Stages.Start)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                if (instantiatedPanel != null) Destroy(instantiatedPanel);
                if (instantiatedPDF != null) Destroy(instantiatedPDF);

                instantiatedPanel = Instantiate(panelPrefab, gameObject.transform);
                instantiatedPDF = Instantiate(pdfDocument, gameObject.transform);
                if (instantiatedTableAndKeys == null) instantiatedTableAndKeys = Instantiate(tableAndKeys, gameObject.transform);
                IsInstantiated = true;
            }
            else if (stage == Stages.Stage1UpArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);

                instantiatedArrow = Instantiate(Stage1UpArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage1DownArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage1DownArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage1Finish)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                IsInstantiated = true;
                //lerp fata laterala
            }
            else if (stage == Stages.Stage2UpArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage2UpArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage2DownArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage2DownArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage2Finish)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                IsInstantiated = true;
                //lerp fata laterala
            }
            else if (stage == Stages.Stage3UpArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage3UpArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage3DownArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage3DownArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage3Finish)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                IsInstantiated = true;
                //lerp fata/spate
            }
            else if (stage == Stages.Stage4UpArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage4UpArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage4DownArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage4DownArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage4Finish)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                IsInstantiated = true;
                //lerp fata/spate
            }
            else if (stage == Stages.Stage5DownArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage5DownArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage5SideArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage5SideArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage5UpArrows)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                instantiatedArrow = Instantiate(Stage5UpArrowsPrefab, gameObject.transform);
                IsInstantiated = true;
                //lerp surub
            }
            else if (stage == Stages.Stage5Finish)
            {
                if (instantiatedArrow != null) Destroy(instantiatedArrow);
                IsInstantiated = true;
                //lerp conv
            }
            else if (stage == Stages.Finish)
            {
                IsInstantiated = true;
                //panel
            }
        }
    }

    public void NextPressed()
    {
        stageCount++;
        if (stageCount > 17)
            stageCount = 17;
        FindObjectOfType<QRLerp>().IsFirstTime = true;
        IsInstantiated = false;
    }

    public void BackPressed()
    {
        stageCount--;
        if (stageCount < 0)
            stageCount = 0;
        FindObjectOfType<QRLerp>().IsFirstTime = true;
        IsInstantiated = false;
    }

    public void ExitPressed()
    {
        FindObjectOfType<QRCodesManager>().IsExitPressed = true;
        Destroy(transform.parent.gameObject);
    }

    private Stages StageSelector(int counter)
    {
        switch (counter)
        {
            case 0:
                currentStage = Stages.Start;
                break;
            case 1:
                currentStage = Stages.Stage1UpArrows;
                break;
            case 2:
                currentStage = Stages.Stage1DownArrows;
                break;
            case 3:
                currentStage = Stages.Stage1Finish;
                break;
            case 4:
                currentStage = Stages.Stage2UpArrows;
                break;
            case 5:
                currentStage = Stages.Stage2DownArrows;
                break;
            case 6:
                currentStage = Stages.Stage2Finish;
                break;
            case 7:
                currentStage = Stages.Stage3UpArrows;
                break;
            case 8:
                currentStage = Stages.Stage3DownArrows;
                break;
            case 9:
                currentStage = Stages.Stage3Finish;
                break;
            case 10:
                currentStage = Stages.Stage4UpArrows;
                break;
            case 11:
                currentStage = Stages.Stage4DownArrows;
                break;
            case 12:
                currentStage = Stages.Stage4Finish;
                break;
            case 13:
                currentStage = Stages.Stage5UpArrows;
                break;
            case 14:
                currentStage = Stages.Stage5DownArrows;
                break;
            case 15:
                currentStage = Stages.Stage5SideArrows;
                break;
            case 16:
                currentStage = Stages.Stage5Finish;
                break;
            case 17:
                currentStage = Stages.Finish;
                break;
        }
        return currentStage;
    }

    public Stages GetStage()
    {
        return currentStage;
    }

}
