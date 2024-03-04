using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Localization.Components;

public class SkillBook : MonoBehaviour
{
    [System.Serializable]
    public class SetEquipSkill
    {
        public string keyCode;
        public SkillItem equipSkill;
        public float coolTime;

        public SetEquipSkill(string _keycode,SkillItem _skill = null , float _coolTime = 0)
        {
            keyCode = _keycode;
            equipSkill = _skill;
            coolTime = _coolTime;
        }
    }

    public static SkillBook instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //배운 스킬 리스트
    public List<SkillItem> learnedSkill;

    //장착 한 스킬
    public SkillItem[] equipSkill;

    public GameObject skillPage;
    public GameObject skillSlot;
    public GameObject skillButtons;
    public SkillButton[] button;

    //스킬 페이지에 기록 가능한 스킬의 갯수
    public int maxAmount = 5;

    public LocalizeStringEvent sequencetextPrefab;
    public string s_text;
   
    private float countdown;
    private float coolTime = 4;

    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI pageText;
    private int pageInt =1;

    private SkillBookUI skillbookUI;

    private bool usetext = true;

    public Dictionary<int, SetEquipSkill> skillButtonInfo = new Dictionary<int, SetEquipSkill>();

    public Sound[] pageSound;

    private void Start()
    {
        foreach (var s in pageSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    public void SetData()
    {
        //장착할 스킬의 갯수는 사용가능한 스킬 버튼의 개수이다. (현재 [A,S,D,F,G]로 5개)
        equipSkill = new SkillItem[skillButtons.transform.childCount];

        //현재 사용 가능한 스킬 버튼 가져오기
        button = skillButtons.GetComponentsInChildren<SkillButton>();

        AudioManager.instance.skillSound = new Sound[skillButtons.transform.childCount];

        skillbookUI = GetComponentInParent<SkillBookUI>();
        skillbookUI.SetData();

        if (DataManager.instance.newGame)
        {
            for (int i = 0; i < button.Length; i++)
            {
                button[i].SetData();
            }
        }
        else
        {
            LoadData();
            //Debug.Log(learnedSkill[0].skillName + learnedSkill[0].skillType + learnedSkill[0].skill);
        }

    }

    public void LoadData()
    {
        learnedSkill = new List<SkillItem>();
        //equipSkill = (SkillItem[])(DataManager.instance.userData.equipSkill).Clone();

        for (int i = 0; i < DataManager.instance.userData.learedSkill.Count; i++)
        {
            LearnSkill(DataManager.instance.userData.learedSkill[i]);
        }
        skillbookUI.CloseUI();

        for (int i = 0; i < skillButtons.transform.childCount; i++)
        {
            SetEquipSkill x = DataManager.instance.userData.skillButtonInfo[i];
            skillButtonInfo.Add(i, new SetEquipSkill(x.keyCode, x.equipSkill, x.coolTime));
            button[i].SetData();
        }
    }

    public void SaveData()
    {
        DataManager.instance.userData.learedSkill = learnedSkill;
        //DataManager.instance.userData.equipSkill = equipSkill;
        for (int i = 0; i < button.Length; i++)
        {
            button[i].SaveData();
            if (DataManager.instance.userData.skillButtonNum.Contains(i))
            {
                DataManager.instance.userData.skillButtonInfo[i] = skillButtonInfo[i];
            }
            else
            {
                DataManager.instance.userData.skillButtonNum.Add(i);
                DataManager.instance.userData.skillButtonInfo.Add(skillButtonInfo[i]);
            }
        }
        
    }

    /*
    private void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
    }
    */


    public bool isLearn(SkillItem _skillItem)
    {
        //배운 스킬 목록에 있는 스킬이면
        if (learnedSkill.Contains(_skillItem))
        {
            /*
            //더이상 배울 수 없음
            if (countdown <= 0)
            {
                SequenceText.instance.SetSequenceText(sequencetextPrefab, s_text);
                countdown = coolTime;
            }
            */

            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(sequencetextPrefab, s_text);
                Invoke("isUseText",4);

            }
            return false;
        }
        //목록에 없는 스킬이면
        else
        {
            //배울 수 있음
            return true;
        }
    }

    public void LearnSkill(SkillItem _skillItem)
    {
        //습득한 스킬 목록에 추가
        learnedSkill.Add(_skillItem);
        skillbookUI.OpenUI();
        //새로운 스킬을 습득시 스킬을 보관할 페이지를 가져옴 (스킬북의 마지막 페이지)
        Transform _skillPage = transform.GetChild(transform.childCount - 1);

        //스킬 페이지가 없다면 생성.
        if(_skillPage == null)
        {
            Instantiate(skillPage, transform);
            Debug.Log("스킬 없음");
        }
        
        //스킬 페이지에 기록된 스킬의 갯수가 최대 소지 갯수를 넘지 않았을 시
        if (_skillPage.childCount < maxAmount)
        {
            //스킬 습득
            GameObject _skillPrefab = Instantiate(skillSlot, transform.GetChild(transform.childCount - 1));
            SkillSlot _skillSlot = _skillPrefab.GetComponent<SkillSlot>();
            _skillSlot.AddSkill(_skillItem);

            transform.GetChild(int.Parse(pageText.text) - 1).gameObject.SetActive(false);
            pageText.text = transform.childCount.ToString();
            transform.GetChild(int.Parse(pageText.text) - 1).gameObject.SetActive(true);

        }
        //스킬 페이지의 기록된 스킬의 갯수가 최대 소지 갯수를 넘었을 시
        else if (_skillPage.childCount == 5 && _skillPage.GetComponent<SkillPage>().isPull == false)
        {
            _skillPage.GetComponent<SkillPage>().isPull = true;

            //새로운 스킬 페이지 생성
            Instantiate(skillPage, transform);            

            //새로운 스킬 페이지 생성 후 스킬 보관
            GameObject _skillPrefab = Instantiate(skillSlot, transform.GetChild(transform.childCount - 1));
            SkillSlot _skillSlot = _skillPrefab.GetComponent<SkillSlot>();
            _skillSlot.AddSkill(_skillItem);

            //전 스킬 페이지를 off하고 새로운 스킬 페이지를 보여준다.
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                pageText.text = transform.childCount.ToString();
            }
        }
    }

