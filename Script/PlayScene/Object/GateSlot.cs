using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class GateSlot : MonoBehaviour
{
    public int gateNumber;
    public string gateName;
    public Vector3 gateCoordinate;
    GateManager gateManager;

    public LocalizeStringEvent gateNameUI;


    /*
    public void SetGate(int _gateNum, string _gateName, Vector3 _gateCoordinate, GameObject _gateManager)
    {
        gateNumber = _gateNum;
        gateName = _gateName;
        gateCoordinate = _gateCoordinate;
        gateManager = _gateManager;
        gateNameUI.text = gateName;
    }
    */

    public void ActiveGate(int _gateNum, string _gateName, Vector3 _gateCoordinate, GateManager _gateManager)
    {
        gateNumber = _gateNum;
        gateName = _gateName;
        gateCoordinate = _gateCoordinate;
        gateManager = _gateManager;
        gateNameUI.StringReference.TableEntryReference = gateName;
    }

    public void UseGate()
    {
        gateManager.Move(gateNumber);
    }
}
