using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class ZombieInvicible : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Material mat;
    Material originalMat;

    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        originalMat = skinnedMeshRenderer.sharedMaterial;
        mat = skinnedMeshRenderer.material;
    }

    public void OpaqueToTransparent()
    {
        mat.SetFloat("_Surface",1);
        mat.SetOverrideTag("RenderType","Transperant");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        mat.SetInt("_ZWrite",0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_AlphaPreMultiplier_ON");
        mat.DisableKeyword("_AlphaTest_ON");

        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        
        if(skinnedMeshRenderer.gameObject.activeInHierarchy == false) return;
        StartCoroutine(MaterialLerpValue());
    }

    IEnumerator MaterialLerpValue()
    {
        Color color = mat.color;
        float val = 1;
        while(val > 0)
        {
            val -= Time.deltaTime;
            color.a = val;
            mat.color = color;
            yield return null;
        }
        color.a = 0;
        mat.color = color;

        Enemy enemy = transform.parent.parent.GetComponent<Enemy>();
        if(enemy.reBirth)
        {
            enemy.ResetEverything();
            transform.parent.GetComponent<EnemyAttack>().ResetEverything();
            enemy.gameObject.SetActive(false);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
    }

    public void TransparentToOpaque()
    {
        mat = new Material(originalMat);
        skinnedMeshRenderer.material = mat;
    }
}
