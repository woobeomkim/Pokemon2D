using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition 
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    // ����ǻ����϶� �޼���
    public string StartMessage { get; set; }

    // Action�� void
    // Func�� ������ȯ�Ҽ�����

    /*
     �� �׸�	    Action<T>	                        Func<T>
     ��ȯ��	����    (void)	                            ��ȯ�� ����
     �⺻ ��� ����	�۾��� ���������� ���� ��ȯ���� ����	�۾��� �����ϰ� ���� ��ȯ��
     �Ű�����	    0�� �̻� ����	                    0�� �̻� ����
     ��ǥ���� ��� ����	Action<int> �� void �Լ�         (int)	Func<int, string> �� string �Լ�(int)
     
    Func<T1, T2, ..., TResult>���� ������ TResult�� ��ȯ Ÿ��
    ���� Ÿ�Ե��� �Ű����� Ÿ��
    �Ű������� ������ Func<TResult> ���� (��: Func<int> �� int ��ȯ)
    ��, Func<int, string>�̸� int�� �޾� string�� ��ȯ�ϴ� �Լ���� �ǹ��Դϴ�! 
     */

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove {  get; set; }
    public Action<Pokemon> onAfterTurn {  get; set; }
}
