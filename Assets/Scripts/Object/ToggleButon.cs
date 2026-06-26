using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButon : MonoBehaviour
{
    [SerializeField] private Button btn;

    void OnEnable()
    {
        btn.gameObject.SetActive(false);
    }
    void OnDisable()
    {
        btn.gameObject.SetActive(true);
    }
}
