using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInitializer : MonoBehaviour
{
    public DamagePopup popupPrefab;

    private void Awake()
    {
        DamagePopup.Init(popupPrefab);
    }
}
