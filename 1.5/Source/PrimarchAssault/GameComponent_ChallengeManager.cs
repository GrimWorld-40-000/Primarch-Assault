using System.Collections.Generic;
using System.Linq;
using PrimarchAssault.External;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimworldModding
{
    public class GameComponent_ChallengeManager : GameComponent
    {
        private static Game _game;

        public GameComponent_ChallengeManager(Game game)
        {
            _game = game;

            //HealthBar = new HealthBarWindow();
        }

        //public HealthBarWindow HealthBar;

        public static GameComponent_ChallengeManager Instance => _game.GetComponent<GameComponent_ChallengeManager>();

        public ChallengeDef QueuedPhaseOne => _queuedPhaseOne;

        private ChallengeDef _queuedPhaseOne;
        private int _queuedPhaseOneTick = -1;
        private Dictionary<ChallengeDef, int> _queuedPhaseTwos = new Dictionary<ChallengeDef, int>();
        private readonly List<ChallengeDef> _tmpChallengesToDo = new List<ChallengeDef>();
        private Dictionary<ChallengeDef, ChampionSpawnData> QueuedChampions => _queuedChampions ??= new Dictionary<ChallengeDef, ChampionSpawnData>();
        private Dictionary<ChallengeDef, ChampionSpawnData> _queuedChampions;
        private List<ChampionTrackerData> ActiveChampions => _activeChampions ??= new List<ChampionTrackerData>();
        private List<ChampionTrackerData> _activeChampions = new List<ChampionTrackerData>();
        

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _activeChampions, "activeChampions", LookMode.Deep);
            Scribe_Defs.Look(ref _queuedPhaseOne, "queuedPhaseOne");
            Scribe_Values.Look(ref _queuedPhaseOneTick, "queuedPhaseOneTick");
            Scribe_Collections.Look(ref _queuedPhaseTwos, "queuedPhaseTwos", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref _queuedChampions, "queuedChampions", LookMode.Def, LookMode.Deep);
            base.ExposeData();
        }

        public void RegisterActiveChampion(int champion, ChallengeDef challenge)
        {
            ActiveChampions.Add(new ChampionTrackerData(champion, challenge));
            //HealthBar.ChallengeDef = challenge;
            //HealthBar.CurrentPawn = champion;
            //Find.WindowStack.Add(HealthBar); 
        }
        
        public void RemoveActiveChampion(int champion)
        {
            ActiveChampions.RemoveWhere(data => data?.Champion == champion || data == null);
            if (ActiveChampions.Empty())
            {
                //HealthBar.Close();
            }
        }

        public bool IsPhaseOneQueued => _queuedPhaseOne != null;


        /// <summary>
        /// Cannot start a challenge if any phase 1 is queued, or its phase 2 is queued
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public bool CanStartNewChallenge(ChallengeDef def)
        {
            if (IsPhaseOneQueued)
            {
                return false;
            }

            return !_queuedPhaseTwos.ContainsKey(def);
        }

        public void StartPhaseOne(ChallengeDef def)
        {
            _queuedPhaseOne = def;
            _queuedPhaseOneTick = Find.TickManager.TicksGame + def.ticksUntilArrival.RandomInRange;
        }
        
        public void StartPhaseTwo(ChallengeDef def)
        {
            _queuedPhaseTwos[def] = Find.TickManager.TicksGame + def.ticksUntilRevenge.RandomInRange;
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            int tickNow = Find.TickManager.TicksGame;
            if (tickNow % 200 != 0) return;
            if (IsPhaseOneQueued && tickNow > _queuedPhaseOneTick)
            {
                _queuedPhaseOne.FirePhaseOne();
                QueuedChampions.Add(_queuedPhaseOne, new ChampionSpawnData(Find.TickManager.TicksGame + _queuedPhaseOne.ticksUntilChampionArrives, false));
                _queuedPhaseOne = null;
            }
            
            _tmpChallengesToDo.Clear();
            
            foreach (var (challengeDef, tick) in _queuedPhaseTwos)
            {
                if (tickNow <= tick) continue;
                challengeDef.FirePhaseTwo();
                QueuedChampions.Add(challengeDef, new ChampionSpawnData(Find.TickManager.TicksGame + challengeDef.ticksUntilChampionArrives, true));
                _tmpChallengesToDo.Add(challengeDef);
            }

            foreach (ChallengeDef challengeDef in _tmpChallengesToDo)
            {
                _queuedPhaseTwos.Remove(challengeDef);
            }

            if (QueuedChampions.NullOrEmpty()) return;
            var (challenge, data) = QueuedChampions.First();

            if (tickNow <= data.TickToSpawn) return;
            challenge.SpawnChampion(data.IsPhaseTwo? null: challenge, data.IsPhaseTwo? challenge.championDrop: null);
            QueuedChampions.Remove(challenge);
        }

        public void StartAllPhaseTwos()
        {
            List <ChallengeDef> challenges = _queuedPhaseTwos.Keys.ToList();
            foreach (var challengeDef in challenges)
            {
                _queuedPhaseTwos[challengeDef] = Find.TickManager.TicksGame;
            }
        }
    }

    public class ChampionSpawnData(int tickToSpawn, bool isPhaseTwo) : IExposable
    {
        public int TickToSpawn = tickToSpawn;
        public bool IsPhaseTwo = isPhaseTwo;
        public void ExposeData()
        {
            Scribe_Values.Look(ref TickToSpawn, "ticksToSpawn");
            Scribe_Values.Look(ref IsPhaseTwo, "isPhaseTwo");
        }
    }

    public class ChampionTrackerData(int champion, ChallengeDef challenge) : IExposable
    {
        public int Champion = champion;
        public ChallengeDef Challenge = challenge;
        public void ExposeData()
        {
            Scribe_Values.Look(ref Champion, "champion");
            Scribe_Defs.Look(ref Challenge, "challenge");
        }
    }
}