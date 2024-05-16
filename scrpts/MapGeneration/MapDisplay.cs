using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public GameObject meshGO;
    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        MeshCollider[] meshColliders = meshGO.GetComponents<MeshCollider>();
        for (int i = 0; i < meshColliders.Length; i++)
        {
            DestroyImmediate(meshColliders[i]);
        }
        meshGO.AddComponent<MeshCollider>();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}