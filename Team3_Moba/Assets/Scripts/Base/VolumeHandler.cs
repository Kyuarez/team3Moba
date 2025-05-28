using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeHandler : MonoBehaviour
{
    public static VolumeHandler Instance { get; private set; }

    [SerializeField] private Volume volume;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayerDeadVolume()
    {
        SetColorAdjustmentSaturation(-100f);
    }
    public void PlayerRespawnVolume()
    {
        SetColorAdjustmentSaturation(0f);
    }
    private void SetColorAdjustmentSaturation(float saturation)
    {
        if (volume != null && volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.saturation.value = saturation;
        }
        else
        {
            Logger.LogWarning("ColorAdjustments component not found in the Volume profile.");
        }
    }
}
