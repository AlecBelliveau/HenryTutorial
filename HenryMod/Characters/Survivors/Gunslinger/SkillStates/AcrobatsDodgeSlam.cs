using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

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
			
			characterBody.AddTimedBuff(GunslingerBuffs.RadiantBuff, GunslingerStaticValues.AcrobatDodgeRadiantDuration);
			outer.SetNextStateToMain();
		}

	}
}