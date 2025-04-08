using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggersLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] LayerMask waterLayer;
    /*  
     �������� { return ... } ���	
     get { return value; }	- C#���� ���� �⺻���� ������Ƽ ����
    -���� �� �ۼ� ����
    - ������ ���� �߰� ����	csharp public int Value { get { return someValue * 2; } }
    
    ����(=>) ǥ���� ���	get => value;	- C# 6.0 �̻󿡼� ����
    - �ܼ��� �� ��ȯ �� �������� ����
    - �� �ٷ� �����ϰ� �ۼ� ����	csharp public int Value => someValue * 2;
     
    ������ �� ��ȯ  => ��� ��õ
    ���� ��, ������ ���� �ʿ� �� { return ... } ��� ��õ  

    �� �ڵ��� ����� ���������� ǥ�� ����� �ٸ��ϴ�.
     */

    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer
    {
        get => solidObjectsLayer; 
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;  
    }

    public LayerMask GrassLayer
    {
        get => grassLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }

    public LayerMask FovLayer
    {
        get => fovLayer;
    }

    public LayerMask PortalLayer
    {
        get => portalLayer;
    }

    public LayerMask LedgeLayer => ledgeLayer;

    public LayerMask WaterLayer => waterLayer;
    public LayerMask TriggerableLayers
    {
        get => grassLayer | fovLayer | portalLayer | triggersLayer | waterLayer;
    }
}