    //스킬을 장착시 해당 스킬과 스킬이 장착될 버튼의 번호를 받아온다.
    //_skillItem은 장착할 스킬 , _dropButtonNum은 장착할 버튼(스킬북에서 스킬을 드래그하여 드롭한 버튼의 번호)
    public void isEquip(SkillItem _skillItem, int _dropButtonNum)
    {
        for (int i = 0; i < equipSkill.Length; i++)
        {
            //장착할 스킬이 이미 장착된 스킬이고 
            if (equipSkill[i] == _skillItem)
            {
                //_skillItem과 같은 skillItem이 장착된 위치(equipSkill[i]의 i)와
                //스킬 장착을 시도한 스킬 버튼의 위치(_dropButtonNum) 이 같다면
                if (i == _dropButtonNum)
                {
                    //같은 칸에 같은 스킬을 장착하라고 하는것이기 떄문에 변화 없음.

                    //button[i].UnequipSkill();
                    //equipSkill[i] = null;
                    break;
                }
                //장착 할 스킬과 버튼에 있는 스킬이 다르다면 (A스킬을 B스킬이 있는 버튼에 장착하려고 한다면)
                else
                {
                    //장착 될 버튼에 있는 스킬과 장착 중인 스킬의 위치를 바꿔준다. (A스킬을 B스킬이 있던 자리로 옮겨주고, B스킬은 A스킬이 있던 자리로 옮긴다.)
                    //Debug.Log(this.name);
                    button[i].swapSkill(button[_dropButtonNum]);
                }
                break;
            }
            //장착한 스킬 목록에 해당 스킬이 없다면 (새로운 스킬을 장착할 시)
            //for문을 전부 돌아 마지막 칸에 도달시
            else if (i == equipSkill.Length - 1)
            {
                //마지막칸을 검사해서 장착된 스킬이 없거나, 스킬이 있어도 새로 장착할 스킬이 아닐시
                //(새로운 스킬이라는 뜻, 중복된다면 기존에 장착한 스킬이기 때문에  if (equipSkill[i] == _skillItem) 구간에서 걸러진다.)
                if (equipSkill[i] == null || equipSkill[i] != _skillItem)
                {
                    //스킬을 장착한다.
                    button[_dropButtonNum].UnequipSkill();
                    button[_dropButtonNum].EquipSkill(_skillItem);
                }
            }
        }
    }

    private void isUseText()
    {
        usetext = true;
    }

    public void PrevPage()
    {
        if (int.Parse(pageText.text) > 1)
        {
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(false);
            pageText.text = (int.Parse(pageText.text) - 1).ToString();
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(true);
            AudioManager.instance.PlayExSound("pageChage");
        }
    }

    public void NextPage()
    {
        if(int.Parse(pageText.text) < transform.childCount)
        {
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(false);
            pageText.text = (int.Parse(pageText.text) + 1).ToString();
            transform.GetChild(int.Parse(pageText.text)-1).gameObject.SetActive(true);
            AudioManager.instance.PlayExSound("pageChage");
        }
    }
    
}
