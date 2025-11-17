using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float ScrollSpeed = 0.1f;
    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;
    private Vector2 _offset;
    private static readonly int MainTexSTProperty = Shader.PropertyToID("_MainTex_ST");

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        _offset = _renderer.sharedMaterial.mainTextureOffset;
    }

    private void Update()
    {
        Vector2 direction = Vector2.up;
        _offset += direction * (ScrollSpeed * Time.deltaTime);

        _mpb.SetVector(MainTexSTProperty, new Vector4(1, 1, _offset.x, _offset.y));
        _renderer.SetPropertyBlock(_mpb);
    }
}