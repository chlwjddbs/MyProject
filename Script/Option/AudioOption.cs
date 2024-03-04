using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AudioOption : MonoBehaviour
{
    private OptionManager optionManager;

    public AudioMixer audioMixer;

    public Slider masterSlider;
    public TextMeshProUGUI masterVolText;
    public int masterVol;

    public Slider bgmSlider;
    public TextMeshProUGUI bgmVolText;
    public int bgmVol;

    public Slider sfxSlider;
    public TextMeshProUGUI sfxVolText;
    public int sfxVol;

    public Slider ambienceSlider;
    public TextMeshProUGUI ambienceVolText;
    public int ambVol;

    private int masterInitial = 20;
    private int otherInitial = 0;

    //private bool isStart = true;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        ambienceSlider.onValueChanged.AddListener(SetAmbienceSliderVolume);
    }

    // Start is called before the first frame update
    void Start()
    {
        optionManager = OptionManager.instance;
    }

    public void GameStart()
    {
        if (PlayerPrefs.HasKey("AudioOption"))
        {
            LoadOption();
        }
        else
        {
            masterVol = masterInitial;
            bgmVol = otherInitial;
            sfxVol = otherInitial;
            ambVol = otherInitial;
        }

        //StartGame();
        StartMasterVolume(masterVol);
        StartBGMVolume(bgmVol);
        StartSFXVolume(sfxVol);
        StartAmbienceSliderVolume(ambVol);
    }

    public void SaveOption()
    {
        PlayerPrefs.SetString("AudioOption", "Save");
        PlayerPrefs.SetInt("MasterVol", masterVol);
        PlayerPrefs.SetInt("BgmVol", bgmVol);
        PlayerPrefs.SetInt("SfxVol", sfxVol);
        PlayerPrefs.SetInt("AmbVol", ambVol);
    }

    public void LoadOption()
    {
        masterVol = PlayerPrefs.GetInt("MasterVol", masterVol);
        bgmVol = PlayerPrefs.GetInt("BgmVol", bgmVol);
        sfxVol = PlayerPrefs.GetInt("SfxVol", sfxVol);
        ambVol =  PlayerPrefs.GetInt("AmbVol", ambVol);
    }

    
    /*
    IEnumerator StartGame()
    {
        StartMasterVolume(masterVol);
        StartBGMVolume(bgmVol);
        StartSFXVolume(sfxVol);
        StartAmbienceSliderVolume(ambVol);

        yield return null;

        //isStart = false;
    }
    */

    public void SetMasterVolume(float volume)
    {
        if (masterVol == (int)volume)
        {
            return;
        }
        
        optionManager.tmp_MasterVol.Add(masterVol);

        masterVol = (int)volume;
        audioMixer.SetFloat("Master", volume);
        masterVolText.text = ((int)(volume + 80f) + "%" ).ToString();


        optionManager.ToggleSaveButton(true);
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmVol == (int)volume)
        {
            return;
        }

        optionManager.tmp_BgmVol.Add(bgmVol);

        bgmVol = (int)volume;
        audioMixer.SetFloat("BGM", volume);
        bgmVolText.text = ((int)(volume + 80f) + "%").ToString();

        optionManager.ToggleSaveButton(true);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxVol == (int)volume)
        {
            return;
        }

        optionManager.tmp_SfxVol.Add(sfxVol);
        
        sfxVol = (int)volume;
        audioMixer.SetFloat("SFX", volume);
        sfxVolText.text = ((int)(volume + 80f) + "%").ToString();

        optionManager.ToggleSaveButton(true);
    }

    public void SetAmbienceSliderVolume(float volume)
    {
        if (ambVol == (int)volume)
        {
            return;
        }
        
        optionManager.tmp_AmbVol.Add(ambVol);
            
        ambVol = (int)volume;
        audioMixer.SetFloat("Effect", volume);
        ambienceVolText.text = ((int)(volume + 80f) + "%").ToString();

        optionManager.ToggleSaveButton(true);
    }

    public void ChangeOption(string order)
    {
        if (order == "Reset")
        {
            masterVol = masterInitial;
            bgmVol = otherInitial;
            sfxVol = otherInitial;
            ambVol = otherInitial;
        }
        else if(order == "Cancel")
        {
            //tmp_ 목록이 0이면 옵션을 바꾼적이 없기 떄문에 기존 볼륨을 그대로 넣어주고
            //0이 아니면 옵션을 변경 했기 때문에 0번(원래 옵션)으로 돌려준다.
            masterVol = optionManager.tmp_MasterVol.Count == 0? masterVol : optionManager.tmp_MasterVol[0];
            bgmVol = optionManager.tmp_BgmVol.Count == 0 ? bgmVol : optionManager.tmp_BgmVol[0];
            sfxVol = optionManager.tmp_SfxVol.Count == 0 ? sfxVol : optionManager.tmp_SfxVol[0];
            ambVol = optionManager.tmp_AmbVol.Count == 0 ? ambVol : optionManager.tmp_AmbVol[0];
        }

        masterSlider.value = masterVol;
        audioMixer.SetFloat("Master", masterVol);
        masterVolText.text = ((int)(masterVol + 80f) + "%").ToString();
      
        bgmSlider.value = bgmVol;
        audioMixer.SetFloat("BGM", bgmVol);
        bgmVolText.text = ((int)(bgmVol + 80f) + "%").ToString();

        sfxSlider.value = sfxVol;
        audioMixer.SetFloat("SFX", sfxVol);
        sfxVolText.text = ((int)(sfxVol + 80f) + "%").ToString();

        ambienceSlider.value = ambVol;
        audioMixer.SetFloat("Effect", ambVol);
        ambienceVolText.text = ((int)(ambVol + 80f) + "%").ToString();
    }

    public void StartMasterVolume(float volume)
    {
        masterSlider.value = volume;
        audioMixer.SetFloat("Master", volume);
        masterVolText.text = ((int)(volume + 80f) + "%").ToString();
    }

    public void StartBGMVolume(float volume)
    {
        bgmSlider.value = volume;
        audioMixer.SetFloat("BGM", volume);
        bgmVolText.text = ((int)(volume + 80f) + "%").ToString();
    }

    public void StartSFXVolume(float volume)
    {
        sfxSlider.value = volume;
        audioMixer.SetFloat("SFX", volume);
        sfxVolText.text = ((int)(volume + 80f) + "%").ToString();
    }

    public void StartAmbienceSliderVolume(float volume)
    {
        ambienceSlider.value = volume;
        audioMixer.SetFloat("Effect", volume);
        ambienceVolText.text = ((int)(volume + 80f) + "%").ToString();
    }
}
