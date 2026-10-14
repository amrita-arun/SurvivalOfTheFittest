using UnityEngine;

namespace SurvivalOfTheFittest
{
    public class EncounterTrigger : MonoBehaviour
    {
        private EncounterDefinition _def;
        private bool _resolved;
        private SpriteRenderer _sr;

        public bool IsResolved => _resolved;
        public EncounterDefinition Definition => _def;

        public void Initialize(EncounterDefinition def)
        {
            _def = def;
            _resolved = false;

            _sr = GetComponent<SpriteRenderer>();
            if (_sr == null)
                _sr = gameObject.AddComponent<SpriteRenderer>();

            _sr.sprite = SpriteFactory.WhiteCircle;
            _sr.color = def.Color;
            _sr.sortingOrder = 4;
            transform.localScale = Vector3.one * 0.7f;

            var col = GetComponent<CircleCollider2D>();
            if (col == null)
                col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.45f;

            CreateLabel(def.DisplayName, def.Color);
        }

        private void CreateLabel(string text, Color color)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, 0.7f, 0f);

            var shortName = text.Length > 14 ? text.Substring(0, 14) : text;
            if (text.Contains("Ex"))
                shortName = "EX";
            else if (text.Contains("App"))
                shortName = "APP";
            else if (text.Contains("Mixed"))
                shortName = "MIXED";

            var tm = go.AddComponent<TextMesh>();
            tm.text = shortName;
            tm.characterSize = 0.1f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.fontSize = 36;
            tm.color = color;
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                      ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.sortingOrder = 21;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_resolved || _def == null)
                return;
            if (!other.CompareTag("Player"))
                return;
            if (GameManager.Instance == null || !GameManager.Instance.AllowsPlayerMovement)
                return;

            EncounterManager.Instance?.BeginEncounter(this);
        }

        public void MarkResolved()
        {
            _resolved = true;
            if (_sr != null)
            {
                var c = _sr.color;
                c.a = 0.25f;
                _sr.color = c;
            }

            var col = GetComponent<CircleCollider2D>();
            if (col != null)
                col.enabled = false;

            gameObject.SetActive(false);
        }

        public void ResetEncounter()
        {
            _resolved = false;
            gameObject.SetActive(true);
            if (_sr != null && _def != null)
                _sr.color = _def.Color;

            var col = GetComponent<CircleCollider2D>();
            if (col != null)
                col.enabled = true;
        }
    }
}
