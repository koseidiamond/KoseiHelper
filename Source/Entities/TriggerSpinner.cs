using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/TriggerSpinner")]
    [Tracked]
    public class TriggerSpinner : Entity
    {
        public CrystalColor color;
        public bool attachToSolid;
        private int randomSeed;
        private bool expanded;
        private bool playerInRange;
        private bool isActivated;
        private List<Image> images = new List<Image>();

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
            color = data.Enum("color", CrystalColor.Blue);
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
                else if (!playerCollides && playerInRange)
                {
                    playerInRange = false;
                    isActivated = true;
                    CreateSprites();
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

        private void CreateSprites()
        {
            if (!expanded) return;
            Calc.PushRandom(randomSeed);
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(GetTextureForColor(this.color));
            MTexture mTexture = Calc.Random.Choose(atlasSubtextures);
            Color spriteColor = (this.color == CrystalColor.Rainbow) ? GetHue(Position) : Color.White;
            if (!SolidCheck(new Vector2(base.X - 4f, base.Y - 4f)))
            {
                images.Add(new Image(mTexture.GetSubtexture(0, 0, 14, 14)).SetOrigin(12f, 12f).SetColor(spriteColor));
            }
            if (!SolidCheck(new Vector2(base.X + 4f, base.Y - 4f)))
            {
                images.Add(new Image(mTexture.GetSubtexture(10, 0, 14, 14)).SetOrigin(2f, 12f).SetColor(spriteColor));
            }
            if (!SolidCheck(new Vector2(base.X + 4f, base.Y + 4f)))
            {
                images.Add(new Image(mTexture.GetSubtexture(10, 10, 14, 14)).SetOrigin(2f, 2f).SetColor(spriteColor));
            }
            if (!SolidCheck(new Vector2(base.X - 4f, base.Y + 4f)))
            {
                images.Add(new Image(mTexture.GetSubtexture(0, 10, 14, 14)).SetOrigin(12f, 2f).SetColor(spriteColor));
            }
            foreach (var image in images)
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

        private string GetTextureForColor(CrystalColor color)
        {
            switch (color)
            {
                case CrystalColor.Blue: return "danger/crystal/fg_blue";
                case CrystalColor.Red: return "danger/crystal/fg_red";
                case CrystalColor.Purple: return "danger/crystal/fg_purple";
                case CrystalColor.Rainbow: return "danger/crystal/fg_white";
                default: return "danger/crystal/fg_blue";
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
                {
                    image.Position += pos;
                }
            }
        }

        private bool IsRiding(Solid solid)
        {
            return CollideCheck(solid);
        }
    }
}