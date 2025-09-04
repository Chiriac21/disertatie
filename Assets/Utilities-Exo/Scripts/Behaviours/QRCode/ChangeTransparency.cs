using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeTransparency : MonoBehaviour
{
    [SerializeField]
    private float trLvl;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        setAlpha(trLvl);
    }

    public void ToFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
    public void setAlpha(float alpha)
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        Color newColor;
        foreach (Transform child in children)
        {
            if (child.childCount == 0)
            {
                ToFadeMode(child.gameObject.GetComponent<MeshRenderer>().material);
                newColor = child.gameObject.GetComponent<MeshRenderer>().material.color;
                newColor.a = alpha;
                child.gameObject.GetComponent<MeshRenderer>().material.color = newColor;
            }
        }
    }
}
