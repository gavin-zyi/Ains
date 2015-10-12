using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains.Logic
{
    [StorageProvider(ProviderName = "AzureStore")]
    public partial class Arena : Grain<ArenaState>, IArena
    {
        public Task Config(IPlayer owner, int size, int numberOfPlayers)
        {
            if (State.Phase != ArenaPhase.Idle)
            {
                throw new Exception("Arena.Configure cannot be called while it is active.");
            }

            State.Owner = owner;
            State.Phase = ArenaPhase.Waiting;
            State.Terrain = new Terrain(size);
            State.Terrain.Populate(new Tile { Name = "Gravel", IsWalkable = true });

            State.NumberOfPlayers = numberOfPlayers;
            State.Players = new List<IPlayer>();
            State.PlayerStates = new Dictionary<IPlayer, ArenaPlayerState>();
            State.Rounds = 1;
            State.Turns = 1;
            State.Actions = new List<ArenaAction>();

            return Join(owner);
        }

        public Task<ArenaPhase> GetPhase()
        {
            return Task.FromResult(State.Phase);
        }

        public Task<ReadOnlyDictionary<Hex, Tile>> GetTerrain()
        {
            return Task.FromResult(State.Terrain.Raw);
        }

        public async Task Join(IPlayer player)
        {            
            if (State.Players.Contains(player))
            {
                throw new Exception($"Player {await player.GetName()} is already in Arena");
            }

            switch (State.Phase)
            {
                case ArenaPhase.Waiting:
                    break;

                case ArenaPhase.Idle:
                case ArenaPhase.Ready:
                case ArenaPhase.Finished:
                    throw new Exception($"Arena is in {State.Phase}");

                default:
                    throw new Exception($"Unknown {State.Phase} state detected in Arena grain");
            }

            if (State.Players.Count >= State.NumberOfPlayers)
            {
                throw new Exception("full");
            }

            State.Players.Add(player);
            State.PlayerStates.Add(player, await ArenaPlayerState.From(player));

            if (State.Players.Count == State.NumberOfPlayers)
            {
                await Act(new ResetAction(null, State.Rounds, State.Turns));

                State.Phase = ArenaPhase.Ready;
            }

            await WriteStateAsync();
        }

        private IPlayer ActivePlayer => State.Players[(State.Turns - 1) % State.Players.Count];
        
        public Task<ArenaInfo> GetInfo()
        {
            if (State.Phase != ArenaPhase.Ready)
            {
                throw new Exception("Arena not ready");
            }

            return Task.FromResult(new ArenaInfo(State.Rounds, State.Turns, ActivePlayer));
        }

        public Task<List<IPlayer>> GetPlayers()
        {
            return Task.FromResult(State.Players);
        }

        public Task<Hex> GetPlayerPosition(IPlayer player)
        {
            if (State.Phase != ArenaPhase.Ready)
            {
                throw new Exception("Arena not ready");
            }

            if (!State.Players.Contains(player))
            {
                throw new Exception("Player not in Arena");
            }

            return Task.FromResult(State.PlayerStates[player].Position);
        }

        public Task<bool> Ready(IPlayer player)
        {
            if (State.Phase != ArenaPhase.Ready)
            {
                throw new Exception("Arena not ready");
            }

            return Task.FromResult(ActivePlayer.Equals(player));
        }


        private async Task Act(ArenaAction action, bool writeState = false)
        {
            State.Actions.Add(action);
            await action.Execute(this);

            if (writeState)
            {
                await WriteStateAsync();
            }
        }

        public Task Move(IPlayer player, Hex offset)
        {
            if (State.Phase != ArenaPhase.Ready)
            {
                throw new Exception("Arena not ready");
            }

            if (!ActivePlayer.Equals(player))
            {
                throw new Exception("Not your turn bitch");
            }

            return Act(new MoveAction(player, State.Rounds, State.Turns, offset), true);            
        }

        public Task Commit(IPlayer player)
        {
            if (State.Phase != ArenaPhase.Ready)
            {
                throw new Exception("Arena not ready");
            }

            if (!ActivePlayer.Equals(player))
            {
                throw new Exception("Not your turn bitch");
            }

            State.Turns++;

            if (State.Turns > State.Players.Count)
            {
                State.Rounds++;
                State.Turns = 1;
            }

            return WriteStateAsync();
        }
    }

    public class ArenaState : GrainState
    {
        public IPlayer Owner { get; set; }
        public ArenaPhase Phase { get; set; }
        public Terrain Terrain { get; set; }

        public int NumberOfPlayers { get; set; }
        public List<IPlayer> Players { get; set; }
        public Dictionary<IPlayer, ArenaPlayerState> PlayerStates { get; set; }

        public int Rounds { get; set; }
        public int Turns { get; set; }
        public List<ArenaAction> Actions { get; set; }
    }

    [Serializable]
    public class ArenaPlayerState
    {
        public IPlayer Player { get; private set; }
        public PlayerStat Stat { get; private set; }
        public Hex Position { get; set; }

        private ArenaPlayerState()
        {

        }

        public static async Task<ArenaPlayerState> From(IPlayer player)
        {
            return new ArenaPlayerState
            {
                Player = player,
                Stat = await player.GetStat()
            };
        }
    }


    [Serializable]
    public abstract class ArenaAction
    {
        private string key;

        public IPlayer Player { get; private set; }
        public int Round { get; private set; }
        public int Turn { get; private set; }

        public virtual bool Distinct { get; } = false;

        public ArenaAction(IPlayer player, int round, int turn)
        {
            Player = player;
            Round = round;
            Turn = turn;
        }

        public async Task Execute(Arena arena)
        {
            var key = arena.GetPrimaryKeyString();

            if (this.key != key)
            {
                if (this.key != null)
                {
                    throw new Exception("Different Arena!");
                }

                this.key = key;
                await OnPrepare(arena);
            }

            await OnExecute(arena);
        }

        protected virtual Task OnPrepare(Arena arena)
        {
            return TaskDone.Done;
        }

        protected abstract Task OnExecute(Arena arena);
    }
}
