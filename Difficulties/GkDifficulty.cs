

using Gatekeeper.Elites;
using Gatekeeper.General;
using RNGNeeds;
using UnityEngine;

namespace GKAPI.Difficulties;

public class GkDifficulty
{
    public string DifficultyName { get; private set; }
    public string TranslationKey { get; private set; }
    public DifficultyValues DifficultyValues { get; private set; }
    
    public GameEventData GameEventData { get; private set; }
    public SirenSpawnData SirenSpawnData { get; private set; }
    public EliteSpawnData ElitesData { get; private set; }
    
    public float InstabilityCapsuleSpeed { get; private set; }
    
    public Color BackgroundColor { get; private set; }
    public Color CheckmarkColor { get; private set; }
    public float EnemyExpLoopPow {get; private set;}
    public float EnemyExpPointsPerEvo {get; private set;}
    public float EnemyExpPointsPerLoop {get; private set;}
    public float EnemyExpPointsPerTimeTick {get; private set;}
    public Arena ArenaValue { get; private set; }
    

    public class Builder
    {
        private string _difficultyName = "Any";
        
        private DifficultyValues _difficultyValues = new DifficultyValues()
        {
            difficultyMultiplier = 0f,
            prismMultiplier = 1f,
            stringPercentage = "any%",
        };
        
        private Color _backgroundColor = Color.white;
        private Color _checkmarkColor = Color.white;
        private float _expLoopPow = 0f;
        private float _expPointsPerEvo = 0f;
        private float _expPointsPerLoop = 0f;
        private float _expPointsPerTimeTick = 0f;

        private GameEventData _gameEventData = new()
        {
            maxSkipLevels = 4,
            minLevel = 6,
            templateStartIndex = 0,
            eventsProbability = DifficultiesAPI.Instance.CreateGameEventsProbabilities(0.2f),
        };
        
        private SirenSpawnData _sirenSpawnData = new()
        {
            twoSirensLocation = 6,
            threeSirensLocation = 21,
        };

        private float _enemyExpRoundPow = 1;
        private float _enemyExpPointsPerRound = 0;
        private float _enemyPowerDifficultyCoefficient = 1;
        private float _fixedSirenDifficultyCoefficient = 0.1f;
        private float _sirenSpawnDifficultyModifier = 0;
        private float _arenaExpPointsPerTime = 0;

        //medium
        private EliteSpawnData _elitesData = new()
        {
            difficultyModifier = 0.7f,
            instantChance = 0,
            instantChanceMax = 0.25f,
            instantChanceMod = 0.05f,
            maxEnemyPower = 2,
            maxEnemyPowerMod = 1,
            templateStartIndex = 0,
        };

        private float _instabilityCapsuleSpeed = 6;

        public Builder WithName(string difficultyName)
        {
            _difficultyName = difficultyName;
            return this;
        }

        public Builder WithDifficultyValues(string percentageName, float difficultyMultiplier, float prismMultiplier)
        {
            var difficultyValues = new DifficultyValues()
            {
                difficultyMultiplier = difficultyMultiplier,
                prismMultiplier = prismMultiplier,
                stringPercentage = percentageName,
            };
            _difficultyValues = difficultyValues;
            return this;
        }

        public Builder WithGameEventsData(int eventsMinLevel, int maxSkipLevels, int templateStartIndex, ProbabilityList<bool> eventsProbability)
        {
            _gameEventData = new()
            {
                minLevel = eventsMinLevel,
                maxSkipLevels = maxSkipLevels,
                templateStartIndex = templateStartIndex,
                eventsProbability = eventsProbability,
            };
            return this;
        }

        public Builder WithSirenSpawnData(int twoSirensLocation, int threeSirensLocation)
        {
            _sirenSpawnData = new()
            {
                twoSirensLocation = twoSirensLocation,
                threeSirensLocation = threeSirensLocation,
            };
            return this;
        }

