using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DestinyGuardiansMod.Survivors.Gunsligner.SkillStates
{
    public class Shoot : BaseSkillState
    {
        public static float damageCoefficient = GunslingerStaticValues.gunDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.5f;
        //delay on firing is usually ass-feeling. only set this if you know what you're doing
        public static float firePercentTime = 0.0f;
        public static float force = 800f;
        public static float recoil = 1f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private float duration;

        private float fireTime;
        private bool hasFired;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            characterBody.SetAimTimer(2f);
            muzzleString = "Muzzle";

            PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime)
            {
                Fire();
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void Fire()
        {
            if (!hasFired)
            {
                hasFired = true;

                characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, gameObject, muzzleString, false);
                Util.PlaySound("HenryShootPistol", gameObject);

				if (isAuthority)
                {
                    Ray aimRay = GetAimRay();
                    AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

					bool crit = RollCrit();
					int cranialSpikeStacks = characterBody.GetBuffCount(GunslingerBuffs.cranialSpikeBuff);

					DamageTypeCombo damageType;
					damageType = DamageTypeCombo.GenericPrimary;
					//if (characterBody.HasBuff(GunslingerBuffs.RadiantBuff))
					//{
					//    damageType = new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Primary);
					//}
					//else
					//{
					//    damageType = DamageTypeCombo.GenericPrimary;
					//}

					new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = (damageCoefficient + cranialSpikeStacks * GunslingerStaticValues.cranialSpikeDamageIncrease) * damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = damageType,
                        falloffModel = BulletAttack.FalloffModel.None,
                        maxDistance = range,
                        force = force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = crit,
                        owner = gameObject,
                        muzzleName = muzzleString,
                        smartCollision = true,
                        procChainMask = default,
                        procCoefficient = procCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet,
                        weapon = null,
                        tracerEffectPrefab = tracerEffectPrefab,
                        spreadPitchScale = 1f,
                        spreadYawScale = 1f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                    }.Fire();

                    //for troubleshooting only
                    //crit = true;
                    if (NetworkServer.active && crit && cranialSpikeStacks < 5)
					{
						characterBody.AddTimedBuff(GunslingerBuffs.cranialSpikeBuff, GunslingerStaticValues.cranialSpikeDuration);
						characterBody.SetTimedBuffDurationIfPresent(GunslingerBuffs.cranialSpikeBuff, 4f, true);
					}
                    else if(NetworkServer.active && crit)
                    {
                        characterBody.SetTimedBuffDurationIfPresent(GunslingerBuffs.cranialSpikeBuff, 4f, true);
                    }
				}
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}