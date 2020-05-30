using Rocket.API;

namespace IAmSilK.SalvageModifier.Configuration
{
    public class SalvageModifierConfiguration : IRocketPluginConfiguration
    {
        public float DefaultSalvageTime;

        public void LoadDefaults()
        {
            DefaultSalvageTime = 8f;
        }
    }
}
