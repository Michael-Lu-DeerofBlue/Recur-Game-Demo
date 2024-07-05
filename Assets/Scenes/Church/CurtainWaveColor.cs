using UnityEngine;

public class CurtainWaveColor : MonoBehaviour
{
    private Renderer curtainRenderer;
    private MaterialPropertyBlock propertyBlock;

    public Color waveColor = Color.white;

    void Start()
    {
        // Initialize the renderer and material property block
        curtainRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        // Set the wave color property
        propertyBlock.SetColor("_WaveColor", waveColor);

        // Apply the property block to the renderer
        curtainRenderer.SetPropertyBlock(propertyBlock);
    }

    // Optional: Update the wave color at runtime
    public void SetWaveColor(Color newColor)
    {
        waveColor = newColor;
        propertyBlock.SetColor("_WaveColor", waveColor);
        curtainRenderer.SetPropertyBlock(propertyBlock);
    }
}
