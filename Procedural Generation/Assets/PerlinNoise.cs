using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;
    public float m_timer = 0;
    public int m_iSeedNumber = 0;
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    
    public Texture2D GetTex()
    {
        return noiseTex;
    }
    private void Awake()
    {
        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        CalcNoise();

    }
    void Start()
    {
        m_timer = Time.deltaTime.GetHashCode();
        m_iSeedNumber = Time.time.GetHashCode();
        if (m_iSeedNumber == 0)
        {
            m_iSeedNumber = System.DateTime.Now.GetHashCode();
        }


    }

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        var rand = new System.Random(m_iSeedNumber);
        var num = rand.Next(0, (int)xOrg + (int)yOrg);
        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
              
                float xCoord = xOrg + x / noiseTex.width * scale+ num;

                float yCoord = yOrg + y / noiseTex.height * scale+ num;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_timer = Time.deltaTime.GetHashCode();
            m_iSeedNumber = Time.time.GetHashCode();
            if (m_iSeedNumber == 0)
            {
                m_iSeedNumber = System.DateTime.Now.GetHashCode();
            }
        };

        CalcNoise();
    }
}
