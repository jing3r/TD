using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BuildMenu buildMenu;
    public int towerIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BuildManager.instance.IsMergeModeActive)
        {
            Node selectedNode = BuildManager.instance.GetSelectedNode();
            if (selectedNode != null)
            {
                selectedNode.ShowTooltip();
            }
        }
        else
        {
            buildMenu.ShowTooltip(towerIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buildMenu.HideTooltip();
    }
}