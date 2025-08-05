using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/TriggerSpinner")]
    [Tracked]
    public class TriggerSpinner : Entity
    {
        public enum TriggerSpinnerColor
        {
            Blue,
            Red,
            Purple,
            Rainbow,
            Custom
        }

        public TriggerSpinnerColor color;
        public string customPathTriggered, customPathIndicator;
        public bool attachToSolid;
        private int randomSeed;
        private bool expanded;
        private bool playerInRange;
        private bool isActivated, hasPlayedSound;
        private Sprite spriteIndicator, spriteGrow;
        private List<Image> images = new List<Image>();
        private string sfx;

        public TriggerSpinner(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Visible = true;
            Collidable = true;
            Collider = new ColliderList(new Circle(6f), new Hitbox(16f, 4f, -8f, -3f));

            attachToSolid = data.Bool("attachToSolid", false);
            if (attachToSolid)
            {
                Add(new StaticMover
                {
                    OnShake = OnShake,
                    SolidChecker = IsRiding,
                    OnDestroy = base.RemoveSelf
                });
            }
            color = data.Enum("color", TriggerSpinnerColor.Blue);
            customPathTriggered = data.Attr("customPathTriggered", "");
            customPathIndicator = data.Attr("customPathIndicator", "objects/KoseiHelper/TriggerSpinner/");
            sfx = data.Attr("sound", "event:/game/general/assist_nonsolid_out");
            Add(spriteIndicator = new Sprite(GFX.Game, customPathIndicator + "indicator"));
            spriteIndicator.AddLoop("indicator", "", 0.1f);
            spriteIndicator.Play("indicator", false, false);
            spriteIndicator.CenterOrigin();

            Add(spriteGrow = new Sprite(GFX.Game, customPathIndicator + "grow"));
            spriteGrow.CenterOrigin();

            randomSeed = Calc.Random.Next();
            Add(new PlayerCollider(OnPlayerTouch));
            Depth = -8500;
            Tag = Tags.TransitionUpdate;
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
            {
                bool playerCollides = CollideCheck(player);
                if (playerCollides && !playerInRange)
                    playerInRange = true;
                else if (!playerCollides && playerInRange && !hasPlayedSound)
                { // Spinner is triggered here
                    hasPlayedSound = true;
                    Add(new Coroutine(Grow(), true));
                }
                if (playerInRange && !expanded)
                    expanded = true;
            }
        }

        private void OnPlayerTouch(Player player)
        {
            if (isActivated)
                player.Die((player.Position - Position).SafeNormalize());
            else
                playerInRange = true;
        }

        private IEnumerator Grow()
        {
            playerInRange = false;
            spriteIndicator.Visible = false;
            spriteGrow.AddLoop("grow", "", 0.1f);
            spriteGrow.Play("grow", false, false);
            spriteGrow.CenterOrigin();
            yield return 0.3f;
            Audio.Play(sfx);
            yield return 0.1f;
            isActivated = true;
            spriteGrow.Visible = false;
            ActivatedSprite();
            yield return null;
        }

        private void ActivatedSprite()
        {
            if (!expanded)
                return;
            Calc.PushRandom(randomSeed);
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(GetTextureForColor(this.color));
            MTexture mTexture = Calc.Random.Choose(atlasSubtextures);
            Color spriteColor = (this.color == TriggerSpinnerColor.Rainbow) ? GetHue(Position) : Color.White;
            if (color != TriggerSpinnerColor.Custom)
            {
                if (!SolidCheck(new Vector2(base.X - 4f, base.Y - 4f)))
                    images.Add(new Image(mTexture.GetSubtexture(0, 0, 14, 14)).SetOrigin(12f, 12f).SetColor(spriteColor));
                if (!SolidCheck(new Vector2(base.X + 4f, base.Y - 4f)))
                    images.Add(new Image(mTexture.GetSubtexture(10, 0, 14, 14)).SetOrigin(2f, 12f).SetColor(spriteColor));
                if (!SolidCheck(new Vector2(base.X + 4f, base.Y + 4f)))
                    images.Add(new Image(mTexture.GetSubtexture(10, 10, 14, 14)).SetOrigin(2f, 2f).SetColor(spriteColor));
                if (!SolidCheck(new Vector2(base.X - 4f, base.Y + 4f)))
                    images.Add(new Image(mTexture.GetSubtexture(0, 10, 14, 14)).SetOrigin(12f, 2f).SetColor(spriteColor));
            }
            else // Custom Texture
                images.Add(new Image(mTexture.GetSubtexture(0, 0, 16, 16)).CenterOrigin().SetColor(spriteColor));
            foreach (Image image in images)
            {
                Add(image);
            }
            Calc.PopRandom();
        }

        private bool SolidCheck(Vector2 position)
        {
            foreach (Solid solid in base.Scene.CollideAll<Solid>(position))
            {
                if (solid is SolidTiles)
                    return true;
            }
            return false;
        }

        private string GetTextureForColor(TriggerSpinnerColor color)
        {
            switch (color)
            {
                case TriggerSpinnerColor.Blue:
                    return "danger/crystal/fg_blue";
                case TriggerSpinnerColor.Red:
                    return "danger/crystal/fg_red";
                case TriggerSpinnerColor.Purple:
                    return "danger/crystal/fg_purple";
                case TriggerSpinnerColor.Rainbow:
                    return "danger/crystal/fg_white";
                case TriggerSpinnerColor.Custom:
                    return customPathTriggered;
                default:
                    return "danger/crystal/fg_blue";
            }
        }

        private Color GetHue(Vector2 position)
        {
            float num = 280f;
            float value = (position.Length() + base.Scene.TimeActive * 50f) % num / num;
            return Calc.HsvToColor(0.4f + Calc.YoYo(value) * 0.4f, 0.4f, 0.9f);
        }
        private void OnShake(Vector2 pos)
        {
            foreach (Component component in base.Components)
            {
                if (component is Image image)
                    image.Position += pos;
            }
        }

        private bool IsRiding(Solid solid)
        {
            return CollideCheck(solid);
        }
    }
}