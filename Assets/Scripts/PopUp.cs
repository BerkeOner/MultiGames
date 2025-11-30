using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public static PopUp Instance;
    void Awake() { if (Instance != null) { return; } Instance = this; }

    public GameObject PopUpWindow;

    public TMP_Text title;
    public TMP_Text description;
    public TMP_Text button;

    public void SetPopUp(string _title, string _description, string _button)
    {
        PopUpWindow.SetActive(true);

        title.text = _title;
        description.text = _description;
        button.text = _button;
    }

    public void _TUTORIAL()
    {
        SetPopUp(
            "Öğretici",
            "Öğreticiler oyun içinde sol üst köşede yer alır",
            "Tamam"
        );
    }

    public void NoConnection()
    {
        SetPopUp(
            "Hata",
            "İnternet bağlantısı yok",
            "Tamam"
        );
    }
}
