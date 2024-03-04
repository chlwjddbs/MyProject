using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Object_1stFloor : ObjectManager
{   
    public Enemy Olaf;
    public Enemy Gubne;

    public Area_C_Gimmick c_Gimmick;

    public override void StartStage()
    {
        base.StartStage();
        startStage = "1st_Floor";
        currentStage = startStage;
        startArea = "≈æ ¿‘±∏";
        currentArea = startArea;
    }

    public override void SaveMapdata()
    {
        base.SaveMapdata();

        for (int i = 0; i < stageEnemy.Count; i++)
        {
            try
            {
                dataManager.userData.eDatas_1F[i] = stageEnemy[i].SaveState();
            }
            catch
            {
                dataManager.userData.eDatas_1F.Add(stageEnemy[i].SaveState());
            }
        }

        for (int i = 0; i < startFieldItems.Count; i++)
        {
            if(startFieldItems[i] == null)
            {
                try
                {
                    dataManager.userData.startFieldItem_1F[i] = false;
                }
                catch
                {
                    dataManager.userData.startFieldItem_1F.Add(false);
                }
            }
            else
            {
                try
                {
                    dataManager.userData.startFieldItem_1F[i] = true;
                }
                catch
                {
                    dataManager.userData.startFieldItem_1F.Add(true);
                }
            }
        }

        dataManager.userData.FiledItme_1F.Clear();
        for (int i = 0; i < dropItemManager.transform.childCount; i++)
        {
            AddItem saveItem = dropItemManager.transform.GetChild(i).GetComponentInChildren<AddItem>();
            dataManager.userData.FiledItme_1F.Add(new DropItemManager.AddDropItem(saveItem, saveItem.transform.position));
        }

        c_Gimmick.SaveData();
    }

    public override void LoadMapData()
    {
        base.LoadMapData();
        for (int i = 0; i < stageEnemy.Count; i++)
        {
            stageEnemy[i].LoadState(dataManager.userData.eDatas_1F[i]);
            stageEnemy[i].isSet = true;
        }

        for (int i = 0; i < dataManager.userData.startFieldItem_1F.Count; i++)
        {
            if (!dataManager.userData.startFieldItem_1F[i])
            {
                Destroy(startFieldItems[i]);
            }
        }

        for (int i = 0; i < DataManager.instance.userData.FiledItme_1F.Count; i++)
        {
            GameObject field = Instantiate(DataManager.instance.userData.FiledItme_1F[i].fieldItem.FieldObject, dropItemManager.transform);
            field.transform.position = DataManager.instance.userData.FiledItme_1F[i].itemPos;
            field.GetComponentInChildren<AddItem>().quantity = DataManager.instance.userData.FiledItme_1F[i].quantity;
        }

        c_Gimmick.LoadData();

        /*
        Olaf.LoadState(0);
        Gubne.LoadState(0);

        for (int i = 0; i < spawnEnemy.Count; i++)
        {
            spawnEnemy[i].LoadState(i);
        }
        */
    }
}
