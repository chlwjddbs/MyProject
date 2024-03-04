using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    protected Fader fader;

    public GameObject player;
    protected string startStage;
    public string currentStage;

    protected string startArea;
    public string currentArea;

    protected DataManager dataManager;

    public DropItemManager dropItemManager;
    public List<GameObject> startFieldItems;

    public List<Enemy> stageEnemy;
    public List<Enemy> spawnEnemy;

    

    private void Awake()
    {
        dataManager = DataManager.instance;
        fader = dataManager.fader;
        SaveFileManager.isMain = false;
    }

    void Start()
    {
        SetData();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            AutoSave();
        }
    }

    public virtual void SetData()
    {
        if (DataManager.instance.newGame)
        {
            StartStage();
        }
        else
        {
            LoadData();
        }

        //플레이어 정보를 먼저 불러오고 아이템을 불러와야 플레이어 정보 세팅에 지장이 없음
        player.GetComponent<PlayerStatus>().SetData();
        Inventory.instance.SetData();
        Equipment.instance.SetData();
        SkillBook.instance.SetData();
        GateManager.instence.SetData();

        //로드시 남은 HP,MP를 불러오고 장비가 장착 되기 때문에 저장전보다 많은 HP가 생긴다.
        //장비 장착이 끝나고 저장된 HP,MP로 돌려준다.
        //코드 정렬 더 생각해보기.
        if (!DataManager.instance.newGame)
        {
            player.GetComponent<PlayerStatus>().remainHealth = dataManager.userData.remainHealth;
            player.GetComponent<PlayerStatus>().playerStatusUI.SetHpBar();
            player.GetComponent<PlayerStatus>().remainMana = dataManager.userData.remainMana;
            player.GetComponent<PlayerStatus>().playerStatusUI.SetMpBar();        
        }
        else
        {
            AutoSave();
        }

        Time.timeScale = 1;
        dataManager.isSet = true;    
    }

    public virtual void StartStage()
    {
        for (int i = 0; i < stageEnemy.Count; i++)
        {          
            stageEnemy[i].SetData();
            stageEnemy[i].isSet = true;
        }
    }

    public virtual void LoadData()
    {
        LoadMapData();
    }

    public virtual void LoadMapData()
    {
        currentStage = dataManager.userData.currentStage;
        currentArea = dataManager.userData.currentArea;
        for (int i = 0; i < stageEnemy.Count; i++)
        {
            stageEnemy[i].SetData();
        }
    }

    public virtual void SaveMapdata()
    {
        dataManager.userData.currentStage = currentStage;
        dataManager.userData.currentArea = currentArea;
    }

    public virtual void SaveData()
    {
        dataManager.userData.currentStage = currentStage;
        dataManager.userData.currentArea = currentArea;

        player.GetComponent<PlayerStatus>().SaveData();
        Inventory.instance.SaveData();
        Equipment.instance.SaveData();
        SkillBook.instance.SaveData();
        GateManager.instence.SaveData();
        SaveMapdata();
    }

    public void AutoSave()
    {
        dataManager.userData.saveTime = DateTime.Now.ToString("yyyy-MM-dd [HH:mm]");
        dataManager.userData.sceneName = SceneManager.GetActiveScene().name;
        SaveData();
        dataManager.AutoSaveData();
    }
}
