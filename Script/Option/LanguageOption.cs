using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Events;

public class LanguageOption : MonoBehaviour
{
    public static UnityAction setSlot;
    public static UnityAction SortingUI;

    public TMP_Dropdown LanguageList;
    public TextMeshProUGUI currentLanguageText;

    public int selectedLanguageNum = 0;
 

    //언어 리스트 드롭다운 변경 시 선택 된 언어로 변경
    public void ChangeLanguage()
    {
        //Debug.Log("언어변경");
        //드롭 다운 변경 시 불러오는 함수임으로 LanguageList.value은 현재 변경 된 언어 옵션
        selectedLanguageNum = LanguageList.value;
        //변경 된 옵션을 저장해준다.
        PlayerPrefs.SetInt("LanguageOption", selectedLanguageNum);
        //Debug.Log(selectedLanguageNum + " " + LanguageList.options[selectedLanguageNum].text);
        
        //언어가 변경 됨에 따라 현재 언어 표시 UI Text도 변경해준다.
        currentLanguageText.text = LanguageList.options[selectedLanguageNum].text;

        StartCoroutine("LoadingLanguage");
        
        //변경 될 언어 적용
        //setSlot?.Invoke();
    }

    IEnumerator LoadingLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selectedLanguageNum];
        yield return LocalizationSettings.InitializationOperation;
        //Debug.Log("로딩");
        setSlot?.Invoke();
        SortingUI?.Invoke();
    }

    /*
    public void SetLanguage()
    {
        Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);
    }
    */
    public void GameStart()
    {
        StartCoroutine(SetLanguage());
    }

    IEnumerator SetLanguage()
    {
        LanguageList.ClearOptions();
        //Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);

        //현지화 언어 리스트 로딩이 완료 될때까지 기다려 준다.
        yield return LocalizationSettings.InitializationOperation;
        //현지화 언어 리스트의 갯수
        //Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);
        //설정된 언어
        //Debug.Log(LocalizationSettings.SelectedLocale);
        //현지화 언어 리스트의 첫번째 국가코드의 문자열
        //Debug.Log(LocalizationSettings.AvailableLocales.Locales[0].ToString());

        //저장된 언어가 있다면
        if (PlayerPrefs.HasKey("LanguageOption"))
        {
            //저장된 언어를 선택된 언어로 변경.
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("LanguageOption")];
        }
        
        //언어리스트의 갯수 만큼 반복문을 실행하여
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            TMP_Dropdown.OptionData dropText = new TMP_Dropdown.OptionData();

            //언어 리스트의 언어를 세팅해준다.
            switch (LocalizationSettings.AvailableLocales.Locales[i].ToString())
            {
                //리스트[0]의 국가 코드가 "English (en)" 경우 영어로 판정
                case "English (en)":
                    //언어리스트 dropbox에 English 옵션 등록
                    dropText.text = "English";
                    LanguageList.options.Add(dropText);

                    //리스트[0]에 있는 언어가 선택 된 언어(영어)라면
                    if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
                    {
                        //현재 선택된 언어 표시 UI에 English test 삽입
                        currentLanguageText.text = dropText.text;
                        //선택된 언어의 리스트 넘버 등록
                        selectedLanguageNum = i;
                    }
                    break;
                case "Korean (ko)":
                    dropText.text = "한국어";
                    LanguageList.options.Add(dropText);
                    if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
                    {
                        currentLanguageText.text = dropText.text;
                        selectedLanguageNum = i;
                    }
                    break;
                default:
                    break;
            }
        }

        //선택된 언어로 보이기 위해 드롭다운 옵션 변경
        //드롭다운과 같은 언어면 변경 안함(변경 할 필요가 없음)
        if (LanguageList.value != selectedLanguageNum)
        {
            //Debug.Log("변경");
            LanguageList.value = selectedLanguageNum;
            //setSlot?.Invoke();
        }
        else
        {
            ChangeLanguage();
        }
    }
}
