using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomalHeal : SkillManager
{
    public float recoveryPoint;
    public ParticleSystem effect;

    public override void UseSkill(PlayerController _player)
    {
        if (isUse)
        {
            //마나소모
            _player.GetComponent<PlayerStatus>().UseMana(cunsumeMana);
            //스킬 모션 등록
            _player.SetAnime(skillMotion);
            //스킬 모션 실행
            _player.SetState(PlayerState.Action);
            _player.SetActionSpeed(actionSpeed);
            PlayerController.isAction = true;
                      
            //recoveryPoint = Mathf.Round(player.maxHealth * (15 / 100));
            //HP회복
            _player.playerStatus.RecoveryHP(recoveryPoint);

            isUse = false;
            remainingTime = coolTime;

            //스킬 이펙트
            ParticleSystem _effect = Instantiate(effect,_player.effectPos);
            Vector3 effectOffset = _player.effectPos.position;
            effectOffset.y = 2f;
            _effect.transform.position = effectOffset;
            
            Destroy(_effect.gameObject, 2f);
        }
    }
}
