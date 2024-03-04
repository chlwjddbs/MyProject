using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

//Jason을 통해 게임 정보를 관리하기 위한 Data Class

//저장
// 1. 저장 데이터를 받음
// 2. 데이터를 Json으로 변경
// 3. Json을 외부에 저장

//불러오기
// 1. 외부에서 Json을 받음
// 2. Json을 데이터로 변경
// 3. 데이터 적용

//저장할 데이터를 받을 클래스 생성


public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    //유저 데이터
    public UserData userData = new UserData();

    //슬롯에 Json정보를 받을 때 사용할 loadData
    //loadData를 사용하지 않고 위의 userData와 함께 사용하면
    //씬이 바뀌어서 넘어갔을시 saveFileManager가 실행되면서 파일들을 불러오면서
    //userData 데이터가 바뀌기떄문에 저장이 정상적으로 작동하지 않고 마지막에 불러온 파일 정보 기준으로 저장된다.
    //그럼으로 loadData를 따로 나누어 관리.
    public UserData loadData = new UserData();

    private string path;
    private string autoSave = "AutoSave";
    //private string fileName;
    public DirectoryInfo direcInfo;

    //public int currentSlot;
    public string slotName = "";
    public SaveSlot selectedSlot;

    public Fader fader;

    public bool newGame = true;

    public bool isSet = false;

    private SaveSlot autoSaveSlot;

    //public Dictionary<int> slotItem;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //유니티에서 자동으로 생성해주는 경로
        //path + fileName 으로 경로 설정을 하기 위에 path에 "/" 추가
        //C:/Users/장은서/AppData/LocalLow/DefaultCompany/My project 2 +/Save/ 
        path = Application.persistentDataPath + "/Save/";

        direcInfo = new DirectoryInfo(path);
    }
    // Start is called before the first frame update

    private void Start()
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("폴더 없음");
            Directory.CreateDirectory(path);
            return;
        }
        if (direcInfo.GetFiles().Length == 0)
        {
            Debug.Log("저장된 파일이 없습니다.");
        }
        else
        {
            Debug.Log("파일 불러오기");
        }     
    }
   
    public void AutoSaveData()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("추가");
        }

        string data = JsonUtility.ToJson(userData);
        File.WriteAllText(path + autoSave, data);
        autoSaveSlot?.SetSlot();
        Debug.Log(path);
    }

    public void CreateSaveData()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string data = JsonUtility.ToJson(userData);
        File.WriteAllText(path + slotName, data);
        selectedSlot.SetSlot();
    }
  
    public void LoadData(string _slotName,SaveSlot _saveSlot)
    {
        if(!File.Exists(path + _slotName))
        {
            //Debug.Log("저장된 파일이 없습니다.");
            return;
        }
        //Debug.Log(_slotName);
        string data = File.ReadAllText(path + _slotName);
        loadData = JsonUtility.FromJson<UserData>(data);
        _saveSlot.userData = loadData;
        //Debug.Log(loadData.userName);
    }

    public void SelectedSlot(string _slotName, SaveSlot _selectedSlot, UserData _userData)
    {
        if(slotName == _slotName)
        {
            Debug.Log("같은버튼");
            selectedSlot.DeSelectSlot();
            ResetSlotData();
            return;
        }

        Debug.Log("다른버튼");
        try
        {
            selectedSlot.DeSelectSlot();
        }
        catch
        {

        }
        slotName = _slotName;
        selectedSlot = _selectedSlot;
        loadData = _userData;
        selectedSlot.SelectSlot();
    }

    public void ResetSlotData()
    {
        slotName = "";
        selectedSlot = null;
        loadData = new UserData();
    }

    public void DeleteData()
    {
        //그렇지 않으면 파일 삭제
        File.Delete(path + slotName);
        selectedSlot.ResetSlot();
        loadData = selectedSlot.userData;
        selectedSlot = null;
    }

    //선택한 슬롯의 저장파일을 삭제하는 함수
    public bool DeleteButtonSelect()
    {
        //슬롯 선택을 취소할 시 ""으로 저장된다. 
        //파일 선택이 되지 않았음으로 return;
        if(slotName == "")
        {
            Debug.Log("선택된 정보 없음");
            return false;
        }

        if(slotName == "AutoSave")
        {
            Debug.Log("삭제 할 수 없는 슬롯");
            return false;
        }

        //선택한 파일을 찾는다.
        //없으면 이미 삭제되었습니다 출력
        if (!File.Exists(path + slotName))
        {
            Debug.Log("이미 삭제되었습니다..");
            return false;
        }

        return true;
       
    }

    public bool SaveButtonEmpty()
    {
        if(loadData.userName == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LoadGame()
    {
        //userData = selectedSlot.GetComponent<SaveSlot>().userData;
        userData = loadData;
        //Time.timeScale = 1;
        fader.SceneLoad(userData.sceneName);
        newGame = false;
        isSet = false;
        ResetSlotData();
        //SceneManager.LoadScene(userData.sceneName);
        Debug.Log(userData.userName);
        //AudioManager.instance.StopBGM(fader.fadeInTime);
    }

    public void GotoMainMenu()
    {
        userData = new UserData();
        loadData = new UserData();
        fader.SceneLoad("MainMenuScene");
    }

    public void GetAutoSaveSlot(SaveSlot _auto)
    {
        autoSaveSlot = _auto;
    }
}

public class UserData
{
    #region 게임 스테이지 데이터
    //게임이 저장 된 시간
    public string saveTime;

    //유저가 게임 시작시 만든 이름
    public string userName = new string("chy");

    //현재 유저가 있는 층
    public string currentStage;

    //현재 유저가 있는 지역이름
    public string currentArea;

    //현재 유저가 있는 씬의 이름
    public string sceneName;
    #endregion

    #region 플레이어 데이터
    public Vector3 playerPos = new Vector3(52.5f, 0, 175f);

    public int level;
    public float currentExp;
    public float nextLvExp;

    public float remainHealth;
    public float health;
    public float remainMana;
    public float mana;

    public float moveSpeed;
    public float basicDamage;
    public float basicDefence;
    #endregion

    #region 플레이어 장비 데이터
    public Item[] inventoryItem;
    //인벤토리 기본 크기 25
    public int invenSize = 25;
    //캐릭터 생성시 사용중인 인벤토리 0
    public int spareSlot= 0;

    public List<int> slotNum = new List<int>();
    public List<Inventory.InvenItem> invenItem = new List<Inventory.InvenItem>();
  
    public EquipItem[] equipmentItem;
    #endregion

    #region 플레이어 스킬 데이터
    public List<SkillItem> learedSkill = new List<SkillItem>();
    public List<int> skillButtonNum = new List<int>();
    public List<SkillBook.SetEquipSkill> skillButtonInfo = new List<SkillBook.SetEquipSkill>();
    #endregion;

    #region 텔레포트 게이트
    public List<int> gateNum;
    public List<GateManager.GateInfo> gateInfo;
    #endregion

    #region 적유닛 관리
    public List<Enemy.EnemyData> eDatas_1F = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_skeletons = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_olaf = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_gubne = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_spawn = new List<Enemy.EnemyData>();
    #endregion

    #region 필드 아이템 관리
    public List<bool> startFieldItem_1F = new List<bool>();
    public List<DropItemManager.AddDropItem> FiledItme_1F = new List<DropItemManager.AddDropItem>();
    #endregion

    #region 필드 퍼즐 관리
    public List<bool> puzzleKey;
    public List<bool> openGate;
    #endregion
}