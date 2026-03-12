using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject Menu;
    public GameObject Single;
    public GameObject All;
    public GameObject Entry;
    public GameObject Edit;
    public GameObject Delete;

    public GameObject currentMenu;


    
    public void MainMenu()
    {
        currentMenu.SetActive(false);
        Menu.SetActive(true);
        currentMenu = Menu;
    }

    public void SingleMenu()
    {
        currentMenu.SetActive(false);
        Single.SetActive(true);
        currentMenu = Single;
    }

    public void AllMenu()
    {
        currentMenu.SetActive(false);
        All.SetActive(true);
        currentMenu = All;
    }

    public void EntryMenu()
    {
        currentMenu.SetActive(false);
        Entry.SetActive(true);
        currentMenu = Entry;
    }

    public void EditMenu()
    {
        currentMenu.SetActive(false);
        Edit.SetActive(true);
        currentMenu = Edit;
    }

    public void DeleteMenu()
    {
        currentMenu.SetActive(false);
        Delete.SetActive(true);
        currentMenu = Delete;
    }
}
