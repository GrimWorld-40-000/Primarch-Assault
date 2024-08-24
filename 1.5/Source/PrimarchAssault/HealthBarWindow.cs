using PrimarchAssault.External;
using UnityEngine;
using Verse;

namespace RimworldModding
{
    public class HealthBarWindow: Window
    {
        public HealthBarWindow()
        {
            doWindowBackground = false;
            doCloseX = false;
            windowRect = new Rect(Current.Camera.scaledPixelWidth / (float)2 - 640, 50, 1280, 128);
            absorbInputAroundWindow = false;
            closeOnAccept = false;
            closeOnCancel = false;
            closeOnClickedOutside = false;
            drawShadow = false;
            focusWhenOpened = false;
        }

        public override Vector2 InitialSize => new Vector2(1280, 128);

        private float _healthPercent;
        private float _shieldPercent;
        public ChallengeDef ChallengeDef;
        public int CurrentPawn;

        private static readonly Color ShieldColor = new Color(.75f, .88f, .88f);
        private static readonly Color HealthColor = new Color(.81f, .24f, .12f);
        
        public override void DoWindowContents(Rect inRect)
        {
            if (ChallengeDef.HealthBarIcon == null) return;
            GUI.DrawTexture(inRect, ChallengeDef.HealthBarIcon);
            Rect shieldBar = new Rect(inRect.xMin + 100, inRect.yMin + 75, 1080, 10);
            Rect healthBar = new Rect(inRect.xMin + 100, inRect.yMin + 93, 1080, 10);
            Widgets.DrawRectFast(shieldBar.LeftPart(_shieldPercent), ShieldColor);
            Widgets.DrawRectFast(healthBar.LeftPart(_healthPercent), HealthColor);
        }

        public void UpdateIfWilling(int championId, float hp, float sp)
        {
            if (championId != CurrentPawn) return;
            _healthPercent = hp;
            _shieldPercent = sp;
        }
    }
}