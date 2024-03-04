using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //sound 파일을 가지고 있을 sound 목록
    public Sound[] sounds;
    public List<Sound> externalSound = new List<Sound>();
    public Sound[] skillSound;

    //볼륨조절을 위한 오디오 믹서
    public AudioMixer audioMixer;
    public AudioMixerGroup[] audioGroups;

    //현재 재생중인 bgm이름
    private string currentBGM;
    private string currentAm;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //오디오 믹서중 "Master" 라는 이름의 그룹을 찾아 넣어준다. 
        //현재는 부모 그룹인 Master, 그룹 멤버인 BGM SFX Effext가 있다.
        audioGroups = audioMixer.FindMatchingGroups("Master");
       
        //준비한 사운드를 재생 및 정지하기 위해 오디오 매니저에 추가해준다.
        foreach (var s in sounds)
        {
            //현재 AudioManager 오브젝트에 새로운 AudioSource 컴포넌트를 부착하여
            s.source = gameObject.AddComponent<AudioSource>();
            //Sound 클래스를 통해 만든 사운드 정보를 가져와 AudioSource에 세팅해 준다.
            //Sound 클래스에는 재생할 오디오 clip과 
            //그 클립에 접근할 오디오 이름, 처음 설정될 volume과 pitch값과 반복 여부를 가져와 세팅해준다.
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            //사운드마다 사용할 용도가 다르기 때문에 용도에 따라 구분하여 믹서 그룹에 저장해 준다.

            //bgm은 bgm 믹서에 저장
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = audioGroups[1];
            }
            //효과음은 sfx에 저장
            else if(s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = audioGroups[2];
            }
            //나머지(현재는 환경음) Ambience에 저장
            else
            {
                s.source.outputAudioMixerGroup = audioGroups[3];
            }
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += SceneStart;
    }

    private void SceneStart(Scene scene, LoadSceneMode mode) 
    {
        PlayBGM(SceneManager.GetActiveScene().name);
    }

    // Start is called before the first frame update
    void Start()
    {       
        /*
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].loop)
            {
                sounds[i].source.Play();
                break;
            }
        }
        */
        //PlayeSound("Footstep");
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayBGM("MainMenuBGM");
        }
    }
    */

    //bgm과 나머지 사운드를 구분하여 Play 함수를 만든것은 bgm은 루프가 돌기 때문에 자동으로 stop을 시키고 다른 bgm을 play하기 위함
    public void PlayBGM(string _bgmName , float fadeTime = 0)
    {
        //현재 재생중인 bgm과 Play하라고 들어온 Bgm이 같다면 리턴 시켜준다.
        //같은 bgm을 다시 재생하면 bgm이 중간에 끊기고 처음부터 재생되기 때문.
        if (currentBGM == _bgmName)
        {
            return;
        }

        //현재 재생중인 bgm을 종료 시킨다.
        Stop(currentBGM);

        Sound sound = null;
        
        //반복문을 통해 요청 받은 파일이 있는지 확인한 후
        foreach (var s in sounds)
        {
            if(s.name == _bgmName)
            {
                currentBGM = _bgmName;
                sound = s;
                break;
            }
        }

        //파일이 없으면 리턴
        if(sound == null) 
        {
            Debug.Log("파일이 없습니다.");
            return;
        }

        StartCoroutine(fadeOutBgm(sound, fadeTime));

        //있으면 파일 재생       
        //sound.source.Play();

        //sound는 얕은 복사가 되었기 때문에 오디오 매니저를 참조하여 sound가 변경되어도 오디오 매니저에 영향을 준다.
        //sound는 지역변수로 매서드 종료 후 사라진다.
    }

    IEnumerator fadeOutBgm(Sound _sound , float fadeTime)
    {     
        float t = 0;
        _sound.source.volume = t;
        _sound.source.Play();

        while (t <= fadeTime)
        {
            t += Time.unscaledDeltaTime;
            _sound.source.volume = t / fadeTime;
            yield return null;
        }

        _sound.source.volume = 1f;
    }

    public void StopBGM(float fadeTime)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if (s.name == currentBGM)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            Debug.Log("재생중인 BGM이 없습니다");
            return;
        }
        
        //bgm 점점 작아지게 하기
        StartCoroutine(fadeInBgm(sound, fadeTime));
        
        currentBGM = null;
    }

    IEnumerator fadeInBgm(Sound _sound ,float fadeTime)
    {
        float t = fadeTime;
        while (0 <= t)
        {
            t -= Time.unscaledDeltaTime;
            _sound.source.volume = t / fadeTime;
            yield return null;
        }

        _sound.source.Stop();
        _sound.source.volume = 1f;
    }

    public void Stop(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if(s.name == _soundName)
            {
                sound = s;
                break;
            }
        }

        if(sound == null)
        {
            return;
        }
        sound.source.Stop();
    }

    public void PlayeSound(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if(s.name == _soundName)
            {
                sound = s;
                break;
            }        
        }

        if(sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.Log(_soundName);
        }
    }

    public void PlayAmSond(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if (s.name == _soundName)
            {
                sound = s;
                break;
            }
        }
        
        if(currentAm == _soundName)
        {
            return;
        }

        if (sound != null)
        {
            currentAm = sound.name;
            sound.source.Play();
        }
        else
        {
            Debug.Log(_soundName);
        }
    }

    public void StopAm(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if (s.name == _soundName)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            return;
        }

        currentAm = null;
        sound.source.Stop();
    }

    public void AddExternalSound(Sound s)
    {
        if (!externalSound.Contains(s))
        {
            externalSound.Add(s);
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
         
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = audioGroups[1];
            }
            else if (s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = audioGroups[2];
            }
            else
            {
                s.source.outputAudioMixerGroup = audioGroups[3];
            }
        }
    }

    public void PlayExSound(string _soundName)
    {
        Sound sound = null;
        foreach (var s in externalSound)
        {
            if (s.name == _soundName)
            {
                sound = s;
                break;
            }
        }

        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.Log(_soundName);
        }
    }

    public void AddSkillSound(Sound s , int buttonNum)
    {
        skillSound[buttonNum] = s;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        if (s.soundType == SoundType.BGM)
        {
            s.source.outputAudioMixerGroup = audioGroups[1];
        }  
        else if (s.soundType == SoundType.SFX)
        {
            s.source.outputAudioMixerGroup = audioGroups[2];
        }
        else
        {
            s.source.outputAudioMixerGroup = audioGroups[3];
        }
    }

    public void RemoveSkillSound(int buttonNum)
    {
        if(skillSound[buttonNum] == null)
        {
            return;
        }

        Destroy(skillSound[buttonNum].source);
        skillSound[buttonNum] = null;
    }

    public void PlayerSkillSound(int buttonNum)
    {
        skillSound[buttonNum].source.Play();
    }

    public void AllAmStop()
    {
        foreach (var item in sounds)
        {
            if(item.soundType == SoundType.Ambience)
            {
                item.source.Stop();
            }
        }

        foreach (var item in externalSound)
        {
            if (item.soundType == SoundType.Ambience)
            {
                item.source.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneStart;
    }
}
