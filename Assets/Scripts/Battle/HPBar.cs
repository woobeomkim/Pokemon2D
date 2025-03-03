using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HPBar�� �����̴� Ŭ����
public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float normalizedHP)
    {
        health.transform.localScale = new Vector3(normalizedHP, 1.0f);
    }
}
