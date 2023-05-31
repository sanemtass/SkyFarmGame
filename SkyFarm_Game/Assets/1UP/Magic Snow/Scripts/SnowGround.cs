using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class PosInfo
{
    public float time;
    public float step;
    public Vector3 pos;
    public List<Vector3> verts = new List<Vector3>()
    {
        new Vector3(), new Vector3(),
        new Vector3(), new Vector3(),
    };

    public Vector3 baseScale;
    private Vector3 localScale;
    private Vector3 timeScale;

    void updateVertices() 
    {
        verts[0] = baseScale -
            new Vector3(
                -timeScale.x,
                -timeScale.y);
        verts[1] = baseScale -
            new Vector3(
                +timeScale.x,
                -timeScale.y);
        verts[2] = baseScale -
            new Vector3(
                +timeScale.x,
                +timeScale.y);
        verts[3] = baseScale -
            new Vector3(
                -timeScale.x,
                +timeScale.y);
    }

    public void genVertices(Vector2 scale, float fixScale)
    {
        localScale = scale * fixScale;
        timeScale = scale;
        baseScale = new Vector3(0.5f - pos.x * scale.x,
            0.5f - pos.z * scale.y);
        updateVertices();
    }

    public void updateTime(float deltaTime, float duration) 
    {
        this.time += this.step * deltaTime;
        timeScale = localScale * (0.4f + 0.6f * (1.0f - (this.time / duration)));
        updateVertices();
    }
}

[System.Serializable]
public class FootInfo
{
    public GameObject targetObject = null;
    public Texture2D snowShape = null;

    public GameObject attachObject = null;
    public float attachOffsetY = 0.0f;

    [Range(0.01f, 5.0f)]
    public float snowInterval = 0.01f;
    [Range(0.1f, 20.0f)]
    public float snowMeltTime = 2.0f;
    [Range(0.0f, 2.0f)]
    public float pathScale = 1.0f;
    private float recordTime = 0.0f;

    [HideInInspector]
    [Range(0.0f, 360.0f)]
    public float fixAngle = 0.0f;
    [HideInInspector]
    public Vector2 boundScale = Vector2.one;
    private Vector3 prevPosition = Vector3.zero;
    [HideInInspector]
    public List<PosInfo> posList = new List<PosInfo>();

    public FootInfo()
    {
        snowInterval = 0.01f;
        snowMeltTime = 2.0f;
    }

    public void updateStep(float deltaTime, 
        Vector3 floorPos, Vector3 rotation) 
    {
        if (targetObject == null) return;
        var targetPos = targetObject.transform.position - floorPos;
        if (recordTime > snowInterval) recordTime = snowInterval;
        recordTime -= deltaTime;
        if (recordTime <= 0.0f)
        {
            if (attachObject == null ||
                (attachObject != null && 
                 attachObject.transform.position.y < attachOffsetY))
            {
                PosInfo info = new PosInfo();
                info.pos = targetPos;
                info.time = 0.0f;
                info.step = Random.Range(0.4f, 2.0f);

                info.genVertices(boundScale, pathScale);
                posList.Add(info);
                recordTime = snowInterval;
                prevPosition = targetPos;
            }
        }

        List<PosInfo> removeList = new List<PosInfo>();
        foreach (var pos in posList)
        {
            pos.updateTime(deltaTime, snowMeltTime);
            if (pos.time > snowMeltTime) 
                removeList.Add(pos);
        }

        foreach (var pos in removeList)
        {
            posList.Remove(pos);
        }
    }
}


public class SnowGround : MonoBehaviour
{
    private RenderTexture pathRT = null;
    Material footMat = null;

    public List<FootInfo> snowGroundAdvance = new List<FootInfo>() { new FootInfo() };
    private Vector2 uvsize = new Vector2(1.0f, 1.0f);
    Vector3 prevPosition = new Vector3();

    void Start() 
    {
        var floorRenderer = this.GetComponent<Renderer>();
        if (floorRenderer == null) return;
        footMat = new Material(Shader.Find("1UP/Magic Snow/FootTex"));

        var floorBounds = floorRenderer.bounds;
        uvsize.x = Mathf.Abs(floorBounds.max.x - floorBounds.min.x);
        uvsize.y = Mathf.Abs(floorBounds.max.z - floorBounds.min.z);
        foreach (var foot in snowGroundAdvance)
        {
            if (foot.targetObject == null) continue;
            var stepRenderer = foot.targetObject.GetComponent<Renderer>();
            var stepBounds = new Bounds(Vector3.zero, Vector3.one);
            if (stepRenderer != null) stepBounds = stepRenderer.bounds;
            foot.boundScale = new Vector2(
                    Mathf.Abs(stepBounds.max.x - stepBounds.min.x) / uvsize.x,
                    Mathf.Abs(stepBounds.max.z - stepBounds.min.z) / uvsize.y
            );
        }
        // stepScale = stepSize / uvsize;
        prevPosition = this.transform.position;
        var mat = new Material(floorRenderer.sharedMaterial);
        if (mat == null) return;
        floorRenderer.material = mat;
        pathRT = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        pathRT.Create();
        mat.SetTexture("_PathTex", pathRT);
    }

    void updateStepList() 
    {
        var deltaTime = Time.deltaTime;
        foreach (var foot in snowGroundAdvance)
        {
            foot.updateStep(deltaTime, 
                this.transform.position,
                this.transform.rotation.eulerAngles);
        }
    }

    void updatePathRT() 
    {
        RenderTexture.active = pathRT;
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.invertCulling = true;
        GL.Clear(true, true, Color.clear);
        foreach (var foot in snowGroundAdvance)
        {
            if (footMat != null &&
                !footMat.SetPass(0)) 
                continue;
            footMat.SetTexture("_FootShape", foot.snowShape);
            GL.Begin(GL.QUADS);
            for (var i = 0; i < foot.posList.Count; i++)
            {
                GL.TexCoord2(0, 0);
                GL.Vertex(foot.posList[i].verts[0]);
                GL.TexCoord2(1, 0);
                GL.Vertex(foot.posList[i].verts[1]);
                GL.TexCoord2(1, 1);
                GL.Vertex(foot.posList[i].verts[2]);
                GL.TexCoord2(0, 1);
                GL.Vertex(foot.posList[i].verts[3]);
            }
            GL.End();
        }
        GL.invertCulling = false;
        GL.PopMatrix();  
    }

    void updateSnowRT()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        if (renderer == null) return;
        var mat = renderer.sharedMaterial;
        if (mat == null) return;
        mat.SetTexture("_PathTex", pathRT);
    }

    void Update() {
        updateStepList();
        updatePathRT();
    }
}