using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace DestinyGuardiansMod.Survivors.Gunsligner.SkillStates
{
    public class ThrowBomb : GenericProjectileBaseState
    {
        public static float BaseDuration = 0.65f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float BaseDelayDuration = 0.0f;

        public static float DamageCoefficient = 16f;

        public override void OnEnter()
        {
            projectilePrefab = GunslingerAssets.bombProjectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = "HenryBombThrow";

            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = BaseDelayDuration;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = 80f;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            recoilAmplitude = 0.1f;
            bloom = 10;

            base.OnEnter();
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectileInfo(ref fireProjectileInfo);
            fireProjectileInfo.damageTypeOverride = DamageTypeCombo.GenericSpecial;
        }

        public override void FixedUpdate()
        {
            Log.Message("Fixed Upade");
            base.FixedUpdate();


        }

        public override void FireProjectile()
        {
			if (base.isAuthority)
			{
				Ray aimRay = aimRayOverride ?? GetAimRay();
				aimRay = ModifyProjectileAimRay(aimRay);
				aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
				FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
				{
					projectilePrefab = projectilePrefab,
					position = aimRay.origin,
					rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
					owner = base.gameObject,
					damage = damageStat * damageCoefficient,
					force = force,
					crit = Util.CheckRoll(critStat, base.characterBody.master),
					fuseOverride = (projectileFuseOverride ?? (-1f)),
					speedOverride = (projectileSpeedOverride ?? (-1f))
				};
                //fireProjectileInfo.passenger = new HealthComponent()
				ModifyProjectileInfo(ref fireProjectileInfo);
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void PlayAnimation(float duration)
        {

            if (GetModelAnimator())
            {
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration);
            }
        }
    }
}