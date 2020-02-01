using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float mouseSensitivity = .032f;
    Vector3 lastPosition;
    public bool isPanning;
    public bool InGame;
    public static CameraMovement Instance;

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        InGame = true;
        isPanning = true;
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

    /// <summary>
    // set in game to true only when returning from 3d to 2d view
    /// </summary>
    public void To2DView()
    {
        InGame = true;
    }


    // Update is called once per frame
    void Update()
    {
        // -------------------Code for Zooming Out------------
        if (Input.GetAxis("Scroll") < 0)
        {
            if (Camera.main.fieldOfView <= 32)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5f;

        }
        // ---------------Code for Zooming In------------------------
        if (Input.GetAxis("Scroll") > 0)
        {
            if (Camera.main.fieldOfView > 2)
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

    }
}
