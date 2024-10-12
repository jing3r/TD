using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject classicMenu;
    public BuildMenu buildMenu;
    public NodeUI nodeUI;

    void Start()
    {
        ShowMainMenu();
        Time.timeScale = 0f;
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        classicMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ShowClassicMenu()
    {
        mainMenu.SetActive(false);
        classicMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SelectFaction(string faction)
    {
        Debug.Log("Classic mode enabled. Selected faction: " + faction);
        buildMenu.SetFactionTowers(faction);
        nodeUI.SetClassicMode();
        classicMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ChooseMastermindMode()
    {
        Debug.Log("Mastermind mode enabled");
        buildMenu.SetMastermindTowers();
        nodeUI.SetMastermindMode();
        mainMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ChooseMergeMode()
    {
        Debug.Log("Merge mode enabled");
        buildMenu.SetMergeMode();
        nodeUI.SetMergeMode();
        mainMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoBack()
    {
        ShowMainMenu();
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}