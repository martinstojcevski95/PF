using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{



    public Image Loading;
    public Text WarningText,CurrentPlayerText;
    public string selectedTextType;
    public static UIManager Instance;
    // Use this for initialization
    public RectTransform SavePlayPopUp;
    public RectTransform LoadPlayPopUp;
    public RectTransform SaveFormationPopUp;
    public RectTransform LoadFormationPopUp;
    public RectTransform NewFormatonAndPlay;
    public RectTransform ContinueNewPlayOrNewFormation;
    public Canvas PopUpCanvas;
    public Button Planning;
    public Button View3D,View2D;
    public Button DefensePlayer;
    public RectTransform Menu;
    public Image fillImage;
    float waitTime;
    bool isClicked;
    public int currentPlayers;
    public Dropdown LoadFormations;



    public Button SaveFormationDropDown,LoadFormationDropDown;
    public Button SavePlayUI;
    public Button Arrow, Block;
    public Button ResetFormation,LoadPlayUI;
    public Button RemoveLines;

    public Button SideViewCamera, TopViewCamera, WireCamCamera, PressBoxView, FlipedViewCamera;
    public Color Selected, Unselected;
    bool isMenuOpened;
    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        RemoveLines.interactable = false;
        LoadPlayUI.interactable = false;
        Block.interactable = false;
        isMenuOpened = true;
        View3D.interactable = false;
      //  View2D.interactable = false;
        SavePlayUI.interactable = false;
        Loading.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        switch (selectedTextType)
        {
            case "error":
                WarningText.color = Color.red;
                break;
            case "info":
                WarningText.color = Color.white;
                break;
            case "warning":
                WarningText.color = Color.yellow;
                break;
            case "success":
                WarningText.color = Color.green;
                break;
            case "none":
                WarningText.text = "";
                break;
        }
        if(currentPlayers >= 0)
        {
            CurrentPlayerText.text = "Active Players in the field " + currentPlayers;
        }
        //if (isClicked)
        //{

        //    waitTime += Time.deltaTime;

        //    if (waitTime >= .5f)
        //    {
        //        Debug.Log("opening menu ");
        //        Menu.SetActive(true);
        //        waitTime = 0;
        //        isClicked = false;
        //        fillImage.fillAmount = 0;
        //        fillImage.GetComponentInChildren<Text>().text = "";

        //    }
        //    fillImage.fillAmount += waitTime;
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    fillImage.GetComponentInChildren<Text>().text = "Opening Menu";
        //    fillImage.transform.position = Input.mousePosition;
        //    isClicked = true;
        //}

    }


    public void OpenMenu()
    {
        if(isMenuOpened)
        {
            Menu.DOAnchorPos(new Vector2(170, -299.35f), 0.5f);
           CameraMovement.Instance.InGame = false;
            isMenuOpened = false;
        }
        else
        {
            CameraMovement.Instance.InGame = true;
            Menu.DOAnchorPos(new Vector2(-190, -299.35f), 0.5f);
            isMenuOpened = true;
        }
        GameManager.Instance.RecenterCamerView();
    }
    public void CloseMenu()
    {
        CameraMovement.Instance.InGame = true;
       // CameraMovement.Instance.EnablePanning();
        Menu.DOAnchorPos(new Vector2(-190, -299.35f), 0.5f);

    }
    /// <summary>
    /// UI Info
    /// </summary>
    /// <param name="DescriptionForTheInfo"></param>
    /// <param name="texttype"></param>
    /// <param name="waitTime"></param>
    public void SelectTextType(string DescriptionForTheInfo, string texttype, float waitTime)
    {
        StopAllCoroutines();
        StartCoroutine(Warnings(DescriptionForTheInfo, texttype, waitTime));
    }

    public void SavePlay(bool t)
    {
        GameManager.Instance.RecenterCamerView();
        for (int i = 0; i < GameManager.Instance.allPlayers.Count; i++)
        {
            GameManager.Instance.allPlayers[i].canMove = false;
        }
        SavePlayPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);
        //   SavePlayPopUp.gameObject.SetActive(t);
        PopUpCanvas.enabled = t;
        CameraMovement.Instance.EnablePanning();
        if (t)
            SavePlayPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);
        else
            SavePlayPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);

    }
    public void  ContinueNewPlayOrMakeNewFormation(bool t)
    {
        PopUpCanvas.enabled = t;

        if (t)
        {
            ContinueNewPlayOrNewFormation.DOAnchorPos(new Vector2(0, 0), 0.5f);
        }
        else
        {
            ContinueNewPlayOrNewFormation.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        }
    }


    public void LoadPlay(bool t)
    {
        GameManager.Instance.RecenterCamerView();
        //   LoadPlayPopUp.gameObject.SetActive(t);
        PopUpCanvas.enabled = t;
        if(t)
        {
            LoadPlayPopUp.DOAnchorPos(new Vector2(-756, -324), 0.5f);

        }
        else
        {
            LoadPlayPopUp.DOAnchorPos(new Vector2(-1500, -324), 0.5f);

            CameraMovement.Instance.EnablePanning();
        }

    }
    public void LoadNewPlayOrFormation(bool t)
    {
        NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 0), 0.5f);
        PopUpCanvas.enabled = t;
        CameraMovement.Instance.EnablePanning();
        if (t)
            NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 0), 0.5f);
        else
            NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 1000), 0.5f);
    }

    public void SaveFormation(bool t)
    {
        // SaveFormationPopUp.gameObject.SetActive(t);

        GameManager.Instance.RecenterCamerView();
        PopUpCanvas.enabled = t;
        CameraMovement.Instance.InGame = true;
        if (t)
            SaveFormationPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);
        else
            SaveFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
    }
    public void LoadPlayLoadAfterFormationLoad()
    {
        PopUpCanvas.enabled = true;
        LoadPlayPopUp.DOAnchorPos(new Vector2(-756, -324), 0.5f);
       // LoadPlayPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);
    }

    public void LoadFormation(bool t)
    {
        GameManager.Instance.RecenterCamerView();
        PopUpCanvas.enabled = t;

        CameraMovement.Instance.EnablePanning();
        if (t)
            LoadFormationPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);
        else
        {
            LoadFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        }
        //  LoadFormationPopUp.gameObject.SetActive(t);

    }
    public void ButtonSelectedColorSwap(Button button)
    {
        button.GetComponent<Image>().color = Color.red;
    }


    public void CloseAllPopUps()
    {
        //SavePlay(false);
        //LoadPlay(false);
        //SaveFormation(false);
        //LoadFormation(false);
        PopUpCanvas.enabled = false;
        SavePlayPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        LoadFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        SaveFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        LoadPlayPopUp.DOAnchorPos(new Vector2(-1500, -324), 0.5f);
      //  LoadPlayPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        ContinueNewPlayOrNewFormation.DOAnchorPos(new Vector2(0, 1000),0.5f);

        NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        // NewFormatonAndPlay.gameObject.SetActive(false);
        //for (int i = 0; i < GameManager.Instance.allPlayers.Count; i++)
        //{
        //    GameManager.Instance.allPlayers[i].canMove = true;
        //}
       // CameraMovement.Instance.EnablePanning();

    }

    IEnumerator Warnings(string DescriptionForTheInfo, string texttype, float waitTime)
    {
        selectedTextType = texttype;
        WarningText.text = DescriptionForTheInfo;
        yield return new WaitForSeconds(waitTime);
        WarningText.text = "";
    }

}
