using UnityEngine;

public class TriangleGenerator : MonoBehaviour
{
    [Header("Texture Settings")]
    public int textureSize = 64; // テクスチャのサイズ（縦横）
    public Color triangleColor = Color.white; // 三角形の色
    public Color backgroundColor = Color.clear; // 背景の色（通常は透明）

    // このメソッドを他のスクリプトから呼び出すか、
    // Start/Awakeで実行してRendererに適用します
    public Texture2D GenerateInvertedTriangle()
    {
        // 1. Texture2Dを初期化
        Texture2D texture = new Texture2D(textureSize, textureSize);
        // 背景色で全てを塗りつぶす
        Color[] pixels = new Color[textureSize * textureSize];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }

        // 2. 逆三角形を描画するロジック
        // 逆三角形は、テクスチャの上端 (y=textureSize-1) から下端 (y=0) に向かって幅が広くなる形
        // 中心のx座標
        float centerX = (float)textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            // y座標を正規化 (0.0 ～ 1.0)
            float normalizedY = (float)y / (textureSize - 1f);

            // 逆三角形の幅（yが0（下端）で最大幅、yが1（上端）で幅0）
            // 逆三角形なので、1.0 - normalizedY は y=0 で 1.0 に、y=textureSize-1 で 0.0 になる
            float halfWidth = centerX * (1f - normalizedY);
            
            // 描画開始x座標と終了x座標
            int startX = Mathf.FloorToInt(centerX - halfWidth);
            int endX = Mathf.CeilToInt(centerX + halfWidth);

            // ピクセルを塗りつぶし
            for (int x = startX; x < endX; x++)
            {
                if (x >= 0 && x < textureSize)
                {
                    // 1次元配列のインデックスを計算 (y * width + x)
                    int index = y * textureSize + x;
                    pixels[index] = triangleColor;
                }
            }
        }

        // 3. ピクセルデータをテクスチャに適用して、GPUにアップロード
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    // 例として、実行時にこのテクスチャをRendererに適用する
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Texture2D triangleTexture = GenerateInvertedTriangle();
            // 新しいマテリアルを作成し、テクスチャを設定
            Material material = new Material(Shader.Find("Sprites/Default")); // 標準のSpriteシェーダーなど
            material.mainTexture = triangleTexture;
            renderer.material = material;
        }
    }
}