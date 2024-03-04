using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public Light levelupLight;
    private ParticleSystem levelupPart;

    private void Start()
    {
        levelupPart = GetComponent<ParticleSystem>();      
    }

    public void LevelUpEffect()
    {
        StopCoroutine(levelupCo());
        StartCoroutine(levelupCo());
    }

    IEnumerator levelupCo()
    {
        DataManager.instance.isSet = false;
        levelupPart.Play();
        levelupLight.enabled = true;
        AudioManager.instance.PlayExSound("Levelup");
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(levelupPart.duration + 0.3f);
        DataManager.instance.isSet = true;
        levelupLight.enabled = false;
        Time.timeScale = 1;
    }
}
