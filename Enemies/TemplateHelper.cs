using System;
using System.Collections.Generic;
using Gatekeeper.Enemy.Base.Data;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.PoolScripts;
using Gatekeeper.Utility.SceneHelper;
using RNGNeeds;

namespace GKAPI.Enemies;

public class TemplateHelper
{
    public static List<SceneDesignation> AuroraScenes { get; } = [
        SceneDesignation.AuroraAltarOfTheAwakened,
        SceneDesignation.AuroraEdgeOfTheContinent,
        SceneDesignation.AuroraLandOfTheAncients,
        SceneDesignation.AuroraRunewood,
    ];
    public static List<SceneDesignation> CelestiumScenes { get; } = [
        SceneDesignation.CelestiumArcaneOrbitals,
        SceneDesignation.CelestiumEnshrinedExpanse,
        SceneDesignation.CelestiumEtherealBridge,
        SceneDesignation.CelestiumTheConfluxHall,
    ];
    public static List<SceneDesignation> PurgatoryScenes { get; } = [
        SceneDesignation.Purgatory17AbodeOfAshes,
        SceneDesignation.Purgatory17BridgeOfTheDamned,
        SceneDesignation.Purgatory17HauntingEnclave,
        SceneDesignation.Purgatory17WellOfSouls,
    ];
    public static List<SceneDesignation> AriduneScenes { get; } = [
        SceneDesignation.AriduneBarrenBasin,
        SceneDesignation.AriduneCursedCrossing,
        SceneDesignation.AriduneOasis,
        SceneDesignation.AriduneThePit,
    ];
    public static List<SceneDesignation> PaliumScenes { get; } = [
        SceneDesignation.PaliumBroodsCrest,
        SceneDesignation.PaliumEldenTree,
        SceneDesignation.PaliumForgottenDepths,
        SceneDesignation.PaliumTheMaw,
    ];
    
    //TODO move to general helper and use generics
    public class ProbListBuilder
    {
        private ProbabilityList<EnemyCharacterInfo> _list = new();

        public ProbListBuilder AddItem(PoolItemID id, float probability)
        {
            _list.AddItem(DatabaseInfoProvider.Enemies.GetCharacterInfoByPoolID(id), probability);
            return this;
        }

        public ProbabilityList<EnemyCharacterInfo> Build()
        {
            float sum = 0;
            foreach (var item in _list.ProbabilityItems)
            {
                sum += item.Probability;
            }
            
            if (Math.Abs(sum - 1.0) > 0.001)
                Plugin.Log.LogWarning("Probabilities of current Template does not add up to 1");
            
            return _list;
        }
    }
}