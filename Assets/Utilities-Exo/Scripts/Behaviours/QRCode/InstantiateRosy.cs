using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRosy : MonoBehaviour
{
    [SerializeField]
    private GameObject rosyPrefab;

    private GameObject simplifiedRosy, instantiatedRosy, okButton;
    
    public GameObject mainMenu, welcomePanel;
    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GameObject.Find("HandMenu");
        simplifiedRosy = GameObject.Find("RosySimplified");
        okButton = GameObject.Find("OK");
        welcomePanel = GameObject.Find("WelcomePanel");
        okButton.SetActive(false);
        simplifiedRosy.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideAndInstantiate()
    {
        simplifiedRosy.SetActive(false);
        okButton.SetActive(false);
        instantiatedRosy = Instantiate(rosyPrefab, gameObject.transform);
        instantiatedRosy.transform.localPosition = simplifiedRosy.transform.localPosition;
        instantiatedRosy.transform.localRotation = simplifiedRosy.transform.localRotation;
    }

    public void ActivateSimplifiedRosy() 
    {
        mainMenu.SetActive(false);
        welcomePanel.SetActive(false);
        simplifiedRosy.transform.localPosition = Camera.main.transform.position + (new Vector3(0, -0.5f, 1.5f));
        simplifiedRosy.transform.localRotation = Quaternion.Euler(0, 90, 0);
        simplifiedRosy.SetActive(true);
        okButton.SetActive(true);
    }

}
