using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWall : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public float fireDageme = 10f;
    private float countdown = 1f;
    private float attackCount = 1f;

    public Sound[] wallSound;

    private ParticleSystem fireWall;

    public FireElemental fireElemental;

    public bool activeWall = false;

    public bool downWall = false;
    public bool isTrigger = false;

    private void Start()
    {
        foreach (var s in wallSound)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[2];
        }

        fireWall = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (activeWall)
        {
            if (playerStatus != null)
            {
                attackCount -= Time.deltaTime;
                if (attackCount <= 0)
                {
                    playerStatus.TakeDamage(fireDageme);
                    attackCount = countdown;
                }
            }
            else
            {
                attackCount = Mathf.Clamp(attackCount, 0, countdown);
                attackCount += Time.deltaTime;
            }
        }

        ActiveWall();

        ActionWall();
    }

    public void ActionWall()
    {
        if (downWall & !isTrigger)
        {
            isTrigger = true;
            
            StartCoroutine(DownWall());
        }
    }

    IEnumerator DownWall()
    {
        fireWall.Stop();
        if (wallSound[1] != null)
        {
            wallSound[1].source.Play();
        }

        for (int i = 0; i < wallSound.Length; i++)
        {
            if (i == 2)
            {
                wallSound[2].source.Stop();
            }
        }

        yield return new WaitForSeconds(wallSound[1].clip.length);

        gameObject.SetActive(false);

    }

    public void ActiveWall()
    {
        if(activeWall & !fireWall.loop)
        {
            if(wallSound[0] != null)
            {
                wallSound[0].source.Play();

            }

            for (int i = 0; i < wallSound.Length; i++)
            {
                if(i == 2)
                {
                    wallSound[2].source.Play();
                }
            }

            fireWall.loop = true;
            fireWall.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activeWall)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            playerStatus = other.GetComponent<PlayerStatus>();
            playerStatus.TakeDamage(fireDageme);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatus = null;        
        }
    }
}
