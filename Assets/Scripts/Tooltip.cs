using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Text tooltipText;
    public GameObject tooltipObject;

    public void ShowTooltip(string text)
    {
        tooltipObject.SetActive(true);
        tooltipText.text = text;
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}