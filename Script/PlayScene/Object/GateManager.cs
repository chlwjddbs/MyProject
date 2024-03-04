using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GateManager : MonoBehaviour
{
    [Serializable]
    public class GateInfo
    {
        public string gateName;
        public Vector3 coordinate;
        public bool isActive;
        public GateInfo(string _gateName, Vector3 _coordinate, bool _isActive = false)
        {
            gateName = _gateName;
            coordinate = _coordinate;
            isActive = _isActive;
        }
    }

    public static GateManager instence;

    public List<GameObject> gateList;

    public Dictionary<int, GateInfo> gateDic;

    public GameObject teleportGateUI;
    private RectTransform teleportGateUIRect;

    public bool isOpen;

    public GameObject player;

    public GameObject gateSlotPrefab;

    //UI에 표시 시켜될 gateSlot이 있는 부모 오브젝트
    public Transform GateListUI;

    //이동하려는 게이트가 게이트를 연 장소인지 확인
    public int startingGate;

    private bool isAllgate  = false;

    private void Awake()
    {
        instence = this;
    }

    // Start is called before the first frame update
    void Update()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
            //player.GetComponent<PlayerController>().SetState(PlayerState.Idle);
        }

        /*
        if(Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.P) && !isAllgate)
        {
            isAllgate = true;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf == true)
                {
                    transform.GetChild(i).gameObject.GetComponent<TeleportGate>().OpenGate();
                }
            }
        }
        */
    }

    public void SetData()
    {
        gateDic = new Dictionary<int, GateInfo>();
        teleportGateUIRect = teleportGateUI.GetComponent<RectTransform>();
        CloseUI();

        if (DataManager.instance.newGame)
        {
            
        }
        else
        {
            LoadData();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out TeleportGate _gate))
            {
                _gate.gateNumber = i;
                _gate.SetData(this);
            }
        }
    }

    private void LoadData()
    {
        foreach (var item in DataManager.instance.userData.gateInfo)
        {
            Debug.Log(item.gateName);
        }
        for (int i = 0; i < DataManager.instance.userData.gateNum.Count; i++)
        {
            GateInfo gateInfo = DataManager.instance.userData.gateInfo[i];
            ActiveGateSlot(DataManager.instance.userData.gateNum[i], gateInfo.gateName, gateInfo.coordinate, gateInfo.isActive);
        }
    }

    public void SaveData()
    {
        DataManager.instance.userData.gateNum = new List<int>(gateDic.Keys);
        DataManager.instance.userData.gateInfo = new List<GateInfo>(gateDic.Values);

        foreach (var _gate in DataManager.instance.userData.gateNum)
        {
            Debug.Log(_gate);
        }
        foreach (var _gate in DataManager.instance.userData.gateInfo)
        {
            Debug.Log(_gate.gateName + " " + _gate.coordinate);
        }
    }

    public void Move(int _gateNum)
    {
        //이동 할 게이트 넘버를 통해 게이트 딕셔너리를 _value 를 가져온다.
        if (gateDic.TryGetValue(_gateNum, out GateInfo _value))
        {
            //_value가 활성화 된 상태이고
            if (_value.isActive)
            {
                //현재 게이트와 이동할 게이트가 같다면 다른 게이트를 선택해 달라는 메시지 출력
                if (startingGate == _gateNum)
                {
                    SequenceText.instance.SetSequenceText(null, "OtherGate");
                }
                //현재 게이트와 이동할 게이트가 다르면 이동할 게이트의 위치로 이동
                else
                {
                    player.GetComponent<NavMeshAgent>().enabled = false;
                    player.transform.position = gateDic[_gateNum].coordinate;
                    CloseUI();
                    player.GetComponent<NavMeshAgent>().enabled = true;
                }
            }
        }
    }

    public void ToggleUI()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            teleportGateUIRect.anchoredPosition = new Vector3(0, 0, 0);
        }
        else
        {
            teleportGateUIRect.anchoredPosition = new Vector3(-800f, 0, 0);
        }

    }

    public void CloseUI()
    {
        isOpen = false;
        teleportGateUIRect.anchoredPosition = new Vector3(-2000f, 0, 0);
    }

    public void OpenUI()
    {
        isOpen = true;
        teleportGateUIRect.anchoredPosition = new Vector3(0, 0, 0);
        //player.GetComponent<PlayerController>().SetState(PlayerState.Action);
        //PlayerController.isAction = true;
    }

    /*
    public void CreateGateSlot(int _gateNum, string _gateName, Vector3 _gateCoordinate)
    {
        gateDic.Add(_gateName, _gateCoordinate);
        GameObject _gateSlot = Instantiate(gateSlot, slotList);
        _gateSlot.GetComponent<GateSlot>().SetGate(_gateNum, _gateName, _gateCoordinate, this.gameObject);
    }
    */

    //GateManager에서 관리되고 있는 게이트들이 사용 가능한 상태가 되었을때 플레이어에게 UI를 제공한다.
    public void ActiveGateSlot(int _gateNum, string _gateName, Vector3 _gateCoordinate, bool _isAcive = false)
    {
        gateDic.Add(_gateNum, new GateInfo(_gateName, _gateCoordinate, _isAcive));
        Debug.Log(GateListUI.GetChild(_gateNum));
        GateListUI.GetChild(_gateNum).gameObject.SetActive(true);
        GateListUI.GetChild(_gateNum).gameObject.GetComponent<GateSlot>().ActiveGate(_gateNum, _gateName, _gateCoordinate, this);
    }

    public void CheckStartingPoint(int _startGateNum)
    {
        startingGate = _startGateNum;
    }
}
