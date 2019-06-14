using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will instantiate a random associated objects
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to instantiate the loot drops specified in its inspector, at the feedback's position (plus an optional offset). You can also optionally set an amount of force to launch the loot from the spawn point.")]
    [FeedbackPath("GameObject/Loot Drop")]
    public class MMFeedbackLootDrops : MMFeedback
    {
        public int minimumNumberOfLootToSpawn = 1;
        public int maximumNumberOfLootToSpawn = 1;

        [Header("Loot Objects")]
        public List<LootItemInfo> ListOfLootToInstantiate = new List<LootItemInfo>();
        private LootItemInfo LootToInstantiate;
        /// the position offset at which to instantiate the vfx object
        public Vector3 LootPositionOffset;

        protected MMMiniObjectPooler _objectPool;
        protected GameObject _newGameObject;


        private int LootObjectWeightTotal = 0;
        public float lootSpawnForceMaxRadius = 1;
        public float extraForceToPickup = 5;
        public float extraForceToPickupRadius = 5;
        public ForceMode forceMode = ForceMode.Impulse;

        /// <summary>
        /// On init we create an object pool if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);

            if (Active && (ListOfLootToInstantiate.Count > 0))
            {
                LootObjectWeightTotal = 0;
                foreach (var lootObject in ListOfLootToInstantiate)
                {
                    LootObjectWeightTotal += lootObject.weightedChance;
                }
            }
        }

        /// <summary>
        /// On Play we instantiate the specified object, either from the object pool or from scratch
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (ListOfLootToInstantiate.Count > 0))
            {
                int numberOfLootToSpawn = Random.Range(minimumNumberOfLootToSpawn, maximumNumberOfLootToSpawn);

                for (int lootNumber = 0; lootNumber < numberOfLootToSpawn; ++lootNumber)
                {
                    int randomNumber = Random.Range(0, LootObjectWeightTotal);

                    foreach (var lootObject in ListOfLootToInstantiate)
                    {
                        randomNumber -= lootObject.weightedChance;
                        if (randomNumber <= 0)
                        {
                            LootToInstantiate = lootObject;
                            break;
                        }
                    }

                    if (LootToInstantiate != null)
                    {
                        for (int i = 0; i < LootToInstantiate.amount; ++i)
                        {
                            _newGameObject = GameObject.Instantiate(LootToInstantiate.gameObject) as GameObject;
                            _newGameObject.transform.position = position + LootPositionOffset;
                            //set a random position spawn position
                            _newGameObject.transform.position += Random.insideUnitSphere * lootSpawnForceMaxRadius;

                            Rigidbody rb = _newGameObject.GetComponent<Rigidbody>();
                            if (rb != null)
                            {

                                //apply force to the loot objects
                                _newGameObject.GetComponent<Rigidbody>().AddExplosionForce(extraForceToPickup, transform.position, extraForceToPickupRadius, 1, forceMode);
                            }
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class LootItemInfo
    {
        public LootItemInfo()
        {
            amount = 1;
            weightedChance = 1;
        }

        public GameObject gameObject;
        public int amount;
        public int weightedChance;
    }
}
