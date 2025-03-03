using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HPBar를 움직이는 클래스
public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float normalizedHP)
    {
        health.transform.localScale = new Vector3(normalizedHP, 1.0f);
    }
}
