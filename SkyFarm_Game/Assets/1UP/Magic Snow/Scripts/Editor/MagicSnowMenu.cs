using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MagicSnowMenu : MonoBehaviour
{
    // Start is called before the first frame update

    [MenuItem("1UP/Magic Snow/Attach MoveCamera", priority = 22)]
    [MenuItem("GameObject/1UP/Magic Snow/Attach MoveCamera", priority = 22)]
    static void CreateCustomGameObjectMoveCamera()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
        {
            Debug.Log("No object selected.");
            return;
        }

        if (obj.GetComponent<MoveCamera>() == null)
        {
            obj.AddComponent<MoveCamera>();
            Debug.Log("Attach moveCamera script successed.");
        }
    }

        [MenuItem("1UP/Magic Snow/Attach 1UP SnowGround", priority = 20)]
    [MenuItem("GameObject/1UP/Magic Snow/Attach 1UP SnowGround", priority = 20)]
    static void CreateCustomGameObjectSnowGround()
    { 
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
        {            
            Debug.Log("No object selected.");
            return;
        }
        if (obj.GetComponent<SnowGround>() == null)
        {
            obj.AddComponent<SnowGround>();
            Debug.Log("Attach SnowFloor script successed.");
        }        
        Renderer renderer = obj.GetComponent<Renderer>();        
        if (renderer == null)        
        {            
            renderer = obj.AddComponent<Renderer>();            
            Debug.Log("Attach Renderer successed.");        
        }        
        Material material = renderer.GetComponent<Material>();        
        if (material == null ||            
            material.shader.name != "1UP/Magic Snow/Floor")        
        {            
            material = new Material(Shader.Find("1UP/Magic Snow/Floor"));            
            renderer.material = material;        
        }        
        else        
        {            
            material = new Material(material);            
            renderer.material = material;        
        }    
    }

    [MenuItem("1UP/Magic Snow/Attach MoveObject", priority = 21)]
    [MenuItem("GameObject/1UP/Magic Snow/Attach MoveObject", priority = 21)]
    static void CreateCustomGameObjectMoveObject()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
        {
            Debug.Log("No object selected.");
            return;
        }

        if (obj.GetComponent<MoveObject>() == null)
        {
            obj.AddComponent<MoveObject>();
            Debug.Log("Attach SnowPlane script successed.");
        }
    }
}
