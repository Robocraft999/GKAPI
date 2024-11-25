

using UnityEngine;

namespace GKAPI.Difficulties;

public class GkDifficulty
{
    public string DifficultyName { get; private set; }
    public string PercentageName { get; private set; }
    public float DifficultyMultiplier { get; private set; }
    public float PrismMultiplier { get; private set; }
    public int EventsMinLevel { get; private set; }
    public Color BackgroundColor { get; private set; }
    public Color CheckmarkColor { get; private set; }

    public class Builder
    {
        private string difficultyName = "Any";
        private string percentageName = "any%";
        private float difficultyMultiplier = 0f;
        private float prismMultiplier = 1f;
        private int eventsMinLevel = 4;
        private Color backgroundColor = Color.white;
        private Color checkmarkColor = Color.white;

        public Builder WithName(string difficultyName)
        {
            this.difficultyName = difficultyName;
            return this;
        }

        public Builder WithPercentageName(string percentageName)
        {
            this.percentageName = percentageName;
            return this;
        }

        public Builder WithDifficultyMultiplier(float difficultyMultiplier)
        {
            this.difficultyMultiplier = difficultyMultiplier;
            return this;
        }

        public Builder WithPrismMultiplier(float prismMultiplier)
        {
            this.prismMultiplier = prismMultiplier;
            return this;
        }

        public Builder WithEventsMinLevel(int eventsMinLevel)
        {
            this.eventsMinLevel = eventsMinLevel;
            return this;
        }

        public Builder WithColors(Color backgroundColor, Color checkmarkColor)
        {
            this.backgroundColor = backgroundColor;
            this.checkmarkColor = checkmarkColor;
            return this;
        }

        public GkDifficulty Build()
        {
            return new GkDifficulty()
            {
                DifficultyName = difficultyName,
                PercentageName = percentageName,
                DifficultyMultiplier = difficultyMultiplier,
                PrismMultiplier = prismMultiplier,
                EventsMinLevel = eventsMinLevel,
                BackgroundColor = backgroundColor,
                CheckmarkColor = checkmarkColor
            };
        }
    }
}