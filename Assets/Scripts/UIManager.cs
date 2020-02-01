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
    public Canvas PopUpCanvas;
    public Button Planning;
    public Button View3D;
    public Button DefensePlayer;
    public RectTransform Menu;
    public Image fillImage;
    float waitTime;
    bool isClicked;
    public int currentPlayers;
    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        View3D.interactable = false;
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
        Menu.DOAnchorPos(new Vector2(170, 0), 0.5f);
        // CameraMovement.Instance.DisablePanning();
        CameraMovement.Instance.InGame = false;
    }
    public void CloseMenu()
    {
        CameraMovement.Instance.InGame = true;
       // CameraMovement.Instance.EnablePanning();
        Menu.DOAnchorPos(new Vector2(-190, 0), 0.5f);
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
    public void LoadPlay(bool t)
    {

        //   LoadPlayPopUp.gameObject.SetActive(t);
        PopUpCanvas.enabled = t;
        if(t)
        {
            LoadPlayPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);

        }
        else
            LoadPlayPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);

        CameraMovement.Instance.EnablePanning();
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
        LoadPlayPopUp.DOAnchorPos(new Vector2(0, 0), 0.5f);
    }

    public void LoadFormation(bool t)
    {
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
        SavePlayPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        LoadFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        SaveFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        LoadPlayPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);


        NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        // NewFormatonAndPlay.gameObject.SetActive(false);
        for (int i = 0; i < GameManager.Instance.allPlayers.Count; i++)
        {
            GameManager.Instance.allPlayers[i].canMove = true;
        }
        CameraMovement.Instance.EnablePanning();

    }

    IEnumerator Warnings(string DescriptionForTheInfo, string texttype, float waitTime)
    {
        selectedTextType = texttype;
        WarningText.text = DescriptionForTheInfo;
        yield return new WaitForSeconds(waitTime);
        WarningText.text = "";
    }

}
