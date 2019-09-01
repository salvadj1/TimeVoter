using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Timers;

using Fougerite;
using Fougerite.Events;

namespace TimeVoter
{
    public class TimeVoter : Fougerite.Module
    {
        public override string Name{get { return "TimeVoter"; }}
        public override string Author{get { return "Salva/juli"; }}
        public override string Description{get { return "TimeVoter"; }}
        public override Version Version{get { return new Version("1.0"); }}

        private string red = "[color #FF0000]";
        private string blue = "[color #81F7F3]";
        private string green = "[color #82FA58]";
        private string yellow = "[color #F4FA58]";
        private string orange = "[color #FF8000]";

        private float actualtime = World.GetWorld().Time;
        private bool activevote = false;
        private int yes = 0;
        private int no = 0;

        public List<ulong> ids;


        public override void Initialize()
        {
            ids = new List<ulong>();
            Hooks.OnServerInit += OnServerInit;
            Hooks.OnCommand += new Hooks.CommandHandlerDelegate(this.On_Command);
        }
        public override void DeInitialize()
        {
            Hooks.OnServerInit -= OnServerInit;
            Hooks.OnCommand -= new Hooks.CommandHandlerDelegate(this.On_Command);
        }

        public void OnServerInit()
        {
            Timer reloj = new Timer();
            reloj.Interval = 10000;
            reloj.AutoReset = true;
            reloj.Elapsed += (x, y) =>
            {
                actualtime = World.GetWorld().Time;
                if (actualtime >= 19 && actualtime <= 19.5)
                {
                    if (!activevote)
                    {
                        Server.GetServer().BroadcastNotice("VOTE DAY START");
                        Server.GetServer().BroadcastFrom("TIME_VOTER", this.blue + "Vote Day --- /yes");
                        Server.GetServer().BroadcastFrom("TIME_VOTER", this.blue + "Vote Night --- /no");
                        activevote = true;
                    }
                }
                if (actualtime >= 19.6 && activevote)
                {
                    activevote = false;
                    ComprobarVotos();
                }
            };
            reloj.Start();
        }
        public void ComprobarVotos()
        {
            if (yes >= no)
            {
                Server.GetServer().BroadcastFrom("TIME_VOTER", this.orange + "RESULTS: " + yes + " Day | " + no + " Night -- " + this.blue + " DAY WIN");
                World.GetWorld().Time = 6;
                BorrarVariables();
            }
            else if (yes <= no)
            {
                Server.GetServer().BroadcastFrom("TIME_VOTER", this.orange + "RESULTS: " + yes + " Day | " + no + " Night -- " + this.blue + " NIGHT WIN");
                BorrarVariables();
            }
        }
        public void BorrarVariables()
        {
            ids.Clear();
            yes = 0;
            no = 0;
        }
        public void On_Command(Fougerite.Player player, string cmd, string[] args)
        {
            if (cmd == "yes")
            {
                if (!activevote)
                {
                    player.MessageFrom("TIME_VOTER", this.red + "NO ACTIVE");
                    return;
                }

                if (ids.Contains<ulong>(player.UID))
                {
                    player.MessageFrom("TIME_VOTER", this.red + "YOU HAVE ALREADY VOTED");
                    return;
                }

                player.MessageFrom("TIME_VOTER", this.orange + "You Vote Day");
                yes += 1;
                ids.Add(player.UID);

                Server.GetServer().BroadcastFrom("TIME_VOTER", this.blue + "VOTES: " + yes + " Day | " + no + " Night -- " + this.orange + player.Name + " [ YES ]");
            }
            else if (cmd == "no")
            {
                if (!activevote)
                {
                    player.MessageFrom("TIME_VOTER", this.red + "NO ACTIVE");
                    return;
                }

                if (ids.Contains<ulong>(player.UID))
                {
                    player.MessageFrom("TIME_VOTER", this.red + "YOU HAVE ALREADY VOTED");
                    return;
                }

                player.MessageFrom("TIME_VOTER", this.orange + "You Vote Night");
                no += 1;
                ids.Add(player.UID);

                Server.GetServer().BroadcastFrom("TIME_VOTER", this.blue + "VOTES: " + yes + " Day | " + no + " Night -- " + this.orange + player.Name + " [ NO ]");
            }
            else if (cmd == "timeday")
            {
                if (player.Admin)
                {
                    World.GetWorld().Time = 8;
                }
            }
            else if (cmd == "vote")
            {
                player.MessageFrom("TIME_VOTER", this.blue + "Vote Day --- /yes");
                player.MessageFrom("TIME_VOTER", this.blue + "Vote Night --- /no");
                player.MessageFrom("TIME_VOTER", this.blue + "CURRENT TIME: " + actualtime);
            }
        }
    }
}
