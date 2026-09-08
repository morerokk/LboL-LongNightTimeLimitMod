using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LBoL.Core;
using LBoL.Presentation;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LongNightTimeLimit.UI
{
    /// <summary>
    /// Shows how many reshuffles are left before Time Limit is applied.
    /// </summary>
    public sealed class ReshuffleCounterBadge : MonoBehaviour
    {
        private const string IconName = "ReshufflesRemainingIndicator";

        private const float MirrorDistance = 0.7f;

        private static Sprite _icon;

        private CardUi _cardUi;
        private CardZoneUpperCountWidget _badge;
        private TextMeshProUGUI _text;

        /// <summary>The vanilla badge, which we copy.</summary>
        private RectTransform _source;

        /// <summary>The draw pile button.</summary>
        private RectTransform _drawZone;

        private RectTransform _rect;

        /// <summary>Where the vanilla badge was when we last placed ourselves.</summary>
        private Vector2 _placedFrom = new Vector2(float.NaN, float.NaN);

        private int _shown = int.MinValue;

        private bool _visible;

        public static void AttachTo(CardUi cardUi)
        {
            if (cardUi == null || cardUi.GetComponent<ReshuffleCounterBadge>() != null)
            {
                return;
            }

            var source = cardUi.drawUpperRoot;
            var drawZone = cardUi.drawButton;
            if (source == null || drawZone == null)
            {
                BepinexPlugin.log.LogWarning("Draw pile badge not found, skipping the reshuffle counter");
                return;
            }

            var self = cardUi.gameObject.AddComponent<ReshuffleCounterBadge>();
            self._cardUi = cardUi;
            self._source = (RectTransform)source.transform;
            self._drawZone = (RectTransform)drawZone.transform;
            self._badge = self.Build(source);
            self._rect = (RectTransform)self._badge.transform;
        }

        /// <summary>
        /// Copies the badge and strips unnecessary tooltips
        /// </summary>
        private CardZoneUpperCountWidget Build(CardZoneUpperCountWidget source)
        {
            var clone = Instantiate(source.gameObject, source.transform.parent, false);
            clone.name = "ReshufflesLeftRoot";

            var widget = clone.GetComponent<CardZoneUpperCountWidget>();

            // Hovering the vanilla badge opens the draw pile preview. Ours is display only.
            foreach (var hover in clone.GetComponentsInChildren<FastDeckViewButton>(true))
            {
                hover.enabled = false;
            }
            foreach (var graphic in clone.GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = false;
            }

            SetIcon(widget);

            _text = clone.GetComponentInChildren<TextMeshProUGUI>(true);

            // Run Awake immediately (lol)
            clone.SetActive(true);
            clone.SetActive(false);

            return widget;
        }

        private static void SetIcon(CardZoneUpperCountWidget widget)
        {
            var candidates = IconImages(widget);

            var sprite = LoadIcon();

            if (sprite == null || candidates.Count == 0)
            {
                BepinexPlugin.log.LogWarning("No " + IconName + " sprite, hiding the badge icon");
                HideIcon(widget);
                return;
            }

            var shown = candidates[candidates.Count - 1];
            for (int i = 0; i < candidates.Count - 1; i++)
            {
                candidates[i].enabled = false;
            }

            shown.enabled = true;
            shown.overrideSprite = null;
            shown.sprite = sprite;
            shown.type = Image.Type.Simple;
            shown.preserveAspect = true;
        }

        private static Sprite LoadIcon()
        {
            if (_icon != null)
            {
                return _icon;
            }

            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(folder, Path.Combine("Resources", IconName + ".png"));
            if (!File.Exists(path))
            {
                BepinexPlugin.log.LogWarning("Badge icon missing at " + path);
                return null;
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.wrapMode = TextureWrapMode.Clamp;
            if (!texture.LoadImage(File.ReadAllBytes(path)))
            {
                BepinexPlugin.log.LogWarning("Badge icon unreadable at " + path);
                return null;
            }

            _icon = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _icon.name = IconName;
            _icon.hideFlags = HideFlags.HideAndDontSave;
            return _icon;
        }

        private static List<Image> IconImages(CardZoneUpperCountWidget widget)
        {
            var found = new List<Image>();
            foreach (var image in widget.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite == null || (widget.bg != null && image.transform.IsChildOf(widget.bg)))
                {
                    continue;
                }

                found.Add(image);
            }

            return found;
        }

        private static void HideIcon(CardZoneUpperCountWidget widget)
        {
            if (widget.icon != null)
            {
                widget.icon.gameObject.SetActive(false);
            }
        }

        private static string Name(Transform transform)
        {
            if (transform == null)
            {
                return "none";
            }

            return transform.parent == null ? transform.name : transform.parent.name + "/" + transform.name;
        }

        private void Update()
        {
            if (_badge == null || _text == null)
            {
                return;
            }

            bool show = ShouldShow();
            if (show)
            {
                Reposition();

                int left = ReshuffleTracker.ReshufflesUntilPenalty();
                if (left != _shown)
                {
                    _shown = left;
                    _text.text = left.ToString();
                }
            }

            if (_visible != show)
            {
                _visible = show;
                _badge.Active(show);
            }
        }

        private bool ShouldShow()
        {
            var gameRun = GameMaster.Instance == null ? null : GameMaster.Instance.CurrentGameRun;

            return gameRun != null
                   && gameRun.Battle != null
                   && gameRun.Puzzles.HasFlag(PuzzleFlag.NightMana)
                   && _cardUi != null;
        }

        private void Reposition()
        {
            if (_source == null || _drawZone == null || _rect == null)
            {
                return;
            }

            var anchored = _source.anchoredPosition;
            if (anchored == _placedFrom)
            {
                return;
            }

            var parent = _source.parent;
            if (parent == null)
            {
                return;
            }

            float middle = parent.InverseTransformPoint(_drawZone.position).x;
            float vanilla = parent.InverseTransformPoint(_source.position).x;

            _rect.anchoredPosition = anchored
                + new Vector2((1f + MirrorDistance) * (middle - vanilla), 0f);
            _placedFrom = anchored;
        }
    }
}