        public Builder WithEliteSpawnData(float difficultyModifier, float instantChance, float instantChanceMax, float instantChanceMod, float maxEnemyPower, float maxEnemyPowerMod, int templateStartIndex)
        {
            _elitesData = new EliteSpawnData()
            {
                difficultyModifier = difficultyModifier,
                instantChance = instantChance,
                instantChanceMax = instantChanceMax,
                instantChanceMod = instantChanceMod,
                maxEnemyPower = maxEnemyPower,
                maxEnemyPowerMod = maxEnemyPowerMod,
                templateStartIndex = templateStartIndex,
            };
            return this;
        }

        public Builder WithInstabilityCapsuleSpeed(float instabilityCapsuleSpeed)
        {
            _instabilityCapsuleSpeed = instabilityCapsuleSpeed;
            return this;
        }

        public Builder WithColors(Color backgroundColor, Color checkmarkColor)
        {
            _backgroundColor = backgroundColor;
            _checkmarkColor = checkmarkColor;
            return this;
        }

        public Builder WithExpLoopPow(float expLoopPow)
        {
            _expLoopPow = expLoopPow;
            return this;
        }

        public Builder WithExpPoints(float expPointsPerEvo, float expPointsPerLoop, float expPointsPerTimeTick)
        {
            _expPointsPerEvo = expPointsPerEvo;
            _expPointsPerLoop = expPointsPerLoop;
            _expPointsPerTimeTick = expPointsPerTimeTick;
            return this;
        }

        public Builder WithArenaValues(float enemyExpRoundPow, float enemyExpPointsPerRound, 
            float enemyPowerDifficultyCoefficient, float fixedSirenDifficultyCoefficient, float sirenSpawnDifficultyModifier, float arenaExpPointsPerTime)
        {
            _enemyExpRoundPow = enemyExpRoundPow;
            _enemyExpPointsPerRound = enemyExpPointsPerRound;
            _enemyPowerDifficultyCoefficient = enemyPowerDifficultyCoefficient;
            _fixedSirenDifficultyCoefficient = fixedSirenDifficultyCoefficient;
            _sirenSpawnDifficultyModifier = sirenSpawnDifficultyModifier;
            _arenaExpPointsPerTime = arenaExpPointsPerTime;
            return this;
        }

        public GkDifficulty Build()
        {
            return new GkDifficulty()
            {
                DifficultyName = _difficultyName,
                TranslationKey = _difficultyName.ToUpper().Replace(" ", "_"),
                
                DifficultyValues = _difficultyValues,
                GameEventData = _gameEventData,
                SirenSpawnData = _sirenSpawnData,
                ElitesData = _elitesData,
                
                InstabilityCapsuleSpeed = _instabilityCapsuleSpeed,
                
                BackgroundColor = _backgroundColor,
                CheckmarkColor = _checkmarkColor,
                EnemyExpLoopPow = _expLoopPow,
                EnemyExpPointsPerEvo = _expPointsPerEvo,
                EnemyExpPointsPerLoop = _expPointsPerLoop,
                EnemyExpPointsPerTimeTick = _expPointsPerTimeTick,
                ArenaValue = new Arena()
                {
                    EnemyExpRoundPow = _enemyExpRoundPow,
                    EnemyExpPointsPerRound = _enemyExpPointsPerRound,
                    EnemyPowerDifficultyCoefficient = _enemyPowerDifficultyCoefficient,
                    FixedSirenDifficultyCoefficient = _fixedSirenDifficultyCoefficient,
                    SirenSpawnDifficultyModifier = _sirenSpawnDifficultyModifier,
                    ArenaExpPointsPerTime = _arenaExpPointsPerTime,
                }
            };
        }
    }

    public class Arena
    {
        public float EnemyExpRoundPow {get; internal set;}
        public float EnemyExpPointsPerRound {get; internal set;}
        public float EnemyPowerDifficultyCoefficient {get; internal set;}
        public float FixedSirenDifficultyCoefficient {get; internal set;}
        public float SirenSpawnDifficultyModifier {get; internal set;}
        public float ArenaExpPointsPerTime {get; internal set;}
    }
}