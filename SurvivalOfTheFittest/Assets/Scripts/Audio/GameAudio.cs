using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Audio hooks. Uses Inspector clips when assigned; otherwise procedural tones.
    /// Missing clips never cause errors.
    /// </summary>
    public class GameAudio : MonoBehaviour
    {
        public static GameAudio Instance { get; private set; }

        [Header("Optional Clips (leave empty to use built-in procedural SFX)")]
        [SerializeField] private AudioClip gemCollect;
        [SerializeField] private AudioClip encounterOpen;
        [SerializeField] private AudioClip diceRoll;
        [SerializeField] private AudioClip success;
        [SerializeField] private AudioClip failure;
        [SerializeField] private AudioClip clarityReveal;
        [SerializeField] private AudioClip win;
        [SerializeField] private AudioClip loss;
        [SerializeField] private AudioClip dash;
        [SerializeField] private AudioClip footstep;
        [SerializeField] private AudioClip blocked;
        [SerializeField] private AudioClip uiClick;

        [Header("Mix")]
        [SerializeField] private float masterVolume = 0.85f;

        private AudioSource _source;
        private AudioSource _footSource;

        private void Awake()
        {
            Instance = this;
            ProceduralSfx.EnsureBuilt();

            _source = gameObject.GetComponent<AudioSource>();
            if (_source == null)
                _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;

            var footGo = new GameObject("Footsteps");
            footGo.transform.SetParent(transform, false);
            _footSource = footGo.AddComponent<AudioSource>();
            _footSource.playOnAwake = false;
            _footSource.volume = 0.45f;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void PlayGem() => Play(gemCollect, ProceduralSfx.Gem, 1f);
        public void PlayEncounter() => Play(encounterOpen, ProceduralSfx.Encounter, 1f);
        public void PlayDice() => Play(diceRoll, ProceduralSfx.Dice, 0.9f);
        public void PlaySuccess() => Play(success, ProceduralSfx.Success, 1f);
        public void PlayFailure() => Play(failure, ProceduralSfx.Failure, 1f);
        public void PlayClarity() => Play(clarityReveal, ProceduralSfx.Clarity, 1f);
        public void PlayWin() => Play(win, ProceduralSfx.Win, 1f);
        public void PlayLoss() => Play(loss, ProceduralSfx.Loss, 1f);
        public void PlayDash() => Play(dash, ProceduralSfx.Dash, 0.85f);
        public void PlayBlocked() => Play(blocked, ProceduralSfx.Blocked, 0.9f);
        public void PlayUiClick() => Play(uiClick, ProceduralSfx.UiClick, 0.7f);

        public void PlayFootstep()
        {
            var clip = footstep != null ? footstep : ProceduralSfx.Footstep;
            if (clip == null || _footSource == null)
                return;
            _footSource.pitch = 0.92f + (Time.frameCount % 5) * 0.03f;
            _footSource.PlayOneShot(clip, masterVolume * 0.35f);
        }

        private void Play(AudioClip overrideClip, AudioClip procedural, float volumeScale)
        {
            var clip = overrideClip != null ? overrideClip : procedural;
            if (clip == null || _source == null)
                return;
            _source.PlayOneShot(clip, masterVolume * volumeScale);
        }
    }
}
