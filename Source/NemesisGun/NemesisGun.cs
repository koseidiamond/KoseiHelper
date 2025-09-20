using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    public class NemesisGun : EverestModule
    {
        public static NemesisGun Instance;

        public NemesisGun()
        {
            Instance = this;
        }

        public static FieldInfo seekerDead;
        public static FieldInfo oshiroStateMachine;
        public static FieldInfo oshiroPreChargeSFX;
        public static FieldInfo oshiroChargeSFX;
        public static FieldInfo dashSwitchPressDirection;
        public static FieldInfo bumperFireMode;
        public static FieldInfo bumperRespawnTimer;
        public static FieldInfo bumperSprite;
        public static FieldInfo bumperSpriteEvil;
        public static FieldInfo bumperLight;
        public static FieldInfo bumperBloom;

        public static MethodInfo seekerGotBouncedOn;
        public static MethodInfo heartGemCollect;
        public static MethodInfo strawberrySeedOnPlayer;
        public static MethodInfo summitGemSmashRoutine;
        public static MethodInfo pufferExplode;
        public static MethodInfo pufferGotoGone;
        public static MethodInfo birdGemCollect;

        private static Vector2 CursorPos => GunInput.CursorPosition;
        private bool GunWasShot => GunInput.GunShot;
        private MTexture gunTexture;
        private int shotCooldown, recoilCooldown;
        public Level level;

        private static Vector2 GetEightDirectionalAim(KoseiHelperModuleSettings.NemesisSettings.GunDirections gunDirections)
        {
            Vector2 value = Input.Aim.Value;
            if (value == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            float angle = value.Angle();
            float angleThreshold = (float)Math.PI / 8f;
            /*if (angle < 0)
            {
                angleThreshold -= Calc.ToRad(5f);
            }*/
            switch (gunDirections)
            {
                case KoseiHelperModuleSettings.NemesisSettings.GunDirections.Horizontal:
                    if (Calc.AbsAngleDiff(angle, 0f) < angleThreshold)
                    {
                        return new Vector2(1f, 0f);
                    }
                    else
                    {
                        return new Vector2(-1f, 0f);
                    }
                case KoseiHelperModuleSettings.NemesisSettings.GunDirections.EightDirections:
                    if (Calc.AbsAngleDiff(angle, 0f) < angleThreshold)
                    {
                        return new Vector2(1f, 0f);
                    }
                    else if (Calc.AbsAngleDiff(angle, (float)Math.PI) < angleThreshold)
                    {
                        return new Vector2(-1f, 0f);
                    }
                    else if (Calc.AbsAngleDiff(angle, -(float)Math.PI / 2f) < angleThreshold)
                    {
                        return new Vector2(0f, -1f);
                    }
                    else if (Calc.AbsAngleDiff(angle, (float)Math.PI / 2f) < angleThreshold)
                    {
                        return new Vector2(0f, 1f);
                    }
                    // Diagonal directions
                    else
                    {
                        if (Calc.AbsAngleDiff(angle, (float)Math.PI / 4) < angleThreshold)
                        {
                            return new Vector2(-1f, -1f);
                        }
                        else if (Calc.AbsAngleDiff(angle, 3 * (float)Math.PI / 4) < angleThreshold)
                        {
                            return new Vector2(1f, -1f);
                        }
                        else if (Calc.AbsAngleDiff(angle, -3 * (float)Math.PI / 4) < angleThreshold)
                        {
                            return new Vector2(1f, 1f);
                        }
                        else if (Calc.AbsAngleDiff(angle, (float)-Math.PI / 4) < angleThreshold)
                        {
                            return new Vector2(-1f, 1f);
                        }
                        else
                        {
                            return new Vector2(Math.Sign(value.X), Math.Sign(value.Y)).SafeNormalize();
                        }
                    }
                default: // Four directions
                    if (Calc.AbsAngleDiff(angle, 0f) < angleThreshold)
                    {
                        return new Vector2(1f, 0f);
                    }
                    else if (Calc.AbsAngleDiff(angle, (float)Math.PI) < angleThreshold)
                    {
                        return new Vector2(-1f, 0f);
                    }
                    else if (Calc.AbsAngleDiff(angle, -(float)Math.PI / 2f) < angleThreshold)
                    {
                        return new Vector2(0f, -1f);
                    }
                    else if (Calc.AbsAngleDiff(angle, (float)Math.PI / 2f) < angleThreshold)
                    {
                        return new Vector2(0f, 1f);
                    }
                    break;
            }
            return Vector2.Zero;
        }


        private static Vector2 GetGunVector(Actor player, Vector2 cursorPos, Facings forceDir)
        {
            float rotation = 3 * MathHelper.Pi / 2;
            Vector2 aim = GetEightDirectionalAim(KoseiHelperModule.Settings.GunSettings.gunDirections);
            if (forceDir == Facings.Right)
                rotation = 0;
            else if (forceDir == Facings.Left)
                rotation = MathHelper.Pi;

            if (aim.X > 0.4)
                rotation = 0;
            if (aim.X < -0.4)
                rotation = MathHelper.Pi;
            if (!(player.OnGround()))
            {
                if (aim.Y > 0.4)
                    rotation = MathHelper.Pi / 2;
            }

            if (aim.Y < -0.4)
                rotation = 3 * MathHelper.Pi / 2;
            if (KoseiHelperModule.Settings.GunSettings.gunDirections == KoseiHelperModuleSettings.NemesisSettings.GunDirections.EightDirections)
            {
                if (Calc.AbsAngleDiff(aim.Angle(), (float)Math.PI / 4) < MathHelper.Pi / 8)
                    rotation = (float)Math.PI / 4 - (float)Math.PI;
                else if (Calc.AbsAngleDiff(aim.Angle(), 3 * (float)Math.PI / 4) < MathHelper.Pi / 8)
                    rotation = 3 * (float)Math.PI / 4 - (float)Math.PI;
                else if (Calc.AbsAngleDiff(aim.Angle(), -3 * (float)Math.PI / 4) < MathHelper.Pi / 8)
                    rotation = -3 * (float)Math.PI / 4 - (float)Math.PI;
                else if (Calc.AbsAngleDiff(aim.Angle(), (float)-Math.PI / 4) < MathHelper.Pi / 8)
                    rotation = (float)-Math.PI / 4 - (float)Math.PI;

            }
            return ToCursor(player, cursorPos).RotateTowards(rotation, MathHelper.TwoPi) * KoseiHelperModule.Settings.GunSettings.SpeedMultiplier * Engine.TimeRate * Engine.TimeRateB;
        }

        public override void LoadContent(bool firstLoad)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            Type type = typeof(Seeker);

            seekerDead = type.GetField("dead", flags);
            seekerGotBouncedOn = type.GetMethod("GotBouncedOn", flags);

            type = typeof(AngryOshiro);

            oshiroStateMachine = type.GetField("state", flags);
            oshiroPreChargeSFX = type.GetField("prechargeSfx", flags);
            oshiroChargeSFX = type.GetField("chargeSfx", flags);

            heartGemCollect = typeof(HeartGem).GetMethod("Collect", flags);

            dashSwitchPressDirection = typeof(DashSwitch).GetField("pressDirection", flags);

            strawberrySeedOnPlayer = typeof(StrawberrySeed).GetMethod("OnPlayer", flags);

            type = typeof(Bumper);

            bumperFireMode = type.GetField("fireMode", flags);
            bumperRespawnTimer = type.GetField("respawnTimer", flags);
            bumperSprite = type.GetField("sprite", flags);
            bumperSpriteEvil = type.GetField("spriteEvil", flags);
            bumperLight = type.GetField("light", flags);
            bumperBloom = type.GetField("bloom", flags);

            summitGemSmashRoutine = typeof(SummitGem).GetMethod("SmashRoutine", flags);

            type = typeof(Puffer);
            pufferExplode = type.GetMethod("Explode", flags);
            pufferGotoGone = type.GetMethod("GotoGone", flags);
            gunTexture = GFX.Game[Extensions.gunTexture];
        }

        public override void OnInputInitialize() => GunInput.RegisterInputs();

        public override void OnInputDeregister() => GunInput.DeregisterInputs();

        public override void Load()
        {
            On.Celeste.Player.Update += PlayerUpdated;
            On.Celeste.Player.Render += PlayerRendered;

            Everest.Events.Level.OnLoadLevel += (level, playerIntro, isFromLoader) =>
            {
                this.level = level;

                GunInput.CursorPosition = new Vector2(1920 / 2, 1080 / 2);
            };
        }

        public override void Unload()
        {
            seekerDead = null;
            seekerGotBouncedOn = null;
            oshiroStateMachine = null;
            oshiroPreChargeSFX = null;
            oshiroChargeSFX = null;
            heartGemCollect = null;
            dashSwitchPressDirection = null;
            strawberrySeedOnPlayer = null;
            bumperFireMode = null;
            bumperRespawnTimer = null;
            bumperSprite = null;
            bumperSpriteEvil = null;
            bumperLight = null;
            bumperBloom = null;
            summitGemSmashRoutine = null;
            pufferExplode = null;
            pufferGotoGone = null;
            birdGemCollect = null;

            On.Celeste.Player.Update -= PlayerUpdated;
            On.Celeste.Player.Render -= PlayerRendered;
        }

#pragma warning disable CL0004
        private void PlayerUpdated(On.Celeste.Player.orig_Update orig, Player self)
        {
            Session session = self.SceneAs<Level>().Session;
            if (self.JustRespawned && Extensions.loseGunOnRespawn)
            {
                session.SetFlag("EnableNemesisGun", false);
                KoseiHelperModule.Settings.GunSettings.GunEnabled = false;
            }
            if (session.GetFlag("EnableNemesisGun") || KoseiHelperModule.Settings.GunSettings.GunEnabled)
            {
                GunInput.UpdateInput(self);
                if (shotCooldown > 0)
                    shotCooldown--;
                if (recoilCooldown > 0 && !KoseiHelperModule.Settings.GunSettings.RecoilOnlyOnInteraction)
                    recoilCooldown--;

                if (self.Scene?.TimeActive > 0 && GunWasShot && (TalkComponent.PlayerOver == null || !Input.Talk.Pressed))
                {
                    if (shotCooldown <= 0 &&
                        (self.Dashes > 0 || KoseiHelperModule.Settings.GunSettings.dashBehavior != KoseiHelperModuleSettings.NemesisSettings.DashBehavior.ConsumesDash))
                    {
                        if (self.StateMachine.state != 11 && self.StateMachine.state != 17 &&
                            (self.StateMachine.state != 19 || KoseiHelperModule.Settings.GunSettings.CanShootInFeather))
                            Gunshot(self, CursorPos);
                        if ((recoilCooldown <= 0 || Extensions.recoilingOnInteraction) && !KoseiHelperModule.Settings.GunSettings.MachineGunMode)
                        {
                            if (GetEightDirectionalAim(KoseiHelperModule.Settings.GunSettings.gunDirections).Y < Math.Sqrt(2) / 2 &&
                            GetEightDirectionalAim(KoseiHelperModule.Settings.GunSettings.gunDirections).Y > -Math.Sqrt(2) / 2 &&
                            !KoseiHelperModule.Settings.GunSettings.RecoilUpwards)
                            {
                                if (self.StateMachine != 1 || (self.StateMachine == 1 && KoseiHelperModule.Settings.GunSettings.RecoilWhileClimbing))
                                    self.Speed.X += KoseiHelperModule.Settings.GunSettings.Recoil * (float)(0 - self.Facing); // Horizontal recoil, by default 80f, same as vanilla backboosts
                            }
                            else if (KoseiHelperModule.Settings.GunSettings.RecoilUpwards)
                                self.Speed.Y -= KoseiHelperModule.Settings.GunSettings.Recoil;
                            // Recoil for all player clones and such (does not work anyways lol)
                            foreach (Entity entity in self.level.Entities)
                            {
                                if (entity is Player extraPlayer && extraPlayer != self)
                                {
                                    if (GetEightDirectionalAim(KoseiHelperModule.Settings.GunSettings.gunDirections).Y < Math.Sqrt(2) / 2 &&
                            GetEightDirectionalAim(KoseiHelperModule.Settings.GunSettings.gunDirections).Y > -Math.Sqrt(2) / 2 && !KoseiHelperModule.Settings.GunSettings.RecoilUpwards)
                                        // Horizontal recoil, by default 80f, same as vanilla backboosts
                                        extraPlayer.Speed.X += KoseiHelperModule.Settings.GunSettings.Recoil * (float)(0 - extraPlayer.Facing);
                                    else if (KoseiHelperModule.Settings.GunSettings.RecoilUpwards)
                                        extraPlayer.Speed.Y -= KoseiHelperModule.Settings.GunSettings.Recoil;
                                }
                            }
                            recoilCooldown = KoseiHelperModule.Settings.GunSettings.RecoilCooldown;
                            Extensions.recoilingOnInteraction = false;
                        }
                        Celeste.Freeze(Engine.DeltaTime * KoseiHelperModule.Settings.GunSettings.FreezeFrames);
                        (self.level).DirectionalShake(GetGunVector(self, CursorPos, self.Facing) / 5);
                        shotCooldown = KoseiHelperModule.Settings.GunSettings.Cooldown;
                        switch (KoseiHelperModule.Settings.GunSettings.dashBehavior)
                        {
                            case KoseiHelperModuleSettings.NemesisSettings.DashBehavior.ConsumesDash:
                                if (self.Dashes > 0)
                                    self.Dashes -= 1;
                                break;
                            case KoseiHelperModuleSettings.NemesisSettings.DashBehavior.None:
                                break;
                            default:
                                Input.Dash.ConsumePress();
                                Input.CrouchDash.ConsumePress();
                                break;
                        }
                    }
                    if (KoseiHelperModule.Settings.GunSettings.dashBehavior == KoseiHelperModuleSettings.NemesisSettings.DashBehavior.ReplacesDash)
                    {
                        Input.Dash.ConsumePress();
                        Input.CrouchDash.ConsumePress();
                    }
                }
            }

            if ((self.JustRespawned || self.IsIntroState) && Extensions.loseGunOnRespawn)
            {
                session.SetFlag("EnableNemesisGun", false);
            }
            orig(self);
        }

        private void PlayerRendered(On.Celeste.Player.orig_Render orig, Player self)
        {
            orig(self);
            Session session = self.SceneAs<Level>().Session;
            if (session.GetFlag("EnableNemesisGun") || KoseiHelperModule.Settings.GunSettings.GunEnabled)
            {
                RenderGun(self, self.Facing);
            }
        }

        private float lastGunRotation = 0f;
        private void RenderGun(Actor player, Facings facing, Vector2? overrideCursorPos = null)
        {
            gunTexture = GFX.Game[Extensions.gunTexture];
            Vector2 gunVector = Vector2.Zero;
            if (!player.SceneAs<Level>().Paused)
            {
                gunVector = GetGunVector(player, overrideCursorPos == null ? CursorPos : (Vector2)overrideCursorPos, facing);
                lastGunRotation = gunVector.ToRotation();
            }
            else
                gunVector = new Vector2((float)Math.Cos(lastGunRotation), (float)Math.Sin(lastGunRotation));
            SpriteEffects effects = gunVector.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (player.SceneAs<Level>().Session.GetFlag("KoseiHelper_NemesisGunDrawOutline"))
                gunTexture.DrawOutline(player.Center, new Vector2(gunTexture.Width / 2, gunTexture.Height / 2), Color.Black, 1, lastGunRotation, effects);
            if (KoseiHelperModule.Settings.GunSettings.CanShootInFeather || (player as Player).StateMachine.state != 19) // make it invisible if the feather can't use it
                gunTexture.DrawCentered(player.Center, Color.White, 1, lastGunRotation, effects);

        }

        private static Vector2 PlayerPosScreenSpace(Actor self)
            => self.Center - (self.Scene as Level).Camera.Position;

        private static Vector2 ToCursor(Actor player, Vector2 cursorPos)
            => Vector2.Normalize((cursorPos / 6) - PlayerPosScreenSpace(player));

        public static void Gunshot(Actor actor, Vector2 cursorPos, Facings facing = Facings.Left)
        {
            if (actor == null || actor.Scene == null)
                return;
            Vector2 actualPlayerPos = actor.Center + new Vector2(-3, -1);
            if (actor is Player player)
                facing = player.Facing;
            // This was supposed to support player clones but it breaks some interactions (like dash blocks) :(
            /*foreach (Entity entity in (actor.Scene as Level).Entities)
            {

                if (entity is Player extraPlayer)
                    new Bullet(extraPlayer.Center + new Vector2(-3, -1), GetGunVector(actor, cursorPos, facing), actor);
            }*/
            new Bullet(actualPlayerPos, GetGunVector(actor, cursorPos, facing), actor);
            Audio.Play(Extensions.gunshotSound, actualPlayerPos);
        }
    }
}