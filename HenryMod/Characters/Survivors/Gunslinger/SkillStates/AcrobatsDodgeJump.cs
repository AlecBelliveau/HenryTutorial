using EntityStates;
using DestinyGuardiansMod.Survivors.Gunsligner;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DestinyGuardiansMod.Survivors.Gunsligner.SkillStates
{
	public class AcrobatsDodgeJump : BaseSkillState
	{
		public static float duration = 0.7f;
		public static float initialSpeedCoefficient = 5f;
		public static float finalSpeedCoefficient = 2.5f;
		public static float leapSpeed = 25f;

		public static string dodgeSoundString = "HenryRoll";
		public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;

		private float rollSpeed;
		private Vector3 forwardDirection;
		private Animator animator;
		private Vector3 previousPosition;

		private Vector3 jumpVector;
		private float yPower;
		private Vector3 jumpAngle;
		public static float jumpPower = 30f;
		public static float minYPower = 0.7f;

		public override void OnEnter()
		{
			base.OnEnter();
			//I stole this from the Sheriff mod because I am an inept coder
			jumpVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector);
			base.characterDirection.forward = jumpVector;
			Ray aimRay = GetAimRay();
			yPower = Mathf.Max(minYPower, aimRay.direction.y);
			jumpAngle = new Vector3(0f, yPower, 0f);
			jumpVector += jumpAngle;
			if (base.isAuthority)
			{
				base.characterDirection.forward = jumpVector;
				base.characterBody.isSprinting = true;
			}
			base.characterMotor.Motor.ForceUnground();
			base.characterMotor.velocity.y = 0f;
			base.characterMotor.velocity += jumpVector * jumpPower;
		}

		private void RecalculateRollSpeed()
		{
			rollSpeed = moveSpeedStat * Mathf.SmoothStep(initialSpeedCoefficient, finalSpeedCoefficient, (fixedAge / duration) / 2f);
			//rollSpeed = moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, fixedAge / duration);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			//float ySpeed = characterMotor.velocity.y;

			if (isAuthority && fixedAge >= duration)
			{
				outer.SetNextState(new AcrobatsDodgeSlam());
				return;
			}
		}

		public override void OnExit()
		{
			if (cameraTargetParams) cameraTargetParams.fovOverride = -1f;
			base.OnExit();

			characterMotor.isAirControlForced = false;
			characterMotor.disableAirControlUntilCollision = false;
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(forwardDirection);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			forwardDirection = reader.ReadVector3();
		}
	}
}