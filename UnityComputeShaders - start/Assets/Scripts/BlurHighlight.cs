﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlurHighlight : BaseCompletePP
{
    [Range(0, 50)]
    public int blurRadius = 20;
    [Range(0.0f, 100.0f)]
    public float radius = 10;
    [Range(0.0f, 100.0f)]
    public float softenEdge = 30;
    [Range(0.0f, 1.0f)]
    public float shade = 0.5f;
    public Transform trackedObject;

    Vector4 center;
    private RenderTexture horzOutput = null;
    private int kernelHorzPassID;

    protected override void Init()
    {
        center = new Vector4();
        kernelName = "Highlight";
        base.Init();

        kernelHorzPassID = shader.FindKernel("HorzPass");
    }

    protected override void CreateTextures()
    {
        base.CreateTextures();
        
        shader.SetTexture(kernelHorzPassID, "source", renderedSource);
        
        CreateTexture(ref horzOutput);
        shader.SetTexture(kernelHorzPassID, "horzOutput", horzOutput);
        shader.SetTexture(kernelHandle, "horzOutput", horzOutput);
    }

    private void OnValidate()
    {
        if(!init)
            Init();
           
        SetProperties();
    }

    protected void SetProperties()
    {
        float rad = (radius / 100.0f) * texSize.y;
        shader.SetFloat("radius", rad);
        shader.SetFloat("edgeWidth", rad * softenEdge / 100.0f);
        shader.SetFloat("shade", shade);
        // int float 不能混用，会出事
        shader.SetInt("blurRadius", blurRadius);
    }

    protected override void DispatchWithSource(ref RenderTexture source, ref RenderTexture destination)
    {
        if (!init) return;

        Graphics.Blit(source, renderedSource);

        shader.Dispatch(kernelHandle, groupSize.x, groupSize.y, 1);
        shader.Dispatch(kernelHorzPassID, groupSize.x, groupSize.y, 1);
        
        Graphics.Blit(output, destination);
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            if (trackedObject && thisCamera)
            {
                Vector3 pos = thisCamera.WorldToScreenPoint(trackedObject.position);
                center.x = pos.x;
                center.y = pos.y;
                shader.SetVector("center", center);
            }
            bool resChange = false;
            CheckResolution(out resChange);
            if (resChange) SetProperties();
            DispatchWithSource(ref source, ref destination);
        }
    }

}
