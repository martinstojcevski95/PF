using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UI;

public class SinglePlayer : MonoBehaviour
{



    int playerID;
    [HideInInspector]
    public NavMeshAgent playerNav;
    [HideInInspector]
    public PathMover pathMover;
    public Color SelectedPlayer;
    [HideInInspector]
    public Text PlayerName;
    public bool canMove;
    bool isSelected;
    //drag and drop
    private Vector3 mOffset;
    private float mZCoord;


    PlayerType playerType;
    GameObject Middle;
    bool isMiddleMoving;
    public Player singlePlayer;
    public Vector3 PlayerLocalPosition;
    // Use this for initialization
    public LineRenderer renderer;
    public Material drawingLineMaterial;
    Vector3[] positionArray = new[] { new Vector3(0f, 0f, 0f) };
    public PathCreator creator;
    public GameObject Pointer;
    string PlayName;
    public bool InFormation;
    public PlayerStats playerStats;
    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };
    public Animator anim;

    void Awake()
    {
        playerNav = GetComponent<NavMeshAgent>();
        pathMover = GetComponent<PathMover>();
        // anim = GetComponent<Animation>();

        var pathcreator = (PathCreator)FindObjectOfType(typeof(PathCreator));
        if (pathcreator != null)
        {
            creator = pathcreator;
        }
    }
    void Start()
    {

        renderer = GetComponent<LineRenderer>();
        renderer.startWidth = 0.15f;
        renderer.endWidth = 0.15f;
        renderer.material = drawingLineMaterial;
        renderer.enabled = false;
        playerStats.PlayerLocalPosition = transform.position;

        playerStats.StartingPlayerLocalPosition = transform.position;
        playerType = PlayerType.player;
        Middle = GameObject.FindGameObjectWithTag("MiddlePlayer");
        canMove = false;
        isMiddleMoving = false;
        if (anim != null)
            StartCoroutine(DelayedAnim());


    }


    IEnumerator DelayedAnim()
    {
        yield return new WaitForSeconds(0.2f);
        anim.speed = 0.7f;
        anim.SetBool("IsActive", true);
        //yield return new WaitForSeconds(0.8f);
        //anim.SetBool("IsActive", false);
    }

    IEnumerator DelayedRun()
    {
        yield return new WaitForSeconds(0.5f);
        if (playerStats.Points != null)
        {
            int points = playerStats.Points.Count;
            Debug.Log(playerStats.Points[points].x);
        }
    }
    public void RunAnimation(bool canRun)
    {

        if (anim != null)
        {
            if (canRun)
                anim.SetBool("Running", true);
            else
            {

                anim.SetBool("Running", false);
            }
            StartCoroutine(DelayedRun());

        }




    }
    /// <summary>
    /// Using this. Populate the data for each player 
    /// </summary>
    /// <param name="formationCountNumber"></param>
    /// <param name="playsCounter"></param>
    public void Populate(int formationCountNumber, int playsCounter)
    {
        if (playerStats.Points != null)
        {
            var play = new GameManager.Player();
            play.PlayerName = playerStats.PlayerName;
            play.PlayerID = playerStats.PlayerID;
            play.PlayerPointerType = playerStats.PlayerPointerType;
            play.Points = playerStats.Points;
            play.PointerPosition = playerStats.PointerPosition;
            play.PointerRotation = playerStats.PointerRotation;
            play.PointerCounter = playerStats.PointerCounter;
            play.PlayerLocalPosition = playerStats.PlayerLocalPosition;// transform.position;
            Debug.Log("data for player " + playerStats.PlayerID + " formation and play" + GameManager.Instance.allFormations.AllFormmations[formationCountNumber].FormationName + GameManager.Instance.allFormations.AllFormmations[formationCountNumber].LinkedPlaysWithFormation[playsCounter].PlayName);
            GameManager.Instance.allFormations.AllFormmations[formationCountNumber].LinkedPlaysWithFormation[playsCounter].LinkedPlayersWithPlays.Add(play);
        }
        else
        {

            var play = new GameManager.Player();
            play.PlayerName = playerStats.PlayerName;
            play.PlayerID = playerStats.PlayerID;
            play.PlayerLocalPosition = playerStats.PlayerLocalPosition;// transform.position;
            GameManager.Instance.allFormations.AllFormmations[formationCountNumber].LinkedPlaysWithFormation[playsCounter].LinkedPlayersWithPlays.Add(play);
            Debug.Log("saving for default formation");
        }

    }



    /// <summary>
    ///Using this. Load the formation data for  each player
    /// </summary>
    public void LoadFormationData()
    {

        transform.position = playerStats.PlayerLocalPosition;
    }

    /// <summary>
    /// Using this. Load the data for each player
    /// </summary>
    public void LoadPlayData(int formation, int play)
    {
        playerStats.CanMove = true; // to not be able to move the players after loading formation
        if (playerStats.Points != null)
        {
            GameManager.Instance.RemovePointersAfterLoadingNewPlay();
            OnNewPathCreated(playerStats.Points);
            renderer.enabled = true;
            renderer.positionCount = playerStats.Points.Count;
            renderer.SetPositions(playerStats.Points.ToArray());
            transform.position = playerStats.PlayerLocalPosition;
            playerStats.PointerCounter = GameManager.Instance.allFormations.AllFormmations[formation].LinkedPlaysWithFormation[play].LinkedPlayersWithPlays[playerStats.PlayerID].PointerCounter;
            if (playerStats.PointerCounter > 0)
            {
                Debug.Log("pointer for player" + playerStats.PlayerID);

                //instantiate i assign kako child ako nema pointer counter brisi pointer odma !!!
                if (playerStats.PlayerPointerType == "arrow")
                {
                    StartCoroutine(LatePointerDataPopulation("arrow", playerStats.PlayerID));

                    //var newArrow = Instantiate(GameManager.Instance.ArrowPref, playerStats.PointerPosition, playerStats.PointerRotation);
                    //Debug.Log(newArrow);
                    //if(newArrow != null)
                    //newArrow.tag = "POINTER";

                    //  newArrow.transform.parent = transform.GetChild(1).transform.parent;
                }
                else if (playerStats.PlayerPointerType == "block")
                {
                    StartCoroutine(LatePointerDataPopulation("block",playerStats.PlayerID));
                    //var newBlock = Instantiate(GameManager.Instance.BlockPref, playerStats.PointerPosition, playerStats.PointerRotation);
                    //Debug.Log(newBlock);
                    //if(newBlock != null)
                    //newBlock.tag = "POINTER";
                    // newBlock.transform.parent = transform.GetChild(1).transform.parent;
                }
            }
            else
            {

            }


        }
        else
        {
            transform.position = playerStats.PlayerLocalPosition;
        }
    }



    IEnumerator LatePointerDataPopulation(string type, int playerID)
    {
        if(type == "block")
        {
            var newBlock = Instantiate(GameManager.Instance.BlockPref, playerStats.PointerPosition, playerStats.PointerRotation);
            Debug.Log(newBlock);
            if (newBlock != null)
            {
                newBlock.tag = "POINTER";
                yield return new WaitForSeconds(0.5f);
                newBlock.AddComponent<Pointer>();
                newBlock.GetComponent<Pointer>().SelectedPlayerPointerID = playerID;
            }
        }
        if(type == "arrow")
        {
            var newArrow = Instantiate(GameManager.Instance.ArrowPref, playerStats.PointerPosition, playerStats.PointerRotation);
            Debug.Log(newArrow);
            if (newArrow != null)
            {
                newArrow.tag = "POINTER";
                newArrow.AddComponent<Pointer>();
                newArrow.GetComponent<Pointer>().SelectedPlayerPointerID = playerID;

            }
        }


    }
    public void PreviewPlayOnlyWithoutPointers()
    {
        playerStats.CanMove = true; // to not be able to move the players after loading formation
        if (playerStats.Points != null)
        {
            Debug.Log("data");
            OnNewPathCreated(playerStats.Points);
            renderer.enabled = true;
            renderer.positionCount = playerStats.Points.Count;
            renderer.SetPositions(playerStats.Points.ToArray());
            transform.position = playerStats.PlayerLocalPosition;
        }

    }
    //save it as position
    //search by name
    //public void GetPlay(string playName)
    //{
    //    StartCoroutine(WaitForThePlayersToBeFound(playName));
    //    StartCoroutine(GetSetPlayPositions(playName));
    //}

    //using this
    //IEnumerator GetSetPlayPositions(string playName)
    //{
    //    RemoveAllDrawingsWhenLoadingNew();
    //    yield return new WaitForSeconds(.1f);
    //    if (ES2.Exists("LinePoint" + singlePlayer.PlayerID + " playname " + playName))
    //    {
    //        singlePlayer.points = ES2.LoadList<Vector3>("LinePoint" + singlePlayer.PlayerID + " playname " + playName);
    //        renderer.positionCount = singlePlayer.points.Count;
    //        renderer.SetPositions(singlePlayer.points.ToArray());
    //    }
    //    GameManager.Instance.LoadinPanel.gameObject.SetActive(false);
    //}

    //IEnumerator GetAndSetPlayPosition(string playname)
    //{
    //    RemoveAllDrawingsWhenLoadingNew();
    //    yield return new WaitForSeconds(.3f);
    //    if (ES2.Exists("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playname))
    //    {
    //        singlePlayer.PointerPositionOnly = ES2.Load<Vector3>("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playname);


    //        yield return new WaitForSeconds(.4f);
    //        renderer.positionCount = 2;
    //        if (singlePlayer.PointerPositionOnly != Vector3.zero)
    //        {

    //            if (singlePlayer.playerPosition != Vector3.zero)
    //            {
    //                renderer.SetPosition(0, new Vector3(singlePlayer.playerPosition.x, singlePlayer.playerPosition.y, singlePlayer.playerPosition.z));
    //                renderer.SetPosition(1, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //            }
    //            else
    //            {
    //                renderer.SetPosition(0, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //                renderer.SetPosition(1, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));

    //            }
    //        }

    //        GameManager.Instance.LoadinPanel.gameObject.SetActive(false);
    //    }

    //}



    //USING THIS
    //public void GetFormation(string formationName)
    //{
    //    RemoveAllDrawingsWhenLoadingNew();
    //    renderer.enabled = true;
    //    //  RemoveAllDrawingsWhenLoadingNew();
    //    renderer.positionCount = 2;

    //    singlePlayer.playerPosition = ES2.Load<Vector3>("FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationName);
    //    if (singlePlayer.playerPosition != Vector3.zero)
    //        transform.position = singlePlayer.playerPosition;
    //    else
    //    {
    //        transform.position = PlayerLocalPosition;
    //    }
    //    Debug.Log(transform.position);
    //    // StartCoroutine(GetFormationAfterFromDB(formationName));
    //}

    //IEnumerator GetFormationAfterFromDB(string formationname)
    //{
    //    renderer.enabled = true;
    //    RemoveAllDrawingsWhenLoadingNew();
    //    renderer.positionCount = 2;
    //    yield return new WaitForSeconds(0.3f);
    //    singlePlayer.playerPosition = ES2.Load<Vector3>("FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationname);
    //    if (singlePlayer.playerPosition != Vector3.zero)
    //        transform.position = singlePlayer.playerPosition;
    //    else
    //    {
    //        transform.position = PlayerLocalPosition;
    //    }
    //    Debug.Log(transform.position);
    //}

    //void RemoveAllDrawingsWhenLoadingNew()
    //{
    //    renderer.enabled = true;
    //    creator.PointerHolder.SetActive(false);
    //    var allDrawings = FindObjectsOfType<LineRenderer>();
    //    foreach (var item in allDrawings)
    //    {
    //        item.positionCount = 0;
    //    }
    //    var allPointers = FindObjectsOfType<Pointer>();
    //    if (allPointers != null)
    //    {
    //        foreach (var item in allPointers)
    //        {
    //            if (item != null)
    //            {
    //                // Destroy(item.transform.parent.gameObject);

    //            }
    //        }
    //    }


    //}




    //IEnumerator WaitForThePlayersToBeFound(string playName)
    //{
    //    renderer.enabled = true;
    //    yield return new WaitForSeconds(.3f);

    //    yield return new WaitForSeconds(.1f);

    //    if (ES2.Exists("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName))
    //    {
    //        Debug.Log("the play exists getting data...");

    //        singlePlayer.PointerPositionOnly = ES2.Load<Vector3>("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName);
    //        Vector3 pointerHolder = singlePlayer.PointerPositionOnly;

    //        yield return new WaitForSeconds(.3f);
    //        if (singlePlayer.PointerPositionOnly != Vector3.zero)
    //        {

    //            if (singlePlayer.playerPosition != Vector3.zero)// if formation is selected
    //            {
    //                renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //                renderer.SetPosition(1, new Vector3(singlePlayer.playerPosition.x, singlePlayer.playerPosition.y, singlePlayer.playerPosition.z));
    //                CheckTheFirstPositionAfterDrawing();
    //            }
    //            else   // if there is drawing on default player positions
    //            {
    //                renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //                renderer.SetPosition(1, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //                CheckTheFirstPositionAfterDrawing();
    //            }
    //            Debug.Log("data for play " + playName + " is loaded");
    //        }
    //        else
    //        {
    //            renderer.positionCount = 0;
    //        }
    //    }



    //    //if (singlePlayer.playerPosition != Vector3.zero)// if formation is selected
    //    //{
    //    //    renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //    //    renderer.SetPosition(1, new Vector3(singlePlayer.playerPosition.x, singlePlayer.playerPosition.y, singlePlayer.playerPosition.z));
    //    //    //CheckTheFirstPositionAfterDrawing();
    //    //}
    //    //else   // if there is drawing on default player positions
    //    //{
    //    //    renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //    //    renderer.SetPosition(1, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //    //    //    CheckTheFirstPositionAfterDrawing();
    //    //}


    //    //if (ES2.Exists("PlayPID" + singlePlayer.PlayerID + " playname " + playName))
    //    //{
    //    //    UIManager.Instance.SelectTextType("none", "warning", 0f);
    //    //    PlayerLocalPosition = ES2.Load<Vector3>("PlayPLP" + singlePlayer.PlayerID + " playname " + playName);
    //    //    // singlePlayer.playerPosition = ES2.Load<Vector3>("playerPositionID" + singlePlayer.PlayerID + " tacticname " + tacticname);
    //    //    singlePlayer.PointerPositionOnly = ES2.Load<Vector3>("PlayPID" + singlePlayer.PlayerID + " playname " + playName);
    //    //    transform.position = singlePlayer.PointerPositionOnly;
    //    //    renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //    //    renderer.SetPosition(1, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //    //    // renderer.SetPosition(0, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //    //    //  renderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z));
    //    //    transform.position = PlayerLocalPosition; // removes the positioning on the players at the end of the line
    //    //    CheckTheFirstPositionAfterDrawing();
    //    //    //var newArrow = Instantiate(Pointer, singlePlayer.PointerPositionOnly, Quaternion.identity);
    //    //    //newArrow.SetActive(true);
    //    //    //newArrow.transform.position = singlePlayer.PointerPositionOnly;

    //    //}
    //    ////else
    //    ////{
    //    ////    //PlayerLocalPosition 
    //    ////    singlePlayer.playerPosition  = ES2.Load<Vector3>("FormationPLP" + singlePlayer.PlayerID + " formationname " + playName);
    //    ////    transform.position = singlePlayer.playerPosition;
    //    ////}

    //    GameManager.Instance.LoadinPanel.gameObject.SetActive(false);

    //}


    //void CheckTheFirstPositionAfterDrawing()
    //{
    //    var first = renderer.GetPosition(0);
    //    Debug.Log(first.magnitude);
    //    if (first.magnitude.Equals(0))
    //    {
    //        renderer.positionCount = 0;
    //    }
    //}

    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {

            if (Input.GetMouseButtonDown(0))
            {
                var foundPlayer = hitInfo.transform.GetComponent<SinglePlayer>();
                if (foundPlayer != null)
                {
                    //  Debug.Log("clicked on player");

                    CameraMovement.Instance.isPanning = false;
                    GameManager.Instance.CanDrawIfSelected = true;
                    if (foundPlayer.playerStats.CanMove)
                        UIManager.Instance.SelectTextType("Drawing for player " + foundPlayer.playerStats.PlayerID, "success", 1f);

                }
                else
                {
                    UIManager.Instance.SelectTextType("", "", 0f);
                    GameManager.Instance.CanDrawIfSelected = false;
                    CameraMovement.Instance.isPanning = true;
                    UIManager.Instance.SelectTextType("", "", 0f);
                }

            }

        }

        if (transform.position.x > 23)
            transform.position = new Vector3(23, transform.position.y, transform.position.z);
        else if (transform.position.x < -23)
            transform.position = new Vector3(-23, transform.position.y, transform.position.z);



        //if(transform.position != null)
        //{
        //    if (GameManager.Instance.InFormation)
        //        playerStats.CanMove = false;f
        //    else
        //        playerStats.CanMove = true;
        //}



        Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit HitInfo;
        if (Physics.Raycast(rayCast, out HitInfo))
        {

            if (Input.GetMouseButtonDown(2))
            {
                var foundPlayer = HitInfo.transform.GetComponent<SinglePlayer>();
                if (foundPlayer != null)
                {
                    foundPlayer.playerStats.Points.Clear();
                    foundPlayer.renderer.positionCount = 0;
                    var pointsandlines = GameObject.FindGameObjectsWithTag("POINTER");
                    if(pointsandlines != null)
                    {
                        foreach (var item in pointsandlines)
                        {
                            if(item.GetComponent<Pointer>() != null)
                            {
                                if (item.GetComponent<Pointer>().SelectedPlayerPointerID == foundPlayer.playerStats.PlayerID)
                                {
                                    Destroy(item.gameObject);
                                }
                            }
                      
                        }
                    }
                  
                }
            }
        }


    }

    public void ActivatelayerPath()
    {
        pathMover = GetComponent<PathMover>();
        if (pathMover != null)
            pathMover.enabled = false;
    }




    void OnMouseOver()
    {
        foreach (var item in GameManager.Instance.allPlayers)
        {
            if (item.GetComponentInChildren<MeshRenderer>() != null)
            {

                item.GetComponentInChildren<MeshRenderer>().enabled = false;

            }
        }
        transform.GetComponentInChildren<MeshRenderer>().enabled = true;


        if (playerStats.CanMove)
        {

            //  pathMover.SetPoints(playerStats.Points);
            GameManager.Instance.SelectedPlayerID = playerStats.PlayerID;
        }



    }

    void OnMouseDown()
    {

        mZCoord = Camera.main.WorldToScreenPoint(
        gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()

    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition;



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }
    void OnMouseDrag()
    {

        if (!playerStats.CanMove)
        {

            transform.position = GetMouseAsWorldPoint() + mOffset;
            playerStats.PlayerLocalPosition = transform.position;
        }

    }







    [Serializable]
    public class Player
    {
        public int PlayerID;
        public string PlayerName;
        public string PlayerType;
        public Vector3 playerPosition;
        public string TacticName;
        public Vector3 PointerPositionOnly;
        public bool isSelected;
        public List<Vector3> points = new List<Vector3>();
    }

    [Serializable]
    public class PlayerStats
    {
        public string PlayerName;
        public int PlayerID;
        public bool CanMove;
        public bool HasDrawnedLine;
        public string PlayerPointerType;
        public Vector3 StartingPlayerLocalPosition;
        public Vector3 PlayerLocalPosition;
        public Vector3 PointerPosition;
        public Quaternion PointerRotation;
        public List<Vector3> Points;
        public int PointerCounter;
    }

}


enum PlayerType
{
    player,
    middlePlayer
}
