
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGesture : MonoBehaviour
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

    public bool isSelected = false;
    private bool gestureActivated = false;
    private bool isButtonPressed = false;
    private Vector3 initialTransformPosition;

    private Vector3 initialRight;
    private Vector3 initialLeft;

    private string currentState = "SuruburiLaterale";
    private float desiredDuration = 3f;
    private float elapsedTime = 0;
    private double distanceRight;
    private double distanceLeft;
    private double distance;

    private Vector3 leftPositionEnd;
    private Vector3 rightPositionEnd;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private GameObject instantiatedRosy;
    private GameObject mainMenu;



    [SerializeField]
    private TrackedHandJoint trackedHandJoint = TrackedHandJoint.IndexTip;

    [SerializeField]
    private Handedness trackedHand = Handedness.Both;

    private IMixedRealityHandJointService handJointService;

    private IMixedRealityHandJointService HandJointService =>
        handJointService ??
        (handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>());

    private MixedRealityPose? previousLeftHandPose;

    private MixedRealityPose? previousRightHandPose;

    private MixedRealityPose? leftHandPose;
    private MixedRealityPose? rightHandPose;

    private MixedRealityPose? GetHandPose(Handedness hand)
    {
        if ((trackedHand & hand) == hand)
        {
            if (HandJointService.IsHandTracked(hand))
            {
                var jointTransform = HandJointService.RequestJointTransform(trackedHandJoint, hand);
                return new MixedRealityPose(jointTransform.position);
            }

        }
        return null;
    }
    public void ButtonPressed()
    {
        instantiatedRosy = Instantiate(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        isButtonPressed = true;
        startPosition = dezasamblare.transform.localPosition;
        endPosition = upPartsPosition.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
        {
        if (isButtonPressed == true)
        {
            if (gestureActivated == false)
            {
                CalculateHandsDistance();

                CheckHandsDistance();
            }
            if (isSelected == true)
            {
                if (leftHandPose != null && rightHandPose != null)
                { 
                    distanceLeft = Vector3.Distance(leftHandPose.Value.Position, initialLeft);
                    distanceRight = Vector3.Distance(rightHandPose.Value.Position, initialRight);
                }

                if (distanceRight > 0.05 && distanceLeft > 0.05 && gestureActivated == false)
                {
                    gestureActivated = true;
                }
                if (gestureActivated == true)
                {
                    switch (currentState)
                    {
                        case "SuruburiLaterale":
                            {

                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                suruburiPartiLaterale.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(suruburiPartiLaterale.transform.localPosition, endPosition) == 0f)
                                {
                                    suruburiPartiLaterale.SetActive(false);

                                    elapsedTime = 0;
                                    MoveLeftPartPosition();
                                    leftPositionEnd = endPosition;
                                    MoveRightPartPosition();
                                    rightPositionEnd = endPosition;
                                    currentState = "PartiLaterale";
                                }
                                break;
                            }
                        case "PartiLaterale":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                parteaLateralaStanga.transform.localPosition = Vector3.Lerp(startPosition, leftPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                parteaLateralaDreapta.transform.localPosition = Vector3.Lerp(startPosition, rightPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(parteaLateralaStanga.transform.localPosition, leftPositionEnd) == 0f
                                    && Vector3.Distance(parteaLateralaDreapta.transform.localPosition, rightPositionEnd) == 0f)
                                {
                                    parteaLateralaStanga.SetActive(false);
                                    parteaLateralaDreapta.SetActive(false);

                                    elapsedTime = 0;
                                    MoveBackPartPosition();
                                    leftPositionEnd = endPosition;
                                    MoveFrontPartPosition();
                                    rightPositionEnd = endPosition;
                                    currentState = "SuruburiOrizontale";
                                }
                                break;
                            }
                        case "SuruburiOrizontale":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                surubOrizontalSpate.transform.localPosition = Vector3.Lerp(startPosition, leftPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                surubOrizontalFata.transform.localPosition = Vector3.Lerp(startPosition, rightPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                               
                                if (Vector3.Distance(surubOrizontalSpate.transform.localPosition, leftPositionEnd) == 0f
                                    && Vector3.Distance(surubOrizontalFata.transform.localPosition, rightPositionEnd) == 0f)
                                {
                                    surubOrizontalSpate.SetActive(false);
                                    surubOrizontalFata.SetActive(false);

                                    elapsedTime = 0;
                                    MoveUpPartsPosition();
                                    currentState = "SuruburiVerticale";
                                }
                                break;
                            }
                        case "SuruburiVerticale":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                surubVertical.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(surubVertical.transform.localPosition, endPosition) == 0f)
                                {
                                    surubVertical.SetActive(false);

                                    elapsedTime = 0;
                                    MoveBackPartPosition();
                                    leftPositionEnd = endPosition;
                                    MoveFrontPartPosition();
                                    rightPositionEnd = endPosition;
                                    currentState = "PartiFataSpate";
                                }
                                break;
                            }
                        case "PartiFataSpate":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                parteaSpate.transform.localPosition = Vector3.Lerp(startPosition, leftPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                parteaFata.transform.localPosition = Vector3.Lerp(startPosition, rightPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(parteaSpate.transform.localPosition, leftPositionEnd) == 0f
                                    && Vector3.Distance(parteaFata.transform.localPosition, rightPositionEnd) == 0f)
                                {
                                    parteaSpate.SetActive(false);
                                    parteaFata.SetActive(false);

                                    elapsedTime = 0;
                                    MoveLeftPartPosition();
                                    leftPositionEnd = endPosition;
                                    MoveRightPartPosition();
                                    rightPositionEnd = endPosition;
                                    currentState = "ConveyorLateral";
                                }
                                break;
                            }
                        case "ConveyorLateral":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                conveyorLateralStanga.transform.localPosition = Vector3.Lerp(startPosition, leftPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                conveyorLateralDreapta.transform.localPosition = Vector3.Lerp(startPosition, rightPositionEnd, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(conveyorLateralStanga.transform.localPosition, leftPositionEnd) == 0f
                                    && Vector3.Distance(conveyorLateralDreapta.transform.localPosition, rightPositionEnd) == 0f)
                                {
                                    conveyorLateralStanga.SetActive(false);
                                    conveyorLateralDreapta.SetActive(false);

                                    elapsedTime = 0;
                                    MoveConveyorDownScrewPosition();
                                    currentState = "ConveyorJos";
                                }
                                break;
                            }
                        case "ConveyorJos":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                conveyorJos.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(conveyorJos.transform.localPosition, endPosition) == 0f)
                                {
                                    conveyorJos.SetActive(false);

                                    elapsedTime = 0;
                                    MoveConveyorPartPosition();
                                    currentState = "ConveyorPrincipal";
                                }
                                break;
                            }
                        case "ConveyorPrincipal":
                            {
                                elapsedTime += Time.deltaTime;
                                var percentageComplete = elapsedTime / desiredDuration;

                                conveyorSus.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
                                conveyorPrincipal.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, percentageComplete));
                                
                                if (Vector3.Distance(conveyorSus.transform.localPosition, endPosition) == 0f
                                    && Vector3.Distance(conveyorPrincipal.transform.localPosition, endPosition) == 0f)
                                {
                                    conveyorSus.SetActive(false);
                                    conveyorPrincipal.SetActive(false);

                                    elapsedTime = 0;
                                    currentState = null;
                                    isButtonPressed = false;
                                }
                                break;
                            }
                    }
                }

            }

        }
    }

    public void MoveLeftPartPosition()
    {
        endPosition = leftPartPosition.localPosition;
    }

    public void MoveRightPartPosition()
    {
        endPosition = rightPartPosition.localPosition;
    }

    public void MoveFrontPartPosition()
    {
        endPosition = frontPartPosition.localPosition;
    }

    public void MoveBackPartPosition()
    {
        endPosition = backPartPosition.localPosition;
    }

    public void MoveConveyorPartPosition()
    {
        endPosition = conveyorPartPosition.localPosition;
    }

    public void MoveConveyorDownScrewPosition()
    {
        endPosition = conveyorDownScrewPosition.localPosition;
    }

    public void MoveUpPartsPosition()
    {
        endPosition = upPartsPosition.localPosition;
    }

    private void CalculateHandsDistance()
    {
        leftHandPose = GetHandPose(Handedness.Left);
        rightHandPose = GetHandPose(Handedness.Right);

        if (leftHandPose != null && rightHandPose != null)
        {
            distance = Vector3.Distance(rightHandPose.Value.Position, leftHandPose.Value.Position);
        }
        else if (leftHandPose == null && rightHandPose == null)
        {
            isSelected = false;
            distance = 100;

            Debug.Log("Gesture Restarted");
        }
    }

    private void CheckHandsDistance() 
    {
        if (distance < 0.02 && isSelected == false)
        {
            isSelected = true;

            Debug.Log("Gesture Triggered");

            initialRight = rightHandPose.Value.Position;
            initialLeft = leftHandPose.Value.Position;

        }
    }

    public void ResetRosy()
    {
        if (!isButtonPressed)
        {
            conveyorJos.transform.localPosition = startPosition;
            conveyorLateralStanga.transform.localPosition = startPosition;
            conveyorLateralDreapta.transform.localPosition = startPosition;
            conveyorSus.transform.localPosition = startPosition;
            conveyorPrincipal.transform.localPosition = startPosition;
            parteaFata.transform.localPosition = startPosition;
            parteaSpate.transform.localPosition = startPosition;
            surubOrizontalFata.transform.localPosition = startPosition;
            surubOrizontalSpate.transform.localPosition = startPosition;
            surubVertical.transform.localPosition = startPosition;
            parteaLateralaStanga.transform.localPosition = startPosition;
            parteaLateralaDreapta.transform.localPosition = startPosition;
            suruburiPartiLaterale.transform.localPosition = startPosition;

            conveyorJos.SetActive(true);
            conveyorLateralStanga.SetActive(true);
            conveyorLateralDreapta.SetActive(true);
            conveyorSus.SetActive(true);
            conveyorPrincipal.SetActive(true);
            parteaFata.SetActive(true);
            parteaSpate.SetActive(true);
            surubOrizontalFata.SetActive(true);
            surubOrizontalSpate.SetActive(true);
            surubVertical.SetActive(true);
            parteaLateralaStanga.SetActive(true);
            parteaLateralaDreapta.SetActive(true);
            suruburiPartiLaterale.SetActive(true);
        }
    }

    public void ExitDissasemble()
    {
        if (gameObject != null)
        {
            FindObjectOfType<InstantiateRosy>().mainMenu.SetActive(true);
            FindObjectOfType<InstantiateRosy>().welcomePanel.SetActive(true);
            Destroy(gameObject);
        }
    }

}



