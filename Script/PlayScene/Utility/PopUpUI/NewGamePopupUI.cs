using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Localization.Components;

public class NewGamePopupUI : MonoBehaviour
{
    public TMP_InputField inputField;

    //공백 및 특수문자를 걸러내주는 정규식 패턴
    private string nameCheck = @"[^a-zA-Zㄱ-ㅎ가-힣0-9ㅏ-ㅣ]";
  
    //@"[^a-zA-Zㄱ-ㅎ가-힣0-9ㅏ-ㅣ]"
    //@ : 정규식 표현 및 파일 경로 설정과정에서
    //백슬레시(\) 를 입력시 escape로 동작을 먼저하기 때문에 \를 문자열로 쓰기 위해서는 \\ 이라고 사용해야한다.
    //하지만 그러면 가독성이 떨어지거나 불편을 야기하기 때문에 @를 붙여주면 \를 문자열로 한번에 인식하도록 해준다.
    //ex) @"\bAA\b" == "\\bAA\\b"

    //a-z 소문자 알파벳
    //A-Z 대문자 알파벳
    //ㄱ-ㅎ 자음
    //ㅏ-ㅣ 모음
    //가-힣 한글
    //0-9 0~9 를 나타내는 숫자

    // [] 대괄호안에 있는 문자를 찾는다.
    //ex) [a-z] 소문자 알파벳을 찾는다. [G] 대문자 알파벳 G를 찾는다.

    //[^] 대괄호안의 ^ 사용시 부정을 뜻한다.
    //ex) [^a-z] 소문자 알파벳을 제외하고 찾는다. [^G] 대문자 알파벳G를 제외하고 찾는다.

    //위를 응용하여 [^a-zA-Zㄱ-ㅎ가-힣0-9ㅏ-ㅣ] 은 공백과 특수문자를 뜻한다.
    //더 정확히는 모든 알파벳과 한글 및 숫자를 제외한 문자를 뜻한다.
    //nameCheck는 공백 및 특수문자를 걸러주는 정규식 패턴이 된다.

    public GameObject nameConfirmedPopupUI;

    public GameObject noticePopupUI;
    //안내 팝업창을 상황에 맞게 쓰기 위한 LocalizeStringEvent 선언.
    public LocalizeStringEvent noticePopupText;

    private DataManager dataManager;

    public string userName;

    
    private void Awake()
    {
        dataManager = DataManager.instance;
        //안내 팝업창은 MainMenu 테이블에 있는 entry를 사용 할것.
        noticePopupText.StringReference.TableReference = "MainMenu";
    }

    public void SetPopup()
    {
        inputField.ActivateInputField();
        inputField.text = "";
    }

    public void OkButton()
    {
        if (inputField.text.Length == 0)
        {
            noticePopupUI.SetActive(true);
            noticePopupText.StringReference.TableEntryReference = "NameEnter";
            Debug.Log("아이디를 입력하세요");
            return;
        }

        NameCheck();
    }

    public void NameCheck()
    {
        if (Regex.IsMatch(inputField.text, nameCheck))
        {
            Debug.Log("특수 문자 및 공백은 사용 할 수 없습니다.");
            noticePopupUI.SetActive(true);
            noticePopupText.StringReference.TableEntryReference = "NameNotaVailable";
        }
        else
        {
            Debug.Log("게임 시작");
            userName = inputField.text;
            nameConfirmedPopupUI.SetActive(true);
        }
    }

    public void CancelButton()
    {
        //취소하기는 MainMenu.MenuClose 를 연결하여 구현 완료 하였음.
    }

    public void OnDisable()
    {
        nameConfirmedPopupUI.SetActive(false);
        noticePopupUI.SetActive(false);
    }
}
