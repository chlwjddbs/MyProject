using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class VideoOption : MonoBehaviour
{
    //OptionManager의 싱글톤
    private OptionManager optionManager;

    //사용 가능한 해상도 목록을 표시해줄 DropDown
    public TMP_Dropdown resDropdown;

    //현재 화면 모드
    public FullScreenMode screenMode;

    //스크린 모드의 초기 값
    private int screenInitial = 0;
    //선택된 스크린 모드의 값
    public int scrModeNum;

    //초기 DropDown의 Value 값
    private int resInitial = 0;
    //선택된 DropDown의 Value
    public int resVal = 0;
    
    //스크린모드 변경시 관리할 토글 그룹
    public ToggleGroup screenModeGroup;

    //해상도를 높은 순서부터 저장해 주기 위한 임시 Resolution List
    private List<Resolution> tempResolutions = new List<Resolution>();

    //모든 해상도 목록
    private List<Resolution> resolutionsAll = new List<Resolution>();
    //16:9 해상도 목록
    private List<Resolution> resolutions16_9 = new List<Resolution>();
    //16:10 해상도 목록
    private List<Resolution> resolutions16_10 = new List<Resolution>();

    //현재 사용중인 해상도 목록
    private List<Resolution> currnetResolutions = new List<Resolution>();

    //해상도 목록의 초기값
    private int ratioInitial = 0;
    //현재 해상도 목록
    //public string str_Ratio = "All";
    public int ratioNum;
    
    //해상도 목록 토글 변경시 관리할 토글 그룹
    public ToggleGroup ratioGroup;

    //모든 화면비
    private List<TMP_Dropdown.OptionData> ratioDataAll = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> ratioData_16_9 = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> ratioData_16_10 = new List<TMP_Dropdown.OptionData>();

    //ss set start
    private bool ssScrMode = true;
    private bool ssRatio = true;

    private void Awake()
    {
        //드롭다운 옵셥은 List로 되어 있어서 Clear 함수를 통해 초기화 해준다.
        resDropdown.options.Clear();

        //해상도 목록을 높은 순서부터 보여주기 위해 Reverse사용
        tempResolutions = new List<Resolution>(Screen.resolutions);
        tempResolutions.Reverse();

        //현재 모니터의 최대 주사율을 저장한다.
        int maxRefreshRate = tempResolutions[0].refreshRate;
       
        //사용 가능한 해상도 목록중에 최대 주사율 해상도만 Dropdown에 넣어준다.
        foreach (Resolution res in tempResolutions)
        {
            if (res.refreshRate == maxRefreshRate)
            {
                //Dropdown에 넣기 위해서는 데이터 형식이 OptionData이기 때문에 새로 생성해 준다.
                TMP_Dropdown.OptionData resData = new TMP_Dropdown.OptionData();

                //해상도의 비율을 구해준다.
                float aspectRatio = (float)res.width / res.height;
                string aspectRatioString = aspectRatio.ToString("F2");
                float aspectRatioF2 = float.Parse(aspectRatioString);

                //DropDownMenu 에 표시용 string
                string DDM_aspectRatio;

                //비율에 따른 string 부여
                switch (aspectRatioF2)
                {
                    case (1.50f):
                        DDM_aspectRatio = "[3:2]";
                        break;

                    case (1.33f):
                        DDM_aspectRatio = "[4:3]";
                        break;

                    case (1.25f):
                        DDM_aspectRatio = "[5:4]";
                        break;

                    case (1.77f):
                    case (1.78f):                       
                        DDM_aspectRatio = "[16:9]";
                        resData.text = (res.width + " x " + res.height + "  " + DDM_aspectRatio);
                        ratioData_16_9.Add(resData);
                        resolutions16_9.Add(res);
                        break;

                    case (1.56f):
                    case (1.60f):
                    case (1.67f):
                        DDM_aspectRatio = "[16:10]";
                        resData.text = (res.width + " x " + res.height + "  " + DDM_aspectRatio);
                        ratioData_16_10.Add(resData);
                        resolutions16_10.Add(res);
                        break;

                    default:
                        DDM_aspectRatio = " ";
                        break;
                }
                
                //데이터화 시킬 해상도를 width,height 와 비율로 표기해 준다. ex) 2560 x 1440 [16:9]
                resData.text = (res.width + " x " + res.height + "  " + DDM_aspectRatio);
                ratioDataAll.Add(resData);
                //만들어진 OptionData를 dropdown.options.Add(data)를 통해 넣어준다.
                resDropdown.options.Add(resData);
                //Debug.Log(res.width + " x " + res.height + " " + res.refreshRate);
                //Debug.Log(res.width + " x " + res.height + " " + DDM_aspectRatio);  
                currnetResolutions.Add(res);
                resolutionsAll.Add(res);
            }
        }       
    }

    private void Start()
    {
        optionManager = OptionManager.instance;      
    }

    public void GameStart()
    {
        if (PlayerPrefs.HasKey("VideoOption"))
        {
            LoadOption();
        }
        else
        {
            Debug.Log(scrModeNum);
            scrModeNum = screenInitial;
            Debug.Log(scrModeNum);
            ratioNum = ratioInitial;
            resVal = resInitial;
        }
        StartScreenMode(scrModeNum);
        StartRatio(ratioNum);
        StartResolution(resVal);

    }

    public void SaveOption()
    {
        PlayerPrefs.SetString("VideoOption", "Save");
        PlayerPrefs.SetInt("ScreenMode", scrModeNum);
        PlayerPrefs.SetInt("Ratio", ratioNum);
        PlayerPrefs.SetInt("Resolution", resVal);
    }

    public void LoadOption()
    {
        scrModeNum = PlayerPrefs.GetInt("ScreenMode", scrModeNum);
        ratioNum = PlayerPrefs.GetInt("Ratio", ratioNum);
        resVal = PlayerPrefs.GetInt("Resolution", resVal);
    }

    //Dropdown의 On Value Changed를 통해 연결하여 Dropdown이 변경 될 때 마다 해상도를 바꿔준다.
    //ex) Dropdown에 3개의 해상도 목록이 존재할 때 2번째 해상도를 선탣하면 Value는 1이 된다.
    //선택 된 Value의 값은 위의 private Resolution[] resolutions에 들어있는 해상도 목록과 같다.
    //선택 된 Value를 통해 resolutions에 접근하면 선택한 해상도로 변경 가능하다.
    public void ChangeResolution()
    {
        optionManager.tmp_ResVale.Add(resVal);
        optionManager.relutions.Add(currnetResolutions[resVal]);
        //선택한 Dropdown의 Value값을 받는다.
        resVal = resDropdown.value;
        //넘겨받은 value를 통해 resolution에 있는 해상도에 접근하여 해상도를 변경한다.
        Screen.SetResolution(currnetResolutions[resVal].width, currnetResolutions[resVal].height, screenMode);
        Debug.Log(currnetResolutions[resVal].width +" x " + currnetResolutions[resVal].height + " " + screenMode);
        optionManager.ToggleSaveButton(true);
        Debug.Log("해상도 변경");
    }

    //토글로부터 string 매개 변수를 받아와 switch을 통해 스크린 모드 변경
    public void ChangeScreenMode(int _screenMode)
    {
        if (ssScrMode)
        {
            ssScrMode = false;
            return;
        }
        Debug.Log(_screenMode);
        if (scrModeNum == _screenMode | !screenModeGroup.transform.GetChild(_screenMode).GetComponent<Toggle>().isOn)
        {
            return;
        }
        Debug.Log(_screenMode);
        optionManager.tmp_Screen.Add(scrModeNum);
        //tmp_ScreenMode = screenMode.ToString();

        SetScreenMode(_screenMode);

        optionManager.ToggleSaveButton(true);
    }

    public void SetScreenMode(int _screenMode)
    {      
        switch (_screenMode)
        {
            case (0):
                scrModeNum = 0;
                screenMode = FullScreenMode.ExclusiveFullScreen;          

                break;
            case (1):
                scrModeNum = 1;
                screenMode = FullScreenMode.FullScreenWindow;
                break;
            case (2):
                scrModeNum = 2;
                screenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }

        Screen.SetResolution(currnetResolutions[resVal].width, currnetResolutions[resVal].height, screenMode);
    }

    /*
    public void ChangeRatio(int _ratio)
    {
        if (!ratioGroup.transform.GetChild(_ratio).GetComponent<Toggle>().isOn)
        {
            return;
        }
        Debug.Log(_ratio);
        if (ratioNum == _ratio)
        {
            return;
        }
        Debug.Log("변경");
        optionManager.tmp_Ratio.Add(ratioNum);
        //tmp_Ratio = str_Ratio;

        switch (_ratio)
        {
            case (0):
                ratioNum = 0;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutionsAll.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioDataAll)
                {
                    resDropdown.options.Add(data);
                    //Debug.Log(data.text + screenMode);
                }
                break;

            case (1):
                ratioNum = 1;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_9.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_9)
                {
                    resDropdown.options.Add(data);
                    //Debug.Log(data.text + screenMode);
                }
                break;

            case (2):
                ratioNum = 2;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_10.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_10)
                {
                    resDropdown.options.Add(data);
                    //Debug.Log(data.text + screenMode);
                }
                break;
            
            default:
                break;
        }

        //str_Ratio = _ratio;
        resDropdown.RefreshShownValue();
        //Debug.Log(currnetResolutions.Count);        
        //ChangeResolution();
        optionManager.ToggleSaveButton(true);
    }
    */

    public void ChangeRatio(int _ratio)
    {
        if (ssRatio)
        {
            ssRatio = false;
            return;
        }

        if (ratioNum == _ratio | !ratioGroup.transform.GetChild(_ratio).GetComponent<Toggle>().isOn)
        {
            return;
        }

        optionManager.tmp_Ratio.Add(ratioNum);
        SetRatioDropBox(_ratio);
        ChangeResolution();
        optionManager.ToggleSaveButton(true);

    }

    public void SetRatioDropBox(int _ratio)
    {
        switch (_ratio)
        {
            case (0):
                ratioNum = 0;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutionsAll.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioDataAll)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (1):
                ratioNum = 1;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_9.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_9)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (2):
                ratioNum = 2;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_10.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_10)
                {
                    resDropdown.options.Add(data);
                }
                break;

            default:
                break;
        }
        resDropdown.RefreshShownValue();
    }

    public void CancelOption(int _screenMode, int _resVale, int _ratio)
    {
        if (_screenMode != -1)
        {
            for (int i = 0; i < screenModeGroup.transform.childCount; i++)
            {
                if (i == _screenMode)
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
        }

        if (_ratio != -1)
        {
            for (int i = 0; i < ratioGroup.transform.childCount; i++)
            {
                if (i == _ratio)
                {
                    ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;                 
                }
                else
                {
                    ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
        }

        if (_resVale != -1)
        {           
            resDropdown.value = _resVale;            
        }
    }

    public void ResetOption()
    {
        for (int i = 0; i < screenModeGroup.transform.childCount; i++)
        {
            //screenModeGroup.gameObject.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            if(i == screenInitial)
            {
                screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
            }
            else
            {
                screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }

        for (int i = 0; i < ratioGroup.transform.childCount; i++)
        {
            if(i == ratioInitial)
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
            }
            else
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }

        resDropdown.value = resInitial;
    }

    public void StartResolution(int _resVal)
    {
        resVal = _resVal;
        resDropdown.value = resVal;
    }

    //토글로부터 string 매개 변수를 받아와 switch을 통해 스크린 모드 변경
    public void StartScreenMode(int _screenMode)
    {
        if (_screenMode != -1)
        {
            for (int i = 0; i < screenModeGroup.transform.childCount; i++)
            {
                if (i == _screenMode)
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
        }

        switch (_screenMode)
        {
            case (0):
                scrModeNum = 0;
                screenMode = FullScreenMode.ExclusiveFullScreen;

                break;
            case (1):
                scrModeNum = 1;
                screenMode = FullScreenMode.FullScreenWindow;
                break;
            case (2):
                scrModeNum = 2;
                screenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }
    }

    public void StartRatio(int _ratio)
    {
        for (int i = 0; i < ratioGroup.transform.childCount; i++)
        {
            if (i == _ratio)
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                

            }
            else
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }

        SetRatioDropBox(_ratio);
        /*
        switch (_ratio)
        {
            case (0):
                ratioNum = 0;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutionsAll.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioDataAll)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (1):
                ratioNum = 1;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_9.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_9)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (2):
                ratioNum = 2;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_10.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_10)
                {
                    resDropdown.options.Add(data);
                }
                break;

            default:
                break;
        }

        resDropdown.RefreshShownValue();
        */
    }

}
