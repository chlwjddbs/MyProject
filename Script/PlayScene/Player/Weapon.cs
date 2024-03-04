using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public PlayerController playerController;

    //공격 시 타격 판정을 할 MeshCollider
    private MeshCollider attackPoint;

    //장착 한 무기가 없는 기본 공격의 Mesh
    public Mesh basicMesh;

    public EquipItem equipWeapon;

    public float attackDamage;

    private bool isDamage;

    public bool isMultiple = false;

    public List<Collider> enemies;

    public UnityAction AddUpdateEquip;

    public AnimationClip backAttackClip;

  
    // Start is called before the first frame update
    void Start()
    {
        //기본 Mesh로 초기화
        attackPoint = GetComponent<MeshCollider>();
        attackPoint.sharedMesh = basicMesh;
        isDamage = true;
        Equipment.instance.UpdateDamage += SetAttackDamage;
    }

    //아이템 장착시
    public void EquipWeapon(EquipItem weapon)
    {
        equipWeapon = weapon;

        //장착한 아이템의 Mesh로 변경
        attackPoint.sharedMesh = equipWeapon.mesh;
        
        playerController.SetAttackAnime(equipWeapon.attackClip);
        
        //장착한 아이템의 공격력 추가
        //playerStatus.equipDamage = equipWeapon.attack;
        //SetAttackDamage();
    }

    //아이템 장착 해제시
    public void UnequipWeapon()
    {
        //무기 공격력 제거
        //playerStatus.equipDamage -= equipWeapon.attack;

        equipWeapon = null;
        //공격 Mesh 기본 Mesh로 변경
        attackPoint.sharedMesh = basicMesh;

        playerController.SetAttackAnime(backAttackClip);
        
        //SetAttackDamage();
    }

    public void SetSkillDage(float _addDage, bool _multiple)
    {
        isDamage = true;
        isMultiple = _multiple;
        attackDamage = (playerStatus.currentDamage) * _addDage;
    }

    public void SetAttackDamage()
    {      
        isDamage = true;
        isMultiple = false;
        attackDamage = playerStatus.currentDamage;
    }

    public void SetDamage()
    {
        attackDamage = playerStatus.currentDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            attackPoint.enabled = false;
        }

        if (other.CompareTag("Enemy"))
        {
            if (!enemies.Contains(other))
            {
                enemies.Add(other);
                if (isMultiple)
                {
                    if (other != null)
                    {
                        if (PlayerController.isAction && isDamage)
                        {
                            Vector3 damageDir = (other.transform.position - transform.position).normalized;
                            //damageDir.y = 0.5f;
                            other.GetComponent<Enemy>().Runaway(damageDir);
                            other.GetComponent<Enemy>().TakeDamage(attackDamage);
                        }
                    }
                }
                else
                {
                    if (enemies.Count < 2)
                    {
                        if (other != null)
                        {
                            if (PlayerController.isAction && isDamage)
                            {
                                Vector3 damageDir = (other.transform.position - transform.position).normalized;
                                //damageDir.y = 0.5f;
                                other.GetComponent<Enemy>().Runaway(damageDir);
                                other.GetComponent<Enemy>().TakeDamage(attackDamage);
                            }
                        }
                    }
                }
            }
        }
    }
}
