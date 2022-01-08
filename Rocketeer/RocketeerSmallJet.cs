using EntityStates;
using EntityStates.Commando;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rocketeer
{
    class RocketeerSmallJet : BaseSkillState
    {
        private bool canJet;
        private float jetAmount = 10;

        public void Awake()
        {
            this.canJet = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            canJet = true;
        }

        public override void FixedUpdate()
        {
            if (canJet)
            {
                if(this.characterBody.characterMotor.isGrounded)
                {
                    canJet = false;
                    return;
                }
                if (this.characterBody.inputBank.skill3.down && !this.characterBody.inputBank.skill3.wasDown)
                {
                    canJet = false;
                    this.characterBody.characterMotor.velocity.y = jetAmount;
                    if (DodgeState.jetEffect)
                    {

                        Util.PlaySound(DodgeState.dodgeSoundString, base.gameObject);
                        ChildLocator component = this.GetModelAnimator().GetComponent<ChildLocator>();
                        if (component)
                        {
                            Transform transform = component.FindChild("LeftJet");
                            Transform transform2 = component.FindChild("RightJet");
                            if (transform)
                            {
                                UnityEngine.Object.Instantiate<GameObject>(DodgeState.jetEffect, transform);
                            }
                            if (transform2)
                            {
                                UnityEngine.Object.Instantiate<GameObject>(DodgeState.jetEffect, transform2);
                            }
                        }
                    }
                }
            }
        }

    }
}
