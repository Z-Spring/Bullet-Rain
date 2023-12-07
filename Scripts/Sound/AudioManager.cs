using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace sound
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClipsSO audioClipsSo;

        private void Start()
        {
            EnemyController.Instance.onPlayFireSound.AddListener(EnemyController_OnPlayFireSound);
            EnemyController.Instance.onPlayFootstepSound.AddListener(EnemyController_OnPlayFootstepSound);
        }


        private void EnemyController_OnPlayFireSound(EnemyController enemy)
        {
            PlaySound(audioClipsSo.handgunFire, enemy.transform.position);
        }

        private void EnemyController_OnPlayFootstepSound(EnemyController enemy)
        {
            PlaySound(audioClipsSo.footstep, enemy.transform.position);
        }

        private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volume);
        }
        
        private void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f)
        {
            PlaySound(audioClips[Random.Range(0, audioClips.Length)], position, volume);
        }
    }
}