

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera TopViewCamera;

    public List<SinglePlayer> allPlayers = new List<SinglePlayer>();

    public static GameManager Instance;
    public PathCreator pathCreator;
    public GameObject PlayerPrefab, DefensePrefab;

    Vector3 Camera3DView, CameraDefaultView;
    Quaternion Camera3DRotation, CameraDefaultRotation;
    Transform newPlayerPosition;
    Transform newDefensePlayerPosition;
    Transform SideLineCameraView, FlippedDefensePovView, PressBoxView;
    public Image LoadinPanel;

    public List<string> FormationsName = new List<string>();

    public List<string> PlaysNames = new List<string>();
    public Dropdown PlayNames;
    public Dropdown FormationNames;
    public float LoadingSpeed;
    public int SelectedPlayerID;

    public Color PlayerSelectedColorHolder;
    public int PlayersCount;
    public string CurrentLinkedName;
    //  public LinkedFormationsAndPlays linkedNames;
    // string formationameHolder;
    public GameObject ArrowPref, BlockPref;
    //public List<int> LinkedPlayerPointerID = new List<int>();
    // int howMuchPointersShouldBeSpawned;
    // public string PointerType;
    public AllFormations allFormations;
    public int formationCounter;
    public int playsCounter;
    public int SelectedFormation;
    public Dropdown FORMATIONS, PLAYS;
    // string FORMATIONNAMEHOLDER, PLAYNAMEHOLDER;
    public bool CanDrawIfSelected;
    public bool InFormation;

    public Camera ScreenShootCamera;
    bool takeScreenShotOnNextFrame;

    int selectedFormationIDForPreview, selectedPlayIDForPreview;

    [SerializeField]
    InputField placeHolder;

    [SerializeField]
    PlaysNameHolder playsNamesHolder;

    bool InNewFormation;
    bool inDrawingMode;
    void Awake()
    {
        LoadingSpeed = 1 / 1.8f;
        Instance = this;
        formationCounter = allFormations.AllFormmations.Count;
        //   playsNamesHolder.PlaysNamesHolder.Add("Default");
        // string js = JsonUtility.ToJson(playsNamesHolder);
        // File.WriteAllText(Application.dataPath + "/PlaysNamesHolder.json", js);
    }



    // Use this for initialization
    void Start()
    {
        LoadFullData();
        InFormation = true;
        CameraDefaultView = Camera.main.transform.position;
        CameraDefaultRotation = Camera.main.transform.rotation;

        newPlayerPosition = GameObject.FindGameObjectWithTag("newplayerpos").transform;
        newDefensePlayerPosition = GameObject.FindGameObjectWithTag("newdefenseplayerpos").transform;
        Camera3DView = GameObject.FindGameObjectWithTag("3DView").transform.position;
        Camera3DRotation = GameObject.FindGameObjectWithTag("3DView").transform.rotation;

        var cam = FindObjectOfType<Camera>();
        if (cam != null)
        {
            if (cam.tag == "ScreenShootCam")
            {
                ScreenShootCamera = cam;
            }
        }

        PLAYS.ClearOptions();
        PLAYS.AddOptions(new List<string> { "Choose Play" });
        FORMATIONS.ClearOptions();
        FORMATIONS.AddOptions(new List<string> { "Choose formation" });


        GetAllDataAtStart();

    }



    //void OnPostRender()
    //{
    //    if (takeScreenShotOnNextFrame)
    //    {
    //        takeScreenShotOnNextFrame = false;
    //        RenderTexture renderTexture = ScreenShootCamera.targetTexture;

    //        Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

    //        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
    //        renderResult.ReadPixels(rect, 0, 0);
    //        byte[] byteArray = renderResult.EncodeToPNG();
    //        if (Application.isEditor)
    //        {
    //            File.WriteAllBytes(Application.dataPath + "/" + screenshotNameHolder + ".png", byteArray);// for editor test
    //        }
    //        else
    //        {
    //            File.WriteAllBytes(Application.dataPath + "/Resources" + "/colson.png", byteArray); // when releasing a build

    //        }


    //        Debug.Log("screenshot has been taken!");
    //        RenderTexture.ReleaseTemporary(renderTexture);
    //        ScreenShootCamera.targetTexture = null;

    //    }
    //}


    //loading this way
    //byte[] test = File.ReadAllBytes(Application.dataPath + "/test.png");
    //temp = new Texture2D(500, 500);
    //temp.LoadImage(test);
    //        WTF.texture = temp;



    //void TakeScreenShot(int height, int width)
    //{

    //    ScreenShootCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
    //    takeScreenShotOnNextFrame = true;
    //}

    /// <summary>
    /// Using this. Loading the full data
    /// </summary>
    public void LoadFullData()
    {
        FormationsName.Clear();
        StartCoroutine(LateLoadEverything());

    }


    /// <summary>
    /// Using this. Saving the formation
    /// </summary>
    /// <param name="formationname"></param>
    public void SavingFormation(InputField formationname)
    {
        if (formationname.text == "")
        {
            UIManager.Instance.SelectTextType("The name for the formation is required!", "error", 4f);

        }
        else
        {
            var formation = new Formation();
            formation.FormationName = formationname.text;

            formation.FormationID = allFormations.AllFormmations.Count;
            allFormations.AllFormmations.Add(formation);
            InFormation = false;
            UIManager.Instance.SelectTextType("Formation has been saved!", "success", 2f);
            UIManager.Instance.SaveFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
            UIManager.Instance.ResetFormation.interactable = false;
            UIManager.Instance.RemoveLines.interactable = true;
            CameraMovement.Instance.InGame = true;
            EnableOrDisableCollidersOnPlayers(true);
            UIManager.Instance.SelectTextType("", "", 0f);
            UIManager.Instance.LoadNewPlayOrFormation(true);

        }
    }

    public void DestroyPointsAndLines()
    {
        var pointsandlines = GameObject.FindGameObjectsWithTag("POINTER");
        if (pointsandlines != null)
        {
            foreach (var item in pointsandlines)
            {
                Destroy(item);
            }
        }
    }

    public void ResetOnlyThePlayersPositionToDefault()
    {

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].transform.position = allPlayers[i].playerStats.StartingPlayerLocalPosition;
            allPlayers[i].playerStats.PlayerLocalPosition = allPlayers[i].playerStats.StartingPlayerLocalPosition;


        }
    }


    public void SaveDefaultPlayOnFormationReset()
    {
        // UIManager.Instance.SaveFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        InFormation = true;
        SavingPlay(placeHolder);
        pathCreator.PointerHolder.SetActive(false);
        UIManager.Instance.SavePlayUI.interactable = false;
        DestroyPointsAndLines();
        // reset the players position to default 
        ResetOnlyThePlayersPositionToDefault();
        // reset player pointer count to 0
        UIManager.Instance.RemoveLines.interactable = false;
    }

    public void StartNewPlayWithOnlyDrawing()
    {
        InFormation = true; // this is new so  the menus are resrting;
        UIManager.Instance.NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        UIManager.Instance.SavePlayUI.interactable = true;
        SavingPlay(placeHolder);
        DrawingMode();
        UIManager.Instance.RemoveLines.interactable = true;
    }

    void SelectAndEditPlay(Dropdown play)
    {
        OnSelectedPlayPreview(play);
    }


    /// <summary>
    /// Using this. Saving the play
    /// </summary>
    /// <param name="playname"></param>
    void SavingPlay(InputField playname)
    {
        formationCounter = allFormations.AllFormmations.Count;
        var newPlay = new Play();

        newPlay.PlayName = playname.text;
        newPlay.PlayID = allFormations.AllFormmations[formationCounter - 1].FormationID;

        if (allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count == 0)
        {
            // first add always allow so the list can be populated and increased +1
            if (newPlay.PlayName == "")
            {
                newPlay.PlayName = allFormations.AllFormmations[formationCounter - 1].FormationName + " Formation";

                UIManager.Instance.SelectTextType("Play has been saved with deault name of the formation!", "success", 2f);
                UIManager.Instance.LoadFormationDropDown.interactable = false;


            }
            var players = FindObjectsOfType<SinglePlayer>();

            allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Add(newPlay);
            playsCounter = allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count;
            if (players != null)
            {
                foreach (var item in players)
                {
                    item.Populate(formationCounter - 1, playsCounter - 1);

                }
            }
            UIManager.Instance.LoadFormationDropDown.interactable = false;
            StartCoroutine(LateSaveEverything());
        }
        else
        {
            if (allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Exists(name => name.PlayName == newPlay.PlayName))
            {
                UIManager.Instance.SelectTextType("Play with that name already exists in formation " + allFormations.AllFormmations[formationCounter - 1].FormationName + " please choose  different name", "error", 3f);

            }
            else
            {
                if (newPlay.PlayName == "")
                {
                    newPlay.PlayName = allFormations.AllFormmations[formationCounter - 1].FormationName + " Formation";
                    UIManager.Instance.SelectTextType("Play has been saved with deault name of the formation!", "success", 2f);
                    UIManager.Instance.LoadFormationDropDown.interactable = false;

                }
                var players = FindObjectsOfType<SinglePlayer>();

                allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Add(newPlay);
                playsCounter = allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count;

                if (players != null)
                {
                    foreach (var item in players)
                    {
                        item.Populate(formationCounter - 1, playsCounter - 1);

                    }

                }

                UIManager.Instance.LoadFormationDropDown.interactable = false;
                StartCoroutine(LateSaveEverything());
            }
        }
        formationCounter += 1;
        pathCreator.PointerHolder.SetActive(false);
        pathCreator.lineRenderer.positionCount = 0;
        pathCreator.enabled = false;

        //  InFormation = true;
    }

    public void QuitApp()
    {
        Application.Quit();
    }


    public void NewFormationAndSaveFormation()
    {
        ReturnToDefaultFormation();
        UIManager.Instance.SaveFormationUIButtonAfterNewFormation(true);
        UIManager.Instance.Menu.DOAnchorPos(new Vector2(-190, -299.35f), 0.5f);
        UIManager.Instance.isMenuOpened = true;

        RecenterCamerView();

    }


    public void ContinueDrawing()
    {
        // remove the newly added players on reset if needed
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].playerStats.Points.Clear();
            allPlayers[i].playerStats.HasDrawnedLine = false;
            allPlayers[i].playerStats.PointerRotation = new Quaternion(0, 0, 0, 0);
            allPlayers[i].GetComponent<LineRenderer>().positionCount = 0;
            allPlayers[i].playerStats.PointerPosition = new Vector3(0, 0, 0);
            allPlayers[i].playerStats.PointerCounter = 0;
        }
        pathCreator.GetComponent<LineRenderer>().positionCount = 0;
        pathCreator.PointerHolder.SetActive(false);
        UIManager.Instance.RemoveLines.interactable = true;
        DestroyPointsAndLines();
        DrawingMode();
    }

    public void EnableOrDisableCollidersOnPlayers(bool collider)
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].GetComponent<BoxCollider>().enabled = collider;
        }
    }



    public void SideLineView()
    {
        CameraMovement.Instance.InGame = false;
        TopViewCamera.transform.position = CameraDefaultView;
        TopViewCamera.transform.rotation = CameraDefaultRotation;

        UIManager.Instance.WireCamCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.PressBoxView.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.TopViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.FlipedViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.SideViewCamera.GetComponent<Image>().color = UIManager.Instance.Selected;
        SideLineCameraView = GameObject.FindGameObjectWithTag("sidelineview").transform;
        TopViewCamera.transform.position = SideLineCameraView.transform.position;
        TopViewCamera.transform.rotation = SideLineCameraView.transform.rotation;
        EnableOrDisableCollidersOnPlayers(false);
        UIManager.Instance.SelectTextType("You are now in SideLine View, from where the coaches spectate the game.", "success", 3f);
        CameraMovement.Instance.InFPSView = false;
        foreach (var item in allPlayers)
        {
            item.transform.position = item.playerStats.PlayerLocalPosition;

        }
        // if (inDrawingMode)
        //   StartCoroutine(PlayerMovementWithDelayForOtherViews());

    }

    public void DefensePOV()
    {
        CameraMovement.Instance.InGame = false;

        TopViewCamera.transform.position = CameraDefaultView;
        TopViewCamera.transform.rotation = CameraDefaultRotation;
        UIManager.Instance.WireCamCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.PressBoxView.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.TopViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.FlipedViewCamera.GetComponent<Image>().color = UIManager.Instance.Selected;
        UIManager.Instance.SideViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        FlippedDefensePovView = GameObject.FindGameObjectWithTag("defensePov").transform;

        TopViewCamera.transform.rotation = FlippedDefensePovView.transform.rotation;
        TopViewCamera.transform.position = FlippedDefensePovView.transform.position;
        EnableOrDisableCollidersOnPlayers(false);
        CameraMovement.Instance.InFPSView = false;
        UIManager.Instance.SelectTextType("You are now in Flipped View, and seeing the field as the other team will see it.", "success", 3f);
        foreach (var item in allPlayers)
        {
            item.transform.position = item.playerStats.PlayerLocalPosition;

        }
        //   if (inDrawingMode)
        //   StartCoroutine(PlayerMovementWithDelayForOtherViews());

    }

    public void PressView()
    {
        CameraMovement.Instance.InGame = false;

        TopViewCamera.transform.position = CameraDefaultView;
        TopViewCamera.transform.rotation = CameraDefaultRotation;

        UIManager.Instance.WireCamCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.PressBoxView.GetComponent<Image>().color = UIManager.Instance.Selected;
        UIManager.Instance.TopViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.FlipedViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.SideViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;

        PressBoxView = GameObject.FindGameObjectWithTag("pressview").transform;

        TopViewCamera.transform.rotation = PressBoxView.transform.rotation;
        TopViewCamera.transform.position = PressBoxView.transform.position;
        EnableOrDisableCollidersOnPlayers(false);
        CameraMovement.Instance.InFPSView = false;
        UIManager.Instance.SelectTextType("You are now in Press View.", "success", 3f);
        foreach (var item in allPlayers)
        {
            item.transform.position = item.playerStats.PlayerLocalPosition;

        }
        // if (inDrawingMode)
        //   StartCoroutine(PlayerMovementWithDelayForOtherViews());

    }

    public void RecenterCamerView()
    {
        Camera.main.transform.position = CameraDefaultView;
        Camera.main.transform.rotation = CameraDefaultRotation;
        EnableOrDisableCollidersOnPlayers(true);
    }


    public void DelayedMovementOnlyForHotKey()
    {
        StartCoroutine(PlayerMovementWithDelayForOtherViews());
    }

    IEnumerator PlayerMovementWithDelayForOtherViews()
    {
        yield return new WaitForSeconds(1f);
        foreach (var item in allPlayers)
        {
            item.pathMover.pathPoints.Clear();
            item.transform.position = item.playerStats.PlayerLocalPosition;
            item.playerNav.enabled = true;
            item.pathMover.enabled = true;

            if (item.playerStats.Points.Count >= 1)
                item.pathMover.SetPoints(item.playerStats.Points);
            else
                Debug.Log("no drawings");
            //item.renderer.SetPosition(0, new Vector3(-100, -100, 0));
        }

    }

    public void InDrawMode(bool t)
    {
        if (t)
            inDrawingMode = true;
        else
            inDrawingMode = false;
    }

    public void ReturnToDefaultFormation()
    {
        UIManager.Instance.SelectTextType("Formation has be reseted to the default one. After done with positioning the players, click on the save formation button on the top middle ", "warning", 5f);

        CameraMovement.Instance.isPanning = true;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].GetComponent<LineRenderer>().positionCount = 0;
            allPlayers[i].playerNav.enabled = false;
            allPlayers[i].pathMover.enabled = false;
            allPlayers[i].playerStats.PointerCounter = 0;
            allPlayers[i].playerStats.Points.Clear();
            allPlayers[i].transform.position = allPlayers[i].playerStats.StartingPlayerLocalPosition;
            allPlayers[i].playerStats.PlayerLocalPosition = allPlayers[i].playerStats.StartingPlayerLocalPosition;
            allPlayers[i].playerStats.CanMove = false;
            allPlayers[i].playerStats.HasDrawnedLine = false;
        }

        pathCreator.GetComponent<LineRenderer>().positionCount = 0;
        pathCreator.enabled = false;
        pathCreator.PointerHolder.SetActive(false);
        DestroyPointsAndLines();
        Camera.main.transform.position = CameraDefaultView;
        Camera.main.transform.rotation = CameraDefaultRotation;
        UIManager.Instance.View3D.interactable = false; // reset this because we don;t have active play
        UIManager.Instance.CloseAllPopUps();
        InFormation = true;
        UIManager.Instance.RemoveLines.interactable = false;

        //UIManager.Instance.LoadFormationUI.interactable = false;
        LoadFullData();
    }





    public void FormationPreviewForPlays(Toggle selectedItem)
    {

        foreach (var item in allFormations.AllFormmations)
        {
            if (item.FormationName.Contains(selectedItem.GetComponentInChildren<Text>().text))
            {
                Debug.Log("wt");
                selectedFormationIDForPreview = item.FormationID;
            }

        }
    }



    //byte[] test = File.ReadAllBytes(Application.dataPath + "/test.png");
    //temp = new Texture2D(500, 500);
    //temp.LoadImage(test);
    //        WTF.texture = temp;


    IEnumerator DelayedPlayersMovement(bool t)
    {
        if (t)
        {
            UIManager.Instance.PressBoxView.GetComponent<Image>().color = UIManager.Instance.Unselected;
            UIManager.Instance.TopViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
            UIManager.Instance.FlipedViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
            UIManager.Instance.SideViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
            Camera.main.transform.position = Camera3DView;
            Camera.main.transform.rotation = Camera3DRotation;
            yield return new WaitForSeconds(1f);
            foreach (var item in allPlayers)
            {
                item.pathMover.pathPoints.Clear();
                item.transform.position = item.playerStats.PlayerLocalPosition;
                item.playerNav.enabled = true;
                item.pathMover.enabled = true;
                item.pathMover.SetPoints(item.playerStats.Points);
                //item.renderer.SetPosition(0, new Vector3(-100, -100, 0));
            }


        }
        else
        {

            Camera.main.transform.position = CameraDefaultView;
            Camera.main.transform.rotation = CameraDefaultRotation;
            foreach (var item in allPlayers)
            {
                item.playerNav.enabled = false;
                item.pathMover.enabled = false;
                item.transform.position = item.playerStats.PlayerLocalPosition;

            }

            UIManager.Instance.PressBoxView.GetComponent<Image>().color = UIManager.Instance.Unselected;
            UIManager.Instance.TopViewCamera.GetComponent<Image>().color = UIManager.Instance.Selected;
            UIManager.Instance.FlipedViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
            UIManager.Instance.SideViewCamera.GetComponent<Image>().color = UIManager.Instance.Unselected;
        }
        CameraMovement.Instance.InFPSView = false;
    }

    public void OverheadView()
    {
        Camera.main.transform.position = CameraDefaultView;
        Camera.main.transform.rotation = CameraDefaultRotation;
        CameraMovement.Instance.InFPSView = false;
    }

    /// <summary>
    /// Using this. Switching the field of the view 2D/3D
    /// </summary>
    /// <param name="t"></param>
    public void SwitchCameraView(bool t)
    {
        StartCoroutine(DelayedPlayersMovement(t));
        EnableOrDisableCollidersOnPlayers(true);
    }




    /// <summary>
    /// Using this. Loading formation data
    /// </summary>
    /// <param name="formation"></param>
    public void OnSelectedFormation(Dropdown formation)
    {
        int menuIndex = formation.GetComponent<Dropdown>().value;
        var formationname = formation.options[menuIndex].text;


        foreach (var item in allFormations.AllFormmations)
        {
            if (item.FormationName == formationname)
            {

                Debug.Log(item.FormationID);
                SelectedFormation = item.FormationID;
                var playsCount = allFormations.AllFormmations[item.FormationID].LinkedPlaysWithFormation.Count;
                PlaysNames.Clear();
                for (int i = 0; i < playsCount; i++)
                {
                    PlaysNames.Add(allFormations.AllFormmations[item.FormationID].LinkedPlaysWithFormation[i].PlayName);

                }

            }

        }


        PLAYS.ClearOptions();
        PLAYS.AddOptions(new List<string> { "Choose Play" });
        PLAYS.AddOptions(PlaysNames);
        PLAYS.RefreshShownValue();
        UIManager.Instance.LoadFormation(false);
        UIManager.Instance.LoadPlayUI.interactable = true;
        CameraMovement.Instance.InGame = true;
    }

    void Update()
    {

        UIManager.Instance.currentPlayers = allPlayers.Count;


        if (Input.GetKeyDown(KeyCode.P))
        {
            DelayedMovementOnlyForHotKey();
        }
    }



    public void OnSelectedPlayPreview(Dropdown play)
    {

        int menuIndex = play.GetComponent<Dropdown>().value - 1;
        //   ResetOnlyThePlayersPositionToDefault();

        if (menuIndex >= 0)
        {
            for (int i = 0; i < allPlayers.Count; i++)
            {
                allPlayers[i].transform.position = allPlayers[i].playerStats.StartingPlayerLocalPosition;
            }
            int playersCountForPlay = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays.Count;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerID == allPlayers[i].playerStats.PlayerID)
                {

                    allPlayers[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].Points;
                    allPlayers[i].playerStats.PlayerPointerType = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerPointerType;
                    allPlayers[i].playerStats.PointerPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerPosition;
                    allPlayers[i].playerStats.PointerRotation = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerRotation;
                    allPlayers[i].playerStats.PlayerLocalPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerLocalPosition;
                    allPlayers[i].PreviewPlayOnlyWithoutPointers();
                    allPlayers[i].LoadFormationData();

                }
            }

            UIManager.Instance.SelectTextType("Preview mode", "success", 3f);
            InFormation = false;
            //PLAYS.value = 1;
            DestroyPointsAndLines();
        }

    }


    // do this undo Lines and check it 
    public void DeleteLineForSelectedPlayer()
    {


        foreach (var item in allPlayers)
        {
            item.playerStats.Points.Clear();
            item.playerStats.PointerPosition = Vector3.zero;
            item.playerStats.PointerRotation = new Quaternion(0, 0, 0, 0);
            item.playerStats.PlayerPointerType = "";
            item.GetComponent<LineRenderer>().positionCount = 0;
            item.playerStats.HasDrawnedLine = false;
            item.playerStats.PointerCounter = 0;
            //   item.canMove = true;
        }
        pathCreator.lineRenderer.positionCount = 0;
        pathCreator.PointerHolder.SetActive(false);
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].transform.position = allPlayers[i].playerStats.PlayerLocalPosition;
            allPlayers[i].playerStats.HasDrawnedLine = false;
            allPlayers[i].pathMover.enabled = false;
            allPlayers[i].playerNav.enabled = false;
            allPlayers[i].playerStats.Points.Clear();
            if (allPlayers[i].pathMover.navmeshagent.hasPath)
            {
                allPlayers[i].pathMover.navmeshagent.isStopped = true;
                allPlayers[i].pathMover.navmeshagent.ResetPath();

            }


        }
        DestroyPointsAndLines();
        // ResetOnlyThePlayersPositionToDefault();
        UIManager.Instance.SelectTextType("All drawn lines are removed", "warning", 1f);
    }


    /// <summary>
    /// Using this. Loading play data
    /// </summary>
    /// <param name="play"></param>
    public void OnSelectedPlay(Dropdown play)
    {
        DestroyPointsAndLines();
        ResetOnlyThePlayersPositionToDefault();
        int menuIndex = play.GetComponent<Dropdown>().value - 1; // because there is choose plays value at index 0;
        var playname = play.options[menuIndex].text;
        Debug.Log("menu index " + menuIndex);
        int playersCountForPlay = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays.Count;

        var players = FindObjectsOfType<SinglePlayer>();
        if (players != null)
        {
            for (int i = 0; i < playersCountForPlay; i++)
            {
                if (allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerID == players[i].playerStats.PlayerID)
                {

                    players[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].Points;
                    players[i].playerStats.PlayerPointerType = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerPointerType;
                    players[i].playerStats.PointerPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerPosition;
                    players[i].playerStats.PointerRotation = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerRotation;
                    players[i].playerStats.PlayerLocalPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerLocalPosition;
                    players[i].LoadPlayData(SelectedFormation, menuIndex);
                    players[i].LoadFormationData();

                }
            }
        }

        PLAYS.value = 0;
        pathCreator.lineRenderer.positionCount = 0;
        pathCreator.PointerHolder.SetActive(false);
        UIManager.Instance.ResetFormation.interactable = true;
        CameraMovement.Instance.InGame = true;

    }

    public void RemovePointersAfterLoadingNewPlay()
    {
        var pointers = FindObjectsOfType<Pointer>();
        if (pointers != null)
        {
            foreach (var item in pointers)
            {
                if (item.PointerLocalRotation.y == 0 || item.PointerLocalRotation.x == 0 || item.PointerLocalRotation.z == 0)
                {
                    Destroy(item.gameObject);
                }

            }
        }
    }

    /// <summary>
    /// Using this. Enable drawing for each player after a formation is saved
    /// </summary>
    public void DrawingMode()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].playerStats.CanMove = true;
            allPlayers[i].playerNav.enabled = false;
            allPlayers[i].pathMover.enabled = false;
            allPlayers[i].GetComponentInChildren<MeshRenderer>().enabled = false;

        }
        pathCreator.enabled = true;
        //  UIManager.Instance.View3D.interactable = true;


    }


    /// <summary>
    /// Using this. Save formation and play data after a play is saved  
    /// </summary>
    /// <returns></returns>
    IEnumerator LateSaveEverything()
    {

        DestroyPointsAndLines();

        yield return new WaitForSeconds(.5f);
        string jsonData = JsonUtility.ToJson(allFormations);
        File.WriteAllText(Application.dataPath + "/Formations-Plays.json", jsonData);
        StartCoroutine(LateLoadEverything());
        if (InFormation)
        {
            EnableOrDisableCollidersOnPlayers(true);

            // after you save formation with default play
        }
        else
        {
            UIManager.Instance.SelectTextType("Play has been saved!", "success", 2f);
            UIManager.Instance.SavePlay(false);
            UIManager.Instance.ContinueNewPlayOrMakeNewFormation(true);
            //    UIManager.Instance.View2D.interactable = false; // using this before
            // after you save a play 
        }

        UIManager.Instance.ResetFormation.interactable = true;
        UIManager.Instance.LoadPlayUI.interactable = false;
        //UIManager.Instance.RemoveLines.interactable = false;
        pathCreator.PointerHolder.transform.position = new Vector3(250, 0, 500);

    }

    IEnumerator LateLoadEverything()
    {
        //for (int i = 0; i < allPlayers.Count; i++)
        //{
        //    allPlayers[i].playerStats.Points.Clear();
        //    allPlayers[i].renderer.positionCount = 0;
        //}
        yield return new WaitForSeconds(.5f);
        string path = Application.dataPath + "/Formations-Plays.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            allFormations = JsonUtility.FromJson<AllFormations>(json);

            FORMATIONS.ClearOptions();
            for (int i = 0; i < allFormations.AllFormmations.Count; i++)
            {

                FormationsName.Add(allFormations.AllFormmations[i].FormationName);
            }

            FORMATIONS.AddOptions(FormationsName);


            //   PopulateDropdownDataForFormationsAndPlays();
        }
        yield return new WaitForSeconds(.3f);
        UIManager.Instance.LoadFormationDropDown.interactable = true;
    }


    public void SaveFormationDataAndDrawForPlay(InputField savedFormationName)
    {
        SavingFormation(savedFormationName);
    }

    public void SavePlayAndFormationAfterDrawing(InputField savedPlayName)
    {
        InFormation = false; // this is new when savinga play it ask you new formation or new play for this play 
        SavingPlay(savedPlayName);
    }


    /// <summary>
    /// Using this. Gtting and setting the players into list and the path creator
    /// </summary>
    void GetAllDataAtStart()
    {

        var foundPlayer = FindObjectsOfType<SinglePlayer>();
        for (int i = 0; i < foundPlayer.Length; i++)
        {
            foundPlayer[i].playerStats.PlayerID = i;
            foundPlayer[i].playerNav.enabled = false;
            foundPlayer[i].pathMover.enabled = false;
            foundPlayer[i].GetComponentInChildren<MeshRenderer>().enabled = false;
            allPlayers.Add(foundPlayer[i]);

        }

        var pathcreator = FindObjectOfType<PathCreator>();
        if (pathcreator != null)
        {
            pathCreator = pathcreator;
            //     pathCreator.gameObject.SetActive(false);
            pathCreator.enabled = false;

        }

        PlayersCount = allPlayers.Count;
        LoadinPanel.gameObject.SetActive(false);
    }



    //adding new player 
    public void RenewTheAllPlayerListOnNewPlayerAdded(string playerType)
    {
        StartCoroutine(AddNewPlayer(playerType));
    }


    // coroutine for adding a new player into the game
    IEnumerator AddNewPlayer(string playerType)
    {
        //  pathCreator.gameObject.SetActive(true);
        //    pathCreator.enabled = true;
        //  DrawingMode();

        int playersCount = allPlayers.Count;
        yield return new WaitForSeconds(.3f);
        Debug.Log("there are " + playersCount + " players in the field");
        if (playerType == "P")
        {
            GameObject newPlayer = Instantiate(PlayerPrefab, new Vector3(newPlayerPosition.transform.position.x, newPlayerPosition.transform.position.y, newPlayerPosition.transform.position.z), Quaternion.identity);
            newPlayer.GetComponent<SinglePlayer>().singlePlayer.PlayerType = "P";
            StartCoroutine(PopulateDataForNewPlayer(newPlayer, playersCount));
        }
        else if (playerType == "D")
        {
            GameObject newPlayer = Instantiate(DefensePrefab, new Vector3(newDefensePlayerPosition.transform.position.x, newDefensePlayerPosition.transform.position.y, newDefensePlayerPosition.transform.position.z), Quaternion.identity);
            newPlayer.GetComponent<SinglePlayer>().singlePlayer.PlayerType = "D";
            var rotation = transform.rotation.eulerAngles;
            rotation.y = -180f;
            //  rotation.x = 0f;
            newPlayer.transform.rotation = Quaternion.Euler(rotation);
            StartCoroutine(PopulateDataForNewPlayer(newPlayer, playersCount));
        }
    }


    //coroutine for populating data for each new spawned player
    IEnumerator PopulateDataForNewPlayer(GameObject newPlayer, int playersCount)
    {

        yield return new WaitForSeconds(.3f);
        var p = newPlayer.GetComponent<SinglePlayer>();
        p.playerStats.PlayerID = playersCount;
        if (InFormation)
        {
            p.playerStats.CanMove = false;
        }
        else
        {
            p.playerStats.CanMove = true;
        }

        allPlayers.Add(p);
        yield return new WaitForSeconds(.3f);
        PlayersCount = allPlayers.Count;


        //  p.playerStats.CanMove = true;
        // p.singlePlayer.PlayerName = p.singlePlayer.PlayerType + p.singlePlayer.PlayerID;
        //  p.gameObject.name = p.singlePlayer.PlayerType + "ID" + p.singlePlayer.PlayerID;
        // p.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = p.singlePlayer.PlayerType + p.singlePlayer.PlayerID;

        //   pathCreator.gameObject.SetActive(false);
        //pathCreator.enabled = false;

    }





    [Serializable]
    public class Formation
    {
        public string FormationName;
        public int FormationID;
        public string FormationTag;
        public List<Play> LinkedPlaysWithFormation = new List<Play>();


    }
    [Serializable]
    public class Play
    {
        public string PlayName;
        public int PlayID;
        public string PlayTag;
        public int PlayOrderNumber;
        public List<Player> LinkedPlayersWithPlays = new List<Player>();


    }
    [Serializable]
    public class Player
    {
        public string PlayerName;
        public int PlayerID;
        public string PlayerPointerType;
        public int PointerCounter;
        public Vector3 StartingPlayerLocalPosition;
        public Vector3 PlayerLocalPosition;
        public Vector3 PointerPosition;
        public Quaternion PointerRotation;
        public List<Vector3> Points = new List<Vector3>();
    }

    [Serializable]
    public class AllFormations
    {
        public List<Formation> AllFormmations;
    }

    [Serializable]
    public class PlaysNameHolder
    {

        public List<string> PlaysNamesHolder = new List<string>();
    }
}
