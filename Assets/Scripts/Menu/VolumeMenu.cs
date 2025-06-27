using UnityEngine;

public class VolumeMenu : MonoBehaviour
{
    public void MasterVolume(float pMasterVolume)
    {
        SoundManager.instance.MasterVolume = pMasterVolume;
    }
}
