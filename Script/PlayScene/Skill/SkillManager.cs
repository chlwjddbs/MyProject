using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //스킬의 현재 남은 스킬의 쿨타임
    public float remainingTime;

    //스킬 사용시 재사용까지 걸리는 시간
    public float coolTime;

    //소모마나
    public float cunsumeMana;
    protected PlayerController player;
    public bool isUse;
    public AnimationClip skillMotion;
    public float actionSpeed;

    private void Update()
    {
        if (!isUse)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                isUse = true;
            }
        }
    }

    protected virtual void SetSkill()
    {
        isUse = true;
        remainingTime = coolTime;
    }

    public virtual void UseSkill(PlayerController _player)
    {

    }

    public void SwapSkill(float _remainingTIme)
    {
        remainingTime = _remainingTIme;
        if (remainingTime <= 0)
        {
            isUse = true;
        }
        else
        {
            isUse = false;
        }
    }
}

