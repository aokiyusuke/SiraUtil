using System;
using Zenject;
using ModestTree;
using System.Collections.Generic;

namespace SiraUtil.Zenject
{
    public class InstallBuilder
    {
        internal Type Type { get; private set; }
        internal string Destination { get; private set; }
        internal object[] Parameters { get; private set; }
		internal List<string> Circuits { get; } = new List<string>();

		internal InstallBuilder() { }
        internal InstallBuilder(Type type)
        {
            Type = type;
        }

        public InstallBuilder WithParameters(params object[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        public InstallBuilder On(string destination)
        {
            Destination = destination;
            return this;
        }

        public InstallBuilder On<T>()
        {
            On(nameof(T));
            return this;
        }

        public InstallBuilder Register<T>() where T : IInstaller
        {
            Type = typeof(T);
            return this;
        }

		public InstallBuilder ShortCircuitOn(string shortCircuiter)
		{
			Plugin.Log.Info(shortCircuiter);
			Plugin.Log.Info(shortCircuiter);
			Plugin.Log.Info(shortCircuiter);
			Plugin.Log.Info(shortCircuiter);
			Circuits.Add(shortCircuiter);
			return this;
		}

		public InstallBuilder ShortCircuitOn<T>()
		{
			return ShortCircuitOn(typeof(T).Name);
		}

		public InstallBuilder ShortCircuitOnStandard()
		{
			return ShortCircuitOn(nameof(StandardLevelScenesTransitionSetupDataSO));
		}

		public InstallBuilder ShortCircuitOnCampaign()
		{
			return ShortCircuitOn(nameof(MissionLevelScenesTransitionSetupDataSO));
		}

		public InstallBuilder ShortCircuitOnTutorial()
		{
			return ShortCircuitOn(nameof(TutorialScenesTransitionSetupDataSO));
		}

		public InstallBuilder ShortCircuitOnMultiplayer()
		{
			return ShortCircuitOn(nameof(MultiplayerLevelScenesTransitionSetupDataSO));
		}

		internal void Validate()
        {
            Assert.IsNotNull(Type, $"Zenject Registration must have a type. {Utilities.ASSERTHIT}");
            Assert.That(Type.DerivesFrom<IInstaller>(), $"Type must be an IInstaller {Utilities.ASSERTHIT}");
            if (string.IsNullOrEmpty(Destination))
            {
                throw new ArgumentNullException($"{nameof(Type)}:{nameof(Destination)}", "Installer registration needs a destination.");
            }
        }
    }
}