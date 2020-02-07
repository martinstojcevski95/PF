using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Material drawnLineMaterial;
    private List<Vector3> points = new List<Vector3>();

    public GameObject PointerHolder;
    public GameObject blockPref;
    public GameObject arrowPref;
    public float RotationSpeed;


    float lockarrowX = 0;
    float lockarrowz = 0;

    public Quaternion _lookRotation;
    public Vector3 _direction;

    [SerializeField]
    PointerType pointerType;


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        PointerHolder = GameObject.Find("PointerHolder");
        if (PointerHolder != null)
        {
            PointerHolder.SetActive(false);
        }

        pointerType = PointerType.block;

        PointerHolder.transform.GetChild(0).gameObject.SetActive(false);
        PointerHolder.transform.GetChild(1).gameObject.SetActive(true);
    }


    public void BlockType(string pointertype)
    {
        PointerHolder.SetActive(true);
        if (pointertype == "Arrow")
        {
            pointerType = PointerType.arrow;
            PointerHolder.transform.GetChild(0).gameObject.SetActive(true);
            PointerHolder.transform.GetChild(1).gameObject.SetActive(false);
            UIManager.Instance.Arrow.interactable = false;
            UIManager.Instance.Block.interactable = true;
        }

        if (pointertype == "Block")
        {
            pointerType = PointerType.block;
            PointerHolder.transform.GetChild(0).gameObject.SetActive(false);
            PointerHolder.transform.GetChild(1).gameObject.SetActive(true);
            UIManager.Instance.Arrow.interactable = true;
            UIManager.Instance.Block.interactable = false;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            points.Clear();

        //DETECTING THE SURFACE AND START COUNTING DRAWN POINTSS
        if (Input.GetButton("Fire1"))
        {
            // arrow.SetActive(true);
            //   PointerHolder.SetActive(true);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (DistanceToLastPoint(hitInfo.point) > 1f)
                {
                    if (GameManager.Instance.CanDrawIfSelected)
                    {
             
                        PointerHolder.transform.position = hitInfo.transform.position;
                        PointerHolder.SetActive(true);
                        points.Add(hitInfo.point);
                        lineRenderer.enabled = true;
                        lineRenderer.positionCount = points.Count;
                        lineRenderer.SetPositions(points.ToArray());
                        //  lineRenderer.SetPosition(0, new Vector3(-100, -1000, 0)); // hide the first line bug
                        if (lineRenderer.positionCount >= 2)
                        {
                        
                            int index = lineRenderer.positionCount - 1;
                            int secondToLastIndex = lineRenderer.positionCount-2;

                            Vector3 arrowloc = lineRenderer.GetPosition(index);

                            Vector3 arrowNearEnd = lineRenderer.GetPosition(secondToLastIndex);

                            //move arrow
                            PointerHolder.transform.position = arrowloc;



                            var heading = arrowloc - arrowNearEnd;
                            var distance = heading.magnitude;
                            var direction = heading / distance; // This is now the normalized direction.
                                                                //Vector3 targetDir = arrowloc - arrowNearEnd;


                            _direction = (arrowloc - arrowNearEnd).normalized;
                            _lookRotation = Quaternion.LookRotation(_direction);
                            PointerHolder.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 90);
                            PointerHolder.transform.LookAt(arrowNearEnd, Vector3.left);
                        }
                    

                    }

                }
            }

        }


        // DRAW NEW LINE AND NAME IT SAVED LINE
        else if (Input.GetButtonUp("Fire1"))
        {

            // ADD POINTS TO EACH  PATH MOVER POINTS

            //   OnNewPathCreated(points);

            lineRenderer.enabled = true;
            var obj = new GameObject();
            obj.name = "Saved Line";
            var newLine = obj.AddComponent<LineRenderer>();
            // obj.SetActive(false);
            if (newLine != null)
            {
                newLine.positionCount = points.Count;
                newLine.SetPositions(points.ToArray());
                newLine.enabled = true;
                newLine.material = drawnLineMaterial;
                newLine.endWidth = 0.15f;
                newLine.startWidth = 0.15f;
                newLine.tag = "POINTER";


                int pointsNumber = points.Count;
                if (newLine.positionCount >= 2)
                {

                    //  newLine.SetPosition(0, new Vector3(-100, -1000, 0)); // hide the first line bug
                    // obj.SetActive(true);
                    var lastpositionVector = points[pointsNumber - 1];

                    if (pointerType == PointerType.arrow)
                    {

                        var newArrow = Instantiate(arrowPref, lastpositionVector, Quaternion.identity);
                        newArrow.SetActive(true);
                        newArrow.transform.parent = newLine.transform;
                        newArrow.transform.rotation = PointerHolder.transform.GetChild(0).transform.rotation;
                        newArrow.AddComponent<Pointer>();
                        if (newArrow.GetComponent<Pointer>() != null)
                        {
                          //newArrow.transform.localScale = new Vector3(2, 2, 2);
                            newArrow.GetComponent<Pointer>().SelectedPlayerPointerID = GameManager.Instance.SelectedPlayerID;
                            newArrow.GetComponent<Pointer>().PointerLocalRotation = newArrow.transform.rotation;
                            newArrow.GetComponent<Pointer>().PointerLocalPosition = newArrow.transform.position;
                            newArrow.GetComponent<Pointer>().SinglePointerType = "arrow";
                            newArrow.GetComponent<Pointer>().PointerCounter = 1;
                            if (GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.HasDrawnedLine == false)
                            {
                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.HasDrawnedLine = true;
                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.Points.Clear();
                                foreach (var item in points)
                                {

                                    GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.Points.Add(item);
                                    // GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].singlePlayer.points.Add(item);
                                }

                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PointerPosition = newArrow.transform.position;// pointer pos
                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PointerRotation = newArrow.transform.rotation; // pointer rot

                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PlayerPointerType = newArrow.GetComponent<Pointer>().SinglePointerType;// pointer type
                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PointerCounter = 1;


                                GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].pathMover.SetPoints(GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.Points); // movign the player for the drawn points
                            }
                            else
                            {
                                Destroy(newLine.gameObject);
                            }


                            //newArrow.GetComponent<Pointer>().PointerLocalPosition = newArrow.transform.position;
                            //GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].singlePlayer.PointerPositionOnly = newArrow.GetComponent<Pointer>().PointerLocalPosition;
                        }

                        //Debug.Log("last added arrow for player with ID " + GameManager.Instance.SelectedPlayerID);
                    }
                    if (pointerType == PointerType.block)
                    {
                        obj.SetActive(true);
                        var newBlock = Instantiate(blockPref, lastpositionVector, Quaternion.identity);
                        newBlock.SetActive(true);
                        newBlock.transform.parent = newLine.transform;
                        newBlock.transform.rotation = PointerHolder.transform.rotation;
                        newBlock.AddComponent<Pointer>();
                        if (newBlock.GetComponent<Pointer>() != null)
                        {
                            newBlock.GetComponent<Pointer>().SelectedPlayerPointerID = GameManager.Instance.SelectedPlayerID;
                            newBlock.GetComponent<Pointer>().PointerLocalRotation = newBlock.transform.rotation;
                            newBlock.GetComponent<Pointer>().PointerLocalPosition = newBlock.transform.position;
                            newBlock.GetComponent<Pointer>().PointerCounter = 1;
                            //GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].singlePlayer.PointerPositionOnly = newBlock.GetComponent<Pointer>().PointerLocalPosition;
                            newBlock.GetComponent<Pointer>().SinglePointerType = "block";
                            if (GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.HasDrawnedLine == false)
                            {
                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.HasDrawnedLine = true;
                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.Points.Clear();
                                foreach (var item in points)
                                {

                                    GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.Points.Add(item);
                                    // GameManager.Instance.allPlayers[newArrow.GetComponent<Pointer>().SelectedPlayerPointerID].singlePlayer.points.Add(item);
                                }

                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PointerPosition = newBlock.transform.position;// pointer pos
                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PointerRotation = newBlock.transform.rotation; // pointer rot

                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PlayerPointerType = newBlock.GetComponent<Pointer>().SinglePointerType;// pointer type
                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.PointerCounter = 1;

                                GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].pathMover.SetPoints(GameManager.Instance.allPlayers[newBlock.GetComponent<Pointer>().SelectedPlayerPointerID].playerStats.Points); // movign the player for the drawn points
                            }
                            else
                            {
                               Destroy(newLine.gameObject);
                            }
                        }

                    }
                }
                else
                {
                   Destroy(newLine.gameObject);
                }
            }
        }

        //DELETE DRAWN LINE
        if (Input.GetButtonUp("Fire3"))
        {
            PointerHolder.SetActive(false);
            lineRenderer.positionCount = 0;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (DistanceToLastPoint(hitInfo.point) > 1f)
                {
                    var selectedplayer = hitInfo.transform.gameObject.GetComponent<SinglePlayer>();
                    if (selectedplayer != null)
                    {
                        selectedplayer.singlePlayer.isSelected = false;
                        selectedplayer.GetComponent<LineRenderer>().positionCount = 0;
                        Pointer[] savedLine = FindObjectsOfType<Pointer>();
                        for (int i = 0; i < savedLine.Length; i++)
                        {
                            if (savedLine[i].SelectedPlayerPointerID == selectedplayer.singlePlayer.PlayerID)
                            {
                                UIManager.Instance.SelectTextType("line has been removed", "info", 1f);
                                savedLine[i].transform.parent.GetComponent<LineRenderer>().positionCount = 0;
                                Destroy(savedLine[i].gameObject);

                            }
                        }
                    }

                }

            }
        }



    }



    private float DistanceToLastPoint(Vector3 point)
    {
        if (!points.Any())
            return Mathf.Infinity;
        return Vector3.Distance(points.Last(), point);
    }
}

enum PointerType
{
    none,
    arrow,
    block
}
