using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class ZombieInvicible : MonoBehaviour
{
    [SerializeField] float bodyDisposeSpeed1 = 0.5f;
    [SerializeField] List<SkinnedMeshRenderer> skinnedMeshRenderer;
    //Material mat;
    List<Material> originalMat = new List<Material>();
    [SerializeField] List<Material> transperantMat;
    MaterialPropertyBlock mpb;
    Enemy enemy;
    EnemyAttack enemyAttack;

    void Awake()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        enemy = transform.parent.GetComponent<Enemy>();
        //skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        mpb = new MaterialPropertyBlock();
        for(int i=0;i<skinnedMeshRenderer.Count;i++)
        {
            originalMat.Add(skinnedMeshRenderer[i].sharedMaterial);
        }
        //mat = skinnedMeshRenderer.material;
    }

    public void OpaqueToTransparent()
    {
        /*mat.SetFloat("_Surface",1);
        mat.SetOverrideTag("RenderType","Transperant");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        mat.SetInt("_ZWrite",0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_AlphaPreMultiplier_ON");
        mat.DisableKeyword("_AlphaTest_ON");

        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);*/
        
        //if(skinnedMeshRenderer.gameObject.activeInHierarchy == false) return;
        
        if(transform.parent.gameObject.activeSelf) { StartCoroutine(MaterialLerpValue()); }
    }

    IEnumerator MaterialLerpValue()
    {
        int count = 0;
        for(int i=0;i<transperantMat.Count;i++)
        {
            for(int j=0;j<skinnedMeshRenderer.Count/2;j++)
            {
                skinnedMeshRenderer[count].sharedMaterial = transperantMat[i];
                count++;
            }   
        }
        
        float val = 1;
        while(val > 0)
        {
            val -= Time.deltaTime*bodyDisposeSpeed1;
            mpb.SetColor("_BaseColor",new Color(1,1,1,val));
            for(int i=0;i<skinnedMeshRenderer.Count;i++){ skinnedMeshRenderer[i].SetPropertyBlock(mpb); }
            yield return null;
        }

        mpb.SetColor("_BaseColor",new Color(1,1,1,0f));
        for(int i=0;i<skinnedMeshRenderer.Count;i++){ skinnedMeshRenderer[i].SetPropertyBlock(mpb); }

        enemy.gameObject.SetActive(false);
        enemyAttack.ResetEverything();
        enemy.ResetEverything();
    }

    public void TransparentToOpaque()
    {
        for(int i=0;i<skinnedMeshRenderer.Count;i++)
        { 
            skinnedMeshRenderer[i].SetPropertyBlock(null);
            skinnedMeshRenderer[i].sharedMaterial = originalMat[i];
        }
    }

    public void ForceReset()
    {
        StopAllCoroutines();
        TransparentToOpaque();
        enemy.gameObject.SetActive(false);
        enemyAttack.ResetEverything();
        enemy.ResetEverything();
    }
}
