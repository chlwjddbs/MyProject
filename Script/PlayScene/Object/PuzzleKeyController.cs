using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleKeyController : MonoBehaviour
{
    public List<ParticleSystem> particles;
    public Light keyLight;

    public bool isOpen = false;
   
    
    public void ParticleOnOff()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            for (int i = 0; i < particles.Count; i++)
            {               
                particles[i].Play();
                keyLight.enabled = isOpen;
            }

        }
        else
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Stop();
                keyLight.enabled = isOpen;
            }
        }
    }

    public void LoadData(bool _data)
    {
        isOpen = _data;
        if (isOpen)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Play();
                keyLight.enabled = isOpen;
            }
        }
        else
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Stop();
                keyLight.enabled = isOpen;
            }
        }
    }
}
