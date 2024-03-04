using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PlaySound : MonoBehaviour,IPointerEnterHandler
{
    public Sound sound;
    public string soundName;



    // Start is called before the first frame update
    void Start()
    {
        /*
        AudioMixerGroup[] audioGroups = AudioManager.instance.audioMixer.FindMatchingGroups("Master");

        sound.source = AudioManager.instance.gameObject.AddComponent<AudioSource>();
        sound.source.name = sound.name;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;

        if (sound.loop)
        {
            sound.source.outputAudioMixerGroup = audioGroups[1];
        }
        else
        {
            sound.source.outputAudioMixerGroup = audioGroups[2];
        }
        */
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlayeSound(soundName);
    }
}
