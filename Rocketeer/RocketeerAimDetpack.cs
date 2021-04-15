using EntityStates.Huntress;
using EntityStates.Toolbot;
using Rocketeer;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace EntityStates.RocketeerStates
{
	// Token: 0x020008F6 RID: 2294
	public class RocketeerAimDetpack : BaseSkillState
	{

		private float airTime = 0.5f;
		private float gt = (0.5f * -Physics.gravity.y * (0.5f * 0.5f));
		private float maxThrowDistance = 30.0f;
		private float force = 1.0f;

		private float startTime = 0.0f;
		private float baseDuration = 0.5f;
		private float duration;

		private bool thrown = false;
		private bool tryThrow = false;

		GameObject targetPrefab;

		private float baseDamage = 6f;
		public override void OnEnter()
		{
			base.OnEnter();
			if(base.isAuthority)
            {
				this.targetPrefab = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
				this.targetPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				this.duration = this.baseDuration / this.attackSpeedStat;
				this.thrown = false;
				this.tryThrow = false;
            }
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
			if(!base.inputBank.skill2.down)
            {
				tryThrow = true;
            }
			if(tryThrow)
            {
				this.throwGrenade();
            }

			if (targetPrefab)
			{
				Ray aimRay = base.GetAimRay();
				RaycastHit raycastHit;
				bool flag2 = Physics.Raycast(aimRay, out raycastHit, maxThrowDistance, LayerIndex.CommonMasks.bullet);
				if (flag2)
				{
					this.targetPrefab.transform.position = raycastHit.point;
					this.targetPrefab.transform.up = raycastHit.normal;
				}
				else
				{
					this.targetPrefab.transform.position = aimRay.GetPoint(maxThrowDistance);
					this.targetPrefab.transform.up = -aimRay.direction;
				}
			}
		}

        public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

        public override void OnExit()
		{
			base.OnExit();
		}

		private void throwGrenade()
        {
			if (base.isAuthority)
			{
				if (!thrown)
				{
					thrown = true;
					this.startTime = base.fixedAge;

					Ray aimRay = base.GetAimRay();
					RaycastHit raycastHit;
					bool flag2 = Physics.Raycast(aimRay, out raycastHit, maxThrowDistance, LayerIndex.CommonMasks.bullet);
					Vector3 targetPos = new Vector3(0.0f, 0.0f, 0.0f);
					if (flag2)
					{
						targetPos += raycastHit.point;
					}
					else
					{
						targetPos += aimRay.GetPoint(maxThrowDistance);
					}

					float yVel = (targetPos.y - aimRay.origin.y + gt) / airTime;
					Vector3 targetVel = (targetPos - aimRay.origin) / airTime;

					targetVel.y = yVel;

					ProjectileManager.instance.FireProjectile(ContentPacks.detPack,
						aimRay.origin,
						Util.QuaternionSafeLookRotation(targetVel.normalized),
						base.gameObject,
						this.baseDamage * base.damageStat,
						this.force,
						base.RollCrit(),
						DamageColorIndex.Default,
						null,
						targetVel.magnitude);


					EntityState.Destroy(this.targetPrefab);
					this.targetPrefab = null;
				}

				if (base.fixedAge - this.startTime >= this.duration)
				{
					this.outer.SetNextStateToMain();
				}
			}
		}
	}

}
