using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class SequenceText : MonoBehaviour
{
    public static SequenceText instance;

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

    public Transform sequencetextPos;
    public LocalizeStringEvent sequencetextPrefab;
    public string s_text;
    public float stextLifeTime = 5f;

    public void SetSequenceText(LocalizeStringEvent _SeuenceText, string _sText)
    {

        if (sequencetextPos.childCount >= 3)
        {
            Destroy(sequencetextPos.GetChild(0).gameObject);
        }

        if(_SeuenceText == null)
        {
            LocalizeStringEvent stext = Instantiate(sequencetextPrefab, sequencetextPos);
            stext.StringReference.TableReference = "SignMessage";
            stext.StringReference.TableEntryReference = _sText;
            Destroy(stext.gameObject, stextLifeTime);
        }
        else
        {
            LocalizeStringEvent stext = Instantiate(_SeuenceText, sequencetextPos);
            stext.StringReference.TableReference = "SignMessage";
            stext.StringReference.TableEntryReference = _sText;
            Destroy(stext.gameObject, stextLifeTime);
        }
        
        
    }
}
