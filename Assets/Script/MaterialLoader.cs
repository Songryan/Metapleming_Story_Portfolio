using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLoader : MonoBehaviour
{
    public Material fullMapMaterial; // .mat 파일을 할당할 변수

    void Start()
    {
        // .mat 파일을 Resources 폴더 내에서 로드
        //fullMapMaterial = Resources.Load<Material>("FullMap"); 
        if (fullMapMaterial == null)
        {
            Debug.LogError("Failed to load FullMap material.");
            return;
        }

        // 게임 오브젝트의 렌더러에 해당 Material을 할당
        GetComponent<Renderer>().material = fullMapMaterial;
    }
}
