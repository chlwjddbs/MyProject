using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class ConfirmedPopup : MonoBehaviour
{
    public Button okButton;
    public Button cancelButton;
    public LocalizeStringEvent popupText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }   
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}


