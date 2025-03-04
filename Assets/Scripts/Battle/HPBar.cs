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

    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHP = health.transform.localScale.x;
        float changeAmt = curHP - newHp;

        while (curHP - newHp > Mathf.Epsilon)
        {
            curHP -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHP, 1.0f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1.0f);
    }
}
