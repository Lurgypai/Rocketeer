using On.RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Rocketeer
{
    class Hooks
    {
        public static void hook()
        {
            CharacterBody.RecalculateStats += new CharacterBody.hook_RecalculateStats(RecalculateStats);
            GlobalEventManager.OnCharacterDeath += new GlobalEventManager.hook_OnCharacterDeath(OnCharacterDeath);
        }
        private static void RecalculateStats(CharacterBody.orig_RecalculateStats orig, RoR2.CharacterBody self)
        {
            orig.Invoke(self);

            if (self)
            {
                if (self.inventory)
                {
                    int count = self.inventory.GetItemCount(ContentPacks.shinyJetpackDef.itemIndex);
                    if (count != 0)
                    {
                        float amount = 0.75f;

                        float moveScale = self.moveSpeed / self.baseMoveSpeed;
                        float jumpScale = self.jumpPower / self.baseJumpPower;
                        if(moveScale > 1)
                        {
                            float extra = moveScale - 1;
                            float scaleAmount = 1 + (extra * ((count * amount) + 1));
                            self.GetType().GetProperty("moveSpeed").SetValue(self, self.moveSpeed * scaleAmount);
                        }
                        if (jumpScale > 1)
                        {
                            float extra = jumpScale - 1;
                            float scaleAmount = 1 + (extra * ((count * amount) + 1));
                            self.GetType().GetProperty("jumpPower").SetValue(self, self.jumpPower * scaleAmount);
                        }
                    }
                }
            }
        }

        private static void OnCharacterDeath(GlobalEventManager.orig_OnCharacterDeath orig, RoR2.GlobalEventManager self, RoR2.DamageReport report)
        {

            RoR2.CharacterBody victim = report.victimBody;
            //We need an inventory to do check for our item
            if (victim.inventory)
            {
                //store the amount of our item we have
                int jetpacks = victim.inventory.GetItemCount(ContentPacks.shinyJetpackDef.itemIndex);
                if (jetpacks > 0)
                {
                    if(NetworkServer.active)
                    {
                        float radius = 25;
                        float damageCoefficient = 100;

                        new RoR2.BlastAttack
                        {
                            attacker = victim.gameObject,
                            attackerFiltering = RoR2.AttackerFiltering.Default,
                            teamIndex = victim.teamComponent.teamIndex,
                            bonusForce = new Vector3(0, 50, 0),
                            baseForce = 0.0f,
                            damageColorIndex = RoR2.DamageColorIndex.Default,
                            inflictor = victim.gameObject,
                            losType = RoR2.BlastAttack.LoSType.None,
                            position = victim.transform.position,
                            damageType = RoR2.DamageType.Generic,
                            baseDamage = damageCoefficient * victim.damage * jetpacks,
                            procCoefficient = 1.0f,
                            radius = radius,
                            crit = victim.RollCrit(),
                            falloffModel = RoR2.BlastAttack.FalloffModel.None
                        }.Fire();

                        GameObject blastEffect1 = Resources.Load<GameObject>("Prefabs/Effects/Impacteffects/ExplosionVFX");
                        blastEffect1.transform.localScale = new Vector3(radius, radius, radius);
                    }
                }
            }
            orig.Invoke(self, report);
        }
    }
}
