using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

public class DebugScript : MonoBehaviour
{
    // BUTON VE KOMUTU. KOMUT ADI = BUTON ADI.

    [Button]
    private void Button() { betterTextArea+="X"; }

    // INFO BOX. (Normal, Warning, Error)
    [InfoBox("INFO BOXES", EInfoBoxType.Normal)]

    // KUTU
    [ReorderableList]
    public string[] listBox;

    // DAHA İYİ TEXTAREA
    [ResizableTextArea] [ShowIf(EConditionOperator.And, "showField")]
    public string betterTextArea;

    // READONLY
    [ReadOnly]
    public string publicAndReadOnlyText = "TRY TO EDIT ME :D";

    // MIN MAX SLIDER
    [MinMaxSlider(0, 10)]
    public Vector2Int retreatAndSpotDistance;

    // SHOW IF VAR IS TRUE
    public bool showField;
}
