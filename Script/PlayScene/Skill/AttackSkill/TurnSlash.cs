using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSlash : SkillManager
{
    public ParticleSystem effect;
    public float skillDamage; 

    public override void UseSkill(PlayerController _player)
    {
        if (isUse)
        {
            _player.GetComponent<PlayerStatus>().UseMana(cunsumeMana);
            _player.GetComponentInChildren<Weapon>().SetSkillDage(skillDamage,true);
            _player.SetAnime(skillMotion);
            _player.SetState(PlayerState.Action);
            _player.SetActionSpeed(actionSpeed);
            PlayerController.isAction = true;     
            
            isUse = false;
            remainingTime = coolTime;

            //Ω∫≈≥ ¿Ã∆Â∆Æ
            ParticleSystem _effect = Instantiate(effect, _player.effectPos);
            Vector3 effectOffset = _player.effectPos.position;
            effectOffset.y = 2.5f;
            _effect.transform.position = effectOffset;
        }
    }
}
