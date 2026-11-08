using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

namespace DestinyGuardiansMod.Survivors.Gunsligner.SkillStates
{
	public class AcrobatsDodgeSlam : BaseSkillState
	{
		private float duration = 10f;
		private float extraAcceleration = 0.5f;
		private bool detonateNextFrame = false;

		public override void OnEnter()
		{
			base.OnEnter();
			characterMotor.isAirControlForced = false;
			characterMotor.disableAirControlUntilCollision = false;
			characterMotor.onHitGroundAuthority += onHitGroundAuthority;

			if (NetworkServer.active)
			{
				base.characterBody.AddTimedBuff(JunkContent.Buffs.IgnoreFallDamage, 10f, 1);
			}
		}


		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterMotor.velocity.y -= extraAcceleration;

			if(!base.isAuthority) { return; }

			if ((bool)base.characterMotor)
			{
				base.characterMotor.moveDirection = base.inputBank.moveVector;
			}

			if (fixedAge >= duration || detonateNextFrame)
			{
				EndDodge();
				return;
			}
		}

		public override void OnExit()
		{
			if (cameraTargetParams) cameraTargetParams.fovOverride = -1f;
			base.OnExit();

			characterMotor.isAirControlForced = false;
			characterMotor.disableAirControlUntilCollision = false;
			characterBody.RemoveBuff(JunkContent.Buffs.IgnoreFallDamage);
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
		}

		private void onHitGroundAuthority(ref CharacterMotor.HitGroundInfo hitGroundInfo)
		{
			detonateNextFrame = true;
		}

		private void EndDodge()
		{

			ApplyRadiant();
			ApplyFire();

			outer.SetNextStateToMain();
		}


		private void ApplyRadiant()
		{
			//the code below applies to self so this line is no longer needed
			//characterBody.AddTimedBuff(GunslingerBuffs.RadiantBuff, GunslingerStaticValues.AcrobatsDodgeRadiantDuration);

			TeamComponent[] array = UnityEngine.Object.FindObjectsOfType<TeamComponent>();
			for (int i = 0; i < array.Length; i++)
			{
				//if not on the same team
				if (array[i].teamIndex != teamComponent.teamIndex) { continue; }

				Vector3 diff = array[i].GetComponent<CharacterBody>().corePosition - characterBody.corePosition;
				//if not within range
				if (diff.magnitude > Gunsligner.GunslingerStaticValues.AcrobatsDodgeSlamRadius) { continue; }

				//Apply radiant if both are true
				array[i].GetComponent<CharacterBody>().AddTimedBuff(GunslingerBuffs.RadiantBuff, GunslingerStaticValues.AcrobatsDodgeRadiantDuration);
			}


		}

		private void ApplyFire()
		{
			//Fire Effect
			EffectManager.SpawnEffect(GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab, new EffectData
			{
				origin = characterBody.corePosition,
				scale = 25f,
				rotation = Util.QuaternionSafeLookRotation(GetAimRay().direction)
			}, transmit: true);

			//Fire Damage
			Vector3 corePosition = characterBody.corePosition;
			GlobalEventManager.igniteOnKillSphereSearch.origin = corePosition;
			GlobalEventManager.igniteOnKillSphereSearch.mask = LayerIndex.entityPrecise.mask;
			GlobalEventManager.igniteOnKillSphereSearch.radius = GunslingerStaticValues.AcrobatsDodgeSlamRadius;
			GlobalEventManager.igniteOnKillSphereSearch.RefreshCandidates();
			GlobalEventManager.igniteOnKillSphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(characterBody.teamComponent.teamIndex));
			GlobalEventManager.igniteOnKillSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
			GlobalEventManager.igniteOnKillSphereSearch.OrderCandidatesByDistance();
			GlobalEventManager.igniteOnKillSphereSearch.GetHurtBoxes(GlobalEventManager.igniteOnKillHurtBoxBuffer);
			GlobalEventManager.igniteOnKillSphereSearch.ClearCandidates();

			float value = (float)(1 + 1) * 0.75f * characterBody.damage;
			for (int i = 0; i < GlobalEventManager.igniteOnKillHurtBoxBuffer.Count; i++)
			{
				HurtBox hurtBox = GlobalEventManager.igniteOnKillHurtBoxBuffer[i];
				if ((bool)hurtBox.healthComponent)
				{
					InflictDotInfo dotInfo = new InflictDotInfo
					{
						victimObject = hurtBox.healthComponent.gameObject,
						attackerObject = characterBody.gameObject,
						totalDamage = value,
						dotIndex = DotController.DotIndex.Burn,
						damageMultiplier = 1f,
						hitHurtBox = hurtBox,
					};

					DotController.InflictDot(ref dotInfo);
				}
			}

			GlobalEventManager.igniteOnKillHurtBoxBuffer.Clear();
			BlastAttack blastAttack = new BlastAttack();
			blastAttack.radius = GunslingerStaticValues.AcrobatsDodgeSlamRadius;
			blastAttack.baseDamage = GunslingerStaticValues.AcrobatsDogdgeBaseDamage;
			blastAttack.procCoefficient = 0f;
			blastAttack.crit = characterBody.RollCrit();
			blastAttack.damageColorIndex = DamageColorIndex.Item;
			blastAttack.attackerFiltering = AttackerFiltering.Default;
			blastAttack.falloffModel = BlastAttack.FalloffModel.None;
			blastAttack.attacker = characterBody.gameObject;
			blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
			blastAttack.position = corePosition;
			blastAttack.Fire();
		}

	}
}