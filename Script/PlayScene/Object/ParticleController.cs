using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public List<ParticleSystem> particles;
    public bool isPlay = false;
    public bool isOpen = false;
    // Start is called before the first frame update

    // Update is called once per frame
    
    public void ParticleOnOff()
    {
        isPlay = !isPlay;
        isOpen = !isOpen;
        if (isPlay)
        {
            for (int i = 0; i < particles.Count; i++)
            {               
                particles[i].Play();
            }

        }
        else
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Stop();
            }
        }
    }

    public void LoadData(bool _data)
    {
        isPlay = _data;
        isOpen = _data;
        if (isPlay)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Play();
            }

        }
        else
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Stop();
            }
        }
    }
}
