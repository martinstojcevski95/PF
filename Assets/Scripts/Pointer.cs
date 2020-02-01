using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    public int SelectedPlayerPointerID;
    public Vector3 PointerLocalPosition;
    public Quaternion PointerLocalRotation;
    public bool isActive;
    public string SinglePointerType;
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}


    public void SavePointerPositionAndRotation()
    {
        if(PointerLocalPosition != Vector3.zero)
        {
            Debug.Log("this pointer needs to be saved");
            ES2.Save(transform.position,  "PointerPosition" + SelectedPlayerPointerID + "PlayerID");
            ES2.Save(transform.rotation, "PointerRotation" + SelectedPlayerPointerID + "PlayerID");
        }
    }


    public void LoadPointerPositionAndRotation()
    {
 

         transform.position = ES2.Load<Vector3>("PointerPosition" + SelectedPlayerPointerID + "PlayerID");
         transform.rotation = ES2.Load<Quaternion>("PointerRotation" + SelectedPlayerPointerID + "PlayerID");

    }

    [Serializable]
    public class SinglePointer
    {

        public string PointerType;
    }
}
