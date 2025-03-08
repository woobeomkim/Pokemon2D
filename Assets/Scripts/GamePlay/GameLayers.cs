using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;


    /*  
     전통적인 { return ... } 방식	
     get { return value; }	- C#에서 가장 기본적인 프로퍼티 문법
    -여러 줄 작성 가능
    - 복잡한 로직 추가 가능	csharp public int Value { get { return someValue * 2; } }
    
    람다(=>) 표현식 방식	get => value;	- C# 6.0 이상에서 도입
    - 단순한 값 반환 시 가독성이 좋음
    - 한 줄로 간결하게 작성 가능	csharp public int Value => someValue * 2;
     
    간단한 값 반환  => 방식 추천
    여러 줄, 복잡한 로직 필요 → { return ... } 방식 추천  

    두 코드의 기능은 동일하지만 표현 방식이 다릅니다.
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

}
