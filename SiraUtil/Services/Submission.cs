using HMUI;
using TMPro;
using System;
using Zenject;
using UnityEngine;
using System.Linq;
using IPA.Utilities;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    public sealed class Submission : IDisposable
    {
        private readonly Data _data;
        private readonly List<Ticket> _tickets = new List<Ticket>();
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        internal Submission(Data data, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _data = data;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
        }

        public void Dispose()
        {
            var disabled = _tickets.Count > 0;
            if (disabled)
            {
                //_gameplayCoreSceneSetupData.SetField("practiceSettings", new SiraPracticeSettings(_gameplayCoreSceneSetupData.practiceSettings));
            }
            _data.Set(disabled, _tickets.ToArray());
        }

        public Ticket[] Tickets()
        {
            return _tickets.Select(x => x.Copy()).ToArray();
        }

        public Ticket DisableScoreSubmission(string source, string subsource = null)
        {
            var ticket = _tickets.FirstOrDefault(x => x.Source == source);
            if (ticket is null)
            {
                ticket = new Ticket(source, Assembly.GetCallingAssembly());
                _tickets.Add(ticket);
                ticket.AddReason(subsource);
            }
            else
            {
                ticket.AddReason(subsource);
            }
            return ticket;
        }

        public void Remove(Ticket ticket)
        {
            _tickets.Remove(ticket);
        }

        public void Remove(string source)
        {
            Remove(_tickets.FirstOrDefault(x => x.Source == source && x.Assembly == Assembly.GetCallingAssembly()));
        }

        public sealed class Ticket
        {
            private readonly HashSet<string> _reasons = new HashSet<string>();
            internal Assembly Assembly { get; }
            public string Source { get; }

            internal Ticket(string source, Assembly assembly)
            {
                Source = source;
                Assembly = assembly;
            }

            public void AddReason(string reason)
            {
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    _reasons.Add(reason);
                }
            }

            public string[] Reasons()
            {
                return _reasons.ToArray();
            }

            public Ticket Copy()
            {
                var ticket = new Ticket(Source, Assembly);
                for (int i = 0; i < _reasons.Count(); i++)
                {
                    ticket.AddReason(_reasons.ElementAt(i));
                }
                return ticket;
            }
        }

        internal sealed class Data
        {
            private string _data = "";

            public bool disabled;

            internal void Set(bool disabled, Ticket[] tickets)
            {
                _data = "";
                this.disabled = disabled;
                foreach (var ticket in tickets)
                {
                    _data += $"{ticket.Source}\n";
                    foreach (var reason in ticket.Reasons())
                    {
                        _data += $"<size=80%><color=#999999>{reason}</color></size>\n";
                    }
                }
            }

            public string Read()
            {
                return _data;
            }
        }

        internal class Display : IInitializable, IDisposable
        {
            private readonly Data _data;
            private readonly ResultsViewController _resultsViewController;
            private const string LOCAL_KEY = "SIRA_SCORESUBMISSION";
            private CurvedTextMeshPro _curvedText;

            internal Display(Data data, ResultsViewController resultsViewController)
            {
                _data = data;
                _resultsViewController = resultsViewController;
            }

            public void Initialize()
            {
                var bottomContainer = _resultsViewController.GetField<Button, ResultsViewController>("_continueButton").transform.parent;
                var textGameObject = new GameObject("SiraUtilSubmissionDisplay");
                _curvedText = textGameObject.AddComponent<CurvedTextMeshPro>();
                textGameObject.transform.SetParent(bottomContainer);
                (textGameObject.transform as RectTransform).sizeDelta = new Vector2(40f, 100f);
                textGameObject.transform.localPosition = new Vector2(0f, -51f);
                textGameObject.transform.localScale = Vector3.one;
                _curvedText.alignment = TextAlignmentOptions.Top;
                _curvedText.lineSpacing = -45f;
                _curvedText.fontSize = 3.5f;
                _curvedText.gameObject.SetActive(false);
                _resultsViewController.didActivateEvent += ResultsViewController_DidActivate;
                _resultsViewController.didDeactivateEvent += ResultsViewController_DidDeactivate;
            }

            private void ResultsViewController_DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
            {
                _data.disabled = false;
                _curvedText.gameObject.SetActive(false);
            }

            private void ResultsViewController_DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
            {
                if (_data.disabled)
                {
                    _curvedText.gameObject.SetActive(true);
                    _curvedText.text = $"<size=115%><color=#f00e0e>{LOCAL_KEY.LocalizationGetOr("Score Submission Disabled By")}</color></size>\n{_data.Read()}";
                }
            }

            public void Dispose()
            {
                _resultsViewController.didActivateEvent -= ResultsViewController_DidActivate;
                _resultsViewController.didDeactivateEvent -= ResultsViewController_DidDeactivate;
            }
        }

        internal class SiraPrepareLevelCompletionResults : PrepareLevelCompletionResults
        {
            private Submission _submission;
            private GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

            [Inject]
            public void Construct(Submission submission, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
            {
                _submission = submission;
                _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
            }

            public override LevelCompletionResults FillLevelCompletionResults(LevelCompletionResults.LevelEndStateType levelEndStateType, LevelCompletionResults.LevelEndAction levelEndAction)
            {
                var results = base.FillLevelCompletionResults(levelEndStateType, levelEndAction);
                if (levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
                {
                    if (_submission._tickets.Count() > 0)
                    {
                        _gameplayCoreSceneSetupData.SetField<GameplayCoreSceneSetupData, PracticeSettings>("practiceSettings", new SiraPracticeSettings(_gameplayCoreSceneSetupData.practiceSettings));
                        results.SetField("rawScore", -results.rawScore);
                    }
                }
                return results;
            }
        }

        internal class SiraPracticeSettings : PracticeSettings
        {
            public readonly PracticeSettings normalPracticeSettings;

            internal SiraPracticeSettings(PracticeSettings normalPracticeSettings)
            {
                this.normalPracticeSettings = normalPracticeSettings;
            }
        }
    }
}