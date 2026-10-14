using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Optional audio hooks. Missing clips never cause errors.
    /// </summary>
    public class GameAudio : MonoBehaviour
    {
        public static GameAudio Instance { get; private set; }

        [Header("Optional Clips")]
        [SerializeField] private AudioClip gemCollect;
        [SerializeField] private AudioClip encounterOpen;
        [SerializeField] private AudioClip diceRoll;
        [SerializeField] private AudioClip success;
        [SerializeField] private AudioClip failure;
        [SerializeField] private AudioClip clarityReveal;
        [SerializeField] private AudioClip win;
        [SerializeField] private AudioClip loss;

        private AudioSource _source;

        private void Awake()
        {
            Instance = this;
            _source = gameObject.GetComponent<AudioSource>();
            if (_source == null)
                _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void PlayGem() => Play(gemCollect);
        public void PlayEncounter() => Play(encounterOpen);
        public void PlayDice() => Play(diceRoll);
        public void PlaySuccess() => Play(success);
        public void PlayFailure() => Play(failure);
        public void PlayClarity() => Play(clarityReveal);
        public void PlayWin() => Play(win);
        public void PlayLoss() => Play(loss);

        private void Play(AudioClip clip)
        {
            if (clip == null || _source == null)
                return;
            _source.PlayOneShot(clip);
        }
    }
}
