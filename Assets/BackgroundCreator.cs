using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCreator : MonoBehaviour
{
    public int Width;
    public int Height;
    public float Depth;
    public Color LayerColor;
    public float SolidThreshold;
    public float NoiseScale = 4f;
    public SpriteRenderer textureRenderer;
    public Transform FollowTarget;

    Vector3 BasePosition;

    // Start is called before the first frame update
    void Start()
    {
        CreateTexture();
        BasePosition = FollowTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (FollowTarget.position - BasePosition) * 1f * (1 - 0.2f * Depth);
    }

    void CreateTexture() {
        Debug.Log("Creating Texture");
        Texture2D texture = new Texture2D(Width, Height);

        Color[] col_map = new Color[Width * Height];

        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (SimplexNoise.Noise(x / NoiseScale, y / NoiseScale, Depth  / NoiseScale * 8) > SolidThreshold) {
                    col_map[y * Width + x] = LayerColor;
                } else {
                    col_map[y * Width + x] = Color.clear;
                }
                
            }
        }

        texture.SetPixels(col_map);
        texture.Apply();

        Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        textureRenderer.sprite = s;
        // textureRenderer.transform.localScale = new Vector3(Width, Height, 1);
    }
}
