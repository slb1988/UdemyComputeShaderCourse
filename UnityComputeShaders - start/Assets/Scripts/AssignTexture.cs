using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTexture : MonoBehaviour
{
    public ComputeShader shader;

    public int texResolution = 256;

    private Renderer rend;

    private RenderTexture outputTexture;

    private int kernelHandle;
    
    // Start is called before the first frame update
    void Start()
    {
        outputTexture = new RenderTexture(texResolution, texResolution, 0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        rend = GetComponent<Renderer>();
        rend.enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
