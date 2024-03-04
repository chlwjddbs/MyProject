using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGate : Interaction
{
    //public GameObject teleportGate;
    public GateManager gateManager;

    public int gateNumber;
    public string gateName;
    public Vector3 gateCoordinate;
    public bool isActive = false;

    /*
    private void Start()
    {
        gateManager = transform.GetComponentInParent<GateManager>();
        gateCoordinate = transform.position;
        if (gateManager.gateDic.ContainsKey(gateNumber))
        {
            isActive = true;
        }
    }
    */

  
    public override void DoAction()
    {      
        if (Input.GetMouseButtonUp(0))
        {
            OpenGate();
        }
    }

    public void OpenGate()
    {
        //gateManager.OpenUI();

        if (isActive)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (gateManager.gateDic.Count < 2)
                {
                    SequenceText.instance.SetSequenceText(null, "NoConnectGate");
                }
                else
                {
                    SequenceText.instance.SetSequenceText(null, "SelectMoveGate");
                    gateManager.CheckStartingPoint(gateNumber);
                    gateManager.OpenUI();
                }
            }
        }
        else
        {
            //gateManager.CreateGateSlot(gateNumber, gateName, gateCoordinate);
            isActive = true;
            gateManager.ActiveGateSlot(gateNumber, gateName, gateCoordinate , isActive);
            SequenceText.instance.SetSequenceText(null, "FoundNewGate");
        }
    }

    public void SetData(GateManager _gateManager)
    {
        gateManager = _gateManager;
       

        if (DataManager.instance.newGame) 
        {
             gateCoordinate = transform.position;
            //gateManager.gateDic.Add(gateNumber, new GateManager.GateInfo(gateName, gateCoordinate, isActive));
        }
        else
        {
            LoadData();
        }
        
    }

    public void LoadData()
    {
        if (gateManager.gateDic.TryGetValue(gateNumber, out GateManager.GateInfo _gateInfo))
        {
            isActive = _gateInfo.isActive;
            gateCoordinate = _gateInfo.coordinate;
            transform.position = gateCoordinate;
            gameObject.SetActive(isActive);
        }
        else
        {
            gateCoordinate = transform.position;
            Debug.Log("NoStoredGateInfo");
        }
    }
}
