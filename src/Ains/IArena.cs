using Orleans;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ains
{
    public interface IArena : IGrainWithStringKey
    {
        Task Config(IPlayer owner, int size, int numberOfPlayers);
        Task<ArenaPhase> GetPhase();
        Task<ReadOnlyDictionary<Hex, Tile>> GetTerrain();

        Task Join(IPlayer player);

        Task<ArenaInfo> GetInfo();
        Task<List<IPlayer>> GetPlayers();

        Task<Hex> GetPlayerPosition(IPlayer player);


        Task<bool> Ready(IPlayer player);
        Task Move(IPlayer player, Hex offset);
        Task Commit(IPlayer player);
    }

    public enum ArenaPhase
    {
        Idle,
        Waiting,
        Ready,
        Finished
    }

    public struct ArenaInfo
    {
        public readonly int Round;
        public readonly int Turn;
        public readonly IPlayer Player;

        public ArenaInfo(int round, int turn, IPlayer player)
        {
            Round = round;
            Turn = turn;
            Player = player;
        }
    }
}
