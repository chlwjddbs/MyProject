using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Skill", menuName = "Skill")]
public class SkillItem : Item
{
    public string skillName;
    public SkillType skillType;

    public GameObject skill;

    public Sound skillSound;
   
    public override void Use(int slotNum)
    {
        SkillBook.instance.LearnSkill(this);
    }
}

public enum SkillType
{
    Attack,          //공격 스킬 
    Defence = 1000,  //방어 스킬
    Buff = 2000,     //버프 스킬
    Utile = 3000,    //유틸 스킬
}
