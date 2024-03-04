using UnityEngine;
using UnityEngine.Localization.Components;

public class SignText : Interaction
{
    public Transform sequencetextPos;
    public LocalizeStringEvent sequencetextPrefab;
    public string s_text;
    public string entryReference;
    public float stextLifeTime = 5f;

    private float countdown;
    private float coolTime = 4;

    public override void Update()
    {
        theDistance = PlayerController.CheckDistance;
        if(countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
    }

    public override void DoAction()
    {
        player.isObject = true;
        if (Input.GetMouseButtonDown(0))
        {
            SetSequenceText();
        }
    }

    private void SetSequenceText()
    {
        if (countdown <= 0)
        {
            if (sequencetextPos.childCount >= 3)
            {
                Destroy(sequencetextPos.GetChild(0).gameObject);
            }

            LocalizeStringEvent stext = Instantiate(sequencetextPrefab, sequencetextPos);

            stext.StringReference.TableReference = "SignMessage";
            stext.StringReference.TableEntryReference = entryReference;

            Destroy(stext.gameObject, stextLifeTime);
            countdown = coolTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            SetSequenceText();
        }
    }
}
