using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ActivatePlayerOnButtonClick : MonoBehaviour {

    // Use this for initialization
    public int PlayerID;
    public Button playerBtn;
	void Start () {

        playerBtn = GetComponent<Button>();
        if (playerBtn != null)
        {
            Button btn = playerBtn.GetComponent<Button>();
            btn.onClick.AddListener(() => SetPlayerOnButtonClick(PlayerID));
        }
    }


	public void SetPlayerOnButtonClick(int playerID)
    {
      //  GameManager.Instance.pathCreator.enabled = true;
      //  GameManager.Instance.pathCreator.gameObject.SetActive(true);
        Debug.Log(" using player with id " + PlayerID);
        for (int i = 0; i < GameManager.Instance.allPlayers.Count; i++)
        {
            //GameManager.Instance.allPlayers[i].gameObject.SetActive(false);
            GameManager.Instance.allPlayers[i].GetComponent<MeshRenderer>().material.color = Color.white;
            GameManager.Instance.allPlayers[i].playerNav.enabled = false;
            GameManager.Instance.allPlayers[i].pathMover.enabled = false;
        }

        //GameManager.Instance.allPlayers[playerID].gameObject.SetActive(true);
        GameManager.Instance.allPlayers[playerID].GetComponent<MeshRenderer>().material.color = Color.red;
        GameManager.Instance.allPlayers[playerID].playerNav.enabled = true;
        GameManager.Instance.allPlayers[playerID].pathMover.enabled = true;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
