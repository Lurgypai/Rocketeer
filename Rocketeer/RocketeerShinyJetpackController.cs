using Rocketeer;
using RoR2;
using RoR2.Stats;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Rocketeer
{
    class RocketeerShinyJetpackController : MonoBehaviour
    {
        private CharacterBody characterBody;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void Start()
        {
            if(NetworkServer.active)
            {
                TryAddItem();
            }
        }

        public void TryAddItem()
        {
            if (this.characterBody.master)
            {
                bool flag = false;
                if (this.characterBody.master.playerStatsComponent)
                {
                    flag = (this.characterBody.master.playerStatsComponent.currentStats.GetStatValueDouble(PerBodyStatDef.totalTimeAlive, BodyCatalog.GetBodyName(this.characterBody.bodyIndex)) > 0.0);
                }
                if (!flag && this.characterBody.master.inventory.GetItemCount(ContentPacks.shinyJetpackDef.itemIndex) <= 0)
                {
                    this.characterBody.master.inventory.GiveItem(ContentPacks.shinyJetpackDef.itemIndex);
                }
            }
        }
    }
}
