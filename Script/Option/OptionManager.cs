using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class OptionManager : MonoBehaviour
{
    public static OptionManager instance;

    private void Awake()
    {
        /*
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }        
        DontDestroyOnLoad(gameObject);
        */
        instance = this;
    }

    public VideoOption videoOption;
    public AudioOption audioOption;
    public LanguageOption languageOption;

    private string screenMode = null;

    //tmp_xxx : 세팅 변경 후 취소를 하거나 저장하여 세팅을 그대로 쓰던가 선택을 한다. 취사 선택에 맞춰 세팅을 맞춰줘야 하기 떄문에 변경 했던 옵션을 저장한다.

    public List<int> tmp_Screen = new List<int>();
    public int resolutionValue = 0;
    public List<int> tmp_ResVale = new List<int>();

    public List<Resolution> relutions = new List<Resolution>();
    
    public string ratio = "All";
    public List<int> tmp_Ratio;

    public List<int> tmp_MasterVol;
    public List<int> tmp_BgmVol;
    public List<int> tmp_SfxVol;
    public List<int> tmp_AmbVol;

    //옵션 변경 후 변경한 옵션을 저장하거나 취소하기 전 선택중인 상태 표시
    public bool isChange = false;

    public Button saveButton;
    
    private void Start()
    {
        //Video옵션의 ScreenMode 등 옵션이 토글일 경우 값이 변경 되면 자동으로 OnChangeValue로 인해 SaveButton의 활성화 여부를 체크하게된다.
        //SaveButton은 SettingMenuUI의 자식임으로 SettingMenuUI가 꺼져있으면 버튼이 올바르게 동작하지 못한다.
        //그래서 SettingMenuUI를 UI밖에 밀어뒀다 게임이 시작할 때 원래 위치로 복구 시켜 주고 꺼준다.       
        GameStart();
    }

    public void GameStart()
    {
        videoOption.GameStart();
        audioOption.GameStart();
        languageOption.GameStart();

        //저장된 세팅을 불러오면서 세팅 값이 바뀌는데 바뀐 값이 임시 리스트에 저장된걸 지워준다.
        ClearList();
        //SettingMenuUI.SetActive(false);

        DataManager.instance.isSet = true;
    }

    public void SaveOption()
    {
        if (!isChange)
        {
            return;
        }

        ClearList();
        Save();


        //screenMode = videoOption.tmp_Screen;
        //resolutionValue = videoOption.resVal;
        //ratio = videoOption.tmp_Ratio;
    }

    public void Save()
    {
        videoOption.SaveOption();
        audioOption.SaveOption();

    }

    //SaveButton의 활성, 비활성화 상태 여부를 정해준다.
    public void ToggleSaveButton(bool _isChane)
    {
        isChange = _isChane;
        if (isChange)
        {
            saveButton.GetComponent<MenuButton>().ChangeButtonState(true);
        }
        else
        {
            saveButton.GetComponent<MenuButton>().ChangeButtonState(false);
        }
    }

    public void CancelOption()
    {
        if (!isChange)
        {
            return;
        }

        /*
        videoOption.resDropdown.value = tmp_ResVale[0];
        videoOption.ChangeResolution();
        */
        
        videoOption.CancelOption(tmp_Screen.Count == 0? -1 : tmp_Screen[0], tmp_ResVale.Count ==0? -1 : tmp_ResVale[0], tmp_Ratio.Count == 0? -1 : tmp_Ratio[0]);
        audioOption.ChangeOption("Cancel");
        ClearList();
        //videoOption.ChangeRatio(ratio);
    }

    public void ResetOption()
    {
        videoOption.ResetOption();
        audioOption.ChangeOption("Reset");
        ClearList();
        Save();
    }

    public void ClearList()
    {
        tmp_Screen.Clear();
        tmp_ResVale.Clear();
        tmp_Ratio.Clear();
        relutions.Clear();
        tmp_MasterVol.Clear();
        tmp_BgmVol.Clear();
        tmp_SfxVol.Clear();
        tmp_AmbVol.Clear();

        ToggleSaveButton(false);
    }

}
