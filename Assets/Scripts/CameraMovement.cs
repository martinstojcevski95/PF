using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    float mouseSensitivity = .032f;
    Vector3 lastPosition;
    public bool isPanning;
    public bool InGame;
    public static CameraMovement Instance;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    Transform FPSViewCamera;


    public bool InFPSView;
    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        InGame = true;
        isPanning = true;
        FPSViewCamera = GameObject.FindGameObjectWithTag("fpsview").transform;
    }

    public void EnablePanning()
    {
        isPanning = true;
        InGame = true;
        var players = FindObjectsOfType<SinglePlayer>();
        if (players != null)
        {
            foreach (var item in players)
            {
                item.creator.enabled = false;
            }
        }
    }
    public void DisablePanning()
    {
        InGame = false;
        isPanning = false;
        var players = FindObjectsOfType<SinglePlayer>();
        if (players != null)
        {
            foreach (var item in players)
            {
                item.creator.enabled = true;
            }
        }
    }

    public void UIInfoAboutWireCam()
    {
        UIManager.Instance.SelectTextType("You are now in WireCam View. The players movement will be animated", "success", 3f);
        UIManager.Instance.View3D.GetComponent<Image>().color = UIManager.Instance.Selected;
        UIManager.Instance.View2D.GetComponent<Image>().color = UIManager.Instance.Unselected;
    }

    public void UIOverheadInfo()
    {
        UIManager.Instance.View3D.GetComponent<Image>().color = UIManager.Instance.Unselected;
        UIManager.Instance.View2D.GetComponent<Image>().color = UIManager.Instance.Selected;
        UIManager.Instance.SelectTextType("You are now in Overhead/TopDown View.", "success", 3f);
    }
    /// <summary>
    // set in game to true only when returning from 3d to 2d view
    /// </summary>
    public void To2DView()
    {
        InGame = true;
    }

    public void PausePanning()
    {
        InGame = false;
    }

    public void ResumePanning()
    {
        InGame = true;
    }

    // Update is called once per frame
    void Update()
    {
        // -------------------Code for Zooming Out------------
        if (Input.GetAxis("Scroll") < 0)
        {
            if (Camera.main.fieldOfView <= 50.9f)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5f;

        }
        // ---------------Code for Zooming In------------------------
        if (Input.GetAxis("Scroll") > 0)
        {
            if (Camera.main.fieldOfView > 6)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5f;
        }


        //panning

        if (InGame)
        {
            if (isPanning)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lastPosition = Input.mousePosition;
                }

                if (Input.GetMouseButton(0))
                {
                    var Vector3 = Input.mousePosition - lastPosition;
                    transform.Translate(Vector3.x * mouseSensitivity, Vector3.y * mouseSensitivity, 0);
                    lastPosition = Input.mousePosition;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            InFPSView = true;
            GameManager.Instance.EnableOrDisableCollidersOnPlayers(false);
            UIManager.Instance.SelectTextType("You are now in the First Person View", "success", 3f);

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InFPSView = false;
          //  GameManager.Instance.OverheadView();
            for (int i = 0; i < GameManager.Instance.allPlayers.Count; i++)
            {
                GameManager.Instance.allPlayers[i].transform.position = GameManager.Instance.allPlayers[i].playerStats.PlayerLocalPosition;
                GameManager.Instance.allPlayers[i].pathMover.enabled = false;
                GameManager.Instance.allPlayers[i].playerNav.enabled = false;



            }
            GameManager.Instance.EnableOrDisableCollidersOnPlayers(true);
          //  UIManager.Instance.SelectTextType("You are now in Overhead/TopDown View.", "success", 3f);
        }



        if (InFPSView)
        {
            Camera.main.transform.position = FPSViewCamera.transform.position;
            Camera.main.transform.rotation = FPSViewCamera.transform.rotation;
            yaw += speedH * Input.GetAxis("Mouse X");
           // pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(0, yaw, 0.0f);

        }
    }
}
